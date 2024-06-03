# Debugging RS485 traffic

This ESP-Home integration comes built it with a RS485 traffic relay on the ESP32 device along with a Windows tool called the Data Viewer to see the RS485 traffic in real time. This is very useful to debug issue or to help build new features. If you are a bit technical, I encorage you to try this and help out. Here are some tasks:

- There is plenty of data in the protocol document that we don't know what it is yet.
- Is there any new commands we don't know about?
- It would be great to have traffic captures of the music device so that we can add music integration.
- For hot tubs without a music device, we can impersonate the device with Home Assistant and make the controls avaialble.
- I would like to control house lights from the hot tub remote? Maybe enable added Jets 3 and 4 that we don't have and connect that to automations?

To enable the RS485-to-TCP relay system, just add a `port:` value under the `iq2020` section of the ESP-Home device configuration. By default this port is 0 and turn off.

```
# Relay UART messages over TCP
iq2020:
   uart_id: SpaConnection
   port: 1234
```

Make the change and re-flash your ESP32 device over WIFI. Once you enable this, you can connect a TCP socket to your device IP address on this port and send/receive RS485 traffic. Note that the port is not secured at all and multiple TCP connections are supported at the same time. You can use my "Data Viewer" Widnows applicatins to connect to this port and see the traffic in real-time.

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/59615021-3164-4cb9-9a7c-036896141e3d)

You could use a USB-to-RS485 dongle to get at the same traffic, but that requires you run wires to the hot tub and generally the wife will not approve of this. This RS485 relay feature can be enabled and the Home Assistant integration will keep working fully. So, you can debug and capture traffic while using the hot tub integration normally. Note that any RS485 traffic sent from the ESP32 device will not be relayed and so, will not show up in the Data Viewer application.

Data Viewer is a C# application built with Visual Studio 2015. I provide a compiled executable here, but it's not code signed.

 - [IQ2020 Data Viewer Executable](https://github.com/Ylianst/ESP-IQ2020/raw/main/DataViewer/IQ2020-DataViewer.exe)
 - [The source code is here](https://github.com/Ylianst/ESP-IQ2020/tree/main/DataViewer)

Once you get the executable, run it, put the IP address and port of your ESP32 device and hit connect. To makes things easy, packets will be shown like in the [RS485 protocol page](https://github.com/Ylianst/ESP-IQ2020/blob/main/documentation/protocol.md), without the starting byte, data length and checksum.

Another thing you may want to do is change the debug level from `ERROR` to `DEBUG` in the ESP-Home confirmation file and reflash. You will see a lot more data in the ESP-Home logs.

```
logger:
  baud_rate: 0
  level: DEBUG
```
