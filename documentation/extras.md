## Extras

In this file, I keep extra tips and tricks for the Home Assistant pro's.

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
