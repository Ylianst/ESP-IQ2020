//
//  ContentView.swift
//  DataViewerXC
//
//  Main UI - SwiftUI Port
//

import SwiftUI

struct ContentView: View {
    @StateObject private var tcpClient = TCPClient()
    @StateObject private var packetProcessor = PacketProcessor()
    @EnvironmentObject var appState: AppState
    
    @State private var address = UserDefaults.standard.string(forKey: "lastAddress") ?? "192.168.1.1:1234"
    @State private var selectedTab = 0
    @State private var packetCommand = "29 99 40 1E010000FFFFFF00FF04FFFFFFFFFF"
    @State private var rawCommand = "1C2101034021010078"
    @State private var selectedFilter = "None"
    @State private var logFileHandle: FileHandle?
    @State private var logFileURL: URL?
    @State private var pollTimer: Timer?
    
    var body: some View {
        VStack(spacing: 0) {
            // Connection bar
            connectionBar
            
            // Main content with tabs
            TabView(selection: $selectedTab) {
                dataPacketsTab
                    .tabItem {
                        Label("Data Packets", systemImage: "list.bullet.rectangle")
                    }
                    .tag(0)
                
                rawDataTab
                    .tabItem {
                        Label("Raw Data", systemImage: "doc.plaintext")
                    }
                    .tag(1)
                
                packetStoreTab
                    .tabItem {
                        Label("Packet Store", systemImage: "tray.full")
                    }
                    .tag(2)
                
                decodedDataTab
                    .tabItem {
                        Label("Decoded Data", systemImage: "chart.bar.doc.horizontal")
                    }
                    .tag(3)
                
                stateTab
                    .tabItem {
                        Label("State", systemImage: "info.circle")
                    }
                    .tag(4)
            }
        }
        .frame(minWidth: 800, minHeight: 600)
        .onAppear {
            setupTCPClient()
        }
        .onReceive(NotificationCenter.default.publisher(for: .clearAll)) { _ in
            clearAll()
        }
        .onReceive(NotificationCenter.default.publisher(for: .toggleLogging)) { _ in
            toggleLogging()
        }
        .onReceive(NotificationCenter.default.publisher(for: .togglePolling)) { _ in
            togglePolling()
        }
        .onReceive(NotificationCenter.default.publisher(for: .setFilter)) { notification in
            if let filterName = notification.userInfo?["filter"] as? String {
                selectedFilter = filterName
                appState.selectedFilter = filterName
                packetProcessor.setFilter(filterType: filterName)
            }
        }
        .onReceive(NotificationCenter.default.publisher(for: .sendCommand)) { notification in
            if let command = notification.userInfo?["command"] as? String {
                tcpClient.sendPacket(command)
            }
        }
        .onReceive(NotificationCenter.default.publisher(for: .setCurrentTime)) { _ in
            setCurrentTime()
        }
        .onReceive(NotificationCenter.default.publisher(for: .annotateLog)) { _ in
            annotateLog()
        }
        .onChange(of: tcpClient.isConnected) { newValue in
            appState.isConnected = newValue
        }
    }
    
    private var connectionBar: some View {
        HStack(spacing: 12) {
            TextField("Address:Port", text: $address)
                .textFieldStyle(.roundedBorder)
                .disabled(tcpClient.isConnected)
                .frame(maxWidth: .infinity)
                .onChange(of: address) { newValue in
                    UserDefaults.standard.set(newValue, forKey: "lastAddress")
                }
            
            Button(tcpClient.isConnected ? "Disconnect" : "Connect") {
                if tcpClient.isConnected {
                    tcpClient.disconnect()
                } else {
                    connectToDevice()
                }
            }
            .buttonStyle(.borderedProminent)
            .frame(width: 120)
        }
        .padding()
        .background(Color(NSColor.controlBackgroundColor))
    }
    
