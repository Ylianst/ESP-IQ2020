# Tested Hot Tubs

In this document, I want to keep notes on the currently tested and confirmed to work hot tubs with notes on configuration changes, etc.

## Hotspring Grandee

- Status: Works great. Lights, temperature, Pump1 with 1 speed, Pump2 with 2 speeds.
- Version: WR4.04de1cE002DK4.00
- Modules: FreshWater® Salt System
- Interface: M5Stack-ATOM + Tail485

Notable Settings:
```
uart:
  id: SpaConnection
  tx_pin: GPIO26
  rx_pin: GPIO32
  baud_rate: 38400

# Jets 1 and 2 setup, others removed
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
```

## HotSpring HotSpot Stride 2021

- Status: Reported to work, Pump1 has 2 speeds but does not report speed correctly.
- Version: ?
- Modules: None
- Interface: M5Stack ATOM Lite ESP32 with ATOMIC RS485

Notable Settings:
```
uart:
  id: SpaConnection
  tx_pin: GPIO19
  rx_pin: GPIO22
  baud_rate: 38400

# Jets 2 and after are removed.
fan:
  - platform: iq2020
    name: Jets 1
    id: jets1
    icon: "mdi:turbine"
    datapoint: 0
    speeds: 2
```

## Hotspring Envoy 2017

 - Status: Works great.
 - Version: WR2.02ad1bE002DK2.02
 - Modules: ACE system

## Hotspring Sovereign 2015

 - Status: Works great but lifetime indicator shows over 29 years which is not accurate.
 - Version: 1.08.D0172E002HL1.02
 - Modules: None

## Hotspring Relay 2024

- Status: Works, but power indicators for Pumps & Heater always shows zero.
- IQ2020 Watkins 1303401-1 controller

## HotSpring HotSpot SX

- Status: Works, however the light color cycle, use the following sensor:
- Version: 112T b5e1A002 v3.0
- M5stack atom lite + Atomic RS485 base

```
sensor:
  lights_main_loop_speed:
    name: Lights Main Loop Speed
```
## Caldera Niagara (Utopia Series) 2022

- Status: Works great, Reports power and everything.
- Version: EU2.039018E0020.00.0

## HotSpring Limelight Pulse 2023

- Status: Working well but ACE boost feature does not work and details on controlling lights still being worked on.
- Version: 2.04D 1 E002HL22.0
- Modules: ACE

On the remote colors are:

```
1 = Blue, 
2 = Aqua, 
3 = Green, 
4 = White, 
5 = Yellow, 
6 = Red, 
7 = Magenta, 
8 = Color Wheel On, 
9 = Color Wheel Off
```

Also, light intensity is 0 to 3, not 0 to 5 like other models.

## Hot Springs Limelight Flash 2018

- Status: Works except that the light control colors are off by one (violet=blue, etc.) and wheel doesn't cycle, although the control panel flashes the light icon like it does when cycling normally.
- Version: 1.14D6ce5E002
- No audio or salt systems to test.

## Hot Spring Sovereign

- Status: The lights names and colors are off compared to the example given on this site. Here is the light settings that works:

```
select:
  - platform: iq2020
    name: Color Underwater
    id: lights1_color
    datapoint: 1
    options:
      - Blue
      - Cyan
      - Green
      - White
      - Yellow
      - Red
      - Magenta
      - Cycle
  - platform: iq2020
    name: Color Waterfall
    id: lights2_color
    datapoint: 2
    options:
      - Blue
      - Cyan
      - Green
      - White
      - Yellow
      - Red
      - Magenta
      - Cycle
  - platform: iq2020
    name: Color Bar Top
    id: lights3_color
    datapoint: 3
    options:
      - Blue
      - Cyan
      - Green
      - White
      - Yellow
      - Red
      - Magenta
      - Cycle
  - platform: iq2020
    name: Color Pillow
    id: lights4_color
    datapoint: 4
    options:
      - Blue
      - Cyan
      - Green
      - White
      - Yellow
      - Red
      - Magenta
      - Cycle
  - platform: iq2020
    name: Color Cycle Speed
    id: lights_cycle_speed
    datapoint: 5

number:
  - platform: iq2020
    id: lights1_intensity
    name: Intensity Underwater
    datapoint: 7
    maximum: 5
  - platform: iq2020
    id: lights2_intensity
    name: Intensity Waterfall
    datapoint: 8
    maximum: 5
  - platform: iq2020
    id: lights3_intensity
    name: Intensity Bar Top
    datapoint: 9
    maximum: 5
  - platform: iq2020
    id: lights4_intensity
    name: Intensity Pillow
    datapoint: 10
    maximum: 5
```

## Tiger River Spa Caspian

Does no work. This hot tub seems to be using a older version of the IQ2020 board, used between 2001 to mid-2009 and this integration does not seems to work. Here is an image of the board.

