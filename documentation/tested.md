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
