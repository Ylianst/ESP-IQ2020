//
//  PacketProcessor.swift
//  DataViewerXC
//
//  Packet decoding and processing logic
//

import Foundation

class PacketProcessor: ObservableObject {
    @Published var decodedData: [String: String] = [:]
    @Published var stateDecoded: String = ""
    @Published var uniquePackets: Set<String> = []
    
    var filter: UInt8 = 0
    var filterCmd1: UInt8 = 0
    var filterCmd2: UInt8 = 0
    
    func processPacket(data: Data) {
        guard data.count >= 5 else { return }
        
        let bytes = [UInt8](data)
        let dataLen = Int(bytes[3])
        
        guard data.count >= (dataLen + 6) else { return }
        
        // Apply filters
        if filter != 0 && bytes[1] != filter && bytes[2] != filter {
            return
        }
        
        if filterCmd1 != 0 && (dataLen < 2 || bytes[5] != filterCmd1) {
            return
        }
        
        if filterCmd2 != 0 && (dataLen < 2 || bytes[6] != filterCmd2) {
            return
        }
        
        // Add to unique packets
        let packetString = formatPacketForStore(bytes: bytes, dataLen: dataLen)
        uniquePackets.insert(packetString)
        
        // Decode specific packet types
        decodePacket(bytes: bytes, dataLen: dataLen)
    }
    
    private func formatPacketForStore(bytes: [UInt8], dataLen: Int) -> String {
        let dst = String(format: "%02X", bytes[1])
        let src = String(format: "%02X", bytes[2])
        let op = String(format: "%02X", bytes[4])
        let data = (5..<(5 + dataLen)).map { String(format: "%02X", bytes[$0]) }.joined()
        return "\(dst) \(src) \(op) \(data)"
    }
    
    private func decodePacket(bytes: [UInt8], dataLen: Int) {
        // Connection Kit responses (0x1F)
        if bytes[1] == 0x1F && bytes[2] == 0x01 && bytes[4] == 0x80 {
            
            // Lights status (0x17 0x05)
            if dataLen >= 2 && bytes[5] == 0x17 && bytes[6] == 0x05 {
                if dataLen >= 20 {
                    decodedData["Lights"] = bytes[24] == 0x01 ? "Enabled" : "Disabled"
                }
            }
            
            // Temperature status (0x02 0x56)
            if dataLen >= 2 && bytes[5] == 0x02 && bytes[6] == 0x56 {
                decodeTemperatureStatus(bytes: bytes, dataLen: dataLen)
            }
            
            // Version string (0x01 0x00)
            if dataLen >= 2 && bytes[5] == 0x01 && bytes[6] == 0x00 {
                if dataLen > 3 {
                    let versionData = Data(bytes[7..<(5 + dataLen)])
                    if let version = String(data: versionData, encoding: .utf8) {
                        decodedData["Version"] = version
                    }
                }
            }
        }
    }
    
