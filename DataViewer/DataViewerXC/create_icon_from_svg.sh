#!/bin/bash
# Icon Creation Script for DataViewer
# Converts SVG to macOS app icon set

SVG_FILE="$1"
ICONSET="AppIcon.appiconset"

if [ -z "$SVG_FILE" ]; then
    echo "Usage: ./create_icon_from_svg.sh input.svg"
    echo ""
    echo "Example:"
    echo "  ./create_icon_from_svg.sh icon_template.svg"
    exit 1
fi

if [ ! -f "$SVG_FILE" ]; then
    echo "Error: File $SVG_FILE not found"
    exit 1
fi

# Check for required tools
if ! command -v rsvg-convert &> /dev/null && ! command -v convert &> /dev/null; then
    echo "Error: Neither rsvg-convert nor ImageMagick found"
    echo "Install with: brew install librsvg imagemagick"
    exit 1
fi

# Create iconset directory
rm -rf "$ICONSET"
mkdir -p "$ICONSET"

# Required sizes for macOS app icons
sizes=(16 32 128 256 512)

echo "🎨 Converting SVG to PNG at multiple sizes..."
echo ""

for size in "${sizes[@]}"; do
    echo "  Creating ${size}x${size}..."
    
    # Standard resolution
    if command -v rsvg-convert &> /dev/null; then
        rsvg-convert -w $size -h $size "$SVG_FILE" -o "$ICONSET/icon_${size}x${size}.png"
    else
        convert -background none -resize ${size}x${size} "$SVG_FILE" "$ICONSET/icon_${size}x${size}.png"
    fi
    
    # Retina (@2x)
    size2x=$((size * 2))
    echo "  Creating ${size}x${size}@2x (${size2x}x${size2x})..."
    if command -v rsvg-convert &> /dev/null; then
        rsvg-convert -w $size2x -h $size2x "$SVG_FILE" -o "$ICONSET/icon_${size}x${size}@2x.png"
    else
        convert -background none -resize ${size2x}x${size2x} "$SVG_FILE" "$ICONSET/icon_${size}x${size}@2x.png"
    fi
done

# Create Contents.json
cat > "$ICONSET/Contents.json" << 'EOF'
{
  "images" : [
    { "size" : "16x16", "idiom" : "mac", "filename" : "icon_16x16.png", "scale" : "1x" },
    { "size" : "16x16", "idiom" : "mac", "filename" : "icon_16x16@2x.png", "scale" : "2x" },
    { "size" : "32x32", "idiom" : "mac", "filename" : "icon_32x32.png", "scale" : "1x" },
    { "size" : "32x32", "idiom" : "mac", "filename" : "icon_32x32@2x.png", "scale" : "2x" },
    { "size" : "128x128", "idiom" : "mac", "filename" : "icon_128x128.png", "scale" : "1x" },
    { "size" : "128x128", "idiom" : "mac", "filename" : "icon_128x128@2x.png", "scale" : "2x" },
    { "size" : "256x256", "idiom" : "mac", "filename" : "icon_256x256.png", "scale" : "1x" },
    { "size" : "256x256", "idiom" : "mac", "filename" : "icon_256x256@2x.png", "scale" : "2x" },
    { "size" : "512x512", "idiom" : "mac", "filename" : "icon_512x512.png", "scale" : "1x" },
    { "size" : "512x512", "idiom" : "mac", "filename" : "icon_512x512@2x.png", "scale" : "2x" }
  ],
  "info" : {
    "version" : 1,
    "author" : "xcode"
  }
}
EOF

# Replace the iconset in the project
echo ""
echo "📦 Copying iconset to project..."
cp -R "$ICONSET" "DataViewer/Assets.xcassets/"

echo ""
echo "✅ Icon created successfully!"
echo ""
echo "📁 Files created:"
ls -lh "$ICONSET"/*.png | awk '{print "  " $9 " (" $5 ")"}'
echo ""
echo "📦 Icon installed to: DataViewer/Assets.xcassets/AppIcon.appiconset/"
echo ""
echo "Next steps:"
echo "  1. Clean build: cd DataViewer.xcodeproj && xcodebuild clean"
echo "  2. Rebuild: ./package.sh"
echo "  3. Your new icon will appear in the app!"
