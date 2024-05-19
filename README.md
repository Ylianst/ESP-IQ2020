# ESP-IQ2020
Connect a ESP-Home device to a Hot Tub with a IQ2020 control board. This project intends to connect the hot tub to Home Assistant. First you will need a ESP32 device with a RS485 interface. For this project I recommand a M5Stack device since they are simple to connect and come in a small case.

If you want my exact hardware, but many alternatives exist:
- [ATOM Lite ESP32 IoT Development Kit](https://shop.m5stack.com/products/atom-lite-esp32-development-kit)
- [ATOM Tail485 - RS485 Converter for ATOM](https://shop.m5stack.com/products/atom-tail485)
- [5 Colors 1Pin 2.54mm Female to Male Breadboard Jumper Wire](https://www.amazon.com/XLX-Breadboard-Soldering-Brushless-Double-end/dp/B07S839W8V/ref=sr_1_3)

I am using Home Assistant with ESP-Home and using the following configuration YAML file to build the device firmware. This firmware will just display all RS485 traffic to the log file and do nothing else. Once the IQ2020 commands are decoded, we can create a correct ESP-Home firmware that will control the hot tub. For now, this is what we need for development.

Create a new ESP-Home device and copy the `logger` and `uart` section. In the `uart` section, `tx_pin` and `rx_pin` may need to be adjusted for your device's RS485 interface. Here, I have have WIFI SSID and password in my secrets.yaml file.

```
esphome:
  name: hot-tub
  friendly_name: Hot Tub
  comment: "Luxury Spa"

esp32:
  board: m5stack-atom

logger:
  baud_rate: 0

# Enable Home Assistant API
api:
  encryption:
    key: "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"

ota:
  password: "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"

wifi:
  ssid: !secret wifi_ssid
  password: !secret wifi_password

uart:
  id: my_uart
  tx_pin: GPIO26
  rx_pin: GPIO32
  baud_rate: 38400
  debug:
    direction: BOTH
    dummy_receiver: true
```

Once done, flash your ESP32 device and you are ready to connect your device to the hot tub. The IQ2020 control board has an expansion connecter that is both RS485 and I2C. We will be using the RS485 pins of this connector.

![IQ2020-ESP1](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/07697b93-9469-46b6-9f8b-8a79d4cd90d3)

Connect 4 male-to-female 1 pin jumper wires to the ESP32 device. It's best to use different colors for each pin.

![IQ2020-ESP2](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/434920d7-ad5b-446c-af8e-142df2a1e9d8)

Then connect the ESP32 to the tub. In the picture below you will notice I have the expansion board attached with 8 expension connectors. This expansion board is not needed unless you are out of free connectors. I have a salt-water device that I don't use, but I will keep it connected here so that some RS485 traffic can be seen by the ESP32 device. To be safe, turn off your hot tub first and double check all the wires. Note that the ESP32 device will be powered from the hot tub connector, so no other wires are required.

![IQ2020-ESP3](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/15f23030-fa9b-4f05-bbbb-0723c9e79cda)

Once you power the hot tub back on, the ESP32 device will power on and connect to your network and Home Assistant using WIFI. At this point, you can go in the ESP-Home panel and take a look at the device logs. They will show all traffic on the RS485 bus in Hex format.

![IQ2020-ESP5](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/93539d99-bc69-487a-a1dc-cb6d2bc41422)

A quick look at the packets show the following probable encoding. If the packets don't look like this, you may have the RS485 A and B wires reversed.

![IQ2020-ESP4](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/dde784aa-72f5-4315-a671-147064789ff4)

This is the start of the project. If anyone can get gather more packets and help with decoding that would be great. Open an issue in this GitHub project and provide any added data or help that you can.
