#!/bin/bash
# Package script for IQ2020 Data Viewer
# Creates distribution packages for macOS and Linux

set -e

echo "=== IQ2020 Data Viewer Packaging Script ==="
echo ""

# Clean previous builds
echo "Cleaning previous builds..."
rm -rf bin/Release/net10.0/*/publish
rm -rf releases
mkdir -p releases

# Build for macOS ARM64
echo ""
echo "Building for macOS ARM64..."
dotnet publish -c Release -r osx-arm64 --self-contained true \
    -p:PublishSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -p:DebugType=None \
    -p:DebugSymbols=false

# Create .app bundle for macOS ARM64
./create-macos-app.sh osx-arm64

# Package macOS ARM64
cd bin/Release/net10.0/osx-arm64/publish
zip -r ../../../../../releases/DataViewerGtk-osx-arm64.zip DataViewerGtk.app
cd ../../../../..
echo "✓ Created releases/DataViewerGtk-osx-arm64.zip"

# Build for macOS x64
echo ""
echo "Building for macOS x64..."
dotnet publish -c Release -r osx-x64 --self-contained true \
    -p:PublishSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -p:DebugType=None \
    -p:DebugSymbols=false

# Create .app bundle for macOS x64
./create-macos-app.sh osx-x64

# Package macOS x64
cd bin/Release/net10.0/osx-x64/publish
zip -r ../../../../../releases/DataViewerGtk-osx-x64.zip DataViewerGtk.app
cd ../../../../..
echo "✓ Created releases/DataViewerGtk-osx-x64.zip"

# Build for Linux x64
echo ""
echo "Building for Linux x64..."
dotnet publish -c Release -r linux-x64 --self-contained true \
    -p:PublishSingleFile=true \
    -p:DebugType=None \
    -p:DebugSymbols=false

# Package Linux x64
cd bin/Release/net10.0/linux-x64/publish
chmod +x DataViewerGtk
tar -czf ../../../../../releases/DataViewerGtk-linux-x64.tar.gz DataViewerGtk
cd ../../../../..
echo "✓ Created releases/DataViewerGtk-linux-x64.tar.gz"

# Copy documentation
echo ""
echo "Adding documentation..."
cp INSTALL.md releases/
cp README.md releases/

# Display results
echo ""
echo "=== Packaging Complete ==="
echo ""
echo "Release packages created in releases/ directory:"
ls -lh releases/
echo ""
echo "Package sizes:"
du -h releases/DataViewerGtk-*
echo ""
echo "Installation instructions are in releases/INSTALL.md"