    private func decodeTemperatureStatus(bytes: [UInt8], dataLen: Int) {
        guard dataLen >= 133 else { return }
        
        // Temperature strings
        if dataLen >= 94 {
            let tempSetData = Data(bytes[90..<94])
            if let tempSet = String(data: tempSetData, encoding: .utf8) {
                decodedData["Temp Set"] = tempSet
            }
        }
        
        if dataLen >= 98 {
            let tempCurrentData = Data(bytes[94..<98])
            if let tempCurrent = String(data: tempCurrentData, encoding: .utf8) {
                decodedData["Temp Current"] = tempCurrent
            }
        }
        
        if dataLen >= 40 {
            let heaterOutletData = Data(bytes[36..<40])
            if let heaterOutlet = String(data: heaterOutletData, encoding: .utf8) {
                decodedData["Heater Outlet"] = heaterOutlet
            }
        }
        
        // Status flags
        if dataLen >= 11 {
            decodedData["JetInfo1"] = String(bytes[9], radix: 2)
            decodedData["JetInfo2"] = String(bytes[10], radix: 2)
        }
        
        // Time and date
        if dataLen >= 138 {
            let seconds = Int(bytes[131])
            let minutes = Int(bytes[132])
            let hours = Int(bytes[133])
            let day = Int(bytes[134])
            let month = Int(bytes[135]) + 1
            let year = Int(bytes[136]) + (Int(bytes[137]) << 8)
            
            if let date = createDate(year: year, month: month, day: day, hour: hours, minute: minutes, second: seconds) {
                let formatter = DateFormatter()
                formatter.dateStyle = .medium
                formatter.timeStyle = .medium
                decodedData["Time"] = formatter.string(from: date)
            }
        }
        
        // Runtime counters
        if dataLen >= 44 {
            decodedData["Heater Total Runtime"] = String(getInt(bytes: bytes, offset: 40))
        }
        
        if dataLen >= 48 {
            decodedData["Jets 1 Total Runtime"] = String(getInt(bytes: bytes, offset: 44))
        }
        
        if dataLen >= 52 {
            decodedData["Lifetime Runtime 1"] = String(getInt(bytes: bytes, offset: 48))
        }
        
        if dataLen >= 56 {
            decodedData["Unknown Counter 1"] = String(getInt(bytes: bytes, offset: 52))
        }
        
        if dataLen >= 64 {
            decodedData["Jets 2 Total Runtime"] = String(getInt(bytes: bytes, offset: 60))
        }
        
        if dataLen >= 68 {
            decodedData["Jets 3 Total Runtime"] = String(getInt(bytes: bytes, offset: 64))
        }
        
        if dataLen >= 72 {
            decodedData["Unknown Counter 2"] = String(getInt(bytes: bytes, offset: 68))
        }
        
        if dataLen >= 76 {
            decodedData["Lights Total Runtime"] = String(getInt(bytes: bytes, offset: 72))
        }
        
        if dataLen >= 82 {
            decodedData["Lifetime Runtime 2"] = String(getInt(bytes: bytes, offset: 78))
        }
        
        if dataLen >= 86 {
            decodedData["Unknown Counter 3"] = String(getInt(bytes: bytes, offset: 82))
        }
        
        if dataLen >= 90 {
            decodedData["Unknown Counter 4"] = String(getInt(bytes: bytes, offset: 86))
        }
        
        // Voltage readings
        if dataLen >= 100 {
            decodedData["Voltage L1"] = String(getShort(bytes: bytes, offset: 98))
        }
        
        if dataLen >= 102 {
            decodedData["Voltage Heater"] = String(getShort(bytes: bytes, offset: 100))
        }
        
        if dataLen >= 104 {
            decodedData["Voltage L2"] = String(getShort(bytes: bytes, offset: 102))
        }
        
        // Current readings
        if dataLen >= 108 {
            decodedData["Current L1"] = String(getShort(bytes: bytes, offset: 106))
        }
        
        if dataLen >= 110 {
            decodedData["Current Heater"] = String(getShort(bytes: bytes, offset: 108))
        }
        
        if dataLen >= 112 {
            decodedData["Current L2"] = String(getShort(bytes: bytes, offset: 110))
        }
        
        // Power readings
        if dataLen >= 116 {
            decodedData["Power L1"] = String(getShort(bytes: bytes, offset: 114))
        }
        
        if dataLen >= 118 {
            decodedData["Power Heater"] = String(getShort(bytes: bytes, offset: 116))
        }
        
        if dataLen >= 120 {
            decodedData["Power L2"] = String(getShort(bytes: bytes, offset: 118))
        }
        
        // Generate detailed decode
        generateStateDecoded(bytes: bytes, dataLen: dataLen)
    }
    
