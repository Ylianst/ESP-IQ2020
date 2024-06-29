## ACE Module Emulation

If your hot tub does not have a ACE or Freshwater salt system, you can have the ESP32 emulate a ACE module on your spa remote. It will allow you to control the salt level from 0 to 10, turn on/off the boost mode, press the text button and see a status bar from 0 to 7. When enabled, all these values will be sent to Home Assistant to be used for automations much like the audio module emulation. To enable this feature, add the `ace_emulation: true` to the `IQ2020` section like this:

```
iq2020-dev:
   uart_id: SpaConnection
   ace_emulation: true
```

Then, add the following in each of the corresponding sections:

```
sensor:
  - platform: iq2020-dev
    buttons:
      name: Buttons

number:
  - platform: iq2020-dev
    id: salt_status
    name: Salt System Status
    datapoint: 6

switch:
  - platform: iq2020-dev
    name: Salt System Boost
    id: salt_system_boost
    datapoint: 8
```

Once done, reflash the ESP32. You will need to reset the hot tub remote to see the new music icon. To do this, press and hold any inactive part of the display for about 15 seconds and the remote will go black and reset. [Video demonstration here](https://youtu.be/od5SB6RIO1s?si=Db0cwpKzg9-m2b_o&t=14). The remote takes a few minutes to reboot, it's not a fast device.

The button sensor will show 0 or unknown, but will momenteraly pulse to the following values when a button is pressed:

```
5 - Test
6 - Boost on
7 - Boost off
```

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/34c13270-c80d-43e1-808c-b342e6a9cd72)



![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/09665e9a-f9d7-43c2-b364-45ced6b1492d)
