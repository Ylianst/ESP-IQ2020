//
//  DecodedData.swift
//  DataViewerXC
//
//  Data models and helper structures
//

import Foundation

struct DecodedDataItem: Identifiable {
    let id = UUID()
    let name: String
    let value: String
}

struct PacketCommand {
    let name: String
    let command: String
}

class CommandPresets {
    static let presets: [PacketCommand] = [
        PacketCommand(name: "Ask Lights Status", command: "01 1F 40 1705"),
        PacketCommand(name: "Ask Status 0x0255", command: "01 1F 40 0255"),
        PacketCommand(name: "Ask Status 0x0256", command: "01 1F 40 0256"),
        PacketCommand(name: "Ask Version", command: "01 1F 40 0100"),
        PacketCommand(name: "Lights On", command: "01 1F 40 1702041100"),
        PacketCommand(name: "Lights Off", command: "01 1F 40 1702041000"),
        PacketCommand(name: "Set Current Time", command: generateSetTimeCommand())
    ]
    
    static func generateSetTimeCommand() -> String {
        let now = Date()
        let calendar = Calendar.current
        let components = calendar.dateComponents([.second, .minute, .hour, .day, .month, .year], from: now)
        
        let second = UInt8(components.second ?? 0)
        let minute = UInt8(components.minute ?? 0)
        let hour = UInt8(components.hour ?? 0)
        let day = UInt8(components.day ?? 1)
        let month = UInt8((components.month ?? 1) - 1)
        let year = components.year ?? 2025
        let yearLow = UInt8(year & 0xFF)
        let yearHigh = UInt8((year >> 8) & 0xFF)
        
        return String(format: "01 1F 40 024C %02X%02X%02X%02X%02X%02X%02X",
                     second, minute, hour, day, month, yearLow, yearHigh)
    }
}

struct FilterOption: Identifiable {
    let id = UUID()
    let name: String
}

class FilterPresets {
    static let filters: [FilterOption] = [
        FilterOption(name: "None"),
        FilterOption(name: "Connection Kit"),
        FilterOption(name: "Connection Kit 0x0256"),
        FilterOption(name: "Freshwater Module"),
        FilterOption(name: "Freshwater IQ Module"),
        FilterOption(name: "Audio Module"),
        FilterOption(name: "ACE Module")
    ]
}
