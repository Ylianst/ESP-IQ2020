#!/bin/bash

# Script to create a macOS .app bundle for DataViewerOSX

ARCH=$1
if [ -z "$ARCH" ]; then
    echo "Usage: $0 <osx-arm64|osx-x64>"
    exit 1
fi

if [ "$ARCH" != "osx-arm64" ] && [ "$ARCH" != "osx-x64" ]; then
    echo "Error: Architecture must be osx-arm64 or osx-x64"
    exit 1
fi

APP_NAME="IQ2020 Data Viewer"
BUNDLE_NAME="IQ2020DataViewer.app"
PUBLISH_DIR="bin/Release/net8.0/$ARCH/publish"
BUNDLE_ID="com.opensource.iq2020dataviewer"
VERSION="1.0.0"

echo "Creating macOS app bundle for $ARCH..."

# Check if publish directory exists
if [ ! -d "$PUBLISH_DIR" ]; then
    echo "Error: Publish directory not found. Run build.sh first."
    exit 1
fi

# Create app bundle structure
echo "Creating bundle structure..."
rm -rf "$BUNDLE_NAME"
mkdir -p "$BUNDLE_NAME/Contents/MacOS"
mkdir -p "$BUNDLE_NAME/Contents/Resources"

# Copy executable
echo "Copying executable..."
cp "$PUBLISH_DIR/DataViewerOSX" "$BUNDLE_NAME/Contents/MacOS/"
chmod +x "$BUNDLE_NAME/Contents/MacOS/DataViewerOSX"

# Create Info.plist
echo "Creating Info.plist..."
cat > "$BUNDLE_NAME/Contents/Info.plist" << EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleName</key>
    <string>$APP_NAME</string>
    <key>CFBundleDisplayName</key>
    <string>$APP_NAME</string>
    <key>CFBundleIdentifier</key>
    <string>$BUNDLE_ID</string>
    <key>CFBundleVersion</key>
    <string>$VERSION</string>
    <key>CFBundleShortVersionString</key>
    <string>$VERSION</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleExecutable</key>
    <string>DataViewerOSX</string>
    <key>CFBundleIconFile</key>
    <string>AppIcon</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.15</string>
    <key>NSHighResolutionCapable</key>
    <true/>
    <key>NSPrincipalClass</key>
    <string>NSApplication</string>
    <key>LSApplicationCategoryType</key>
    <string>public.app-category.utilities</string>
</dict>
</plist>
EOF

echo "✓ Created $BUNDLE_NAME"
echo ""
echo "To run the application:"
echo "  open $BUNDLE_NAME"
echo ""
echo "To distribute, create a ZIP file:"
echo "  zip -r IQ2020DataViewer-$ARCH.zip \"$BUNDLE_NAME\""