    private func generateStateDecoded(bytes: [UInt8], dataLen: Int) {
        let totalLen = dataLen + 6
        let hex = (1..<totalLen).map { String(format: "%02X", bytes[$0]) }.joined()
        
        var decode = "Hex: \(hex)\n"
        
        if hex.count >= 20 {
            decode += "\(hex.prefix(4))     - Unknown\n"
            decode += "\(hex.dropFirst(4).prefix(2))       - Flags\n"
            decode += "\(hex.dropFirst(6).prefix(2))       - Flags\n"
        }
        
        if hex.count >= 42 {
            decode += "\(hex.dropFirst(8).prefix(18)) - Unknown\n"
            decode += "\(hex.dropFirst(26).prefix(4))     - Flags, 0x4000 is Celsius\n"
        }
        
        if dataLen >= 40 {
            let heaterOutletData = Data(bytes[36..<40])
            if let str = String(data: heaterOutletData, encoding: .utf8) {
                decode += "Heater outlet temp: \"\(str)\"\n"
            }
        }
        
        if dataLen >= 44 {
            decode += String(format: "Heater total runtime: %d seconds\n", getInt(bytes: bytes, offset: 40))
        }
        
        if dataLen >= 48 {
            decode += String(format: "Jets 1 total runtime: %d seconds\n", getInt(bytes: bytes, offset: 44))
        }
        
        if dataLen >= 52 {
            decode += String(format: "Lifetime: %d seconds\n", getInt(bytes: bytes, offset: 48))
        }
        
        if dataLen >= 98 {
            let tempSetData = Data(bytes[90..<94])
            let tempCurrentData = Data(bytes[94..<98])
            if let tempSet = String(data: tempSetData, encoding: .utf8),
               let tempCurrent = String(data: tempCurrentData, encoding: .utf8) {
                decode += "Target Temperature: \"\(tempSet)\"\n"
                decode += "Current Temperature: \"\(tempCurrent)\"\n"
            }
        }
        
        if dataLen >= 120 {
            decode += String(format: "Voltage L1: %d\n", getShort(bytes: bytes, offset: 98))
            decode += String(format: "Voltage Heater: %d\n", getShort(bytes: bytes, offset: 100))
            decode += String(format: "Voltage L2: %d\n", getShort(bytes: bytes, offset: 102))
            decode += String(format: "Current L1: %d\n", getShort(bytes: bytes, offset: 106))
            decode += String(format: "Current Heater: %d\n", getShort(bytes: bytes, offset: 108))
            decode += String(format: "Current L2: %d\n", getShort(bytes: bytes, offset: 110))
            decode += String(format: "Power L1: %d\n", getShort(bytes: bytes, offset: 114))
            decode += String(format: "Power Heater: %d\n", getShort(bytes: bytes, offset: 116))
            decode += String(format: "Power L2: %d\n", getShort(bytes: bytes, offset: 118))
        }
        
        if dataLen >= 138 {
            let seconds = Int(bytes[131])
            let minutes = Int(bytes[132])
            let hours = Int(bytes[133])
            let day = Int(bytes[134])
            let month = Int(bytes[135]) + 1
            let year = Int(bytes[136]) + (Int(bytes[137]) << 8)
            decode += String(format: "Time: %02d:%02d:%02d\n", hours, minutes, seconds)
            decode += String(format: "Date: %d-%d-%d\n", day, month, year)
        }
        
        stateDecoded = decode
    }
    
    private func getInt(bytes: [UInt8], offset: Int) -> Int {
        guard offset + 3 < bytes.count else { return 0 }
        return Int(bytes[offset]) + (Int(bytes[offset + 1]) << 8) + 
               (Int(bytes[offset + 2]) << 16) + (Int(bytes[offset + 3]) << 24)
    }
    
    private func getShort(bytes: [UInt8], offset: Int) -> Int {
        guard offset + 1 < bytes.count else { return 0 }
        return Int(bytes[offset]) + (Int(bytes[offset + 1]) << 8)
    }
    
    private func createDate(year: Int, month: Int, day: Int, hour: Int, minute: Int, second: Int) -> Date? {
        var components = DateComponents()
        components.year = year
        components.month = month
        components.day = day
        components.hour = hour
        components.minute = minute
        components.second = second
        return Calendar.current.date(from: components)
    }
    
    func setFilter(filterType: String) {
        switch filterType {
        case "Connection Kit":
            filter = 0x1F
            filterCmd1 = 0
            filterCmd2 = 0
        case "Connection Kit 0x0256":
            filter = 0x1F
            filterCmd1 = 0x02
            filterCmd2 = 0x56
        case "Freshwater Module":
            filter = 0x29
            filterCmd1 = 0
            filterCmd2 = 0
        case "Freshwater IQ Module":
            filter = 0x37
            filterCmd1 = 0
            filterCmd2 = 0
        case "Audio Module":
            filter = 0x33
            filterCmd1 = 0
            filterCmd2 = 0
        case "ACE Module":
            filter = 0x24
            filterCmd1 = 0
            filterCmd2 = 0
        default:
            filter = 0
            filterCmd1 = 0
            filterCmd2 = 0
        }
    }
    
    func clearAll() {
        decodedData.removeAll()
        stateDecoded = ""
        uniquePackets.removeAll()
    }
}
