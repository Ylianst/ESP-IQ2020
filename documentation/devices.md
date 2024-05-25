# M5Stack ATOM Lite ESP32 IoT Development Kit

I have tried two ESP32 devices with the IQ2020. The device I recommand for most people is the M5Stack ATOM Lite ESP32 IoT Development Kit. It's very small and is perfect for this usage. The part you need are:

- [ATOM Lite ESP32 IoT Development Kit](https://shop.m5stack.com/products/atom-lite-esp32-development-kit)
- [ATOM Tail485 - RS485 Converter for ATOM](https://shop.m5stack.com/products/atom-tail485)

Once assembled with the wires, the device looks like this:

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/07834e9c-5ef3-4304-add4-3f378a2ccf0a)

In the ESP-Home configuration, you will need to use the following settings. Note the GPIO pin numbers.

```
# M5Stack ATOM Lite
uart:
  id: SpaConnection
  tx_pin: GPIO26
  rx_pin: GPIO32
  baud_rate: 38400
```

# M5StickC PLUS ESP32-PICO Mini IoT Development Kit

The second device I tried and confirm to be working well is the M5StickC PLUS ESP32-PICO Mini IoT Development Kit. I don't recommand this device as it's more expensive and has a display which is not useful for this project. We could show hot tub data on the display, but if your going to leave it in the IQ2020 enclosure, no one will see it and there is probably an added power draw to the display. One benefit for me is that I can power this device using USB before powering the hot tub and so, I can capture the initial messages sent on the RS485 bus by the controller. However, this is not the device I use day to day.

- [M5StickC PLUS ESP32-PICO Mini IoT Development Kit](https://shop.m5stack.com/products/m5stickc-plus-esp32-pico-mini-iot-development-kit)
- [M5StickC RS485 HAT AOZ1282CI](https://shop.m5stack.com/products/m5stickc-rs485-hat-aoz1282ci)

Once assembled with the wires, the device looks like this:

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/f02fd62a-e8ac-497d-a5d7-e9208d35bb3d)

In the ESP-Home configuration, you will need to use the following settings. Note the GPIO pin numbers. The TX Pin is on GPIO 0.

```
# M5StickC PLUS ESP32-PICO Mini
uart:
  id: SpaConnection
  tx_pin: GPIO0
  rx_pin: GPIO26
  baud_rate: 38400
```
