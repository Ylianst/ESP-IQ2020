# M5Stack ATOM Lite ESP32 IoT Development Kit

I have tried two ESP32 devices with the IQ2020. The device I recommend for most people is the M5Stack ATOM Lite ESP32 IoT Development Kit. It's very small and is perfect for this usage. The parts you need are:

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

The second device I tried and confirm to be working well is the M5StickC PLUS ESP32-PICO Mini IoT Development Kit. I don't recommend this device as it's more expensive and has a display which is not useful for this project. We could show hot tub data on the display, but if you're going to leave it in the IQ2020 enclosure, no one will see it and there is probably an added power draw to the display. One benefit for me is that I optionally power this device using USB before powering the hot tub and so, I can capture the initial messages sent on the RS485 bus by the controller. The device will work just fine with 12v tub power. This said, this is not the device I use day to day. Here are the parts:

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

# M5Stack ATOM Lite ESP32 IoT Development Kit (2)

Same device at the first option, but a different RS485 interface.

- [ATOM Lite ESP32 IoT Development Kit](https://shop.m5stack.com/products/atom-lite-esp32-development-kit)
- [ATOMIC RS485 Base](https://shop.m5stack.com/products/atomic-rs485-base)

I use this device and interface for a different project and it works great. Once assembled, the device looks like this.

![334186423-5f81d5d2-d047-4ceb-b9f0-aef23833bf64](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/5cef14f5-2575-4ac1-9393-596dd468fda1)

In the ESP-Home configuration, you will need to use the following settings. Note the GPIO pin numbers.

```
# M5Stack ATOM Lite
uart:
  id: SpaConnection
  tx_pin: GPIO19
  rx_pin: GPIO22
  baud_rate: 38400
```

# Industrial ESP32-S3 Control Board

<img width="800" height="544" alt="image" src="https://github.com/user-attachments/assets/b47dbfeb-c8c4-4d53-8268-9baee3f004d3" />

If you are in Europe, one option is to use the following device:

- [Amazon UK Link](https://www.amazon.co.uk/dp/B0FN4LWLH9)
- [Device Wiki](https://www.waveshare.com/wiki/ESP32-S3-RS485-CAN)

It's not as compact as the devices above, but may be more easily available. The wireing looks like this:

<img width="1202" height="374" alt="image" src="https://github.com/user-attachments/assets/088514e9-91a9-43c4-a3ba-167db5b9f02e" />

In the ESP-Home configuration, you will need to use the following settings. Note the GPIO pin numbers and the 'flow_control_pin'l.

```
uart:
  id: SpaConnection
  tx_pin: GPIO17
  rx_pin: GPIO18
  baud_rate: 38400
  parity: NONE
  stop_bits: 1
  rx_buffer_size: 1024

iq2020:
  uart_id: SpaConnection
  flow_control_pin: GPIO21
  port: 1234
```

# ESP32 + RS485MAX

I don't suggest this setup as it's not as nice as the ones above and you will need to do extra work to power the ESP32 and RS485MAX modules using the 12v from the hot tub or have a separate power source.

I don't have this setup and have not tested it and using the RS485MAX module requires an extra GPIO pin to tell the module what is the current traffic direction (send or receive). In order to support this module, this integration supports an extra `flow_control_pin`. The device looks like this and has the following connectors:

![RS485MAX](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/f27715c6-e463-4dc1-b3ed-ff575076ff0c)

![max485](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/20253758-e7fa-41a9-ab1b-d55aff3e9bba)

Here is a suggested wiring I found. Some of these modules are 3v instead of 5v and so, take care to wire correctly.

![rs485_esp32_wireing](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/8822b497-0453-4daf-bf64-abf90327809f)

In this case, the configuration should look like:

```
uart:
  id: SpaConnection
  tx_pin: GPIO19
  rx_pin: GPIO18
  baud_rate: 38400

iq2020:
   uart_id: SpaConnection
   flow_control_pin: GPIO4
   port: 1234
```

This said, if you have a setup like this that works. Please let me know so I can update this section. This integration should lower the `flow_control_pin` when receiving and raise it when transmitting.


# Creating a Correct Cable

The current recommandation for most people is to just use 4 breadboard cables to connect the ESP32 to the Hottub. It's easy and works perfectly well and you get more breadboard cables for other projects. I have not done this myself, but it's possible to create your own "correct" cable. The connector you need is part number 3-644540-8, you can find it on [DigiKey here](https://www.digikey.com/en/products/detail/te-connectivity-amp-connectors/3-644540-8/698326).

Alternatively, somone suggested using TE Connectivity part number 1375820-8 with terminals 1375819-3. These are available from most electronics retailers such as Mouser, Farnell, Digikey, RS. These can be crimped with an inexpensive crimper designed for small terminals (e.g. Engineer PA-09).

If you try this, please let us know your experience and send pictures!