![PXL_20240801_080032524 MP](https://github.com/user-attachments/assets/17ea8c9a-3403-4e8d-94f5-eeb5c6b922ac)

## Hotsprings Limelight Flash (2021)

- Status: Works

Yaml Changes Made in ESPHome

```
fan:
  - platform: iq2020
    name: Jets 1
    id: jets1
    icon: "mdi:turbine"
    datapoint: 0
    speeds: 2 #to 2 from 1
  - platform: iq2020
    name: Jets 2
    id: jets2
    icon: "mdi:turbine"
    datapoint: 1
    speeds: 1 #to 1 from 2
```

Result:
Jets 1 has off/low/high speeds
Jets 2 has off/high speeds

Controller image:
![356781975-cb7addd6-c6bf-4cb5-93f2-518f8f7cbe94](https://github.com/user-attachments/assets/c7d7875d-1afa-4b7b-8cc7-b8bb5f2661f0)

## HotSpring HotSpot SX 2017

Status: Works, but lights can't be controlled correctly, it's a single color lights with off/dim/full setting. Pump power sensor shows always zero.
Version: 110T 7b54A002 v2.6

![369696446-a95b9ca2-f8eb-4d17-891e-87551b0073e5](https://github.com/user-attachments/assets/8045b8af-f5ee-48ee-9921-df06cd55e7de)

## 2004 Hotspring Prodigy-P

Status: Does NOT work. This is a pre-2014 spa. The connector will look the same but the pins for the data and voltage are different.

![image](https://github.com/user-attachments/assets/7ea47713-c6d8-47fc-a8b3-646dad773929)

## 2017 Grandee NXT

Status: Works. Initially, a faulty ATOM light was received, but once replaced everything worked ([details here](https://github.com/Ylianst/ESP-IQ2020/issues/21)).

![369496199-53dcbaa4-0ff2-40b5-b804-684b93b56ca4](https://github.com/user-attachments/assets/c79767f3-677e-4a3f-9009-c761b0b4175a)

## Hot Spring Envoy, model KKN.

Status: "Posting another success" - Reported as working.

## Caldera Vanto 2023
Status: Works great. Lights, temperature, Pump1 with 2 speeds, Pump2 with 1 speed, cleaning cycle.
Version: 112T b5e1A002 v2.6
Interface: M5Stack-ATOM + Tail485
Watkins P/N: 1303401-1 Rev K

![393649169-27c5d517-d762-4576-a487-793084ab2127](https://github.com/user-attachments/assets/746e17c0-71bb-4ddf-89ab-541cf601b703)

Notable Settings:

```
uart:
  id: SpaConnection
  tx_pin: GPIO26
  rx_pin: GPIO32
  baud_rate: 38400

iq2020:
   uart_id: SpaConnection
#   flow_control_pin: GPIO0
   port: 1234
   audio_emulation: false


# Example configuration entry
web_server:
  port: 80

# Sync time with Home Assistant. Enable time component to reset energy at midnight.
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
    jet1_low_total_runtime:
      name: Jets 1 Low Runtime
      id: jets1_low_total_runtime      
    jets2_total_runtime:
      name: Jets 2 Runtime
      id: jets2_total_runtime          
    power_on_counter:
      name: Power On Counter

    #Energy sensors
    power_l1:
      name: Pump Power
      id: power_l1
    power_heater:
      name: Controller Power
      id: power_heater
    power_l2:
      name: Heater Power
      id: power_l2
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
    lights_underwater_intensity:
      name: Underwater Light Intensity
    lights_underwater_color:
      name: Underwater Light Color
  - platform: total_daily_energy
    name: 'Pump Energy'
    power_id: power_l1
    unit_of_measurement: 'kWh'
    state_class: total_increasing
    device_class: energy
    accuracy_decimals: 3
    filters:
      # Multiplication factor from W to kW is 0.001
      - multiply: 0.001
    
  - platform: total_daily_energy
    name: 'Controller Energy'
    power_id: power_heater
    unit_of_measurement: 'kWh'
    state_class: total_increasing
    device_class: energy
    accuracy_decimals: 3
    filters:
      # Multiplication factor from W to kW is 0.001
      - multiply: 0.001
  
  - platform: total_daily_energy
    name: 'Heater Energy'
    power_id: power_l2
    unit_of_measurement: 'kWh'
    state_class: total_increasing
    device_class: energy
    accuracy_decimals: 3
    filters:
      # Multiplication factor from W to kW is 0.001
      - multiply: 0.001
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
    unit_of_measurement: '°F'
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
    name: Color Underwater
    id: lights1_color
    datapoint: 1
    options:
      - Blue
      - Cyan
      - Green
      - White
      - Yellow
      - Red
      - Violet
  - platform: iq2020
    name: Color Cycle Speed
    id: lights_cycle_speed
    datapoint: 5

number:
  - platform: iq2020
    id: lights1_intensity
    name: Intensity Underwater
    datapoint: 7
    maximum: 3

text:

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
    speeds: 2
  - platform: iq2020
    name: Jets 2
    id: jets2
    icon: "mdi:turbine"
    datapoint: 1
    speeds: 1

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

## Hotspring Jetsetter 2016
- Status: Works great.
- Model: JJ-1U1260
- Extra Modules: Ace Salt Water Sanitizing System
- Version: 1.11.D11efE002HL1.02
