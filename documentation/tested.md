# Tested Hot Tubs

In this document, I want to keep notes on the currently tested and confirmed to work hot tubs with notes on configuration changes, etc.

## Hotspring Grandee

- Status: Works great. Lights, temperature, Pump1 with 1 speed, Pump2 with 2 speeds.
- Version: WR4.04de1cE002DK4.00
- Modules: FreshWater® Salt System
- Interface: M5Stack-ATOM + Tail485

Notable Settings:
```
uart:
  id: SpaConnection
  tx_pin: GPIO26
  rx_pin: GPIO32
  baud_rate: 38400

# Jets 1 and 2 setup, others removed
fan:
  - platform: iq2020
    name: Jets 1
    id: jets1
    icon: "mdi:turbine"
    datapoint: 0
    speeds: 1
  - platform: iq2020
    name: Jets 2
    id: jets2
    icon: "mdi:turbine"
    datapoint: 1
    speeds: 2
```

## HotSpring HotSpot Stride 2021

- Status: Reported to work, Pump1 has 2 speeds but does not report speed correctly.
- Version: ?
- Modules: None
- Interface: M5Stack ATOM Lite ESP32 with ATOMIC RS485

Notable Settings:
```
uart:
  id: SpaConnection
  tx_pin: GPIO19
  rx_pin: GPIO22
  baud_rate: 38400

# Jets 2 and after are removed.
fan:
  - platform: iq2020
    name: Jets 1
    id: jets1
    icon: "mdi:turbine"
    datapoint: 0
    speeds: 2
```

## Hotspring Envoy 2017

 - Status: Works great.
 - Version: WR2.02ad1bE002DK2.02
 - Modules: ACE system

## Hotspring Sovereign 2015

 - Status: Works great but lifetime indicator shows over 29 years which is not accurate.
 - Version: 1.08.D0172E002HL1.02
 - Modules: None

## Hotspring Relay 2024

- Status: Works, but power indicators for Pumps & Heater always shows zero.
- IQ2020 Watkins 1303401-1 controller

## HotSpring HotSpot SX

- Status: Works, however the light color cycle, use the following sensor:
- Version: 112T b5e1A002 v3.0
- M5stack atom lite + Atomic RS485 base

```
sensor:
  lights_main_loop_speed:
    name: Lights Main Loop Speed
```
## Caldera Niagara (Utopia Series) 2022

- Status: Works great, Reports power and everything.
- Version: EU2.039018E0020.00.0

## HotSpring Limelight Pulse 2023

- Status: Working well but ACE boost feature does not work and details on controlling lights still being worked on.
- Version: 2.04D 1 E002HL22.0
- Modules: ACE

On the remote colors are:

```
1 = Blue, 
2 = Aqua, 
3 = Green, 
4 = White, 
5 = Yellow, 
6 = Red, 
7 = Magenta, 
8 = Color Wheel On, 
9 = Color Wheel Off
```

Also, light intensity is 0 to 3, not 0 to 5 like other models.

## Hot Springs Limelight Flash 2018

- Status: Works except that the light control colors are off by one (violet=blue, etc.) and wheel doesn't cycle, although the control panel flashes the light icon like it does when cycling normally.
- Version: 1.14D6ce5E002
- No audio or salt systems to test.

## Hot Spring Sovereign

- Status: The lights names and colors are off compared to the example given on this site. Here is the light settings that works:

```
select:
  - platform: iq2020
    name: Color Underwater
    id: lights1_color
    datapoint: 1
    options:
      - Blue
      - Cyan
      - Green
      - White
      - Yellow
      - Red
      - Magenta
      - Cycle
  - platform: iq2020
    name: Color Waterfall
    id: lights2_color
    datapoint: 2
    options:
      - Blue
      - Cyan
      - Green
      - White
      - Yellow
      - Red
      - Magenta
      - Cycle
  - platform: iq2020
    name: Color Bar Top
    id: lights3_color
    datapoint: 3
    options:
      - Blue
      - Cyan
      - Green
      - White
      - Yellow
      - Red
      - Magenta
      - Cycle
  - platform: iq2020
    name: Color Pillow
    id: lights4_color
    datapoint: 4
    options:
      - Blue
      - Cyan
      - Green
      - White
      - Yellow
      - Red
      - Magenta
      - Cycle
  - platform: iq2020
    name: Color Cycle Speed
    id: lights_cycle_speed
    datapoint: 5

number:
  - platform: iq2020
    id: lights1_intensity
    name: Intensity Underwater
    datapoint: 7
    maximum: 5
  - platform: iq2020
    id: lights2_intensity
    name: Intensity Waterfall
    datapoint: 8
    maximum: 5
  - platform: iq2020
    id: lights3_intensity
    name: Intensity Bar Top
    datapoint: 9
    maximum: 5
  - platform: iq2020
    id: lights4_intensity
    name: Intensity Pillow
    datapoint: 10
    maximum: 5
```

## Tiger River Spa Caspian

Does no work. This hot tub seems to be using a older version of the IQ2020 board, used between 2001 to mid-2009 and this integration does not seems to work. Here is an image of the board.

![PXL_20240801_080032524 MP](https://github.com/user-attachments/assets/17ea8c9a-3403-4e8d-94f5-eeb5c6b922ac)

## Hotsprings Limelight Flash (2021)

- Status: Works

Yaml Changes Made in ESPHome

```
fan:
  - platform: iq2020
    name: Jets 1
    id: jets1
    icon: "mdi:turbine"
    datapoint: 0
    speeds: 2 #to 2 from 1
  - platform: iq2020
    name: Jets 2
    id: jets2
    icon: "mdi:turbine"
    datapoint: 1
    speeds: 1 #to 1 from 2
```

Result:
Jets 1 has off/low/high speeds
Jets 2 has off/high speeds

Controller image:
![356781975-cb7addd6-c6bf-4cb5-93f2-518f8f7cbe94](https://github.com/user-attachments/assets/c7d7875d-1afa-4b7b-8cc7-b8bb5f2661f0)

## HotSpring HotSpot SX 2017

Status: Works, but lights can't be controlled correctly, it's a single color lights with off/dim/full setting. Pump power sensor shows always zero.
Version: 110T 7b54A002 v2.6

![369696446-a95b9ca2-f8eb-4d17-891e-87551b0073e5](https://github.com/user-attachments/assets/8045b8af-f5ee-48ee-9921-df06cd55e7de)

## 2004 Hotspring Prodigy-P

Status: Does NOT work. This is a pre-2014 spa. The connector will look the same but the pins for the data and voltage are different.

![image](https://github.com/user-attachments/assets/7ea47713-c6d8-47fc-a8b3-646dad773929)

