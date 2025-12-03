//
//  TCPClient.swift
//  DataViewerXC
//
//  TCP Client for ESP32 communication
//

import Foundation
import Network

class TCPClient: ObservableObject {
    @Published var isConnected = false
    @Published var connectionState: ConnectionState = .disconnected
    @Published var rawDataLog: [String] = []
    @Published var packetDataLog: [String] = []
    
    private var connection: NWConnection?
    private var queue = DispatchQueue(label: "TCPClient")
    private var readBuffer = Data()
    
    var onDataReceived: ((Data) -> Void)?
    var onPacketReceived: ((String) -> Void)?
    
    enum ConnectionState {
        case disconnected
        case connecting
        case connected
    }
    
    func connect(host: String, port: UInt16) {
        disconnect()
        
        let endpoint = NWEndpoint.hostPort(host: NWEndpoint.Host(host), port: NWEndpoint.Port(rawValue: port)!)
        connection = NWConnection(to: endpoint, using: .tcp)
        
        let hostString = host
        let portValue = port
        
        connection?.stateUpdateHandler = { [weak self] state in
            DispatchQueue.main.async {
                switch state {
                case .ready:
                    self?.connectionState = .connected
                    self?.isConnected = true
                    self?.appendPacketData("Connected to \(hostString):\(portValue)")
                    self?.startReceiving()
                case .waiting, .preparing:
                    self?.connectionState = .connecting
                    self?.isConnected = false
                case .failed(let error):
                    self?.connectionState = .disconnected
                    self?.isConnected = false
                    self?.connection = nil
                    self?.appendPacketData("Connection failed: \(error.localizedDescription)")
                case .cancelled:
                    self?.connectionState = .disconnected
                    self?.isConnected = false
                    self?.connection = nil
                @unknown default:
                    break
                }
            }
        }
        
        connectionState = .connecting
        connection?.start(queue: queue)
    }
    
    func disconnect() {
        if connection != nil {
            DispatchQueue.main.async { [weak self] in
                self?.appendPacketData("Connection closed")
            }
        }
        connection?.cancel()
        connection = nil
        connectionState = .disconnected
        isConnected = false
        readBuffer.removeAll()
    }
    
    private func startReceiving() {
        connection?.receive(minimumIncompleteLength: 1, maximumLength: 2000) { [weak self] data, _, isComplete, error in
            guard let self = self else { return }
            
            if let data = data, !data.isEmpty {
                DispatchQueue.main.async {
                    self.appendRawData(data)
                }
                self.readBuffer.append(data)
                self.processBuffer()
            }
            
            if isComplete {
                DispatchQueue.main.async {
                    self.appendPacketData("Connection closed by remote")
                    self.disconnect()
                }
                return
            }
            
            if let error = error {
                DispatchQueue.main.async {
                    self.appendPacketData("Receive error: \(error.localizedDescription)")
                    self.disconnect()
                }
                return
            }
            
            self.startReceiving()
        }
    }
    
    private func appendRawData(_ data: Data) {
        let hexString = data.map { String(format: "%02X", $0) }.joined()
        let logEntry = " <-- \(hexString)"
        rawDataLog.append(logEntry)
    }
    
    func appendPacketData(_ message: String) {
        DispatchQueue.main.async { [weak self] in
            self?.packetDataLog.append(message)
        }
    }
    
    private func processBuffer() {
        while true {
            let consumed = processData()
            guard consumed > 0 else {
                break
            }
            
            if consumed < readBuffer.count {
                readBuffer = readBuffer.advanced(by: consumed)
            } else {
                readBuffer.removeAll()
            }
        }
    }
    
