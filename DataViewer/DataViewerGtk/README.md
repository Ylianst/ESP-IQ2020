# IQ2020 Data Viewer - GTK# Port

A cross-platform GTK# port of the IQ2020 Data Viewer application for monitoring and controlling IQ2020-compatible hot tub systems.

## Features

This GTK# port includes **complete feature parity** with the original Windows Forms application:

### Connection & Communication
- TCP client connection to IQ2020 devices
- Real-time packet display and logging
- Command sending capabilities
- Automatic packet decoding
- Network scanning functionality

### Data Views
- **Data Packets**: Decoded packet view with command interface
- **Raw Data**: Complete hex data stream
- **Packet Store**: Unique packet history
- **Decoded Data**: Key-value pairs of system information
- **State**: Detailed hex decode with annotations

### Control Features
- **File Menu:**
  - Log to File - Record all communications
  - Annotate Log - Add custom notes to logs
  - Clear - Clear all displays
  
- **Actions Menu:**
  - Poll State - Auto-query device every 2 seconds
  - Scan - Scan all command codes (0-65535)
  - FIQ Test 1/2 - FreshWater IQ module testing

- **Filter Menu:**
  - None, Connection Kit, Fresh Water, Freshwater IQ Module, Audio, ACE Module

- **Commands Menu:**
  - Ask Lights Status, Ask Status (0x0255/0x0256)
  - Lights On/Off
  - Ask Versions
  - Set Current Time

## Quick Start

### Prerequisites

**macOS:**
```bash
brew install gtk+3
```

**Linux (Ubuntu/Debian):**
```bash
sudo apt-get install libgtk-3-0
```

### Download Pre-Built Packages

See [INSTALL.md](INSTALL.md) for detailed installation instructions and download links for:
- macOS (Apple Silicon & Intel)
- Linux (x64)

### Building from Source

```bash
cd DataViewerGtk
dotnet restore
dotnet build -c Release
```

### Running

```bash
dotnet run
```

Or use the convenience script:
```bash
./run.sh
```

## Creating Release Packages

To create distribution packages for all platforms:

```bash
cd DataViewerGtk
./package.sh
```

This creates self-contained packages in the `releases/` directory:
- `DataViewerGtk-osx-arm64.zip` - macOS Apple Silicon
- `DataViewerGtk-osx-x64.zip` - macOS Intel
- `DataViewerGtk-linux-x64.tar.gz` - Linux x64

## Usage

1. **Launch the application**
2. **Enter device address** in format `192.168.1.100:1234`
3. **Click Connect** to establish connection
4. **Use menu commands** to interact with your system

Configuration is automatically saved to `~/.iq2020dataviewer`

## Development

### Requirements
- .NET 8.0 SDK or later
- GTK3 development libraries
- GtkSharp 3.x NuGet packages

### Project Structure
- `Program.cs` - Application entry point
- `MainWindow.cs` - Main UI and all application logic
- `DataViewerGtk.csproj` - Project configuration

### Publishing

Create platform-specific packages:

```bash
# macOS ARM64
dotnet publish -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true

# macOS x64
dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true

# Linux x64
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
```

## Differences from Windows Version

- Uses GTK3 instead of Windows Forms
- Configuration saved to `~/.iq2020dataviewer` instead of Windows Registry
- Native file dialogs for each platform
- Cross-platform compatible (Linux, macOS, BSD)
- Single executable with self-contained .NET runtime

## Troubleshooting

### Connection Issues
- Verify device IP address and port
- Check network connectivity
- Ensure firewall allows the connection
- Try pinging the device first

### Display Issues
- Ensure GTK3 is properly installed
- Check that display server is running (X11/Wayland on Linux)
- Try running from terminal to see error messages

### Build Issues
- Verify .NET SDK version: `dotnet --version` (should be 8.0+)
- Confirm GTK3 installation: `pkg-config --modversion gtk+-3.0`
- Clear build cache: `dotnet clean && dotnet restore`

See [INSTALL.md](INSTALL.md) for complete troubleshooting guide.

## License

This software is provided as-is under the same license as the original ESP-IQ2020 project.

## Credits

- **Original Windows Forms Application**: Ylian Saint-Hilaire
- **GTK# Port**: Completed November 2025
- **Project Repository**: https://github.com/Ylianst/ESP-IQ2020

## Support

For issues, questions, or contributions:
- GitHub Issues: https://github.com/Ylianst/ESP-IQ2020/issues
- Original Project: https://github.com/Ylianst/ESP-IQ2020
