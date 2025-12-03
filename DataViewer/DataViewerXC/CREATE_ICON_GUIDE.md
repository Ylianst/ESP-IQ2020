# Creating a Custom Icon for DataViewer

## Quick Option: Using SF Symbols (Native macOS Icons)

The easiest way to create a Mac app icon is to use SF Symbols, which provides professional icons that match macOS design guidelines.

### Option 1: Use SF Symbols App (Recommended)

1. **Download SF Symbols** from Apple:
   - Go to: https://developer.apple.com/sf-symbols/
   - Download and open the SF Symbols app

2. **Choose an Icon**:
   - Recommended symbols for DataViewer:
     - `chart.line.uptrend.xyaxis` (data chart)
     - `waveform.path.ecg` (data waveform)
     - `gauge.with.dots.needle.67percent` (monitoring)
     - `network` (network connection)
     - `eye.circle` (data viewer)

3. **Export the Icon**:
   - Select your chosen symbol
   - File → Export Symbol
   - Choose SVG format
   - Save as `icon.svg`

4. **Convert to macOS Icon**:
   ```bash
   # Use the provided script below
   ./create_icon_from_svg.sh icon.svg
   ```

## Option 2: Create Custom Icon with Preview.app

1. **Create a 1024x1024 PNG** with your design
2. **Open in Preview**
3. **File → Export** and choose appropriate sizes
4. **Use the iconutil command** (see script below)

## Option 3: Professional Icon Design

### Recommended Design Elements:
- **Color Scheme**: 
  - Primary: Blue (#007AFF) - matches macOS accent
  - Secondary: Green (#34C759) - for data/connection
  - Accent: Orange (#FF9500) - for activity/monitoring

### Icon Concept:
- Central element: Data waveform or chart
- Background: Circular gradient
- Style: Modern, flat design with subtle shadows
- Must work at all sizes: 16x16 to 1024x1024

## Automated Icon Creation Script

Save this as `create_icon_from_svg.sh`:

```bash
#!/bin/bash
# This script requires: 
# - SVG input file
# - ImageMagick or rsvg-convert for SVG to PNG conversion
# - iconutil (included with Xcode)

SVG_FILE="$1"
ICONSET="AppIcon.appiconset"

if [ -z "$SVG_FILE" ]; then
    echo "Usage: ./create_icon_from_svg.sh input.svg"
    exit 1
fi

if [ ! -f "$SVG_FILE" ]; then
    echo "Error: File $SVG_FILE not found"
    exit 1
fi

# Create iconset directory
mkdir -p "$ICONSET"

# Required sizes for macOS app icons
sizes=(16 32 64 128 256 512 1024)

echo "Converting SVG to PNG at multiple sizes..."

for size in "${sizes[@]}"; do
    # Standard resolution
    if command -v rsvg-convert &> /dev/null; then
        rsvg-convert -w $size -h $size "$SVG_FILE" -o "$ICONSET/icon_${size}x${size}.png"
    elif command -v convert &> /dev/null; then
        convert -background none -resize ${size}x${size} "$SVG_FILE" "$ICONSET/icon_${size}x${size}.png"
    else
        echo "Error: Neither rsvg-convert nor ImageMagick found"
        echo "Install with: brew install librsvg imagemagick"
        exit 1
    fi
    
    # Retina (@2x) - except 1024
    if [ $size -lt 1024 ]; then
        size2x=$((size * 2))
        if command -v rsvg-convert &> /dev/null; then
            rsvg-convert -w $size2x -h $size2x "$SVG_FILE" -o "$ICONSET/icon_${size}x${size}@2x.png"
        else
            convert -background none -resize ${size2x}x${size2x} "$SVG_FILE" "$ICONSET/icon_${size}x${size}@2x.png"
        fi
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
cp -R "$ICONSET" "DataViewer/Assets.xcassets/"

echo "✅ Icon created successfully!"
echo "📦 New iconset copied to DataViewer/Assets.xcassets/AppIcon.appiconset/"
echo ""
echo "Next steps:"
echo "1. Open DataViewer.xcodeproj in Xcode"
echo "2. Clean build folder (Cmd+Shift+K)"
echo "3. Build and run to see your new icon"
```

## Simple Icon Template (SVG)

Here's a basic SVG template you can customize:

```svg
<svg width="1024" height="1024" xmlns="http://www.w3.org/2000/svg">
  <defs>
    <linearGradient id="bgGradient" x1="0%" y1="0%" x2="100%" y2="100%">
      <stop offset="0%" style="stop-color:#007AFF;stop-opacity:1" />
      <stop offset="100%" style="stop-color:#5856D6;stop-opacity:1" />
    </linearGradient>
  </defs>
  
  <!-- Background circle with rounded square -->
  <rect width="1024" height="1024" rx="226" fill="url(#bgGradient)"/>
  
  <!-- Waveform/Chart representation -->
  <g transform="translate(200, 400)">
    <!-- Data line -->
    <path d="M 50 200 Q 150 50, 250 150 T 450 100 T 624 150" 
          stroke="white" stroke-width="40" fill="none" 
          stroke-linecap="round" stroke-linejoin="round"/>
    
    <!-- Data points -->
    <circle cx="50" cy="200" r="30" fill="white"/>
    <circle cx="250" cy="150" r="30" fill="white"/>
    <circle cx="450" cy="100" r="30" fill="white"/>
    <circle cx="624" cy="150" r="30" fill="white"/>
  </g>
  
  <!-- Connection indicator (optional) -->
  <circle cx="850" cy="170" r="40" fill="white" opacity="0.9"/>
  <circle cx="850" cy="170" r="25" fill="#34C759"/>
</svg>
```

Save this as `icon_template.svg` and customize as needed.

## Quick Start

1. **Install required tools**:
   ```bash
   brew install librsvg imagemagick
   ```

2. **Create or download your icon** (SVG format)

3. **Run the conversion script**:
   ```bash
   chmod +x create_icon_from_svg.sh
   ./create_icon_from_svg.sh your_icon.svg
   ```

4. **Rebuild your project**:
   ```bash
   ./package.sh
   ```

## Professional Icon Services (If You Prefer)

- **Fiverr**: Affordable custom icons ($5-$50)
- **99designs**: Professional design contests
- **IconJar**: Icon management tool with templates
- **Sketch/Figma**: Design your own

## Notes

- macOS app icons use a specific rounded square shape
- Must provide multiple sizes for different displays
- Consider how the icon looks at small sizes (16x16)
- Test on both light and dark menu bars
- Use the system color palette for best integration

## Current Icon Location

The current icon files are in:
```
DataViewer/Assets.xcassets/AppIcon.appiconset/
```

Replace the contents of this folder with your new icon set.
