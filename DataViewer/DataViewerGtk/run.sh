#!/bin/bash

# IQ2020 Data Viewer GTK# Runner Script

echo "Building IQ2020 Data Viewer..."
dotnet build DataViewerGtk.csproj

if [ $? -eq 0 ]; then
    echo ""
    echo "Starting application..."
    dotnet run --project DataViewerGtk.csproj
else
    echo ""
    echo "Build failed. Please check the errors above."
    exit 1
fi
