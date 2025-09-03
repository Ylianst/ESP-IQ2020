# ESP-IDF Framework Support

ESP-IQ2020 fully supports both Arduino and ESP-IDF frameworks in ESPHome. This document provides detailed information about using ESP-IDF with your hot tub integration.

## Framework Comparison

| Feature | Arduino Framework | ESP-IDF Framework |
|---------|------------------|-------------------|
| **Binary Size** | Larger (~1.1MB) | Smaller (~926KB) |
| **RAM Usage** | Higher (~47KB) | Lower (~35KB) |
| **Flash Usage** | Higher (~61.8%) | Lower (~50.5%) |
| **Development** | Traditional Arduino | Native Espressif |
| **ESPHome Default** | Current default | Default from 2026.1.0 |

*Binary size comparison based on actual measurements from issue #57*

## Configuration Differences

### Arduino Framework
```yaml
esp32:
  board: m5stack-atom
  framework:
    type: arduino

logger:
  baud_rate: 0  # Arduino requires explicit baud_rate
  level: ERROR
```

### ESP-IDF Framework
```yaml
esp32:
  board: m5stack-atom
  framework:
    type: esp-idf

logger:
  # Note: baud_rate not needed for ESP-IDF
  level: ERROR
```

## Migration from Arduino to ESP-IDF

Migration is straightforward - simply change the framework type in your configuration:

1. Change `type: arduino` to `type: esp-idf` in your `esp32` section
2. Remove the `baud_rate: 0` line from your `logger` section
3. Recompile and flash your device

**No other changes are required** - ESP-IQ2020 uses ESPHome abstractions that work identically with both frameworks.

## Benefits of ESP-IDF

### Smaller Binary Size
ESP-IDF produces significantly smaller binaries:
- **Arduino**: 1,133,394 bytes (61.8% flash usage)
- **ESP-IDF**: 926,322 bytes (50.5% flash usage)
- **Savings**: ~207KB (18% reduction)

### Better Memory Usage
ESP-IDF is more memory efficient:
- **Arduino**: 47,512 bytes RAM usage (14.5%)
- **ESP-IDF**: 34,908 bytes RAM usage (10.7%)
- **Savings**: ~12.6KB RAM (26% reduction)

### Performance Improvements
- More efficient task scheduling
- Better interrupt handling
- Optimized network stack
- Lower power consumption

### Future Compatibility
ESPHome will default to ESP-IDF for ESP32 boards starting with version 2026.1.0, making ESP-IDF the recommended choice for new installations.

## Troubleshooting

### Common Issues

**Build errors with ESP-IDF:**
- Ensure you're using a recent ESPHome version (2023.4.0+)
- Clear build cache if switching from Arduino framework
- Check that all external components support ESP-IDF

**Logger configuration:**
- Remove `baud_rate` setting when using ESP-IDF
- ESP-IDF automatically configures UART logging

**Compatibility:**
- All ESP-IQ2020 features work identically with both frameworks
- No configuration changes needed beyond framework selection

### Getting Help

If you encounter issues with ESP-IDF:

1. Check the [ESPHome ESP-IDF migration guide](https://esphome.io/guides/esp32_arduino_to_idf/)
2. Open an issue in the [ESP-IQ2020 GitHub repository](https://github.com/Ylianst/ESP-IQ2020/issues)
3. Include your full configuration and error messages

## Technical Implementation

ESP-IQ2020 achieves framework compatibility by:

- Using ESPHome's UART component instead of framework-specific APIs
- Leveraging ESPHome's GPIO abstractions for pin control
- Using standard C++ timing functions compatible with both frameworks
- Avoiding direct framework dependencies in favor of ESPHome abstractions

This design ensures that the same code works seamlessly with both Arduino and ESP-IDF without requiring framework-specific implementations.