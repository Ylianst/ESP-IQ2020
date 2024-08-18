# ACE Module Emulation

If your hot tub does not have a ACE or Freshwater salt system, you can have the ESP32 emulate a ACE module on your spa remote. It will allow you to control the salt level from 0 to 10, turn on/off the boost mode, press the test button and see a status bar from 0 to 7. When enabled, all these values will be sent to Home Assistant to be used for automations much like the [audio module emulation](https://github.com/Ylianst/ESP-IQ2020/blob/main/documentation/audio.md). To enable this feature, add the `ace_emulation: true` to the `IQ2020` section like this:

```
iq2020:
   uart_id: SpaConnection
   ace_emulation: true
```

Then, add the following in each of the corresponding sections. If you use the audio module emulation, you will already have the `buttons` sensor.

```
sensor:
  - platform: iq2020
    buttons:
      name: Buttons

number:
  - platform: iq2020
    id: salt_status
    name: Salt System Status
    datapoint: 6

switch:
  - platform: iq2020
    name: Salt System Boost
    id: salt_system_boost
    datapoint: 8
```

Once done, reflash the ESP32. You will need to reset the hot tub remote to see the new music icon. To do this, press and hold any inactive part of the display for about 15 seconds and the remote will go black and reset. [Video demonstration here](https://youtu.be/od5SB6RIO1s?si=Db0cwpKzg9-m2b_o&t=14). The remote takes a few minutes to reboot, it's not a fast device.

The button sensor will show 0 or unknown, but will momenteraly pulse to the following values when a button is pressed:

```
6 - Test
7 - Boost on
8 - Boost off
```

The button pressed will show rapidly and so, no need to increase the polling rate to get a quick response. The "Salt System Status" is set by Home Assistant, but read-only on the remote. Other values can be changes on both remote and Home Assistant. The new sensors will look like this.

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/34c13270-c80d-43e1-808c-b342e6a9cd72)

You will see a new screen on your remote that looks like this.

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/09665e9a-f9d7-43c2-b364-45ced6b1492d)

If you change values from Home Assistant, you will need to move out of the ACE screen and back to see the new values. Otherwise, it's up to you to create your own Home Assistant automations to make this screen do anything you like.
