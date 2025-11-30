# IQ2020 Data Viewer - macOS Native

A fully native macOS application for viewing and analyzing IQ2020 spa controller data, built using AppKit (Cocoa).

## Features

- **Native macOS UI**: Built with AppKit for true macOS look and feel
- **TCP Connection**: Connect to IQ2020 spa controllers over network
- **Data Visualization**: Multiple tabs for different data views
  - Data Packets: View formatted packet data
  - Raw Data: See raw hex data stream
  - Packet Store: History of received packets
  - Decoded Data: Human-readable decoded values
  - State: Detailed state information
- **Packet Sending**: Send custom packets to the controller
- **Filtering**: Filter packets by module type
- **Logging**: Save session data to file with annotations
- **Commands**: Pre-configured commands for common operations
- **Scanning**: Automated command scanning functionality

## Requirements

- macOS 10.15 (Catalina) or later
- .NET 8.0 or later

## Building from Source

### Prerequisites

```bash
# Install .NET 8 SDK
brew install dotnet-sdk
```

### Build

```bash
cd DataViewerOSX
dotnet build -c Release
```

### Run

```bash
dotnet run
```

### Create Distributable Package

For Apple Silicon (ARM64):
```bash
dotnet publish -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

For Intel (x64):
```bash
dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

## Usage

1. Launch the application
2. Enter the IP address and port of your IQ2020 controller (e.g., `192.168.2.184:1234`)
3. Click "Connect"
4. Use the tabs to view different data representations
5. Use the Commands menu for common operations
6. Use File > Log to File to start capturing data

## Architecture

This is a pure native macOS application that uses:
- **AppKit/Cocoa**: Native macOS UI framework
- **NSWindow**: Native window management
- **NSTableView**: Native table controls
- **NSTextView**: Native text display with monospaced font
- **NSComboBox**: Native combo box controls
- **NSMenu**: Native menu system
- **NSTimer**: Native timer for periodic operations

All UI elements follow macOS Human Interface Guidelines and integrate seamlessly with the operating system.

## Differences from GTK# Version

- Uses native AppKit instead of GTK#
- Better macOS integration and appearance
- Native macOS menus and dialogs
- No external dependencies (GTK libraries not required)
- More efficient on macOS systems

## License

Open Source - See main repository for license details

## Project Structure

```
DataViewerOSX/
├── Program.cs                 # Application entry point
├── MainWindowController.cs    # Main window and application logic
├── DataViewerOSX.csproj      # Project configuration
└── README.md                  # This file
