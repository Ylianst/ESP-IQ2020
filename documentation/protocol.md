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
0x21 - Coolzone
0x24 - ACE Salt System
0x29 - Freshwater Salt System
0x33 - Audio/Music
0x37 - Freshwater IQ
0x38 - Unknown module
0x1D - Audio/Music
0x1F - Spa Connection Kit
```

# Freshwater Salt System

The Freshwater system will run current thru the salt water to generate chlorene. Note that there is also a Freshwater IQ module that is completely different. This module uses address `0x29` on the RS485 bus and seems to have a single `0x1E01` command that the IQ2020 controller polls regurarly with data. The communication between the IQ2020 and Freshwater system is super weird. I can't figure it out. As a result, there is currently no emulation for the Freshwater module in this integration.

Here is an example of a request from IQ2020 to the Freshwater module and the response:

```
29 01 40 1E010000FFFFFF00FF04FFFFFFFFFF    <-- Request
01 29 80 1E010000033500000000B310006841    <-- Response
```

When looking at the request, this is some data I have observed.

```
29 01 40 1E010000FFFFFF00FF04FFFFFFFFFF
             AABB        CCCCCCCC
AA = State, FF = No Module, 00 = Active?, 01 = Active?
BB = Power, 00 to 04 for salt power level.
CC = Indicates what buttons have been pressed on the user interface
       0x0101FFFF = Go to mode 1
       0xFF01FFFF = Read State, Mode 1
       0xFF0402FF = Go to mode 4
       0xFF04FFFF = Read State, Mode 4
       0x020102FF = Reset Cartridge Age
       0xFF01FF01 = Wheel press in mode 1
       0xFF04FF01 = Wheel press in mode 4
       0xFF0101FF = Goto boost mode (5 minutes)
```

Looking at the response, here is some things I have noticed:

```
01 29 80 1E010000073500030000B310006841
                   AABBCCDD
AA = Cartridge age, 00 is new.
BB = Unknown value 0x00, 0x06, 0x07, 0x08
CC = Boost Mode, 01 or 03 is normal, 23 is boost.
DD = Error code
```

I don't personally use the Freshwater module, but if someone uses it and can capture it's usage over time and figure out the protocol, please let me know.

# Freshwater IQ

Freshwater Salt System and Freshwater IQ modules are different. The Freshwater Salt System module will send current thru the water to create chlorine, the IQ module will monitors Ph and Chlorine levels in the hot tub. The IQ module uses addresses `0x37` on the RS485 bus. The list of commands that have been seen in logs are:

```
23A1 - Unknown
23A3 - Unknown
23D1 - Chlorine, Ph and more
23D5 - Unknown
23DC - Unknown
```

The most common command is the IQ2020 controller polling for data using command `0x23D5`. The first command polls the data and the second is the data returned from the Freshwater IQ module. This happens frequently, not sure what this contains.

```
<-- 37 01 40 23D500
<-- 01 37 80 23D5000000003234313130304E30A76EBD3915FF7E9C30303744000000040000000400000004000031500000271000002710000007E2000007E2
                                                                                                                 AAAAAAAABBBBBBBB

AAAAAAAA - Cartridge use in hours (Small-Endian)
BBBBBBBB - Cartridge use in hours (Small-Endian)
```

This next command we know nothing about. It seems to set a value.

```
<-- 37 01 40 23A1016E
<-- 01 37 80 23A100
```

This next command we know nothing about.

```
<-- 37 01 40 23A300
<-- 01 37 80 23A300
```

Every hour, we see this `0x23D1` command. This seems to contain the Chlorine, Ph and life remaining for the Freshwater IQ cartridge in hours. The command seems to return 7 x 32bit integers.

```
<-- 37 01 40 23D100
<-- 01 37 80 23D100000000000000000000087A000003090000000F0000004B00001F2C
                 AAAAAAAABBBBBBBBCCCCCCCCDDDDDDDDEEEEEEEEFFFFFFFFGGGGGGGG

AA = Unknown, always zero.
BB = Unknown, always zero.
CC = Unknown, 32 bit small-endian.
DD = Unknown, 32 bit small-endian.
EE = Chlorine in 10ths of PPM, 32 bit small-endian, Convert to decimal and divide by 10.
FF = Ph in 10ths, 32 bit small-endian, Convert to decimal and divide by 10.
GG = Hours remaining count-down, 32 bit small-endian.
```

This next command is completely unknown. It seems to contain 9 x 32 bit integers.

```
<-- 37 01 40 23DC00
<-- 01 37 80 23DC000000000000003B0000004C0000044C0000059B0000022F000002770000022700000272
                 AAAAAAAABBBBBBBBCCCCCCCCDDDDDDDDEEEEEEEEFFFFFFFFGGGGGGGGHHHHHHHHIIIIIIII

