# Audio Module Emulation

This integration allows the ESP32 device to emulate the music module and so, show the extra music option on the SPA remote. Once shown, buttons and volume values will be relayed back to Home Assistant for processing. This allows you to control home lights and trigger automations based on button presses on the spa remote. This is much like the [ACE module emulation](https://github.com/Ylianst/ESP-IQ2020/blob/main/documentation/ace.md). This works really well and is fast.

To get this working, add the line `audio_emulation: true` in the `iq2020` section like this:

```
iq2020:
   uart_id: SpaConnection
   audio_emulation: true
```

Also, add the following sensor in the `sensor` section. If the use the ACE module emulation, you will already have this sensor.

```
sensor:
    buttons:
      name: Buttons
```

Them, replace the empty `select`, `number`, and `text` sections with this:

```
select:
  - platform: iq2020
    name: Audio Source
    id: audio_source
    datapoint: 0

number:
  - platform: iq2020
    name: Volume
    id: audio_volume
    datapoint: 0
  - platform: iq2020
    name: Treble
    id: audio_treble
    datapoint: 1
  - platform: iq2020
    name: Bass
    id: audio_bass
    datapoint: 2
  - platform: iq2020
    name: Balance
    id: audio_balance
    datapoint: 3
  - platform: iq2020
    name: Subwoofer
    id: audio_subwoofer
    datapoint: 4

text:
  - platform: iq2020
    name: Song Title
    id: song_title
    datapoint: 0
    mode: text
    value: "Home Assistant"
  - platform: iq2020
    name: Artist Name
    id: artist_name
    datapoint: 1
    mode: text
    value: "Remote Control"
```

Once done, reflash and you should see many more sensors in your hot tub device:

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/9ba450ab-4dee-41e3-9b33-7bee60b0c5c4)

You will need to reset the hot tub remote to see the new music icon. To do this, press and hold any inactive part of the display for about 15 seconds and the remote will go black and reset. [Video demonstration here](https://youtu.be/od5SB6RIO1s?si=Db0cwpKzg9-m2b_o&t=14). The remote takes a few minutes to reboot, it's not a fast device.

The button sensor will show 0 or unknown, but will momenteraly pulse to the following values when a button is pressed:

```
1 - Play
2 - Pause
3 - Next
4 - Back
```

There is a RS485 command sent on the bus right away when a button is pressed, so there is no delay. The pulse is very quick and is made so you can register automations. When entity `buttons` goes to a value (In the example below, 4 is the back button) you can trigger the automation. Pressing the same button multiple times will pulse the entity multiple times, so triggering a automation that toggles a light will work correctly.

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/4733fbae-1796-4a15-81e1-31ec6ab28036)

You can show any text you like for the Song Title and Artist Name, but there is a 20 character limit.

![audio1](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/a69a4daf-988e-4551-9632-7e24f7df4380)

![audio2](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/f4381d2f-4501-499c-9310-606992fa220d)

Known Issues:

- The balance, bass and treble are values that go from -5 to +5. You can change them from the spa remote and it will show up correctly in Home Assistant. However, trying to change these values from Home Assistant will not work in the negative ranges. The root cause looks like a bug in the IQ2020 firmware. There are commands for these 3 settings, but they don't seem to work for negavive values. Not a big surprise, these commands may never have gotten any use outside of this integration.
