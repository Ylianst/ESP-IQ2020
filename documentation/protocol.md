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
0x21 - Unknown device 1 (Audio or Coolzone)
0x24 - ACE Salt System
0x29 - Freshwater Salt System (This is the one I have)
0x33 - Unknown device 2 (Audio or Coolzone)
0x1F - Spa Connection Kit
```

# Freshwater Salt System

I happen to own the Salt Water device (0x29), however I don't use the salt water system so I normally keep it disconnected. It has been useful as a way to generate added traffic on the bus. I can send data frames to the salt water module from and address and it will answer back to that address. For example:

```
29 99 40 1E010000FFFFFF00FF04FFFFFFFFFF    <-- Request
99 29 80 1E010000033500000000B310006841    <-- Response
29 99 40 00                                <-- Request
99 29 80 1E010000033500000000B310006841    <-- Response
```

Above, I use 0x99 as by source address and send a request to 0x29. The salt water system will respond to 0x99. It does not seem to matter what data I provide, it always responds the same. Looking at the Salt Water device, here is my first try at the protocol decoding. Here are a sample request packet and a sample response.

![IQ2020-ESP6](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/662a59c9-f473-450a-a7df-adae60d68dd2)

The echanges between the IQ2020 and this Salt Water device on address 0x29 always contain 15 bytes of data. The IQ2020 device will send requests to the module every few seconds and sometimes even faster.


# Spa Connection Kit

The Spa Connection Kit sends all it's commands from address 0x1F. Sending the commands from a different address will not work.

Read FreshWater Salt Module Data (Polled)
```
<-- 01 1F 40 1E03
<-- 1F 01 80 1E030003FFFF00FF006800FF07FF0000B3100040
```

Jet 1 ON
```
<-- 01 1F 40 0B0203
<-- 1F 01 80 0B0202
```

Jet 1 OFF
```
<-- 01 1F 40 0B0201
<-- 1F 01 80 0B0200
```

Jet 2 MEDIUM
```
<-- 01 1F 40 0B0302
<-- 1F 01 80 0B0301
```

Jet 2 FULL
```
<-- 01 1F 40 0B0303
<-- 1F 01 80 0B0302
```

Jet 2 OFF
```
<-- 01 1F 40 0B0301
<-- 1F 01 80 0B0300
```

Lights ON
```
<-- 01 1F 40 1702041100
<-- 1F 01 80 170206
```

Lights OFF
```
<-- 01 1F 40 1702041000
<-- 1F 01 80 170206
```

Spa Lock ON
```
<-- 01 1F 40 0B1D02
<-- 1F 01 80 0B1D01
```

Spa Lock OFF
```
<-- 01 1F 40 0B1D01
<-- 1F 01 80 0B1D00
```

Temperature Lock ON
```
<-- 01 1F 40 0B1E02
<-- 1F 01 80 0B1E01
```

Temperature Lock OFF
```
<-- 01 1F 40 0B1E01
<-- 1F 01 80 0B1E00
```

Read light status (Polled)
```
<-- 01 1F 40 1705
<-- 1F 01 80 17050005020202000000000000000005010101010000
                   AABBCCDD                WWXXYYZZOO

AA = Underwater light intensity (0x05 = High ... 0x01 = Low, 0x00 = Off)
BB = Bartop light intensity
CC = Pillow light intensity
DD = Exterior light intensity
WW = Underwater light color     (0x01 = Violet, 0x02 = Blue, 0x03 = Cyan, 0x04 = Green, 0x05 = White, 0x06 = Yellow, 0x07 = Red, 0x07 = Color Cycle)
XX = Bartop light color
YY = Pillow light color
ZZ = Exterior light color
OO = 00 for lights are off, 01 for lights are on.
```

Change Temperature 1 step up or down
Temp limits are 80 to 104 Fahrenheit, 26.5 to 40 Celsius
Steps are 1 fahrenheit or 0.5 celsius
XX = 0x01 for +1, 0x02 for +2, 0xFF for -1, 0xFE for -2...
```
<-- 01 1F 40 0109FFXX
<-- 1F 01 80 010906
```

Read main status (Polling)
```
<-- 01 1F 40 0256
<-- 1F 01 80 02560008000400000604000A0622F21100201C201C201C840360540000000031303346C87D740048C70A007944DB054500000000000000B2CB0C0000000000000000005C870F0002005F0BDB050000000052720A0031303146313032467B00FC00FC000000000000000000000000004D0000000000013C001E00005F7900331F061200D40701
                                                                                                                                                                                       XXXXXXXXXXXXXXXX