    private var dataPacketsTab: some View {
        VStack(spacing: 0) {
            ScrollView {
                ScrollViewReader { proxy in
                    VStack(alignment: .leading, spacing: 2) {
                        ForEach(Array(tcpClient.packetDataLog.enumerated()), id: \.offset) { index, line in
                            Text(line)
                                .font(.system(.body, design: .monospaced))
                                .foregroundColor(.black)
                                .textSelection(.enabled)
                                .id(index)
                        }
                    }
                    .frame(maxWidth: .infinity, alignment: .leading)
                    .padding(8)
                    .onChange(of: tcpClient.packetDataLog.count) { _ in
                        if let last = tcpClient.packetDataLog.indices.last {
                            proxy.scrollTo(last, anchor: .bottom)
                        }
                    }
                }
            }
            .background(Color(red: 1.0, green: 0.93, blue: 0.82))
            
            // Command input
            HStack(spacing: 12) {
                TextField("Command", text: $packetCommand)
                    .textFieldStyle(.roundedBorder)
                    .font(.system(.body, design: .monospaced))
                    .disabled(!tcpClient.isConnected)
                
                Button("Send") {
                    tcpClient.sendPacket(packetCommand)
                }
                .disabled(!tcpClient.isConnected)
                .frame(width: 80)
            }
            .padding()
            .background(Color(NSColor.controlBackgroundColor))
        }
    }
    
    private var rawDataTab: some View {
        VStack(spacing: 0) {
            ScrollView {
                ScrollViewReader { proxy in
                    VStack(alignment: .leading, spacing: 2) {
                        ForEach(Array(tcpClient.rawDataLog.enumerated()), id: \.offset) { index, line in
                            Text(line)
                                .font(.system(.body, design: .monospaced))
                                .foregroundColor(.black)
                                .textSelection(.enabled)
                                .id(index)
                        }
                    }
                    .frame(maxWidth: .infinity, alignment: .leading)
                    .padding(8)
                    .onChange(of: tcpClient.rawDataLog.count) { _ in
                        if let last = tcpClient.rawDataLog.indices.last {
                            proxy.scrollTo(last, anchor: .bottom)
                        }
                    }
                }
            }
            .background(Color(red: 1.0, green: 0.93, blue: 0.82))
            
            // Raw command input
            HStack(spacing: 12) {
                TextField("Raw Hex", text: $rawCommand)
                    .textFieldStyle(.roundedBorder)
                    .font(.system(.body, design: .monospaced))
                    .disabled(!tcpClient.isConnected)
                
                Button("Send") {
                    tcpClient.sendRawData(rawCommand)
                }
                .disabled(!tcpClient.isConnected)
                .frame(width: 80)
            }
            .padding()
            .background(Color(NSColor.controlBackgroundColor))
        }
    }
    
    private var packetStoreTab: some View {
        List {
            ForEach(Array(packetProcessor.uniquePackets.sorted()), id: \.self) { packet in
                Text(packet)
                    .font(.system(.body, design: .monospaced))
                    .textSelection(.enabled)
            }
        }
        .listStyle(.plain)
    }
    
    private var decodedDataTab: some View {
        List {
            ForEach(packetProcessor.decodedData.sorted(by: { $0.key < $1.key }), id: \.key) { key, value in
                HStack {
                    Text(key)
                        .font(.system(.body, design: .monospaced))
                        .frame(width: 250, alignment: .leading)
                    Text(value)
                        .font(.system(.body, design: .monospaced))
                        .textSelection(.enabled)
                }
            }
        }
        .listStyle(.plain)
    }
    
    private var stateTab: some View {
        ScrollView {
            Text(packetProcessor.stateDecoded)
                .font(.system(.body, design: .monospaced))
                .foregroundColor(.black)
                .textSelection(.enabled)
                .frame(maxWidth: .infinity, alignment: .leading)
                .padding()
        }
        .background(Color(red: 1.0, green: 0.93, blue: 0.82))
    }
    
    private func setupTCPClient() {
        tcpClient.onDataReceived = { data in
            packetProcessor.processPacket(data: data)
        }
    }
    
    private func connectToDevice() {
        let parts = address.split(separator: ":")
        
        guard parts.count == 2,
              let host = parts.first,
              let portString = parts.last,
              let port = UInt16(portString) else {
            DispatchQueue.main.async {
                tcpClient.packetDataLog.append("ERROR: Invalid address format. Use IP:Port (e.g., 192.168.1.183:1234)")
            }
            return
        }
        
        tcpClient.connect(host: String(host), port: port)
    }
    
