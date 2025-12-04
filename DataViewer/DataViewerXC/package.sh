#!/bin/bash
set -e

echo "=========================================="
echo "DataViewer Packaging Script"
echo "=========================================="
echo ""

# Configuration
PROJECT_DIR="/Users/default/Desktop/ESP-IQ2020/DataViewer/DataViewerXC"
PROJECT_NAME="DataViewer"
SCHEME_NAME="DataViewer"
OUTPUT_DIR="$PROJECT_DIR/releases"
VERSION="1.0"

cd "$PROJECT_DIR"

# Clean output directory
echo "📁 Preparing output directory..."
rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"

# Build for Apple Silicon (arm64)
echo ""
echo "🔨 Building for Apple Silicon (arm64)..."
xcodebuild -project "$PROJECT_NAME.xcodeproj" \
    -scheme "$SCHEME_NAME" \
    -configuration Release \
    -arch arm64 \
    clean build \
    ONLY_ACTIVE_ARCH=NO \
    CODE_SIGN_IDENTITY="-" \
    CODE_SIGN_STYLE=Manual \
    | grep -E "(BUILD|error|warning)" || true

ARM64_APP="/Users/default/Library/Developer/Xcode/DerivedData/$PROJECT_NAME-*/Build/Products/Release/$PROJECT_NAME.app"

# Build for Intel (x86_64)
echo ""
echo "🔨 Building for Intel (x86_64)..."
xcodebuild -project "$PROJECT_NAME.xcodeproj" \
    -scheme "$SCHEME_NAME" \
    -configuration Release \
    -arch x86_64 \
    clean build \
    ONLY_ACTIVE_ARCH=NO \
    CODE_SIGN_IDENTITY="-" \
    CODE_SIGN_STYLE=Manual \
    | grep -E "(BUILD|error|warning)" || true

X64_APP="/Users/default/Library/Developer/Xcode/DerivedData/$PROJECT_NAME-*/Build/Products/Release/$PROJECT_NAME.app"

# Find the actual build output
echo ""
echo "📦 Locating build artifacts..."
ARM64_APP_FOUND=$(find /Users/default/Library/Developer/Xcode/DerivedData -name "$PROJECT_NAME.app" -path "*/Release/*" 2>/dev/null | tail -1)
X64_APP_FOUND="$ARM64_APP_FOUND"

if [ -z "$ARM64_APP_FOUND" ]; then
    echo "❌ Error: Could not find built application"
    exit 1
fi

echo "✅ Found: $ARM64_APP_FOUND"

# Create packages
echo ""
echo "📦 Creating distribution packages..."

# Copy app to output directory
cp -R "$ARM64_APP_FOUND" "$OUTPUT_DIR/$PROJECT_NAME.app"

# Sign the application properly
echo ""
echo "🔐 Signing application..."
# Remove any existing signature
codesign --remove-signature "$OUTPUT_DIR/$PROJECT_NAME.app" 2>/dev/null || true

# Sign with proper flags for macOS compatibility
# --deep signs all nested components
# --force replaces existing signatures
# --options runtime enables hardened runtime
codesign --deep --force --sign - --options runtime "$OUTPUT_DIR/$PROJECT_NAME.app"

# Verify the signature
echo "✅ Verifying signature..."
codesign --verify --verbose "$OUTPUT_DIR/$PROJECT_NAME.app"
if [ $? -eq 0 ]; then
    echo "✅ Signature valid"
else
    echo "⚠️  Warning: Signature verification failed"
fi

# Create DMG-style folder structure
DMG_DIR="$OUTPUT_DIR/dmg-temp"
mkdir -p "$DMG_DIR"
cp -R "$OUTPUT_DIR/$PROJECT_NAME.app" "$DMG_DIR/"

# Create README
cat > "$DMG_DIR/README.txt" << 'EOF'
DataViewer - ESP32 Data Viewer
================================

Installation:
1. Drag DataViewer.app to your Applications folder
2. Double-click to run

Requirements:
- macOS 13.0 or later
- Network access to ESP32 device

Usage:
1. Enter ESP32 IP address and port (e.g., 192.168.1.183:1234)
2. Click "Connect" to establish connection
3. Use menu commands to interact with device

Features:
- Real-time packet monitoring
- Data logging with annotations
- Multiple filter presets
- Packet decoding and analysis

For support, visit: https://github.com/Ylianst/ESP-IQ2020
EOF

# Create zip archive (most compact)
echo "📦 Creating ZIP archive..."
cd "$DMG_DIR"
zip -r -9 -q "$OUTPUT_DIR/$PROJECT_NAME-v$VERSION.zip" .
cd "$PROJECT_DIR"

# Get sizes
APP_SIZE=$(du -sh "$OUTPUT_DIR/$PROJECT_NAME.app" | cut -f1)
ZIP_SIZE=$(du -sh "$OUTPUT_DIR/$PROJECT_NAME-v$VERSION.zip" | cut -f1)

# Create installation script
cat > "$OUTPUT_DIR/install.sh" << 'EOF'
#!/bin/bash
echo "Installing DataViewer..."
if [ -d "/Applications/DataViewer.app" ]; then
    echo "Removing existing installation..."
    rm -rf "/Applications/DataViewer.app"
fi
cp -R "DataViewer.app" "/Applications/"
echo "✅ DataViewer installed to /Applications"
echo "You can now run it from Launchpad or Applications folder"
EOF

chmod +x "$OUTPUT_DIR/install.sh"

# Clean up temp directory
rm -rf "$DMG_DIR"

echo ""
echo "=========================================="
echo "✅ Packaging Complete!"
echo "=========================================="
echo ""
echo "📊 Package Information:"
echo "  • Application size: $APP_SIZE"
echo "  • ZIP archive size: $ZIP_SIZE"
echo "  • Version: $VERSION"
echo ""
echo "📦 Output files:"
echo "  • $OUTPUT_DIR/$PROJECT_NAME.app (standalone)"
echo "  • $OUTPUT_DIR/$PROJECT_NAME-v$VERSION.zip (compact)"
echo "  • $OUTPUT_DIR/install.sh (installation helper)"
echo ""
echo "� To install:"
echo "  1. Unzip: unzip $PROJECT_NAME-v$VERSION.zip"
echo "  2. Run: ./install.sh"
echo "  Or drag $PROJECT_NAME.app to Applications folder"
echo ""
echo "📤 To distribute:"
echo "  Share the ZIP file ($ZIP_SIZE)"
echo ""
