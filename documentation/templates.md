# Home Assistant Templates

A lot of the sonsor values returned by this integration are large numbers of seconds like the lifetime of the hot tub, jets or lights. To turn these values into something more practical, edit your Home Assistant configuration.yaml file and add the following sensor template:

Note that you may need to change `hot_tub_` to the name of your own device.

```
  - platform: template
    sensors:
      hot_tub_heater_runtime_str:
        friendly_name: "Hot Tub Heater Runtime"
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
        friendly_name: "Hot Tub Jets 2 Runtime"
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
        friendly_name: "Hot Tub Jets 2 Runtime"
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

Once added, you can restart Home Assistant and you will have new sensors you can then display on the dashboard like this:

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/86192bbf-501d-47da-b2f1-c1a93cf524d8)

If you run into any issues or want to make changes, go to the Home Assistant "Developer Tools", select the "Template" tab and paste the `value_template` above. You can see the result on the right and make changes as you wish.

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/ffa2b8ac-4193-411d-b864-b04e8ea1b068)
