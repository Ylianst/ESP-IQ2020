# Sensor metadata validation

Local validation date: November 13, 2026.

IQ remaining life and several salt-system counters lack duration metadata, making their units and history semantics ambiguous in Home Assistant. This change labels hours/days explicitly while preserving raw values, entity keys and counter scaling.

## Changes

- IQ measuring-sensor remaining life: hours, duration device class, measurement state class.
- Salt cartridge age and time since manual confirmation: days, duration device class, existing measurement class.
- Salt generation time: hours and duration device class, retaining `total_increasing`.
- Correct IQ big-endian and decoded equipment-counter little-endian notes; distinguish IQ measuring-sensor life from salt-generator age and label unconfirmed protocol fields as uncertain.
- Document float32 Unix-timestamp precision; use an explicit float cast without claiming a precision fix. RTC text/raw fields remain the route for second-level comparisons.

## Validation

- One schema test passes, with assertions for all four fields, units, device/state classes and absence of value-transforming filters.
- One full-feature ESP32/M5Stack Atom firmware configuration compiles with ESPHome 2026.10.2 and ESP-IDF.
- Maintained/dev source copies match. This branch is tested independently against upstream `305ced65d0dc71cdd61ae14b9f30874fb474e704`; local results do not imply GitHub-hosted CI success.

## Remaining hardware / Home Assistant validation

- Review existing HA statistics and templates that supplied custom units before rollout; verify continuity of recorder/statistics behavior after metadata changes.
- Observe IQ remaining hours, cartridge changes, manual confirmations and generation counters over time. Their reset/replacement meanings remain distinct; no universal sensor-life formula or end-of-life threshold is asserted.
- Confirm controller-specific runtime scaling and RTC timezone behavior from real captures. Numeric float32 timestamp precision is still limited.
- No live HA, spa access or firmware upload was used. No water-quality thresholds, dosing controls, inferred fault enums or freshness alarms are introduced.


## Reproduce

Use Python 3.12 and install `requirements-test.txt`. Native tests also need Clang
or GCC with address/undefined-behavior sanitizers.

```sh
python tests/esphome/test_metadata_schemas.py -v
python tests/esphome/check.py --stage compile --case full
```

Generated credentials are dummy values. These commands never upload firmware.

## Companion contribution compatibility

The independent protocol, control/platform and metadata branches were merged in
a temporary validation tree without conflicts. That combined tree passes all
38 native tests, five schema tests and a full-feature firmware build. Its 55
maintained/legacy production files match the reviewed complete implementation
(byte-for-byte except for a C++ helper relocation and comments). The source
checkout and its uncommitted changes were preserved.

Companion drafts: [protocol safety #135](https://github.com/Ylianst/ESP-IQ2020/pull/135),
[control/platform #136](https://github.com/Ylianst/ESP-IQ2020/pull/136), and
[metadata #137](https://github.com/Ylianst/ESP-IQ2020/pull/137). Each targets upstream
`main` independently; shared test helpers are identical additions. Hardware
validation above remains pending even when local or hosted checks pass.
