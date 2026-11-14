# Protocol safety validation

Local validation date: November 13, 2026.

Receive-buffer compaction can pass overlapping memory to `memcpy`, and audio replies can copy more than the 20-byte field capacity. Temperature parsing also reads beyond the four-byte wire field and turns malformed text into a reported zero. This change bounds those operations and reports invalid temperature fields as unknown.

## Changes

- Use `memmove` for receive compaction; reject nonpositive input and command payloads too short for dispatch.
- Limit song title and artist replies to 20 wire bytes and reserve the outgoing checksum byte when checking frame capacity.
- Parse target, current and outlet temperatures as exact four-byte ASCII fields. Retain valid zero values; reject malformed fields. Cancel pending temperature intent when the observed units change.
- Mirror the maintained component in `iq2020-dev`. Backport only receive/audio/outgoing-capacity fixes to the archived legacy component.

## Validation

- 14 native tests pass with AddressSanitizer and UndefinedBehaviorSanitizer; no expected failures or skipped tests.
- One full-feature ESP32/M5Stack Atom firmware configuration compiles with ESPHome 2026.10.2 and ESP-IDF.
- Tests extract production methods and cover concatenated/fragmented/corrupt frames, resynchronization, oversized/multibyte audio, outgoing capacity, all three temperature fields and targeted legacy memory paths.
- The independent branch is tested against upstream `305ced65d0dc71cdd61ae14b9f30874fb474e704`. The workflow reproduces the scoped native and firmware checks; local results do not imply GitHub-hosted CI has passed.

## Remaining hardware validation

- Confirm Fahrenheit/Celsius temperature fields and unit changes on real controllers, including unknown display behavior in Home Assistant.
- Exercise real audio displays and sustained UART traffic after deployment. The 20-byte wire limit does not promise UTF-8 codepoint preservation at the boundary.
- No Home Assistant, live spa, firmware upload or current-ESPHome legacy firmware test was available. The packet fixture is a public upstream example, not a capture from the owner's spa.


## Reproduce

Use Python 3.12 and install `requirements-test.txt`. Native tests also need Clang
or GCC with address/undefined-behavior sanitizers.

```sh
python -m unittest discover -s tests -p test_protocol.py -v
python tests/esphome/check.py --stage compile --case full
```

Generated credentials are dummy values. These commands never upload firmware.