All values are unknown.
```

# Spa Connection Kit

The Spa Connection Kit sends all it's commands from address 0x1F. Sending the commands from a different address will not work. Each request command from 0x1F to the IQ2020 on address 0x01 starts with two bytes. I sent all possible commands (64k of them) and recorded any responses. I found the following commands this way

```
0100 - Get version string
0109 - Change temperature
0241 - Unknown (02413C001E0000)
024C - Unknown (024C1B0C110300E80701)
0255 - Get status short
0256 - Get status long
0273 - System reset
0B02 - Set Jets 1 speed - (0B028F)
0B03 - Set Jets 2 speed - (0B038E)
0B04 - Set Jets 3 speed - (0B0400)
0B07 - Set Blower
0B1C - Summer Timer
0B1D - Spa Lock
0B1E - Temperature Lock
0B1F - Clean Cycle
0B20 - Unknown (0B2001)
0B27 - Change interior light level
1702 - Set Lights on/off
1705 - Get Lights status
1900 - Audio Module Command (190015)
1901 - Get Audio Module Data (190100190000000B0004010000)
1D07 - Get/Set Heat Pump Mode (1D07FFFF)
1E02 - Freshwater Salt System - Set Power / Start Test
1E03 - Get FreshWater Salt Module Data
```

Here are the known commands:

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

Decreases Interior lights Dim level
```
<-- 01 1F 40 0B2702
<-- 1F 01 80 0B2700
```

Increase Interior lights Dim level
```
<-- 01 1F 40 0B2703
<-- 1F 01 80 0B2700
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

Summer Timer ON
```
<-- 01 1F 40 0B1C02
<-- 1F 01 80 0B1C00
```

Summer Timer OFF
```
<-- 01 1F 40 0B1C01
<-- 1F 01 80 0B1C01
```

Clean Cycle ON
```
<-- 01 1F 40 0B1F02
<-- 1F 01 80 0B1F01
```

Clean Cycle Off
```
<-- 01 1F 40 0B1F01
<-- 1F 01 80 0B1F00
```

Read light status (Polled)
```
<-- 01 1F 40 1705
<-- 1F 01 80 17050005020202000000000000000005010101010000
<-- 1F 01 80 17050003000000010000000300000003010101010000
                 FFAABBCCDD        LLMMNNOOWWXXYYZZOO

FF = Flags (0x01 = Lights Timer On)
AA = Underwater light intensity (0x05 = High ... 0x01 = Low, 0x00 = Off)
BB = Bartop light intensity
CC = Pillow light intensity
DD = Exterior light intensity
LL = Underwater light cycle speed - Main loop speed
MM = Bartop light cycle speed
NN = Pillow light cycle speed
OO = Exterior light cycle speed
WW = Underwater light color     (0x01 = Violet, 0x02 = Blue, 0x03 = Cyan, 0x04 = Green, 0x05 = White, 0x06 = Yellow, 0x07 = Red, 0x08 = Color Cycle)
XX = Bartop light color
YY = Pillow light color
ZZ = Exterior light color
OO = 00 for lights are off, 01 for lights are on.
```

