## Full Configuration

Below is an example of a full configuration with most sensors turned on. The example below has a lot of great ideas for enhancing sensors, using substitutions and more. As always, you are encoraged to adapt this configuration to your own needs. Images of the sensors below.

```
substitutions:
  celcius_farenheit: c
  device: jacuzzi
  device_1: circulation_pump
  device_2: heater
  device_3: lights
  device_4: jets1
  device_5: jets2
  device_6: jets2_low
  sensor_update_frequency: 1s

esphome:
  name: jacuzzi
  friendly_name: Hotspring Envoy 2017
  comment: "Luxury Spa"

esp32:
  board: m5stack-atom

logger:
  baud_rate: 0
  level: ERROR

# Enable Home Assistant API
api:
  encryption:
    key: "xxxxxxxxxxxxxxxxxxxxxxxx"

ota:
  - platform: esphome
    password: "xxxxxxxxxxxxxxxxxxxxxxxx"

wifi:
  ssid: !secret wifi_ssid
  password: !secret wifi_password

  # Enable fallback hotspot (captive portal) in case wifi connection fails
  ap:
    ssid: "${device} Fallback Hotspot"
    password: "1231231231"

captive_portal:

external_components:
  - source: github://ylianst/esp-iq2020
    refresh: 30s

uart:
  id: SpaConnection
  tx_pin: GPIO26
  rx_pin: GPIO32
  baud_rate: 38400

iq2020:
   uart_id: SpaConnection
#   flow_control_pin: GPIO0
   port: 1234
   audio_emulation: true


# Example configuration entry
web_server:
  port: 80

# Sync time with Home Assistant.
time:
  - platform: homeassistant
    id: homeassistant_time

button:
  - platform: restart
    name: "ESP Restart"
    
binary_sensor:    
  - platform: status
    name: ESP Status

# If using celsius units on the hot tub remote, replace _f_ with _c_ in the three entries below.
# Feel free to remove any sensor that are not relevent for your hot tub.
sensor:
    #measurement counters
  - platform: iq2020
    current_${celcius_farenheit}_temperature:
      name: Current Temperature
    target_${celcius_farenheit}_temperature:
      name: Target Temperature
    outlet_${celcius_farenheit}_temperature:
      name: Heater Outlet
    pcb_${celcius_farenheit}_temperature:
      name: Controller Temperature
    salt_content:
      name: Salt Content

    #Lifetime counters
    lifetime_runtime:
      name: Lifetime Runtime
      on_value:
        then:
          - lambda: |-
              ESP_LOGD("main", "The current version is %s", x.c_str());
      id: jacuzzi_total_runtime
    heater_total_runtime:
      name: Heater Runtime
      id: heater_total_runtime
    circulation_pump_total_runtime:
      name: Circulation Pump Runtime
      id: circulation_pump_total_runtime
    lights_total_runtime:
      name: Lights Runtime
      id: lights_total_runtime      
    jets1_total_runtime:
      name: Jets 1 Runtime
      id: jets1_total_runtime
#      jet1_low_total_runtime:
#      name: Jets 1 Low Runtime
#      id: jets1_low_total_runtime      
    jets2_total_runtime:
      name: Jets 2 Runtime
      id: jets2_total_runtime      
    jet2_low_total_runtime:
      name: Jets 2 Low Runtime
      id: jets2_low_total_runtime      
#    jets3_total_runtime:
#      name: Jets 3 Runtime
#      id: jets3_total_runtime      
    power_on_counter:
      name: Power On Counter

    #Energy sensors
    power_l1:
      name: Pump Power
    power_heater:
      name: Small Heater Power
    power_l2:
      name: Heater Power
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

    #light sensors      
    logo_lights:
      name: Logo Lights
    audio_buttons:
      name: Audio Buttons
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

    #network sensors 
  - platform: wifi_signal # Reports the WiFi signal strength/RSSI in dB
    name: "ESP WiFi Signal dB"
    id: wifi_signal_db
    update_interval: 60s
    entity_category: "diagnostic"
  - platform: copy # Reports the WiFi signal strength in %
    source_id: wifi_signal_db
    name: "ESP WiFi Signal Percent"
    filters:
      - lambda: return min(max(2 * (x + 100.0), 0.0), 100.0);
    unit_of_measurement: "Signal %"
    entity_category: "diagnostic"
    device_class: ""

    #esp sensors 
  - platform: uptime
    name: ESP Uptime Sensor
  - platform: internal_temperature
    name: "ESP Internal Temperature"
  - platform: template
    id: esp_memory
    icon: mdi:memory
    name: ESP Free Memory
    lambda: return heap_caps_get_free_size(MALLOC_CAP_INTERNAL) / 1024;
    unit_of_measurement: 'kB'
    state_class: measurement
    entity_category: "diagnostic"

select:
  - platform: iq2020
    name: Audio Source
    id: audio_source
    datapoint: 0

number:
  #jacuzzi audio control
  - platform: iq2020
    name: Volume
    id: audio_volume
    datapoint: 0
  - platform: iq2020
    name: Treble
    id: audio_treble
    datapoint: 1
  - platform: iq2020
    name: Bass
    id: audio_bass
    datapoint: 2
  - platform: iq2020
    name: Balance
    id: audio_balance
    datapoint: 3
  - platform: iq2020
    name: Subwoofer
    id: audio_subwoofer
    datapoint: 4
  #jacuzzi salt control
  - platform: iq2020
    id: salt_power
    name: Salt System Power
    datapoint: 5

text:
  #audio sensors 
  - platform: iq2020
    name: Song Title
    id: song_title
    datapoint: 0
    mode: text
    value: "Home Assistant"
  - platform: iq2020
    name: Artist Name
    id: artist_name
    datapoint: 1
    mode: text
    value: "Remote Control"
    
switch:
  #jacuzzi control
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
  #jacuzzi jet control
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
  #  celsius: true

text_sensor:
    #esp sensors 
  - platform: version
    name: "ESP Device Version"
    id: device_esphome_version
    internal: true
    icon: 'mdi:chevron-right'
  - platform: wifi_info
    ip_address:
      name: ESP IP Address
    ssid:
      name: ESP Connected SSID
    mac_address:
      name: ESP Mac Wifi Address
    scan_results:
      name: ESP Latest Scan Results
    dns_address:
      name: ESP DNS Address

    #jacuzzi sensors 
  - platform: iq2020
    versionstr:
      name: Version
  - platform: template
    id: total_runtime_$device
    name: "Total Runtime_$device"
    update_interval: $sensor_update_frequency
    icon: "mdi:timer-sand"
    lambda: |-
      int seconds = round(id(${device}_total_runtime).state);
      int years = seconds / (24 * 3600);
      int months = seconds / (24 * 3600);
      int days = seconds / (24 * 3600);
      seconds = seconds % (24 * 3600);
      int hours = seconds / 3600;
      seconds = seconds % 3600;
      int minutes = seconds /  60;
      seconds = seconds % 60;
        return {
          ((days ? String(days) + " days, ": "") +
          (hours ? String(hours) + ":": "") +
          (minutes ? String(minutes) + ":": "") +
          (String(seconds)) 
          ).c_str()};
  - platform: template
    id: total_runtime_$device_1
    name: "Total Runtime_$device_1"
    update_interval: $sensor_update_frequency
    icon: "mdi:timer-sand"
    lambda: |-
      int seconds = round(id(${device_1}_total_runtime).state);
      int years = seconds / (24 * 3600);
      int months = seconds / (24 * 3600);
      int days = seconds / (24 * 3600);
      seconds = seconds % (24 * 3600);
      int hours = seconds / 3600;
      seconds = seconds % 3600;
      int minutes = seconds /  60;
      seconds = seconds % 60;
        return {
          ((days ? String(days) + " days, ": "") +
          (hours ? String(hours) + ":": "") +
          (minutes ? String(minutes) + ":": "") +
          (String(seconds)) 
          ).c_str()};
  - platform: template
    id: total_runtime_$device_2
    name: "Total Runtime_$device_2"
    update_interval: $sensor_update_frequency
    icon: "mdi:timer-sand"
    lambda: |-
      int seconds = round(id(${device_2}_total_runtime).state);
      int years = seconds / (24 * 3600);
      int months = seconds / (24 * 3600);
      int days = seconds / (24 * 3600);
      seconds = seconds % (24 * 3600);
      int hours = seconds / 3600;
      seconds = seconds % 3600;
      int minutes = seconds /  60;
      seconds = seconds % 60;
        return {
          ((days ? String(days) + " days, ": "") +
          (hours ? String(hours) + ":": "") +
          (minutes ? String(minutes) + ":": "") +
          (String(seconds)) 
          ).c_str()};
  - platform: template
    id: total_runtime_$device_3
    name: "Total Runtime_$device_3"
    update_interval: $sensor_update_frequency
    icon: "mdi:timer-sand"
    lambda: |-
      int seconds = round(id(${device_3}_total_runtime).state);
      int years = seconds / (24 * 3600);
      int months = seconds / (24 * 3600);
      int days = seconds / (24 * 3600);
      seconds = seconds % (24 * 3600);
      int hours = seconds / 3600;
      seconds = seconds % 3600;
      int minutes = seconds /  60;
      seconds = seconds % 60;
        return {
          ((days ? String(days) + " days, ": "") +
          (hours ? String(hours) + ":": "") +
          (minutes ? String(minutes) + ":": "") +
          (String(seconds)) 
          ).c_str()};
  - platform: template
    id: total_runtime_$device_4
    name: "Total Runtime_$device_4"
    update_interval: $sensor_update_frequency
    icon: "mdi:timer-sand"
    lambda: |-
      int seconds = round(id(${device_4}_total_runtime).state);
      int years = seconds / (24 * 3600);
      int months = seconds / (24 * 3600);
      int days = seconds / (24 * 3600);
      seconds = seconds % (24 * 3600);
      int hours = seconds / 3600;
      seconds = seconds % 3600;
      int minutes = seconds /  60;
      seconds = seconds % 60;
        return {
          ((days ? String(days) + " days, ": "") +
          (hours ? String(hours) + ":": "") +
          (minutes ? String(minutes) + ":": "") +
          (String(seconds)) 
          ).c_str()};
  - platform: template
    id: total_runtime_$device_5
    name: "Total Runtime_$device_5"
    update_interval: $sensor_update_frequency
    icon: "mdi:timer-sand"
    lambda: |-
      int seconds = round(id(${device_5}_total_runtime).state);
      int years = seconds / (24 * 3600);
      int months = seconds / (24 * 3600);
      int days = seconds / (24 * 3600);
      seconds = seconds % (24 * 3600);
      int hours = seconds / 3600;
      seconds = seconds % 3600;
      int minutes = seconds /  60;
      seconds = seconds % 60;
        return {
          ((days ? String(days) + " days, ": "") +
          (hours ? String(hours) + ":": "") +
          (minutes ? String(minutes) + ":": "") +
          (String(seconds)) 
          ).c_str()};
  - platform: template
    id: total_runtime_$device_6
    name: "Total Runtime_$device_6"
    update_interval: $sensor_update_frequency
    icon: "mdi:timer-sand"
    lambda: |-
      int seconds = round(id(${device_6}_total_runtime).state);
      int years = seconds / (24 * 3600);
      int months = seconds / (24 * 3600);
      int days = seconds / (24 * 3600);
      seconds = seconds % (24 * 3600);
      int hours = seconds / 3600;
      seconds = seconds % 3600;
      int minutes = seconds /  60;
      seconds = seconds % 60;
        return {
          ((days ? String(days) + " days, ": "") +
          (hours ? String(hours) + ":": "") +
          (minutes ? String(minutes) + ":": "") +
          (String(seconds)) 
          ).c_str()};
```

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/a9e5ae66-2a6f-4c07-8f1f-cb7c52e3e7d9)

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/546b25bf-d984-46ab-9516-90899fe588a2)

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/c5b25972-1409-4477-964a-1dc21645407e)

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/1561b409-8329-48f7-a581-f6841265b1d5)