    private func clearAll() {
        tcpClient.clearLogs()
        packetProcessor.clearAll()
    }
    
    private func toggleLogging() {
        if appState.isLoggingToFile {
            stopLogging()
        } else {
            startLogging()
        }
    }
    
    private func startLogging() {
        let savePanel = NSSavePanel()
        savePanel.allowedContentTypes = [.plainText]
        savePanel.nameFieldStringValue = "logfile.txt"
        savePanel.title = "Capture Traffic"
        
        savePanel.begin { response in
            if response == .OK, let url = savePanel.url {
                do {
                    if !FileManager.default.fileExists(atPath: url.path) {
                        FileManager.default.createFile(atPath: url.path, contents: nil)
                    }
                    logFileHandle = try FileHandle(forWritingTo: url)
                    logFileHandle?.seekToEndOfFile()
                    logFileURL = url
                    appState.isLoggingToFile = true
                    
                    // Start logging new packets
                    tcpClient.onPacketReceived = { packet in
                        logPacket(packet)
                    }
                } catch {
                    print("Error opening log file: \(error)")
                }
            }
        }
    }
    
    private func stopLogging() {
        logFileHandle?.closeFile()
        logFileHandle = nil
        logFileURL = nil
        appState.isLoggingToFile = false
        tcpClient.onPacketReceived = nil
    }
    
    private func logPacket(_ packet: String) {
        guard let handle = logFileHandle else { return }
        let timestamp = DateFormatter.localizedString(from: Date(), dateStyle: .short, timeStyle: .medium)
        let logEntry = "\(timestamp) \(packet)\n"
        if let data = logEntry.data(using: .utf8) {
            handle.write(data)
        }
    }
    
    private func togglePolling() {
        appState.isPolling.toggle()
        
        if appState.isPolling {
            pollTimer = Timer.scheduledTimer(withTimeInterval: 2.0, repeats: true) { _ in
                tcpClient.sendPacket("01 1F 40 0256")
            }
        } else {
            pollTimer?.invalidate()
            pollTimer = nil
        }
    }
    
    private func setCurrentTime() {
        let now = Date()
        let calendar = Calendar.current
        let components = calendar.dateComponents([.year, .month, .day, .hour, .minute, .second], from: now)
        
        guard let year = components.year,
              let month = components.month,
              let day = components.day,
              let hour = components.hour,
              let minute = components.minute,
              let second = components.second else {
            return
        }
        
        // Format: ss mm hh dd MM-1 YYYY_low YYYY_high
        let bytes = [
            UInt8(second),
            UInt8(minute),
            UInt8(hour),
            UInt8(day),
            UInt8(month),
            UInt8(year & 0xFF),
            UInt8(year >> 8)
        ]
        
        let hexString = bytes.map { String(format: "%02X", $0) }.joined()
        tcpClient.sendPacket("01 1F 40 024C \(hexString)")
    }
    
    private func annotateLog() {
        guard let handle = logFileHandle else { return }
        
        let alert = NSAlert()
        alert.messageText = "Annotate Log"
        alert.informativeText = "Enter annotation text:"
        alert.alertStyle = .informational
        alert.addButton(withTitle: "OK")
        alert.addButton(withTitle: "Cancel")
        
        let inputTextField = NSTextField(frame: NSRect(x: 0, y: 0, width: 300, height: 24))
        inputTextField.placeholderString = "Annotation text"
        alert.accessoryView = inputTextField
        
        alert.window.initialFirstResponder = inputTextField
        
        let response = alert.runModal()
        
        if response == .alertFirstButtonReturn {
            let annotation = inputTextField.stringValue
            if !annotation.isEmpty {
                let timestamp = DateFormatter.localizedString(from: Date(), dateStyle: .short, timeStyle: .medium)
                let logEntry = "\(timestamp) *** \(annotation) ***\n"
                if let data = logEntry.data(using: .utf8) {
                    handle.write(data)
                }
            }
        }
    }
}

#Preview {
    ContentView()
        .frame(width: 900, height: 700)
}
