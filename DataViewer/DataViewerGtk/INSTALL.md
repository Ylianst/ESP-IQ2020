# IQ2020 Data Viewer - Installation Guide

## Overview

IQ2020 Data Viewer is a cross-platform GTK# application for monitoring and controlling IQ2020-compatible hot tub systems. This is a port of the original Windows Forms application to GTK#, providing native support for macOS and Linux.

## System Requirements

### macOS
- macOS 10.15 (Catalina) or later
- GTK3 runtime libraries

### Linux
- GTK3 libraries (usually pre-installed)
- X11 or Wayland display server

## Quick Start

### macOS Installation

#### Prerequisites
Install GTK3 using Homebrew:
```bash
brew install gtk+3
```

#### Installation Steps

1. **Download the appropriate package:**
   - For Apple Silicon (M1/M2/M3): `DataViewerGtk-osx-arm64.zip`
   - For Intel Macs: `DataViewerGtk-osx-x64.zip`

2. **Extract the archive:**
   ```bash
   unzip DataViewerGtk-osx-arm64.zip
   ```

3. **Run the application:**
   ```bash
   open DataViewerGtk.app
   ```
   
   Or double-click `DataViewerGtk.app` in Finder.

**Note for macOS users:** If you get a security warning about the app being from an unidentified developer:
1. Go to System Preferences → Security & Privacy
2. Click "Open Anyway" for DataViewerGtk
3. Or run: `xattr -cr DataViewerGtk.app` to remove quarantine attributes

### Linux Installation

#### Prerequisites
Install GTK3 (if not already installed):

**Ubuntu/Debian:**
```bash
sudo apt-get update
sudo apt-get install libgtk-3-0
```

**Fedora/RHEL:**
```bash
sudo dnf install gtk3
```

**Arch Linux:**
```bash
sudo pacman -S gtk3
```

#### Installation Steps

1. **Download the package:**
   - `DataViewerGtk-linux-x64.tar.gz`

2. **Extract the archive:**
   ```bash
   tar -xzf DataViewerGtk-linux-x64.tar.gz
   ```

3. **Make the application executable:**
   ```bash
   chmod +x DataViewerGtk
   ```

4. **Run the application:**
   ```bash
   ./DataViewerGtk
   ```

## Building from Source

If you want to build the application from source:

### Prerequisites
- .NET 8.0 SDK or later
- GTK3 development libraries

### Build Instructions

```bash
cd DataViewerGtk
dotnet restore
dotnet build -c Release
```

### Create Self-Contained Package

**macOS ARM64:**
```bash
dotnet publish -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true
```

**macOS x64:**
```bash
dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true
```

**Linux x64:**
```bash
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
```

The compiled binaries will be in `bin/Release/net10.0/[runtime]/publish/`

## Usage

1. **Launch the application**
2. **Enter your device's IP address and port** in the format `192.168.1.100:1234`
3. **Click Connect** to establish a connection
4. **Use the menu commands** to interact with your hot tub system

### Main Features

- **File Menu:**
  - Log to File - Record all communications to a log file
  - Annotate Log - Add custom notes to the log
  - Clear - Clear all display data

- **Actions Menu:**
  - Poll State - Automatically query device status every 2 seconds
  - Scan - Scan through all possible commands
  - FIQ Test 1/2 - Test FreshWater IQ modules

- **Filter Menu:**
  - Filter displayed packets by module type

- **Commands Menu:**
  - Query device status
  - Control lights
  - Set time
  - Request version information

## Configuration

The application saves your last used IP address in:
- **macOS/Linux:** `~/.iq2020dataviewer`

## Troubleshooting

### Application won't start on macOS
- Ensure GTK3 is installed: `brew list | grep gtk+3`
- Check security settings in System Preferences
- Try removing quarantine: `xattr -cr DataViewerGtk.app`

### Application won't start on Linux
- Verify GTK3 is installed: `pkg-config --modversion gtk+-3.0`
- Check that the executable has execute permissions: `ls -l DataViewerGtk`
- Run from terminal to see any error messages

### Cannot connect to device
- Verify the IP address and port are correct
- Ensure your computer is on the same network as the device
- Check firewall settings
- Try pinging the device: `ping [device-ip]`

### Display issues
- Ensure your system has proper display server (X11 or Wayland on Linux)
- Try different GTK themes if text appears unreadable

## Support

For issues, questions, or contributions, please visit:
https://github.com/Ylianst/ESP-IQ2020

## License

This software is provided as-is. Please refer to the main project repository for license information.

## Credits

- Original Windows Forms application by Ylian Saint-Hilaire
- GTK# port completed November 2025
