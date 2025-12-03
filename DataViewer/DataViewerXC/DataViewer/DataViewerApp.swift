//
//  DataViewerXCApp.swift
//  DataViewerXC
//
//  IQ2020 Data Viewer - Swift Port
//

import SwiftUI

@main
struct DataViewerApp: App {
    @StateObject private var appState = AppState()
    
    var body: some Scene {
        WindowGroup {
            ContentView()
                .environmentObject(appState)
        }
        .windowStyle(.hiddenTitleBar)
        .windowToolbarStyle(.unified)
        .commands {
            CommandGroup(replacing: .newItem) {
                // Remove default "New" command
            }
            
            CommandMenu("Capture") {
                Button(action: {
                    NotificationCenter.default.post(name: .toggleLogging, object: nil)
                }) {
                    Text(appState.isLoggingToFile ? "Stop Logging" : "Start Logging...")
                }
                
                Button("Annotate Log...") {
                    NotificationCenter.default.post(name: .annotateLog, object: nil)
                }
                .disabled(!appState.isLoggingToFile)
                
                Divider()
                
                Button("Clear") {
                    NotificationCenter.default.post(name: .clearAll, object: nil)
                }
                .keyboardShortcut("k", modifiers: .command)
            }
            
            CommandMenu("Actions") {
                Button("Ask Lights Status") {
                    NotificationCenter.default.post(
                        name: .sendCommand,
                        object: nil,
                        userInfo: ["command": "01 1F 40 1705"]
                    )
                }
                .disabled(!appState.isConnected)
                
                Button("Ask Temp Status") {
                    NotificationCenter.default.post(
                        name: .sendCommand,
                        object: nil,
                        userInfo: ["command": "01 1F 40 0256"]
                    )
                }
                .disabled(!appState.isConnected)
                
                Button("Ask Status") {
                    NotificationCenter.default.post(
                        name: .sendCommand,
                        object: nil,
                        userInfo: ["command": "01 1F 40 0255"]
                    )
                }
                .disabled(!appState.isConnected)
                
                Button("Ask Versions") {
                    NotificationCenter.default.post(
                        name: .sendCommand,
                        object: nil,
                        userInfo: ["command": "01 1F 40 0100"]
                    )
                }
                .disabled(!appState.isConnected)
                
                Divider()
                
                Button("Lights On") {
                    NotificationCenter.default.post(
                        name: .sendCommand,
                        object: nil,
                        userInfo: ["command": "01 1F 40 1702041100"]
                    )
                }
                .disabled(!appState.isConnected)
                
                Button("Lights Off") {
                    NotificationCenter.default.post(
                        name: .sendCommand,
                        object: nil,
                        userInfo: ["command": "01 1F 40 1702041000"]
                    )
                }
                .disabled(!appState.isConnected)
                
                Button("Set Current Time") {
                    NotificationCenter.default.post(name: .setCurrentTime, object: nil)
                }
                .disabled(!appState.isConnected)
                
                Divider()
                
                Button(action: {
                    NotificationCenter.default.post(name: .togglePolling, object: nil)
                }) {
                    Text(appState.isPolling ? "Stop Polling" : "Poll State")
                }
                .disabled(!appState.isConnected)
            }
            
            CommandMenu("Filter") {
                ForEach(FilterPresets.filters) { filter in
                    Button(action: {
                        appState.selectedFilter = filter.name
                        NotificationCenter.default.post(
                            name: .setFilter,
                            object: nil,
                            userInfo: ["filter": filter.name]
                        )
                    }) {
                        if appState.selectedFilter == filter.name {
                            Label(filter.name, systemImage: "checkmark")
                        } else {
                            Text(filter.name)
                        }
                    }
                }
            }
            
            CommandMenu("Commands") {
                ForEach(CommandPresets.presets, id: \.name) { preset in
                    Button(preset.name) {
                        NotificationCenter.default.post(
                            name: .sendCommand,
                            object: nil,
                            userInfo: ["command": preset.command]
                        )
                    }
                    .disabled(!appState.isConnected)
                }
            }
        }
    }
}

class AppState: ObservableObject {
    @Published var isLoggingToFile = false
    @Published var isPolling = false
    @Published var isConnected = false
    @Published var selectedFilter = "None"
}

extension Notification.Name {
    static let clearAll = Notification.Name("clearAll")
    static let toggleLogging = Notification.Name("toggleLogging")
    static let togglePolling = Notification.Name("togglePolling")
    static let setFilter = Notification.Name("setFilter")
    static let sendCommand = Notification.Name("sendCommand")
    static let setCurrentTime = Notification.Name("setCurrentTime")
    static let annotateLog = Notification.Name("annotateLog")
}
