# Tested Hot Tubs

In this document, I want to keep notes on the currently tested and confirmed to work hot tubs with notes on configuration changes, etc.

## Pre-2014 IQ2020 Boards

This integration uses RS485 serial protocol to talk to IQ2020, this is the same protocol used by the official "SPA Internet Connection Kit" and so, both have the same requirement that your IQ2020 board must support RS485. My theory is that prior to 2014, IQ2020 boards used a I2C bus to communicate between modules and later added or replaced it with RS485. The expansion boards connector is designed to support both I2C and RS485 so there are pins for both. However, I don't know if a hot tub has ever supported both. It may be possible for someone to create a I2C integration for older pre-2014 models, but this is not something I can or will do. It may be possible to update the IQ2020 board ([video](https://www.youtube.com/watch?v=7U2TA-5Is2c)), but this is not something I am knowledgable about.

## List of models compatible with ACE 77401 controller.
There likely all use RS485 and so, would also be compatible with this integration. Hot tubs made after 2014 should all work.
```
Hot Spring	Aria (AR)	2012, 2013, 2014, 2015, 2016, 2017, 2018
Hot Spring	Aria NXT (ARN)	2017, 2018, 2019, 2020, 2021
Limelight	Beam I (BMI)	2018, 2019, 2020, 2021
Limelight	Beam II (BMII)	2018, 2019, 2020, 2021
Limelight	Flair (FLR)	2012, 2013, 2014, 2015, 2016, 2017, 2018, 2019, 2020, 2021
Limelight	Flash (FSH)	2018, 2019, 2020, 2021
Hot Spring	Grandee (GG)	2012, 2013, 2014, 2015, 2016, 2017, 2018
Hot Spring	Grandee NXT (GGN)	2014, 2015, 2016, 2017, 2018, 2019, 2020, 2021
Hot Spring	Prodigy (H)	2012, 2013, 2014, 2015, 2016, 2017, 2018
Hot Spring	Prodigy (HN)	2019, 2020, 2021
Hot Spring	Sovereign (II)	2012, 2013, 2014, 2015, 2016, 2017, 2018
Hot Spring	Sovereign (IIN)	2019, 2020, 2021
Hot Spring	Jetsetter (JJ)	2012, 2013, 2014, 2015, 2016, 2017, 2018
Hot Spring	Jetsetter (JJN)	2019, 2020, 2021
Hot Spring	Jetsetter LX (JTN)	2014, 2015, 2016, 2017, 2018, 2019, 2020, 2021
Hot Spring	Envoy (KK)	2012, 2013, 2014, 2015, 2016, 2017, 2018
Hot Spring	Envoy NXT (KKN)	2014, 2015, 2016, 2017, 2018, 2019, 2020, 2021
Limelight	Pulse (PLS)	2012, 2013, 2014, 2015, 2016, 2017, 2018, 2019, 2020, 2021
Limelight	Prism (PSM)	2018, 2019, 2020, 2021
Hot Spring	Vista (SS)	2012, 2013
Hot Spring	Triumph (TRHN)	2019, 2020, 2021
Hot Spring	Vanguard (VV)	2012, 2013, 2014, 2015, 2016, 2017, 2018
Hot Spring	Vanguard NXT (VVN)	2017, 2018, 2019
Limelight	Bolt (BLT)	2012, 2013, 2014, 2015, 2016, 2017
Limelight	Gleam (GLM)	2013, 2014, 2015, 2016, 2017
Limelight	Glow (GLW)	2012, 2013, 2014, 2015, 2016, 2017
```

## Hotspring Grandee

- Status: Works great. Lights, temperature, Pump1 with 1 speed, Pump2 with 2 speeds.
- Version: WR4.04de1cE002DK4.00
- Modules: FreshWater® Salt System
- Interface: M5Stack-ATOM + Tail485

<details><summary>Notable Settings</summary>

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
                
</details>

## HotSpring HotSpot Stride 2021

- Status: Reported to work, Pump1 has 2 speeds but does not report speed correctly.
- Version: ?
- Modules: None
- Interface: M5Stack ATOM Lite ESP32 with ATOMIC RS485

<details><summary>Notable Settings</summary>
  
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

</details>

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

## HotSpring Limelight Pulse 2024

- Status: Working
- Version:  EG24.104bdE002LL24.1
- Modules: FreshWater® Salt System, FreshWater® IQ
- Interface: M5Stack ATOM Lite + ATOM Tail485

