#!/bin/bash
set -e

echo "Starting DataViewerXC → DataViewer renaming..."

cd "/Users/default/Desktop/ESP-IQ2020/DataViewer/DataViewerXC"

# 1. Rename the inner source folder
echo "Renaming DataViewerXC folder to DataViewer..."
mv DataViewerXC DataViewer

# 2. Rename the App file
echo "Renaming DataViewerXCApp.swift to DataViewerApp.swift..."
mv DataViewer/DataViewerXCApp.swift DataViewer/DataViewerApp.swift

# 3. Rename the entitlements file
echo "Renaming DataViewerXC.entitlements to DataViewer.entitlements..."
mv DataViewer/DataViewerXC.entitlements DataViewer/DataViewer.entitlements

# 4. Update file contents - DataViewerApp.swift
echo "Updating DataViewerApp.swift struct name..."
sed -i '' 's/struct DataViewerXCApp:/struct DataViewerApp:/' DataViewer/DataViewerApp.swift

# 5. Update project.pbxproj references
echo "Updating project.pbxproj file references..."
cd DataViewer.xcodeproj
sed -i '' 's/DataViewerXCApp\.swift/DataViewerApp.swift/g' project.pbxproj
sed -i '' 's/DataViewerXC\.entitlements/DataViewer.entitlements/g' project.pbxproj
sed -i '' 's/path = DataViewerXC;/path = DataViewer;/g' project.pbxproj
sed -i '' 's/"DataViewerXC"/"DataViewer"/g' project.pbxproj
sed -i '' 's/name = DataViewerXC;/name = DataViewer;/g' project.pbxproj
sed -i '' 's/productName = DataViewerXC;/productName = DataViewer;/g' project.pbxproj
sed -i '' 's/CODE_SIGN_ENTITLEMENTS = DataViewerXC\/DataViewerXC\.entitlements;/CODE_SIGN_ENTITLEMENTS = DataViewer\/DataViewer.entitlements;/g' project.pbxproj

# 6. Update scheme file
echo "Updating scheme references..."
cd project.xcworkspace/xcshareddata
if [ -d "xcschemes" ]; then
    cd xcschemes
    if [ -f "DataViewerXC.xcscheme" ]; then
        mv DataViewerXC.xcscheme DataViewer.xcscheme
        sed -i '' 's/DataViewerXC/DataViewer/g' DataViewer.xcscheme
    fi
fi

echo "Renaming complete!"
echo ""
echo "Summary of changes:"
echo "  ✓ Renamed DataViewerXC/ folder to DataViewer/"
echo "  ✓ Renamed DataViewerXCApp.swift to DataViewerApp.swift"
echo "  ✓ Renamed DataViewerXC.entitlements to DataViewer.entitlements"
echo "  ✓ Updated struct name in DataViewerApp.swift"
echo "  ✓ Updated all references in project.pbxproj"
echo "  ✓ Updated scheme files"
echo ""
echo "Next: Open DataViewer.xcodeproj in Xcode and build"
