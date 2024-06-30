## Extras

In this file, I keep extra sensors, tips and tricks for the Home Assistant pro's.

## Controlling Lights Color, Intensity and Cycle Speed

You can control the color, intensite and cycle speed of the lights in your hot tub from Home Assistant. The controls will look like this:

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/65da5635-d19f-42cb-a938-febb1737da7e)

```
select:
  - platform: iq2020-dev
    name: Color Underwater
    id: lights1_color
    datapoint: 1
  - platform: iq2020-dev
    name: Color Bartop
    id: lights2_color
    datapoint: 2
  - platform: iq2020-dev
    name: Color Pillow
    id: lights3_color
    datapoint: 3
  - platform: iq2020-dev
    name: Color Exterior
    id: lights4_color
    datapoint: 4
  - platform: iq2020-dev
    name: Color Cycle Speed
    id: lights_cycle_speed
    datapoint: 5

number:
  - platform: iq2020
    id: lights1_intensity
    name: Intensity Underwater
    datapoint: 7
  - platform: iq2020
    id: lights2_intensity
    name: Intensity Bartop
    datapoint: 8
  - platform: iq2020
    id: lights3_intensity
    name: Intensity Pillow
    datapoint: 9
  - platform: iq2020
    id: lights4_intensity
    name: Intensity Exterior
    datapoint: 10
```

## Lights Intensity and Color (Read only)

These sensors may be removed in the future as new sensors that can monitor and change the lights have been added. You can add sensors to look at the current light color and intensity of each color zones. Add the following in the `sensor` section if needed:

```
sensor:
    lights_intensity:
      name: Light Intensity
    lights_underwater_intensity:
      name: Underwater Light Intensity
    lights_bartop_intensity:
      name: Bartop Light Intensity
    lights_pillow_intensity:
      name: Pillow Light Intensity
    lights_exterior_intensity:
      name: Exterior Light Intensity
    lights_underwater_color:
      name: Underwater Light Color
    lights_bartop_color:
      name: Bartop Light Color
    lights_pillow_color:
      name: Pillow Light Color
    lights_exterior_color:
      name: Exterior Light Color
```

Intensity values are from 0 for off to 5 for maximum brightness. For colors, here are the possible values for the color sensors:

```
1 = Violet
2 = Blue
3 = Cyan
4 = Green
5 = White
6 = Yellow
7 = Red
8 = Cycle
```

## ACE / Freshwater salt systems

If you have a ACE or Freshwater module attached to your hot tub, this integration will allow you to see the salt sensor and control the power level (0 to 10) of the salt system. To do this, add the following under the `number`, `sensor` and `switch` sections:

```
number:
  - platform: iq2020
    id: salt_power
    name: Salt Power
    datapoint: 5

sensor:
    salt_content:
      name: Salt Content

switch:
  - platform: iq2020
    name: Salt Boost
    id: salt_system_boost
    datapoint: 8
```

Salt content is a value from 0 to 7 where values 3 or 4 are ideal.

## Logo Lights

The green and blue light at the front of the hot tub can also be reported by this integration. You can, for example, have a picture your your hot tub in Home Assistant and make the lights on the image match the light on the actual hot tub. In the `sensor` section of the configuration file, add the `logo_lights` sensor.

```
sensor:
    logo_lights:
      name: Logo Lights
```

Once reflashed, this new sensor will be an integer. The values are as follows:

```
0 = Unknown
1 = Power on
2 = Power on & ready on
3 = Ready flash
4 = Power flash
5 = Power and ready flash
6 = Power and ready alternate
7 = Power and ready salt error
```

## Adding WIFI signal strength sensor

You can setup two additional sensors in the ESP Home integration to get a report of the WIFI signal strength of the ESP32 device to your WIFI access point. It's easy, just add the follow lines in the `sensor:` section of the ESP home configuration:

```
sensor:
  - platform: wifi_signal # Reports the WiFi signal strength/RSSI in dB
    name: "WiFi Signal dB"
    id: wifi_signal_db
    update_interval: 60s
    entity_category: "diagnostic"
  - platform: copy # Reports the WiFi signal strength in %
    source_id: wifi_signal_db
    name: "WiFi Signal Percent"
    filters:
      - lambda: return min(max(2 * (x + 100.0), 0.0), 100.0);
    unit_of_measurement: "Signal %"
    entity_category: "diagnostic"
    device_class: ""
```
Note that you can't have two `sensor:` sections, so just add all of these lines except `sensor:` to your existing section. You can add them at the top or bottom, order is not important. Once re-flashed, you should see two added values in the diagnostic section of Home Assistant.

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/cff708ec-8ba3-48f8-b337-1d83b5bb00e0)


## Adding Current and Voltage sensors

You can add additional voltage and current sensors. In general you don't need them since the power sensor in watts are a more accurate value of (voltage * current).

```
sensor:
    voltage_l1:
      name: Voltage L1
    voltage_heater:
      name: Voltage Heater
    voltage_l2:
      name: Voltage L2
    current_l1:
      name: Current L1
    current_heater:
      name: Current Heater
    current_l2:
      name: Current L2
```
Note that you can't have two `sensor:` sections, so just add all of these lines except `sensor:` to your existing section. You can add them at the top or bottom, order is not important. Once re-flashed, you should see new values in the diagnostic section of Home Assistant.

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/14deafa5-322c-42ff-8983-7fd3f1e8e878)

