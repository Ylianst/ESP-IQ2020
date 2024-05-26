# IQ2020 ESP-Home External Component

I wanted to build a ESP-Home integration into Home Assistant that accomplishes two things. First, it has to be able to control the hot tub just like you would expect, controlling temperature and more. Second, I want the integration to also allow an easy way to view the raw traffic on the RS485 bus. This is because I would like it to be easy for others to see that traffic is showing up on their hot tub and report back what is going on so we can improve the integration and add more features or fix any bugs. As a developer, the second objective is most important.

This ESP-Home IQ2020 integration started using code from [esphome-stream-server](https://github.com/oxan/esphome-stream-server) by Oxan van Leeuwen. When looking at the ESP-Home configuration yaml file, you see an optional "port" setting:

```
external_components:
  - source: github://ylianst/esp-iq2020

uart:
  id: SpaConnection
  tx_pin: GPIO26
  rx_pin: GPIO32
  baud_rate: 38400

iq2020:
   uart_id: SpaConnection
   port: 1234                              <--- Traffic viewing port
```

When specified the ESP32 device will listen to this port for incoming TCP connection and will send all RS485 traffic to connected clients and also take and traffic from clients and send it on the bus. You can then use the IQ2020 Data Viewer tool to connect to your device on this port and see the traffic.

This makes it easy to contribute data captures to this project. For example, I don't have a music module on my hot tub, but if someone has one and can figure out the protocol, I would like to have this ESP-Home integration to optionally be able to impersonate a music device and so, you could control the volume of music being played by Home Assistant.