Notes: Installation required an expansion board due to 2 pre-installed modules consuming both available ports.
Hot Spring I2C Communication Board, 5-Port (74150) was installation required trimming the length of the pin
on the existing ports (slightly) to allow the board to fully seat on the posts/stays.

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

<details><summary>Notable Settings</summary>
  
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

</details>

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

<details>
<summary>Notable Settings</summary>

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
    name: 'Circulation Energy'
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

</details>

## Hotspring Jetsetter 2016
- Status: Works great.
- Model: JJ-1U1260
- Extra Modules: Ace Salt Water Sanitizing System
- Version: 1.11.D11efE002HL1.02

## Hotspring Vanguard 2013
- Status: Works great. Lights, temperature, Pump1 with 1 speed, Pump2 with 1 speed.
- Version: 1.06.H03adE0020.70.f
- Model: VV-4R1756
- Interface: M5Stack-ATOM + Tail485
- Only needed to change the code for dual single speed pumps.
- I suspect this will work with hot tubs as old as 2012 using the "Eagle" version of the IQ2020 controller.


## Tiger River Spa Caspian
Tiger River Spa Caspian with a 50hz Eagle board retrofit and this is the light config that works for me. The difference from the document version is the colours were off by one, cycle didn't work until I took out the Cycle colour and 3 intensity levels instead of 5.

<details><summary>Notable Settings</summary>
  
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
```
  
</details>

## 2014 Hot Spring Highlife Aria

Status: Works. The original software on this hot tub can be updated to add more features around the lights.

- 1.06.H03adE0020.70f - Primarily works, but only allow for lights on/off, individual light control including color and intensity does not work
- 1.10.H6388E0020.70.f - Fully working

Regarding the control of the lights, I was able to update the firmware to the latest version and it solved the problem. The integration actually improves the hot tub functionality as it allows the accessory lighting (bartop, pillow, water feature) to be controlled independently. The tub control panel only allows you to turn them on/off and set a single color for all three. The integration allows each of these to be set to a different color and intensity independently. The order of the lights/assignments are different, attached my configuration for anyone that needs it.

<details><summary>Settings</summary>

```
select:
  - platform: iq2020
    name: Color Underwater
    id: lights1_color
    datapoint: 1
    options:
      - Blue
      - Aqua
      - Green
      - White
      - Yellow
      - Red
      - Magenta
      - Cycle
  - platform: iq2020
    name: Color Water Feature
    id: lights2_color
    datapoint: 2
    options:
      - Violet
      - Blue
      - Cyan
      - Green
      - White
      - Yellow
      - Red
      - Cycle
  - platform: iq2020
    name: Color Bartop
    id: lights3_color
    datapoint: 3
    options:
      - Violet
      - Blue
      - Cyan
      - Green
      - White
      - Yellow
      - Red
      - Cycle
  - platform: iq2020
    name: Color Pillow
    id: lights4_color
    datapoint: 4
    options:
      - Violet
      - Blue
      - Cyan
      - Green
      - White
      - Yellow
      - Red
      - Cycle

number:
  - platform: iq2020
    id: lights1_intensity
    name: Intensity Underwater
    datapoint: 7
    maximum: 5
  - platform: iq2020
    id: lights2_intensity
    name: Intensity Water Feature
    datapoint: 8
    maximum: 5
  - platform: iq2020
    id: lights3_intensity
    name: Intensity Bartop
    datapoint: 9
    maximum: 5
  - platform: iq2020
    id: lights4_intensity
    name: Intensity Pillow
    datapoint: 10
    maximum: 5

text:

# If using Celsius units on the hot tub remote, replace _f_ with _c_ in the three entries below.
# Feel free to remove any sensor that are not relevant for your hot tub.
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
      name: Circ Pump Power
    power_l2:
      name: Heater Power
    pcb_f_temperature:
      name: Controller Temperature

    logo_lights:
      internal: true
      name: Logo Lights Number
      on_value:
        then:
          - text_sensor.template.publish:
              id: logo_lights_text
              state: !lambda 'return (to_string(int(x)).c_str());'

    #light sensors
    lights_underwater_intensity:
      name: Underwater Light Intensity
    lights_bartop_intensity:
      name: Water Feature Light Intensity
    lights_pillow_intensity:
      name: Bartop Light Intensity
    lights_exterior_intensity:
      name: Pillow Light Intensity
    lights_underwater_color:
      name: Underwater Light Color
    lights_bartop_color:
      name: Water Feature Light Color
    lights_pillow_color:
      name: Bartop Light Color
    lights_exterior_color:
      name: Pillow Light Color

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
  - platform: template
    id: logo_lights_text
    name: Logo Lights
    filters:
      - map:
        - 0 -> Unknown
        - 1 -> Power on
        - 2 -> Power on & ready on
        - 3 -> Ready flash
        - 4 -> Power flash
        - 5 -> Power and ready flash
        - 6 -> Power and ready alternate
        - 7 -> Power and ready salt error
```

</details>

## 2021 Hot Springs Sovereign

- Status: Works
- Firmware WR4.04de1cE002DK4.00
- Jet 1 setup with 2 speeds
- Jet 2 not enabled
- Salt system enabled
- Audio system emulated

## 2013 Hotspot E Stride

- Status: Works

## 2025 Hot Springs Rhythm

Worked using the m5stack-atom. Some changes for 2025 Rhythm -
- Jet 1 is two speed, Jet 2 is single speed
- Light sections not individually controllable - all share the same light1
- Still working on the salt system
- Added in a dummy sensor to indicate if the tub should preheat before peak hours or just turn down
- I also got the lights working under a light template entity for better control/color selection if anyone is interested!

<img width="1764" height="1328" alt="479529927-7731ecf7-e1ed-4368-9144-c090cf303a80" src="https://github.com/user-attachments/assets/9acfc27d-6f44-4f75-a86f-3cb3686fd888" />

<details><summary>Full YAML</summary>

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
    key: ""

ota:
  - platform: esphome
    password: ""

wifi:
  ssid: !secret wifi_ssid
  password: !secret wifi_password

  # Enable fallback hotspot (captive portal) in case wifi connection fails
  ap:
    ssid: "Hot-Tub Fallback Hotspot"
    password: "hottubbin"

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

select: #all for light color selection
  - platform: iq2020
    name: Light Color
    id: lights1_color
    datapoint: 1
    options:
      - Violet
      - Blue
      - Cyan
      - Green
      - White
      - Yellow
      - Red
      - Cycle
  - platform: iq2020
    name: Color Cycle Speed
    id: lights_cycle_speed
    datapoint: 5

text: # empty sections like this are required to compile correctly.

binary_sensor:    
  - platform: iq2020
    salt_level_confirmed:
      name: Salt Level Confirmed
    status_state1:
      name: Status State 1
    status_state2:
      name: Status State 2

number: #all for light brightness selection
  - platform: iq2020
    id: lights1_intensity
    name: Light Brightness
    datapoint: 7
    maximum: 5

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
    salt_content:
      name: Salt Content
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
    power_on_counter:
      name: Power On Counter
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
    unit_of_measurement: "%"
    entity_category: "diagnostic"
    device_class: ""

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
  - platform: iq2020
    name: Salt Boost
    id: salt_system_boost
    datapoint: 8
  - platform: template #dummy switch to indicate if pre-peak preheat should be ON
    name: "Pre-Peak Heat Boost"
    id: prepeak_heatboost
    optimistic: True

fan:
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

climate:
  - platform: iq2020
    name: Temperature
    celsius: false

text_sensor:
  - platform: iq2020
    versionstr:
      name: Version
```

</details>


## 2025 Hot Springs Hot Spot Pace

Info: This hot tub is called Propel in NZ. Have the salt system installed.
Status: Works. I have the temp dropping to 36 degrees C at 8pm, then back up to 40 degrees C at 11am. It then uses my solar PV to heat for about 3 hours during the midday.

A few configuration changes: Jet 1 is two speed, Jet 2 is not used. Salt system working by adding:

<details><summary>YAML Changes</summary>

```
sensor:
    salt_content:
    name: Salt Content

switch:
    platform: iq2020
    name: Salt Boost
    id: salt_system_boost
    datapoint: 8

binary_sensor:
    platform: iq2020
    salt_level_confirmed:
    name: Salt Level Confirmed
    status_state1:
    name: Status State 1
    status_state2:
    name: Status State 2

number:
    platform: iq2020
    id: salt_power
    name: Salt Power
    datapoint: 5
```

</details>