## Runtime Sensors

The IQ2020 controller keeps a running count of the usage of the jets, heater, lights and more. You can add the following sensors if you would like to see these values:

```
sensors:
    heater_total_runtime:
      name: Heater Runtime
    jets1_total_runtime:
      name: Jets 1 Runtime
    lifetime_runtime:
      name: Lifetime Runtime
    jets2_total_runtime:
      name: Jets 2 Runtime
    lights_total_runtime:
      name: Lights Runtime
    circulation_pump_total_runtime:
      name: Circulation Pump Runtime
    jet1_low_total_runtime:
      name: Jets 1 Low Runtime
    jet2_low_total_runtime:
      name: Jets 2 Low Runtime
    power_on_counter:
      name: Power On Counter
```

## Templates for seconds sensors

A lot of the sensor values returned by this integration are large numbers of seconds like the lifetime of the hot tub, jets or lights. On this page, we have Home Assistant template sensors you can add to change the seconds value into anything you like. Below in the templates that will make the seconds show up as days and hours like this:

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/04f2112f-6d3e-407e-ab42-3bf2d6b780c5)

To do this, edit your Home Assistant configuration.yaml file and add the following `sensor:` template. Note that you may need to change `hot_tub_` to the name of your own device.

```
sensor:
  - platform: template
    sensors:
      hot_tub_heater_runtime_str:
        friendly_name: "Hot Tub Heater Runtime"
        icon_template: mdi:clock-outline
        value_template: >
          {% set seconds = states('sensor.hot_tub_heater_runtime') | int %}
          {% set days = seconds // 86400 %}
          {% set hours = (seconds % 86400) // 3600 %}
          {% set s1 = 's' if days > 1 else '' %}
          {% if days > 0 %}
            {% set s2 = 's' if hours > 1 else '' %}
            {{ '%d day%s, %d hour%s' | format(days, s1, hours, s2) }}
          {% else %}
            {{ '%d hour%s' | format(hours, s2) }}
          {% endif %}
      hot_tub_jets_1_runtime_str:
        friendly_name: "Hot Tub Jets 1 Runtime"
        icon_template: mdi:clock-outline
        value_template: >
          {% set seconds = states('sensor.hot_tub_jets_1_runtime') | int %}
          {% set days = seconds // 86400 %}
          {% set hours = (seconds % 86400) // 3600 %}
          {% set s1 = 's' if days > 1 else '' %}
          {% if days > 0 %}
            {% set s2 = 's' if hours > 1 else '' %}
            {{ '%d day%s, %d hour%s' | format(days, s1, hours, s2) }}
          {% else %}
            {{ '%d hour%s' | format(hours, s2) }}
          {% endif %}
      hot_tub_jets_2_runtime_str:
        friendly_name: "Hot Tub Jets 2 Runtime"
        icon_template: mdi:clock-outline
        value_template: >
          {% set seconds = states('sensor.hot_tub_jets_2_runtime') | int %}
          {% set days = seconds // 86400 %}
          {% set hours = (seconds % 86400) // 3600 %}
          {% set s1 = 's' if days > 1 else '' %}
          {% if days > 0 %}
            {% set s2 = 's' if hours > 1 else '' %}
            {{ '%d day%s, %d hour%s' | format(days, s1, hours, s2) }}
          {% else %}
            {{ '%d hour%s' | format(hours, s2) }}
          {% endif %}
      hot_tub_lifetime_runtime_str:
        friendly_name: "Hot Tub Lifetime"
        icon_template: mdi:clock-outline
        value_template: >
          {% set seconds = states('sensor.hot_tub_lifetime_runtime') | int %}
          {% set days = seconds // 86400 %}
          {% set hours = (seconds % 86400) // 3600 %}
          {% set s1 = 's' if days > 1 else '' %}
          {% if days > 0 %}
            {% set s2 = 's' if hours > 1 else '' %}
            {{ '%d day%s, %d hour%s' | format(days, s1, hours, s2) }}
          {% else %}
            {{ '%d hour%s' | format(hours, s2) }}
          {% endif %}
      hot_tub_lights_runtime_str:
        friendly_name: "Hot Tub Lights Runtime"
        icon_template: mdi:clock-outline
        value_template: >
          {% set seconds = states('sensor.hot_tub_lights_runtime') | int %}
          {% set days = seconds // 86400 %}
          {% set hours = (seconds % 86400) // 3600 %}
          {% set s1 = 's' if days > 1 else '' %}
          {% if days > 0 %}
            {% set s2 = 's' if hours > 1 else '' %}
            {{ '%d day%s, %d hour%s' | format(days, s1, hours, s2) }}
          {% else %}
            {{ '%d hour%s' | format(hours, s2) }}
          {% endif %}
```

Once added, you can restart Home Assistant and you will have new sensors you can then display on the dashboard. If you run into any issues or want to make changes, go to the Home Assistant "Developer Tools", select the "Template" tab and paste the `value_template` above. You can see the result on the right and make changes as you wish.

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/ffa2b8ac-4193-411d-b864-b04e8ea1b068)
