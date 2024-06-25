# Tested Hot Tubs

In this document, I want to keep notes on the currently tested and confirmed to work hot tubs with notes on configuration changes, etc.

## Hotsprings Grandee

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

- IQ2020 Watkins 1303401-1 controller
- Status: Works, but power indicators for Pumps & Heater always shows zero.

## HotSpring HotSpot SX

- Version string: 112T b5e1A002 v3.0
- M5stack atom lite + Atomic RS485 base
- Status: Works, however the light colors and cycles are not currently supported correctly.
