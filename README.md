# ESP-IQ2020
-- Only compatible with 2014 spas and newer --

Connect your IQ2020 powered Hot Tub to Home Assistant to make your Hot Tub a lot smarter. You will be able to remotely control temperatue, lights, jets, lock the spa remote, monitor power usage and more. You can graph temperature and power usage, control temperature for time-of-day electric rates or when on vacation, get notified when the tub is in use, lock the spa remote control when the house in away mode, blink the tub lights when someone rings the doorbell and more.

![IQ2020-ESP1b](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/0ba0a473-8653-4b65-8338-052c8237fb5b)

There is a video shows off the integration and how to get it installed.

[![IQ2020 Hot Tub connected to Home Assistant](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/5d55095e-eba0-4b3b-8058-8033d20e062b)](https://youtu.be/egX6bspzuqo)

[Short Video (5:38)](https://www.youtube.com/watch?v=egX6bspzuqo), [Long Video (24:42)](https://youtu.be/OjBa2vJ3cmw)

The IQ2020 is the control board used by a lot of hot tubs, so check if you have this board. You will need to have [Home Assistant](https://www.home-assistant.io/) as your smart home controller and to buy a small device, flash the right firmware on the device using ESP-Home and then connect the device using 4 wires to your hot tub. The device will be powered by the hot tub and has built-in WIFI, so everything stays within the control box, no messy wires. First you will need to buy a ESP32 device and a RS485 interface for it. I recommand this exact hardware (~26$ US):

- [ATOM Lite ESP32 IoT Development Kit](https://shop.m5stack.com/products/atom-lite-esp32-development-kit)
- [ATOM Tail485 - RS485 Converter for ATOM](https://shop.m5stack.com/products/atom-tail485)
- [5 Colors 1Pin 2.54mm Female to Male Breadboard Jumper Wire](https://www.amazon.com/XLX-Breadboard-Soldering-Brushless-Double-end/dp/B07S839W8V/ref=sr_1_3)

Once you get the device, connect it to your computer using a USB-C table, create a new ESP home device, call it "Hot Tub" or anything you like, select `ESP32`. Once created, edit the configuration file to look like the one below. You should keep your own API encryption key and OTA password, but everything else can be copied from this example.

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
  - platform: esphome
    password: "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"

wifi:
  ssid: !secret wifi_ssid
  password: !secret wifi_password

external_components:
  - source: github://ylianst/esp-iq2020

# Make sure tx/rx pins are correct for your device.
# GPIO26/32 is ok for M5Stack-ATOM + Tail485, look in GitHub devices link for your device.
uart:
  id: SpaConnection
  tx_pin: GPIO26
  rx_pin: GPIO32
  baud_rate: 38400

iq2020:
   uart_id: SpaConnection
   polling_rate: 65
   port: 1234

select:
text:
number:

# If using celsius units on the hot tub remote, replace _f_ with _c_ in the three entries below.
# Feel free to remove any sensor that are not relevent for your hot tub.
sensor:
  - platform: iq2020
    current_f_temperature:
      name: Current Temperature
    target_f_temperature:
      name: Target Temperature
    outlet_f_temperature:
      name: Heater Outlet
    power_l1:
      name: Pumps Power
    power_heater:
      name: Power Heater
    power_l2:
      name: Heater Power
    pcb_f_temperature:
      name: Controller Temperature

switch:
  - platform: iq2020
    name: Lights
    id: lights_switch
    icon: "mdi:lightbulb"
    datapoint: 0
  - platform: iq2020
    name: Spa Lock
    id: spa_lock_switch
    icon: "mdi:lock"
    datapoint: 1
  - platform: iq2020
    name: Temperature Lock
    id: temp_lock_switch
    icon: "mdi:lock"
    datapoint: 2
  - platform: iq2020
    name: Clean Cycle
    id: clean_cycle_switch
    icon: "mdi:vacuum"
    datapoint: 3
  - platform: iq2020
    name: Summer Timer
    id: summer_timer_switch
    icon: "mdi:sun-clock"
    datapoint: 4

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

# Set "celsius" to "true" if using celsius units.
climate:
  - platform: iq2020
    name: Temperature
    celsius: false

text_sensor:
  - platform: iq2020
    versionstr:
      name: Version
```

You may need to make a few changes. If your hot tub is setup to display temperature in celsius, replace all `xxx_f_temperature` with `xxx_c_temperature` and set `celsius` to true in the `climate` section. Make sure you put your WIFI SSID and Password in `secrets.yaml`. Once ready, go ahead and flash your device over USB-C. At this point, the device should be visible over WIFI when powered using USB-C even if it's not connected to the computer.

Next, grab 4 breadboard jumper wires and connect them to the RS485 module. I recommand using 4 different color wires. Idealy blue, yellow, red and black. Put the male end in the RS485 module and tighten using a small screw driver. Double check the wires don't come off. Your device should look like this.

![IQ2020-ESP2](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/434920d7-ad5b-446c-af8e-142df2a1e9d8)

Next, power off your hot tub and connect your new device like this:

![image](https://github.com/user-attachments/assets/98956d57-a019-4aae-a9b1-cd4c24e18b67)

In the picture below you will notice I have the expansion board attached with 8 expension connectors, your hot tub will generally have 1 or 2 expansion connectors. If they are all busy, you will need to get an expansion board. Double check all the wires, you should not need to force anything, the wires should fit just right.

![IQ2020-ESP3](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/c52b676b-e35c-474c-8919-2fc57302d0fb)

Once done, power your hot tub back on and you should see data flowing into Home Assistant. You can see the current temperature, set the target temperature, lock the remote control, turn on lights & jets and graph the temperature and power usage. The spa data is polled by the device every minute, so, if you change a setting using the tub's remote, it may take up to a minute to update on Home Assistant. If something does not work right, [please open an issue in GitHub](https://github.com/Ylianst/ESP-IQ2020/issues). As with all Home Assistant integrations, you can use automations. For example, I am on a electric time-of-day plan and so, I adjust lower the temperature automatically a few minutes before 5pm and turn it back up at 9pm. There are also sensors provided so you can create tracking graphs.

For added features and details:
  - [More Sensors, Templates and Extras](https://github.com/Ylianst/ESP-IQ2020/blob/main/documentation/extras.md)
  - [Audio Module Emulation](https://github.com/Ylianst/ESP-IQ2020/blob/main/documentation/audio.md)
  - [ACE Module Emulation](https://github.com/Ylianst/ESP-IQ2020/blob/main/documentation/ace.md)
  - [Using different ESP32 devices](https://github.com/Ylianst/ESP-IQ2020/blob/main/documentation/devices.md)
  - [Details on the RS485 serial protocol](https://github.com/Ylianst/ESP-IQ2020/blob/main/documentation/protocol.md)
  - [Debugging RS485 traffic](https://github.com/Ylianst/ESP-IQ2020/blob/main/documentation/debugging.md)
  - [Variable Electric Rate Savings](https://github.com/Ylianst/ESP-IQ2020/blob/main/documentation/electric.md)
  - [Full configuration example](https://github.com/Ylianst/ESP-IQ2020/blob/main/documentation/configuration.md)
  - [Tested Hot Tubs](https://github.com/Ylianst/ESP-IQ2020/blob/main/documentation/tested.md)

Known Issues:
  - Older IQ2020 board used between 2001 to mid-2009 may not work with this integration. See [Tested Hot Tubs](https://github.com/Ylianst/ESP-IQ2020/blob/main/documentation/tested.md) for details.
  - Jet 3 may work but will not show the running state properly, but if we have such a hot tub and can send me traffic logs, I can probably add support for it.
