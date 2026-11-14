# Control and optional-platform validation

Local validation date: November 13, 2026.

An unchanged or value-less controller reply can incorrectly settle a newer request, while the shared retry counter can replenish itself or consume another control's attempts. A temperature ACK can also discard a newer target. This change separates requested and reported state, bounds retries per control, and uses actual temperature status to calculate the next queued delta.

## Changes

- Validate platform datapoints, speed counts, runtime requests and received control values before indexing or calculating relative steps. Reject unsupported fan datapoint 3, whose previous arithmetic route reaches salt boost.
- Publish reported number/select values, retain a differing newer target, and treat value-less ACKs as a reason to poll rather than confirmation of the latest request.
- Allow three additional control attempts; relative light retries require feedback and expire after five seconds. Temperature requests allow two further readback-driven attempts and expire after 15 seconds. Disabling active mode clears queued intent.
- Preserve omitted fields in partial fan calls; allow output zero through the existing FreshWater and ACE routes.
- Guard optional platform dependencies, initialize optional sensor pointers, implement read-only `sensor.salt_power`, make version text optional, discover IQ for any configured IQ measurement, and preserve discovery attempts during startup delay/passive mode.

Optional-platform fixes are included because they guard the same state-publication paths. Keeping these edits together makes this branch independently buildable, including configurations without fan, switch, climate or number entities. Maintained/dev source copies match; the archived legacy control implementation is unchanged.

## Validation

- 24 native tests pass with AddressSanitizer and UndefinedBehaviorSanitizer; no expected failures or skips.
- Four schema tests pass, with subcases for all six datapoint platforms, speed counts and optional version text.
- Eight ESP32/M5Stack Atom firmware configurations compile with ESPHome 2026.10.2 / ESP-IDF: minimal, chemistry, text-only, salt-sensor, pH-only, controls, select-without-number and full.
- Tested independently against upstream `305ced65d0dc71cdd61ae14b9f30874fb474e704`. CI definitions are included; these are local results, not a claim of GitHub-hosted success.

## Remaining hardware validation

- **Salt output zero remains pending physical acceptance** on FreshWater and ACE modules. Tests prove the outgoing routes/checksums, not panel/module acceptance.
- Check actual readback cadence, rapid changes, delayed/duplicate ACKs, packet loss and reconnects on real controllers. The protocol provides no sequence number for distinguishing arbitrary stale reports; the chosen retry/deadline values need field validation.
- Verify all installed jets, single/two-speed fan calls, light zones and CoolZone/native temperature bounds. UI confirmation may take longer because number/select state follows reports.
- Confirm optional entities and startup discovery in Home Assistant. No live HA, spa connection or firmware upload was used.


## Reproduce

Use Python 3.12 and install `requirements-test.txt`. Native tests also need Clang
or GCC with address/undefined-behavior sanitizers.

```sh
python -m unittest discover -s tests -p test_controls.py -v
python tests/esphome/test_controls_schemas.py -v
python tests/esphome/check.py --stage compile
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
