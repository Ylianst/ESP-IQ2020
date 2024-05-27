# IQ2020 RS485 Serial Protocol

The IQ2020 expansion communication port has both I2C and RS485 pins on the connector, but the RS485 pins are the most often used for various devices. The port is set to the 38400 baud N,8,1 serial configuration. The encoding of each frame is as follows:

- 1 byte, always 0x1C
- 1 byte, destination address
- 1 byte, source address
- 1 byte, data length
- 1 byte, operation flags (0x40 = request, 0x80 = response)
- n byte, data.
- 1 byte, checksum that is the sum of previous bytes starting at the destination address and XOR with 0xFF.

Here are some example frames send over RS485:

```
1C:33:01:03:40:19:01:00:6E
1C:21:01:03:40:21:01:00:78
1C:29:01:0F:40:1E:01:00:00:FF:FF:FF:00:FF:04:FF:FF:FF:FF:FF:6C    <-- Request
1C:01:29:0F:80:1E:01:00:00:03:35:00:00:00:00:B3:10:00:68:41:83    <-- Response
```

If you look at the last two frames in the previous example, the source and destination numbers are switched. This is because one is a request to 0x29 from 0x01 and the last frame is the response from 0x29 to 0x01.

When decoding these frames, we can simplify them and show only the important information. Here, I show only the destination, source, operation flags and data for the same packets as above. This is how the IQ2020 DataViewer tool will show the packets.

```
33 01 40 190100
21 01 40 210100
29 01 40 1E010000FFFFFF00FF04FFFFFFFFFF    <-- Request
01 29 80 1E010000033500000000B310006841    <-- Response
```

From now on, I will just use the simplified decoded frames. Next, we can look at what packets are sent over RS485 when the hot tub is first turned on and no devices are attached to the serial bus. We see these packets repeated:

```
24 01 40 1E01FF00FFFFFF00FF01FFFFFFFF00
29 01 40 1E01FF00FFFFFF00FFFFFFFFFFFFFF
33 01 40 190100
21 01 40 210100
```

The source address is 0x01 which is the address of the IQ2020 controller on the bus. I happen to know that 0x29 is the Salt System. When the Salt System responds on 0x29, the IQ2020 controller will stop sending periodic packets to 0x24. So, the theory is that 0x29 and 0x24 are two different versions of the salt water system with only one of the two expected to be present. 0x21 and 0x33 could be the same, for example it could be a music module or something else. So far, we know of the following addresses:

```
0x01 - IQ2020 controller
0x21 - Unknown device 1
0x24 - Salt water system 1
0x29 - Salt water system 2
0x33 - Unknown device 2
```

I happen to own the Salt Water device (0x29), however I don't use the salt water system so I normally keep it disconnected. It has been useful as a way to generate added traffic on the bus. I can send data frames to the salt water module from and address and it will answer back to that address. For example:

```
29 99 40 1E010000FFFFFF00FF04FFFFFFFFFF    <-- Request
99 29 80 1E010000033500000000B310006841    <-- Response
29 99 40 00                                <-- Request
99 29 80 1E010000033500000000B310006841    <-- Response
```

Above, I use 0x99 as by source address and send a request to 0x29. The salt water system will respond to 0x99. It does not seem to matter what data I provide, it always responds the same. Looking at the Salt Water device, here is my first try at the protocol decoding:

![IQ2020-ESP6](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/08bfedeb-2f7b-41b9-adbb-fd7b7b47f5d0)

The echanges between the IQ2020 and this Salt Water device on address 0x29 always contain 15 bytes of data. The IQ2020 device will send requests to the module every few seconds and sometimes even faster.

