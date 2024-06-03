# ESP-IQ2020
Connect your IQ2020 powered Hot Tub to Home Assistant. You will be able to remotely control temperatue, lights, jets, lock the spa remote, monitor power usage and more. It will make your hot tub and home a lot smarter. You can graph temperature and power usage, control temperature for time-of-day electric rates or when on vacation, get notified when the tub is in use, lock the spa remote control when the house in away mode, blink the tub lights when someone rings the doorbell and more.

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/b3143b60-84fe-421e-9225-c157ae8f650c)

The IQ2020 is the control board used by a lot of hot tubs, so check if you have this board. You will need to buy a small device, flash the right firmware on the device using ESP-Home and then connect the device using 4 wires to your hot tub. The device will be powered by the hot tub and has built-in WIFI, so everything stays within the control box, no messy wires. First you will need to buy a ESP32 device and a RS485 interface for it. I recommand this exact hardware (~26$ US):

- [ATOM Lite ESP32 IoT Development Kit](https://shop.m5stack.com/products/atom-lite-esp32-development-kit)
- [ATOM Tail485 - RS485 Converter for ATOM](https://shop.m5stack.com/products/atom-tail485)
- [5 Colors 1Pin 2.54mm Female to Male Breadboard Jumper Wire](https://www.amazon.com/XLX-Breadboard-Soldering-Brushless-Double-end/dp/B07S839W8V/ref=sr_1_3)

Once you get the device, connect it to your computer using a USB-C table, create a new ESP home device, call it "Hot Tub" or anything you like, select `ESP32`. Once created, edit the configuration file to look like the one below. You should keep you own API encryption key and OTA password, but everything else can be copied from this example.

```
esphome:
  name: hot-tub
  friendly_name: Hot Tub
  comment: "Luxury Spa"

esp32:
  board: m5stack-atom

logger:
  baud_rate: 0
  level: ERROR

# Enable Home Assistant API
api:
  encryption:
    key: "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"

ota:
  password: "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"

wifi:
  ssid: !secret wifi_ssid
  password: !secret wifi_password

external_components:
  - source: github://ylianst/esp-iq2020

uart:
  id: SpaConnection
  tx_pin: GPIO26
  rx_pin: GPIO32
  baud_rate: 38400

iq2020:
   uart_id: SpaConnection
   port: 1234

# If using celsius units on the hot tub remote, replace _f_ with _c_ in the two entries below. 
sensor:
  - platform: iq2020
    current_f_temperature:
      name: Current Temperature
    target_f_temperature:
      name: Target Temperature
    heater_wattage:
      name: Heater Power

switch:
  - platform: iq2020
    name: Lights
    id: lights_switch
    icon: "mdi:lightbulb"
    switch_datapoint: 0
  - platform: iq2020
    name: Spa Lock
    id: spa_lock_switch
    icon: "mdi:lock"
    switch_datapoint: 1
  - platform: iq2020
    name: Temp Lock
    id: temp_lock_switch
    icon: "mdi:lock"
    switch_datapoint: 2
  - platform: iq2020
    name: Clean Cycle
    id: clean_cycle_switch
    icon: "mdi:vacuum"
    switch_datapoint: 3
  - platform: iq2020
    name: Summer Timer
    id: summer_timer_switch
    icon: "mdi:sun-clock"
    switch_datapoint: 4

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

climate:
  - platform: iq2020
    name: Temperature
```

You may need to make a few changes. If your hot tub is setup to display temperature in celsius, replace `current_f_temperature` and `current_f_temperature` with `target_c_temperature` and `target_c_temperature`. Make sure you put your WIFI SSID and Password in `secrets.yaml`. Once ready, go ahead and flash your device over USB-C. At this point, the device should be able be visible over WIFI when powered using USB-C even if it's not connected to the computer.

Next, grab 4 breadboard jumper wires and connect them to the RS485 module. I recommand using 4 different color wires, idealy blue, green, red and black. Put the male end in the RS485 module and tighten using a small screw driver. Double check the wires don't come off. Your device should look like this.

![IQ2020-ESP2](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/434920d7-ad5b-446c-af8e-142df2a1e9d8)

Next, power off your hot tub and connect your new device like this:

![IQ2020-ESP1](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/07697b93-9469-46b6-9f8b-8a79d4cd90d3)

In the picture below you will notice I have the expansion board attached with 8 expension connectors, your hot tub will generally have 1 or 2 expansion connectors. If they are all busy, you will need to get an expansion board. Double check all the wires, you should not need to force anything, the wires should fit just right.

![IQ2020-ESP3](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/c52b676b-e35c-474c-8919-2fc57302d0fb)

Once done, power your hot tub back on and you should see data flowing into Home Assistant. You can see the current temperature, set the target temperature, lock the remote control, turn on lights & jets and graph the temperature and power usage. If a jet has two power levels, you should be able to see and control them. The spa data is polled by the device every minute, so, if you change a setting using the tub's remote, it may take up to a minute to update on Home Assistant. If something does not work right, please open an issue in GitHub. As with all Home Assistant integrations, you can use automations. For example, I an electric time-of-day plan and so, I adjust the temperature accordingly. There are also sensors provided so you can create tracking graphs.

For added details:
  -
  -
  -
