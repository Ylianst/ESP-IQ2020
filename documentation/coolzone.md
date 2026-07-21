# Coolzone

The Coolzone is an optional heat pump that can be attached to the hot tub. In addition to heating the water, it can also actively cool it, which makes it useful in warm climates. If you have this module, this integration can show its current mode and let you change it right from the Home Assistant climate control.

## Enabling Coolzone

Coolzone support is off by default. To turn it on, add `coolzone: true` in the `iq2020` section of your configuration like this:

```yaml
iq2020:
   uart_id: SpaConnection
   coolzone: true
```

Once enabled, the integration will poll the Coolzone heat pump for its state and add the Coolzone modes to your existing `climate:` control. If you don't specify `coolzone`, it's false by default and nothing changes.

If you don't already have a climate control, add one in the `climate:` section:

```yaml
climate:
  - platform: iq2020
    name: Temperature
```

## Setting the mode and temperature

With Coolzone enabled, the same climate control lets you set both the target temperature and the heat pump mode. The heat pump has 5 operating modes. Rather than a single long list, they are split into a **Mode** (Heat, Cool or Auto) and a **Preset** (Saver or Boost). The two combine to select the operating mode:

| Mode | Preset | Coolzone mode |
| ---- | ------ | ------------- |
| Heat | Boost  | Heat w/Boost  |
| Heat | Saver  | Heat Saver    |
| Cool | (any)  | Chill         |
| Auto | Boost  | Auto w/Boost  |
| Auto | Saver  | Auto Saver    |

Cooling combined with boost is not a valid combination, so when the mode is `Cool` the preset is ignored and the heat pump simply chills the water.

The `Boost` preset allows the controller to use the hot tub's induction heating in addition to the heat pump. This heats the water faster at the cost of using more power. The `Saver` preset relies on the heat pump alone.

The climate control also reflects what the heat pump's compressor is doing right now. It shows **red** when the water is being heated (either by the heat pump or induction heating) and **blue** when the heat pump is actively cooling the water.

## Coolzone raw sensors (optional)

The Coolzone reports two values: the current mode and the current compressor state. If you would like to monitor these raw values directly, or help capture values that are not yet documented, you can add two optional sensors in the `sensor:` section:

```yaml
sensor:
  - platform: iq2020
    coolzone_mode_raw:
      name: Coolzone Mode Raw
    coolzone_state_raw:
      name: Coolzone State Raw
```

The mode raw value maps to the modes as follows:

```
0 = Heat w/Boost
1 = Heat Saver
2 = Chill
3 = Auto w/Boost
4 = Auto Saver
```

The compressor state raw value tells you what the compressor is currently doing:

```
0 = Off (assumed, not yet observed)
1 = Standby
2 = Heating
4 = Cooling
```

If you see any value that is not in these lists, please [let me know](https://github.com/Ylianst/ESP-IQ2020/issues) so the documentation can be updated. More details on the protocol are available in the [protocol documentation](https://github.com/Ylianst/ESP-IQ2020/blob/main/documentation/protocol.md#coolzone).

## Note about the official Spa Connection Kit

If you have the official Spa Connection Kit installed, it already polls the Coolzone heat pump on its own. In that case, this integration will not send its own Coolzone polling requests to avoid adding needless traffic on the RS485 bus. It will simply listen to the responses from the official module, so your mode, state and raw sensors will still update normally.
