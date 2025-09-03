#pragma once

// Framework detection and compatibility layer for ESP-IQ2020
// This header ensures compatibility between Arduino and ESP-IDF frameworks

// Detect framework being used
#ifdef ARDUINO
    #define IQ2020_USING_ARDUINO 1
    #define IQ2020_USING_ESP_IDF 0
#elif defined(ESP_PLATFORM)
    #define IQ2020_USING_ARDUINO 0
    #define IQ2020_USING_ESP_IDF 1
#else
    #define IQ2020_USING_ARDUINO 0
    #define IQ2020_USING_ESP_IDF 0
#endif

// Framework-specific includes (if needed in the future)
#if IQ2020_USING_ESP_IDF
    // ESP-IDF specific includes can be added here if needed
    // Currently not needed as we use ESPHome abstractions
#endif

#if IQ2020_USING_ARDUINO
    // Arduino specific includes can be added here if needed
    // Currently not needed as we use ESPHome abstractions
#endif

// Ensure we have a supported framework
#if !IQ2020_USING_ARDUINO && !IQ2020_USING_ESP_IDF
    #warning "IQ2020: Unknown framework detected. ESP-IQ2020 supports Arduino and ESP-IDF frameworks."
#endif