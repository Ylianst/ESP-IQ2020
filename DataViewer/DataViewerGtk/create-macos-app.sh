#!/bin/bash
# Create macOS .app bundle for DataViewerGtk

set -e

RUNTIME=$1
if [ -z "$RUNTIME" ]; then
    echo "Usage: ./create-macos-app.sh [osx-arm64|osx-x64]"
    exit 1
fi

APP_NAME="DataViewerGtk"
BUNDLE_NAME="${APP_NAME}.app"
PUBLISH_DIR="bin/Release/net10.0/${RUNTIME}/publish"

echo "Creating ${BUNDLE_NAME} for ${RUNTIME}..."

# Create app bundle structure
mkdir -p "${PUBLISH_DIR}/${BUNDLE_NAME}/Contents/MacOS"
mkdir -p "${PUBLISH_DIR}/${BUNDLE_NAME}/Contents/Resources"

# Move executable into bundle
mv "${PUBLISH_DIR}/${APP_NAME}" "${PUBLISH_DIR}/${BUNDLE_NAME}/Contents/MacOS/"

# Create Info.plist
cat > "${PUBLISH_DIR}/${BUNDLE_NAME}/Contents/Info.plist" << 'EOF'
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleDevelopmentRegion</key>
    <string>en</string>
    <key>CFBundleExecutable</key>
    <string>DataViewerGtk</string>
    <key>CFBundleIdentifier</key>
    <string>com.opensource.dataviewergtk</string>
    <key>CFBundleInfoDictionaryVersion</key>
    <string>6.0</string>
    <key>CFBundleName</key>
    <string>DataViewerGtk</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleShortVersionString</key>
    <string>1.0.0</string>
    <key>CFBundleVersion</key>
    <string>1</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.15</string>
    <key>NSHighResolutionCapable</key>
    <true/>
    <key>LSUIElement</key>
    <false/>
</dict>
</plist>
EOF

echo "✓ Created ${BUNDLE_NAME}"
