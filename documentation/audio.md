# Audio Module Emulation

This is early development right now, but this integration allows the ESP32 device to emulate the music module and so, show the extra music option on the SPA remote. Once shown, buttons and volume values will be relayed back to Home Assistant for processing. This allows you to control home lights and trigger automations based on button presses on the spa remote. This works really well, but the interface to Home Assistant is very basic rigth now as I just got this working a few hours ago, so expect more changes in the future for this as the integration will get better.

To get this working, add the line `audio_emulation: true` in the `iq2020` section like this:

```
iq2020:
   uart_id: SpaConnection
   audio_emulation: true
```

Also, add the follwing two sensors in the `sensor` section:

```
sensor:
    audio_buttons:
      name: Audio Buttons
    audio_volume:
      name: Audio Volume
```

Once done, reflash and you should see two more sensors in your hot tub device:

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/c8cbca21-a56a-45c0-b055-8db8baaf21e1)

You will need to reset the hot tub remote to see the new music icon. To do this, press and hold any inactive part of the display for about 15 seconds and the remote will go black and reset. [Video demonstration here](https://youtu.be/od5SB6RIO1s?si=Db0cwpKzg9-m2b_o&t=14). The remote takes a few minutes to reboot, it's not a fast device.

The volume sensor is as you would expect, except it goes from 15 to 40 right now, I will fix this to go from 0 to 100 in the future.

The audio button sensor will show 0 or unknown, but will momenteraly pulse to the following values when a button is pressed:

```
1 - Play
2 - Pause
3 - Next
4 - Back
```

The pulse is very quick, but you can register automations. When entity `audio button` goes to a value (In the example below, 4 is the back button) you can trigger the automation.

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/4733fbae-1796-4a15-81e1-31ec6ab28036)

That's it for now. I can see the bass, balance and other settings and in Bluetooth mode, I can display text on the remote. So, this should allow for something a lot more fancy.

![audio1](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/a69a4daf-988e-4551-9632-7e24f7df4380)

![audio2](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/f4381d2f-4501-499c-9310-606992fa220d)
