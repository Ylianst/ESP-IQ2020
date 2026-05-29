# Salt Module Protocol Guide (IQ2020)

This document is the implementation guide for the **Freshwater salt module** on the IQ2020 RS485 bus.

## Scope

- Salt module address: `0x29`
- Controller address: `0x01`
- Message directions:
  - Notification: `01 29 80` (salt module -> controller)
  - Request: `29 01 40` (controller -> salt module)

## Frame Formats

### Notification Payload (`01 29 80`)

```text
1E 01 [PP] [CA] [SS] [CB] [MM] [MF] [EH] [EL] [GC] 00 00 69 [ST]
```

#### Notification Byte Definitions

| Byte | Field | Typical Value(s) | Description |
|---|---|---|---|
| 1 | `HDR` | `0x1E` | Fixed payload header |
| 2 | `CMD` | `0x01` | Fixed command type |
| 3 | `PP` | `0x03..0x05` | Production level (system-config dependent) |
| 4 | `CA` | `0x00..0xFF` | Cartridge age counter (days) |
| 5 | `SS` | `0x00..0x78` | Salt level value (0..120 display scale) |
| 6 | `CB` | `0x00..0xFF` | Cartridge age mirror (same as `CA`) |
| 7 | `MM` | varies | Mode/state byte |
| 8 | `MF` | `0x03,0x07,0x0A,0x0B,...` | Mode flag / panel state context |
| 9 | `EH` | `0x00..0xFF` | Error code high byte |
| 10 | `EL` | `0x00..0xFF` | Error code low byte |
| 11 | `GC` | `0x00..0xFF` | Generation-hours counter |
| 12 | — | `0x00` | Reserved |
| 13 | — | `0x00` | Reserved |
| 14 | — | `0x69` | Fixed marker byte |
| 15 | `ST` | `0x41,0x81,0x01,...` | Status flags |

### Request Payload (`29 01 40`)

```text
1E 01 [PL] [PW] FF FF [BT] 00 [C0] [C1] [C2] [C3] FF FF FF
```

#### Request Byte Definitions

| Byte | Field | Typical Value(s) | Description |
|---|---|---|---|
| 1 | `HDR` | `0x1E` | Fixed payload header |
| 2 | `CMD` | `0x01` | Fixed command type |
| 3 | `PL` | `0x01..0x0A`, `0xFF` | Production level or `0xFF` for no-change |
| 4 | `PW` | `0x02` | Keep fixed at `0x02` |
| 5 | — | `0xFF` | Fixed |
| 6 | — | `0xFF` | Fixed |
| 7 | `BT` | `0xFF`, `0x01`, `0x02` | Boost trigger: none/on/off |
| 8 | — | `0x00` | Fixed |
| 9 | `C0` | varies | Command code byte 0 |
| 10 | `C1` | varies | Command code byte 1 |
| 11 | `C2` | varies | Command code byte 2 |
| 12 | `C3` | varies | Command code byte 3 |
| 13 | — | `0xFF` | Fixed |
| 14 | — | `0xFF` | Fixed |
| 15 | — | `0xFF` | Fixed |

## Command Set

Use these request payloads with header `29 01 40`.

| Action | Request payload |
|---|---|
| Standard poll | `1E 01 FF 02 FF FF FF 00 FF 01 FF FF FF FF FF` |
| Set production level (`PL=1..10` in component) | `1E 01 [PL] 02 FF FF FF 00 FF 01 FF FF FF FF FF` |
| Boost ON | `1E 01 FF 02 FF FF 01 00 FF 01 FF FF FF FF FF` |
| Boost OFF | `1E 01 FF 02 FF FF 02 00 FF 01 FF FF FF FF FF` |
| Salt test | `1E 01 FF 02 FF FF FF 00 FF 01 FF 01 FF FF FF` |
| Reset cartridge age | `1E 01 FF 02 FF FF FF 00 02 01 02 FF FF FF FF` |

Safety baseline:

- Keep all fixed bytes unchanged.
- Only vary `PL`, `BT`, and `C0..C3` for supported actions.

## Sensor and Entity Mapping

This section maps protocol fields to entities in `components/iq2020`.

- `salt_content` sensor: from notification byte 5 (`SS`)
  - Freshwater (`0x29`): raw byte value
  - ACE legacy compatibility (`0x24`): value is decoded differently in component logic
- `salt_cartridge_age_days`: notification byte 4 (`CA`)
- `salt_generation_hours`: notification byte 11 (`GC`)
- `salt_error_code`: `(EH << 8) | EL` from bytes 9 and 10
- `salt_module_status` text sensor:
  - `idle` when `ST bit6` is set
  - `generating_chlorine` when `ST bit7` is set and idle is not set
  - `cleaning_cartridge` otherwise
- `salt_level_friendly` text sensor from `SS` zones:
  - `<24`: `red-low`
  - `<36`: `yellow-low`
  - `<84`: `green`
  - `<96`: `yellow-high`
  - `>=96`: `red-high`

## Salt Level Scale

`SS` is a 0..120 display scale.

Operational zone guidance used by the component:

- `0..23`: red-low
- `24..35`: yellow-low
- `36..83`: green
- `84..95`: yellow-high
- `96..120`: red-high

Approximate ppm conversion used for interpretation (**just a guess**):

- ~`20.83 ppm` per display unit
- Example: `SS=84` -> ~`1750 ppm`

## Status and Mode Notes

- Boost state is represented through request trigger `BT` and observed response mode flags.
- `ST` bit usage:
  - bit 6 (`0x40`): idle indicator
  - bit 7 (`0x80`): active polarity/status bit used in active states
- `MM`/`MF` encode operation phases; for control logic, use the documented command triggers and stable outputs above.

## Compatibility Note (ACE vs Freshwater)

A spa will have either:

- Legacy ACE path (`0x24`), or
- Freshwater salt module (`0x29`)

The main `components/iq2020` implementation supports both by branching behavior where needed. New advanced entities and command semantics in this guide are targeted at the Freshwater (`0x29`) protocol.