Change light status
```
--> 01 1F 40 17020102
<-- 1F 01 80 170206

--> 01 1F 40 17020102
                 NNAA
NN = Light Number 0x00 to 0x03 for individual light, 0x04 for all lights. 0x05 to 0x08 seem to also work?
AA = Action
  02 = Lower Brightness (0 to 3/5 depending on model)
  03 = Increase Brightness (0 to 3/5 depending on model)
  04 = Previous color (1 to 7 and cycles back)
  05 = Next color (1 to 7 and cycles back)
  06 = Decrease cycle speed (0 to 3)
  07 = Increase cycle speed (0 to 3)
  08 = Enable lights cycling
  09 = Disable lights cycling
  10 = Turn off
  11 = Turn on
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
0256              - Main Status Data Command.
0008              - ?
00                - Flags: 0x01 = Temp Lock, 0x02 = Spa Lock, 0x04 = Jet1 Full, 0x08 = Jet2 Full, 0x10 = Clean Cycle, 0x20 = Summer Timer, 0x40 = Salt Level Confirmed
04                - Flags: 0x01 = Jet1 Medium, 0x02 = Jet2 Medium, 0x04 = AlwaysSet?. 4 upper bits are spa lights: 0x0 = Off, 0x1 = Min Bright, 0x5 = Max Bright
0000              - ?
06                - Model type.
0400              - ?
0A                - Logo status right. 0x0A both blue & green on, 0x02 only blue is on... see below for all states.
0622F2            - ?
11                - Flags: 0x40 is Celsius
00                - ?
201C201C201C      - Pump1, Pump2, Pump3 timeouts (3 x 16bit Big-Endian)
8403              - Blower timeout
6054              - Lights timeout
00000000          - Pump1 speed, pump2 speed, pump3 speed, blower speed
31303346          - "103F" ASCII string, the heater outlet temperature.
02937400          - Heater total runtime in seconds (Big-Endian)
ABC90A00          - Jets 1 total runtime in seconds (Big-Endian)
033CDC05          - Lifetime 1 runtime in seconds (Big-Endian)
45000000          - Power on / Boot counter (Big-Endian)
00000000          - Flags: stm_state, spa_lock_state, temp_lock_state, clean_lock_state
7BD30C00          - Jets 2 total runtime in seconds (Big-Endian)
00000000          - Jets 3 total runtime in seconds (Big-Endian)
00000000          - Blower total runtime in seconds (Big-Endian)
B8AF0F00          - Lights total runtime in seconds (Big-Endian)
02                - SPA state
00                - SPA light state
E902DC05          - Circulation pump runtime in seconds (Big-Endian)
00000000          - Jets 1 low operation in seconds (Big-Endian)
C2790A00          - Jets 2 low operation in seconds (Big-Endian)
3130314631303246  - "101F102F" ASCII String, the target temp and current temp.
7800              - Voltage L1 (Big-Endian)
F700              - Voltage Heater (Big-Endian)
F700              - Voltage L2 (Big-Endian)
0000              - Voltage Jet 3 (Big-Endian)
0000              - Current L1 (Amps)
0000              - Current Heater (Amps)
0F00              - Current L2 (Heater power: 0x0000 = Off, 0x0F00 = 15 Amps)
0000              - Current Jet 3 (Amps)
0000              - Power L1 wattage (Big-Endian)
5C00              - Power Heater wattage (Big-Endian)
F30E              - Power L2 wattage - Water heater wattage (Big-Endian)
0000              - Power Jet 3 (Big-Endian)
01                - Daily Clear Cycle
3C00              - Filter Time 1
1E00              - Filter Time 2
00                - Flags: Hawk status econ / Hawk status circ
6A                - PCB temperature
6F                - Peripheral Current
00080800          - Real-Time-Clock SS:MM:HH Seconds (0 to 59), Minutes (0 to 59), Hours (0 to 24).
1300D407          - Real-Time-Clock DD:MM:YYYY Days (1 to 31), Month (0 to 11), Year (2 byte Big-Endian).
01                - Real-Time-Clock Status
```

Logo lights state
```
Ready flash                =   xx == 0x06
Power and ready flash      =   xx == 0x05
Power flash                =   xx == 0x09
Power & ready on           =   xx == 0x0A
Power on                   =   xx == 0x02
Power & ready alternate    =   xx & 35 == 32
Power & ready salt error   =   xx & 28 == 16
```

Temperature string. The value encode the temperature set point and current temprature in ASCII encoding. The F indicates fahrenheit. For example:
```
" 92F102F" = 2039324631303246 (fahrenheit)
" 99F102F" = 2039394631303246 (fahrenheit)
"101F102F" = 3130314631303246 (fahrenheit)
"38.539.0" = 33382E3533392E30 (celsius)
"38.039.0" = 33382E3033392E30 (celsius)
```

Read FreshWater/ACE Salt Module Data (Polled)
```
<-- 01 1F 40 1E03
<-- 1F 01 80 1E030003FFFF00FF006800FF07FF0000B3100040
```

FreshWater/ACE Salt Module Data Decoding
```
1E03
00 - spa_usage (0 to 10)
03 - spa_size
FF - g3_sensor_data
FF - g3_sensor_status
00 - dosing_state
FF - ?
00 - g3_level2_errors
6800FF - ?
07 - salline_test (salinity test)
FF - g3_chlor_test_data (chlorine test)
00 - g3_ph_test_data (Ph)
00 - g3_clrmtr_test_data (colorimeter test)
B3100040 - ?
```

Freshwater Salt System - Set Power
```
 <-- 01 1F 40 1E0201XX00                                - XX is 0x00 (0) to 0x0A (10) power level
 <-- 1F 01 80 1E0206
 <-- 1F 01 40 1E03XX03FFFF00FF006800FF07FF0000B3100040  - XX is 0x00 (0) to 0x0A (10) power level
```

Freshwater Salt System - Test Start
```
 <-- 01 1F 40 1E02030400
 <-- 1F 01 80 1E0206
 <-- 1F 01 40 1E030003FFFF00FF006800FF07FF0000B3100040
```

Freshwater Salt System - Test Stop
```
 <-- 01 1F 40 1E02030000
 <-- 1F 01 80 1E0206
 <-- 1F 01 40 1E030003FFFF00FF006800FF07FF0000B3100040
```