```

Main status decoding
```
1F                - Destination Spa Connection Kit (0x1F).
01                - Source IQ2020 (0x01).
80                - Response (0x40 = Request, 0x80 = Response).
0256              - Main Status Data.
0008              - ?
00                - Flags: 0x01 = Temp Lock, 0x02 = Spa Lock
04                - Jets active & cleaning cycle status.
00000604000A0622F21100201C201C201C8403605400000000  - ?
31303346          - "103F" ASCII string, not sure what this is. See temprature string below.
02937400ABC90A00  - ?
033CDC05          - Big endian counter
45000000000000007BD30C000000000000000000B8AF0F000200  - ?
E902DC05          - Big endian counter
00000000C2790A00  - ?
3130314631303246  - "101F102F" ASCII String, the target temp and current temp.
7800F900F900000000000000000000000000   - ?
5C                - Variable from 0x4D to 0xEA, changes almost at every poll.
00                - ? Could be part of the previous byte.
00                - Mostly 0x00, but occasionaly changes to 0x01 and 0x02.
000000013C001E00006A6F00  - ?
080800            - SS:MM:HH Seconds (0 to 59), Minutes (0 to 59), Hours (0 to 24).
13                - Number of days.
00D40701          - Maybe months and years?
```

Temperature string. The value encode the temperature set point and current temprature in ASCII encoding. The F indicates fahrenheit. For example:
```
" 92F102F" = 2039324631303246 (fahrenheit)
" 99F102F" = 2039394631303246 (fahrenheit)
"101F102F" = 3130314631303246 (fahrenheit)
"38.539.0" = 33382E3533392E30 (celsius)
"38.039.0" = 33382E3033392E30 (celsius)
```

Get Versions
```
--> 01 1F 40 0100
<-- 1F 01 80 01005752342E30346465316345303032444B342E303006
```

Get and set timestamp (in seconds). Send >60 in data to read the value.
```
--> 01 1F 40 024C99
<-- 1F 01 80 024C3431011400D40701
```

Lots more information using command 0x0255, This look a lot like 0x0256.
```
--> 01 1F 40 0255
<-- 1F 01 80 0255000800040000060400020622F21100201C201C201C84036054000000003130334617A3740006CC0A0061A5DD054500000000000000BBD30C0000000000000000005FB30F000200476CDD0500000000C2790A0020393546313032467800F800F800000000000000000000000000510000000000
```

Other Unknown Periodic Polling
Not sure what this is.
```
<-- 01 1F 40 1901
<-- 1F 01 80 190100190000000B0004010000
<-- 01 1F 40 1D07FF
<-- 1F 01 80 1D07FFFF
```

Sample Temperature Status Responses
This is not fully decoded.
```
 <-- 1F 01 80 0256000800540000060400020622F21100201C201C201C840360540000000031303346CE7E740048C70A00D84DDB054500000000000000B2CB0C000000000000000000258F0F000200BE14DB050000000052720A0020393246313032467900F900F900000000000000000000000000520006000000013C001E0000632F02320B071200D40701
 <-- 1F 01 80 0256000800540000060400020622F21100201C201C201C840360540000000031303346CE7E740048C70A00D44EDB054500000000000000B2CB0C00000000000000000021900F000200BA15DB050000000052720A0020393946313032467900F900F90000000000000000000000000053000E000000013C001E00006333030210071200D40701
 <-- 1F 01 80 0256000800540000060400020622F21100201C201C201C840360540000000031303346CE7E740048C70A004F50DB054500000000000000B2CB0C0000000000000000009C910F0002003517DB050000000052720A0020393946313032467800F900F900000000000000000000000000560005000000013C001E00006301041516071200D40701
 <-- 1F 01 80 0256000800540000060400020622F21100201C201C201C840360540000000031303346CE7E740048C70A000C51DB054500000000000000B2CB0C00000000000000000059920F000200F217DB050000000052720A0020393946313032467800F600F600000000000000000000000000520002000000013C001E00006323031E19071200D40701
 <-- 1F 01 80 0256000800540000060400020622F21100201C201C201C840360540000000031303346CE7E740048C70A00CA51DB054500000000000000B2CB0C00000000000000000017930F000200B018DB050000000052720A0020393946313032467800F600F600000000000000000000000000550005000000013C001E000063C304281C071200D40701
 <-- 1F 01 80 02560008005400000604000A0622F21100201C201C201C840360540000000031303346CE7E740048C70A00C353DB054500000000000000B2CB0C00000000000000000010950F000200A91ADB050000000052720A0031303146313032467600F300F300000000000000000000000000550002000000013C001E000064F5030525071200D40701
 <-- 1F 01 80 02560008005400000604000A0622F21100201C201C201C840360540000000031303346CE7E740048C70A003D55DB054500000000000000B2CB0C0000000000000000008A960F000200231CDB050000000052720A0031303146313032467800F700F70000000000000000000000000059000B000000013C001E000064F302172B071200D40701
 <-- 1F 01 80 02560008005400000604000A0622F25100201C201C201C840360540000000033392E35CE7E740048C70A009C5EDB054500000000000000B2CB0C0000000000000000008F9F0F0002008225DB050000000052720A0033382E3533392E307800F600F600000000000000000000000000550004000000013C001E000064AD021617081200D40701
 <-- 1F 01 80 02560008005400000604000A0622F25100201C201C201C840360540000000033392E35CE7E740048C70A00985FDB054500000000000000B2CB0C0000000000000000008BA00F0002007E26DB050000000052720A0033382E3033392E307800F700F700000000000000000000000000580006000000013C001E000063E802231B081200D40701
```
