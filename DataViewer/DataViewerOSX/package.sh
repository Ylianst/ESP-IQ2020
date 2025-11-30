#!/bin/bash

# Package DataViewerOSX for distribution
# This script creates a universal .app bundle that can run on both Intel and Apple Silicon Macs

set -e

echo "=========================================="
echo "DataViewerOSX Packaging Script"
echo "=========================================="
echo ""

# Clean previous builds
#echo "Cleaning previous builds..."
#rm -rf bin/Release
#rm -rf releases
#mkdir -p releases

# Build for both architectures
#echo ""
#echo "Building for macOS (ARM64)..."
#dotnet publish -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=false -p:PublishTrimmed=true
#dotnet publish -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false

#echo ""
#echo "Building for macOS (x64)..."
#dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=false -p:PublishTrimmed=true
#dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false

# Copy the built .app bundles
echo ""
echo "Copying ARM64 .app bundle..."
APP_NAME="IQ2020 Data Viewer"
ARM64_APP="releases/${APP_NAME}-arm64.app"
X64_APP="releases/${APP_NAME}-x64.app"

# Copy ARM64 bundle (already created by build system)
cp -r bin/Release/net10.0-macos/osx-arm64/DataViewerOSX.app "${ARM64_APP}"

# Copy x64 bundle
echo ""
echo "Copying x64 .app bundle..."
cp -r bin/Release/net10.0-macos/osx-x64/DataViewerOSX.app "${X64_APP}"

# Remove quarantine attributes that may have been added during copy
echo ""
echo "Removing quarantine attributes..."
xattr -cr "${ARM64_APP}"
xattr -cr "${X64_APP}"

# Ad-hoc code sign the bundles so they can run on other machines
echo ""
echo "Code signing bundles..."
codesign --force --deep --sign - "${ARM64_APP}"
codesign --force --deep --sign - "${X64_APP}"

# Create ZIP archives for distribution
echo ""
echo "Creating distribution archives..."
cd releases
zip -r "${APP_NAME}-arm64.zip" "${APP_NAME}-arm64.app"
zip -r "${APP_NAME}-x64.zip" "${APP_NAME}-x64.app"
cd ..

# Create README for releases
cat > releases/README.md << 'EOF'
# IQ2020 Data Viewer for macOS

Native macOS application for viewing and interacting with IQ2020 spa controller data.

## Installation

1. Download the appropriate version for your Mac:
   - **Apple Silicon (M1, M2, M3, etc.)**: Download `IQ2020 Data Viewer-arm64.zip`
   - **Intel Mac**: Download `IQ2020 Data Viewer-x64.zip`

2. Unzip the downloaded file

3. Drag `IQ2020 Data Viewer.app` to your Applications folder

4. **First Launch**: Right-click the app and select "Open" (required for first launch due to Gatekeeper)

## Features

- **Native macOS UI** - Built with AppKit for optimal performance
- **Dark Mode Support** - Automatically adapts to system appearance
- **TCP Networking** - Connect to IQ2020 spa controllers
- **Real-time Data Display** - View packets across 5 different tabs
- **File Logging** - Save captured data with annotations
- **Comprehensive Menus** - File, Actions, Filter, and Commands

## System Requirements

- macOS 12.0 (Monterey) or later
- Either Apple Silicon or Intel processor

## Usage

1. Launch the application
2. Enter the IP address and port of your IQ2020 controller (format: `192.168.x.x:1234`)
3. Click "Connect"
4. View real-time data in the various tabs
5. Use menu commands to interact with the controller

## Support

For issues or questions, please visit the project repository.

## Version

1.0.0 - Initial Release
EOF

echo ""
echo "=========================================="
echo "Packaging Complete!"
echo "=========================================="
echo ""
echo "Distribution files created in releases/:"
echo "  - IQ2020 Data Viewer-arm64.app (Apple Silicon)"
echo "  - IQ2020 Data Viewer-x64.app (Intel)"
echo "  - IQ2020 Data Viewer-arm64.zip (Apple Silicon - for distribution)"
echo "  - IQ2020 Data Viewer-x64.zip (Intel - for distribution)"
echo "  - README.md (Installation instructions)"
echo ""
echo "To distribute, share the appropriate .zip file for the target architecture."
echo ""