Freshwater Salt System - Boost Start
```
 <-- 01 1F 40 1E02030800
 <-- 1F 01 80 1E0206
 <-- 1F 01 40 1E030003FFFF00FF006800FF07FF0000B3100040
```

Freshwater Salt System - Boost Stop
```
 <-- 01 1F 40 1E02030000
 <-- 1F 01 80 1E0206
 <-- 1F 01 40 1E030003FFFF00FF006800FF07FF0000B3100040
```

Get Versions
```
--> 01 1F 40 0100
<-- 1F 01 80 01005752342E30346465316345303032444B342E303006
```

Get Current Time
```
--> 01 1F 40 024C
<-- 1F 01 80 024C3431011400D40701
                 SSMMHHDDMMYYYY
Encoded as:
SS:MM:HH Seconds (0 to 59), Minutes (0 to 59), Hours (0 to 24).
DD:MM:YYYY Days (1 to 31), Month (0 to 11), Year (2 byte Big-Endian).
```

Set Current Time
```
--> 01 1F 40 024C2C2B140405E807
<-- 1F 01 80 024C2C2B140405E80701

Same encoding as Get Current Time.
```

Command 0x0255 is just like command 0x0256 but shorter. Some information at the end is missing.
```
--> 01 1F 40 0255
<-- 1F 01 80 0255000800040000060400020622F21100201C201C201C84036054000000003130334617A3740006CC0A0061A5DD054500000000000000BBD30C0000000000000000005FB30F000200476CDD0500000000C2790A0020393546313032467800F800F800000000000000000000000000510000000000
```

Poll for audio module status
```
<-- 01 1F 40 1901
<-- 1F 01 80 190100190000000B0004010000
```

Audio module data is as follows
```
1F       - Destination Spa Connection Kit (0x1F).
01       - Source IQ2020 (0x01).
80       - Response (0x40 = Request, 0x80 = Response).
1901     - Command 0x1D07 Audio Module Status
00       - Power (0 = Off, 1 = On)
19       - Volume
00       - Treble
00       - Bass
00       - Balance
0B       - Subwoofer volume
00       - Play/pause status
04       - Source selection (2 = Wireless, 3 = Aux, 4 = Bluetooth)
01       - Wireless channel
00       - Radio signal strength
00       - Bluetooth pairing
```

Audio commands from and to the internet kit
```
01 1F 40 1900 030400        - Audio source Bluetooth
01 1F 40 1900 030300        - Audio source aux
01 1F 40 1900 030200        - Audio source wireless
01 1F 40 1900 040100        - Audio on
01 1F 40 1900 040000        - Audio off
01 1F 40 1900 01xx          - Set Volume
01 1F 40 1900 02xx          - Set Play/Pause/Next/Back
01 1F 40 1900 03xx          - Set Source
01 1F 40 1900 05xx          - Set Treble
01 1F 40 1900 06xx          - Set Base
01 1F 40 1900 07xx          - Set Balance
01 1F 40 1900 08xx          - Set Subwoofer
01 1F 40 1900 09xx          - Set Unknown ?
1F 01 80 1900 06            - Confirmation
```

Audio commands to the audio module
```
33 01 40 190001130B000800
               VVTTCCBBSS

VV = Volume (0x0F to 0x18)
TT = Treble (0xFB to 0x05)
CC = Bass (0xFB to 0x05)
BB = Balance (0xFB to 0x05)
SS = Subwoofer (0x00 to 0x0B)

33 01 40 1901 00            - Poll state
33 01 40 1901 01            - Audio Play
33 01 40 1901 02            - Audio Pause
33 01 40 1901 03            - Audio Next
33 01 40 1901 04            - Audio Previous
33 01 40 1903 02            - Audio source wireless (TV)
33 01 40 1903 03            - Audio source aux
33 01 40 1903 04            - Audio source Bluetooth
33 01 40 1906 00            - Poll song name
33 01 40 1907 00            - Poll artist name
01 33 80 1901 00190000000B0004010000 - Confirm with state
```

Return song/artist name
```
01 33 80 1906 xxxx          - ASCII song title, 20 chars max
01 33 80 1907 xxxx          - ASCII artist name, 20 chars max
```

Poll for heat pump status
```
01 1F 40 1D07 FF
1F 01 80 1D07 FFFF
```

Heat pump data is as follows
```
1F        - Destination Spa Connection Kit (0x1F).
01        - Source IQ2020 (0x01).
80        - Response (0x40 = Request, 0x80 = Response).
1D07      - Command 0x1D07 Heat Pump Status
FF        - Heat pump current operational status
FF        - Heat pump current mode setting
```

IQ2020 Reboot Command
```
01 1F 40 0273 3487E5
```
