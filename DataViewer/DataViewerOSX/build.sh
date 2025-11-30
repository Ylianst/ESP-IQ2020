#!/bin/bash

echo "=== Building IQ2020 Data Viewer (macOS Native) ==="
echo ""

# Clean previous builds
echo "Cleaning previous builds..."
rm -rf bin obj

# Build for Apple Silicon (ARM64)
echo ""
echo "Building for Apple Silicon (ARM64)..."
dotnet publish -c Release -r osx-arm64 --self-contained true

if [ $? -eq 0 ]; then
    echo "✓ ARM64 build successful"
    echo "  Output: bin/Release/net10.0-macos/osx-arm64/publish/DataViewerOSX"
else
    echo "✗ ARM64 build failed"
    exit 1
fi

# Build for Intel (x64)
echo ""
echo "Building for Intel (x64)..."
dotnet publish -c Release -r osx-x64 --self-contained true

if [ $? -eq 0 ]; then
    echo "✓ x64 build successful"
    echo "  Output: bin/Release/net10.0-macos/osx-x64/publish/DataViewerOSX"
else
    echo "✗ x64 build failed"
    exit 1
fi

echo ""
echo "=== Build Complete ==="
echo ""
echo "To run the application:"
echo "  Apple Silicon: ./bin/Release/net10.0-macos/osx-arm64/publish/DataViewerOSX"
echo "  Intel:         ./bin/Release/net10.0-macos/osx-x64/publish/DataViewerOSX"
