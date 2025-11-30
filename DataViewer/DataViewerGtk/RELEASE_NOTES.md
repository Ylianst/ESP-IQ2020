# Release Notes - IQ2020 Data Viewer GTK# Port

## Version 1.0.0 (November 2025)

### Overview
Complete GTK# port of the IQ2020 Data Viewer Windows Forms application, providing cross-platform support for macOS and Linux with full feature parity.

### New Platform Support
- **macOS**: Native support for both Apple Silicon (ARM64) and Intel (x64) processors
- **Linux**: x64 support with all major distributions
- **Self-Contained Packages**: No .NET runtime installation required

### Complete Feature Implementation

#### File Menu
- ✅ Log to File - Toggle logging of all communications
- ✅ Annotate Log - Add timestamped annotations to log files
- ✅ Clear - Clear all display data
- ✅ Exit - Clean shutdown with proper resource disposal

#### Actions Menu
- ✅ Poll State - Automatic status polling every 2 seconds
- ✅ Scan - Command scanner (0-65535) for device discovery
- ✅ FIQ Test 1 - 7-parameter FreshWater IQ test packet generator
- ✅ FIQ Test 2 - 9-parameter FreshWater IQ test packet generator

#### Filter Menu
- ✅ None - Show all packets
- ✅ Connection Kit (0x1F) - Main control module
- ✅ Connection Kit 0x0256 - Specific status packets
- ✅ Fresh Water (0x29) - Water quality system
- ✅ Freshwater IQ Module (0x37) - Advanced water management
- ✅ Audio (0x33) - Audio system
- ✅ ACE Module (0x24) - Advanced control

#### Commands Menu
- ✅ Ask Lights Status - Query light state
- ✅ Ask Status 0x0255 - General status query
- ✅ Ask Status 0x0256 - Detailed status query
- ✅ Lights On/Off - Control lighting
- ✅ Ask Versions - Request firmware versions
- ✅ Set Current Time - Synchronize device clock

#### Data Decoding
- ✅ Complete packet protocol implementation
- ✅ Temperature readings (set, current, heater outlet)
- ✅ All runtime counters (heater, jets, lights, circulation pump)
- ✅ Full electrical measurements (voltage, current, power for L1, L2, heater, jets)
- ✅ Time and date decoding
- ✅ Version string extraction
- ✅ Detailed hex state decode with annotations

### Technical Improvements

#### Architecture
- Modern async/await pattern for network I/O
- Proper GTK+ 3 event handling
- Thread-safe UI updates using Application.Invoke
- Clean resource management with IDisposable patterns

#### User Interface
- Native GTK3 dialogs and widgets
- Monospace font for hex data display
- Tabbed interface for organized data views
- Responsive layout with proper scrolling
- Status bar with connection state

#### Configuration
- Cross-platform settings storage (~/.iq2020dataviewer)
- Persistent connection address
- Automatic state restoration

#### Packaging
- Self-contained single-file executables
- No external dependencies (except GTK3)
- Optimized release builds with no debug symbols
- Automated packaging script for all platforms

### Distribution Files

#### macOS
- `DataViewerGtk-osx-arm64.zip` - Apple Silicon (M1/M2/M3)
- `DataViewerGtk-osx-x64.zip` - Intel processors

#### Linux
- `DataViewerGtk-linux-x64.tar.gz` - x64 architecture

### Documentation
- `README.md` - Quick start and development guide
- `INSTALL.md` - Comprehensive installation instructions
- `RELEASE_NOTES.md` - This file

### Known Limitations
- Windows platform not supported (use original WinForms version)
- Requires GTK3 runtime libraries (platform-specific installation)
- macOS security warnings require manual approval or attribute removal

### Migration from Windows Version
Users migrating from the Windows Forms version will find:
- Identical functionality and feature set
- Same packet protocol and commands
- Compatible log file format
- Familiar UI layout and workflow

Configuration is stored differently:
- **Windows**: Registry at `HKEY_CURRENT_USER\SOFTWARE\Open Source\IQ2020DataViewer`
- **macOS/Linux**: File at `~/.iq2020dataviewer`

### System Requirements

#### Minimum
- **macOS**: 10.15 (Catalina) or later
- **Linux**: GTK3 3.22 or later
- **RAM**: 100 MB
- **Disk**: 50 MB

#### Recommended
- **macOS**: 12.0 (Monterey) or later
- **Linux**: Recent distribution with GTK3 3.24+
- **RAM**: 256 MB
- **Disk**: 100 MB

### Building from Source
Requires:
- .NET 8.0 SDK or later
- GTK3 development headers
- Standard build tools

See README.md for detailed build instructions.

### Credits
- **Original Application**: Ylian Saint-Hilaire
- **GTK# Port**: November 2025
- **Testing**: Community contributors

### License
Same open-source license as the original ESP-IQ2020 project.

### Support
- GitHub: https://github.com/Ylianst/ESP-IQ2020
- Issues: https://github.com/Ylianst/ESP-IQ2020/issues

---

**Note**: This is the initial release of the GTK# port. Please report any issues or bugs through the GitHub issue tracker.
