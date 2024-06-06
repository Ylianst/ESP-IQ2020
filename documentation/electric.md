# Optimizing your hot tub for variable electric rates

Power on the electric grid is not generated at a fixed price all day and year long. The power compagny pays more at some times and less at other times. In some parts of the world, you can sign up for variable electric rates where you get charged less at nights and weekends and more during peak hours. Here in Oregon, the power compagny has the a [time of day rates](https://portlandgeneral.com/about/info/pricing-plans/time-of-day) that offer significant discounts at night and on weekends. A hot tub is makes use of significant abount of electricity. In the first 6 months of 2024, mine used over 1500KWh of energy. For me in 2024, the power rates are as follows:

 - Off-peak = 8.39c / KWh
 - Mid-peak = 15.77c / KWh
 - On-peak = 41.11c / KWh

So, it makes a big difference if you can power your hot tub using off-peak power rates. With this IQ2020 integration, we can do that by dynamically setting the hot tub temperature during the day use as much cheap power as possible.

First, I have 3 automations in Home Assistant that trigger at Off-Peak, Mid-Peak and On-Peak rates. I do a bunch of things with these to cut power use during the On-Peak times. For the hot tub, I lower the temperature at mid-peak and lower again at on-peak. Here is what the temperature graph looks like for 48 hours.

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/5c56901e-731b-40b2-af83-fdf881bc15ce)

We need in red the set temperature changing depending on the electricity rate. In light blue the exaust temperature of the water heater and in darker blue the temperature of the water. This is a relatively hot day so, heat loss is low. Notice that on the second day, I automatically raised up the temperature of the tub by one degree one hour before mid-peak. This allows the tub to coast thruout the day at a higher temperature.
