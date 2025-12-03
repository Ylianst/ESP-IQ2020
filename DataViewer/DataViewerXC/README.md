# DataViewerXC

**IQ2020 Data Viewer - Swift/Xcode Port**

A native macOS application for connecting to ESP32 devices via TCP and viewing/analyzing protocol data. This is a complete Swift port of the original C# DataViewer application.

## Overview

DataViewerXC connects to ESP32-based IQ2020 devices over TCP/IP, allowing you to:
- Monitor raw TCP data and decoded packets in real-time
- Send custom commands to the device
- Decode and display device status (temperature, lights, power readings, etc.)
- Filter packets by module type
- Log traffic to file
- Store and review unique packets

## Features

### Five Tab Interface

1. **Data Packets** - Decoded packet display with command input
2. **Raw Data** - Raw hex data stream with raw command input
3. **Packet Store** - Collection of unique packets seen during session
4. **Decoded Data** - Parsed device status information (temperature, voltage, current, power, runtime counters, etc.)
5. **State** - Detailed hex dump with field annotations

### Functionality

- **TCP Connection** - Connect/disconnect to ESP32 devices via IP:Port
- **Packet Processing** - Automatic packet validation with checksum verification
- **Data Decoding** - Automatic decoding of device responses:
  - Temperature readings (set point and current)
  - Lights status
  - Power metrics (voltage, current, power for L1, L2, heater)
  - Runtime counters for pumps, heater, lights
  - Device time and date
  - Version information
- **Filtering** - Filter by module type:
  - Connection Kit
  - Connection Kit 0x0256
  - Freshwater Module
  - Freshwater IQ Module
  - Audio Module
  - ACE Module
- **Commands Menu** - Quick access to common commands:
  - Ask Lights Status
  - Ask Status 0x0255/0x0256
  - Ask Version
  - Lights On/Off
  - Set Current Time
- **Logging** - Log packets to file with timestamps
- **Polling** - Automatic status polling every 2 seconds

## Requirements

- macOS 13.0 or later
- Xcode 15.0 or later
- Swift 5.9 or later

## Building

1. Open `DataViewerXC.xcodeproj` in Xcode
2. Select your target architecture (Apple Silicon or Intel)
3. Build and run (⌘R)

## Usage

1. **Connect to Device**
   - Enter the device IP address and port (e.g., `192.168.1.100:1234`)
   - Click "Connect"
   - Status bar will show "Connected" when ready

2. **Send Commands**
   - Use the command field at the bottom of Data Packets or Raw Data tabs
   - Or use the Commands menu for quick access to common commands
   - Format: Space-separated hex bytes (e.g., `01 1F 40 0256`)

3. **View Data**
   - Switch between tabs to see different views of the data
   - Decoded Data tab shows parsed device information
   - State tab shows detailed field-by-field breakdown

4. **Filter Packets**
   - Use the Filter menu to show only specific module traffic
   - Useful for focusing on particular subsystems

5. **Log Traffic**
   - File menu → "Log to File..." to start logging
   - All packets will be saved with timestamps
   - "Stop Logging" to close the log file

## Protocol Details

The application uses a custom binary protocol:

### Packet Structure
```
[0x1C][DST][SRC][LEN][OP][DATA...][CHECKSUM]
```

- `0x1C` - Sync byte
- `DST` - Destination address
- `SRC` - Source address
- `LEN` - Data length
- `OP` - Operation (0x40 = command, 0x80 = response)
- `DATA` - Variable length payload
- `CHECKSUM` - XOR checksum of all bytes except sync

### Common Addresses
- `0x01` - Controller/Master
- `0x1F` - Connection Kit
- `0x29` - Freshwater Module
- `0x33` - Audio Module
- `0x37` - Freshwater IQ Module
- `0x24` - ACE Module

## Architecture

The Swift port uses modern SwiftUI patterns:

- **TCPClient.swift** - TCP networking using NNetwork framework
- **PacketProcessor.swift** - Binary packet parsing and decoding logic
- **DecodedData.swift** - Data models and command presets
- **ContentView.swift** - SwiftUI interface with five tabs
- **DataViewerXCApp.swift** - App entry point and menu commands

## Differences from C# Version

- Native macOS UI using SwiftUI instead of Windows Forms
- Uses Apple's Network framework instead of System.Net.Sockets
- Persistent address storage using UserDefaults instead of Registry
- Modern Swift async patterns for TCP communication
- Menu-driven interface integrated with macOS menu bar

## Known Issues

- App sandbox requires network client entitlement
- File logging requires user-selected file access

## License

This is an open source port of the ESP-IQ2020 DataViewer application.

## Credits

Original C# application: ESP-IQ2020 DataViewer
Swift port: DataViewerXC

---

For issues or contributions, please refer to the original ESP-IQ2020 project repository.