    private func processData() -> Int {
        guard readBuffer.count >= 5 else { return 0 }
        
        let bytes = [UInt8](readBuffer)
        
        // Check packet sync byte
        if bytes[0] != 0x1C {
            // Out of sync - look for next sync byte
            DispatchQueue.main.async { [weak self] in
                self?.appendPacketData("Out of sync")
            }
            
            for i in 1..<bytes.count {
                if bytes[i] == 0x1C {
                    return i
                }
            }
            return bytes.count
        }
        
        // Validate packet structure
        if (bytes[1] != 0x01 && bytes[2] != 0x01) && (bytes[4] != 0x40 && bytes[4] != 0x80) {
            for i in 1..<bytes.count {
                if bytes[i] == 0x1C {
                    return i
                }
            }
            return bytes.count
        }
        
        let dataLen = Int(bytes[3])
        let totalLen = dataLen + 6
        
        guard readBuffer.count >= totalLen else { return 0 }
        
        // Verify checksum
        let checksum = computeChecksum(bytes: bytes, start: 1, end: dataLen + 5)
        if checksum != bytes[dataLen + 5] {
            DispatchQueue.main.async { [weak self] in
                self?.appendPacketData("Invalid checksum")
            }
            for i in 1..<bytes.count {
                if bytes[i] == 0x1C {
                    return i
                }
            }
            return bytes.count
        }
        
        // Valid packet - process it
        let packetString = formatPacket(bytes: bytes, dataLen: dataLen)
        DispatchQueue.main.async { [weak self] in
            self?.appendPacketData(" <-- \(packetString)")
            self?.onPacketReceived?(packetString)
        }
        
        if let callback = onDataReceived {
            callback(Data(bytes[0..<totalLen]))
        }
        
        return totalLen
    }
    
    private func formatPacket(bytes: [UInt8], dataLen: Int) -> String {
        let dst = toHex(bytes[1])
        let src = toHex(bytes[2])
        let op = toHex(bytes[4])
        let data = (5..<(5 + dataLen)).map { toHex(bytes[$0]) }.joined()
        return "\(dst) \(src) \(op) \(data)"
    }
    
    private func toHex(_ byte: UInt8) -> String {
        String(format: "%02X", byte)
    }
    
    func sendPacket(_ textPacket: String) {
        guard isConnected else { return }
        
        let cleaned = textPacket.replacingOccurrences(of: "[<\\-:, ]", with: "", options: .regularExpression)
        guard let raw = hexStringToBytes(cleaned), raw.count >= 4 else { return }
        
        let dst = raw[0]
        let src = raw[1]
        let op = raw[2]
        let data = Array(raw[3...])
        
        let totalLen = 6 + data.count
        var packet = [UInt8](repeating: 0, count: totalLen)
        
        packet[0] = 0x1C
        packet[1] = dst
        packet[2] = src
        packet[3] = UInt8(data.count)
        packet[4] = op
        
        for (i, byte) in data.enumerated() {
            packet[5 + i] = byte
        }
        
        packet[totalLen - 1] = computeChecksum(bytes: packet, start: 1, end: totalLen - 1)
        
        let packetData = Data(packet)
        
        DispatchQueue.main.async { [weak self] in
            guard let self = self else { return }
            let hexString = packet.map { String(format: "%02X", $0) }.joined()
            self.rawDataLog.append(" --> \(hexString)")
            self.appendPacketData(" --> \(self.formatPacket(bytes: packet, dataLen: data.count))")
        }
        
        connection?.send(content: packetData, completion: .contentProcessed { error in
            if error != nil {
                DispatchQueue.main.async { [weak self] in
                    self?.disconnect()
                }
            }
        })
    }
    
    func sendRawData(_ hexString: String) {
        guard isConnected else { return }
        
        let cleaned = hexString.replacingOccurrences(of: "[<\\-:, ]", with: "", options: .regularExpression)
        guard let bytes = hexStringToBytes(cleaned) else { return }
        
        let data = Data(bytes)
        
        DispatchQueue.main.async { [weak self] in
            let hex = bytes.map { String(format: "%02X", $0) }.joined()
            self?.rawDataLog.append(" --> \(hex)")
        }
        
        connection?.send(content: data, completion: .contentProcessed { error in
            if error != nil {
                DispatchQueue.main.async { [weak self] in
                    self?.disconnect()
                }
            }
        })
    }
    
    private func hexStringToBytes(_ hex: String) -> [UInt8]? {
        guard hex.count % 2 == 0 else { return nil }
        
        var bytes = [UInt8]()
        var index = hex.startIndex
        
        while index < hex.endIndex {
            let nextIndex = hex.index(index, offsetBy: 2)
            let byteString = hex[index..<nextIndex]
            
            guard let byte = UInt8(byteString, radix: 16) else { return nil }
            bytes.append(byte)
            index = nextIndex
        }
        
        return bytes
    }
    
    private func computeChecksum(bytes: [UInt8], start: Int, end: Int) -> UInt8 {
        var sum: UInt32 = 0
        for i in start..<end {
            sum += UInt32(bytes[i])
        }
        sum ^= 0xFFFFFFFF
        return UInt8(sum & 0xFF)
    }
    
    func clearLogs() {
        rawDataLog.removeAll()
        packetDataLog.removeAll()
    }
}
