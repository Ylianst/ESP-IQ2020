# Optimizing your hot tub for variable electric rates

Power on the electric grid is not generated at a fixed price all day and year long. The power compagny pays more at some times and less at other times. In some parts of the world, you can sign up for variable electric rates where you get charged less at nights and weekends and more during peak hours. Here in Oregon, the power compagny has the a [time of day rates](https://portlandgeneral.com/about/info/pricing-plans/time-of-day) that offer significant discounts at night and on weekends. A hot tub makes use of significant amount of electricity. In the first 6 months of 2024, mine used over 1500KWh of energy. For me in 2024, the power rates are as follows:

 - Off-peak = 8.39¢ / KWh
 - Mid-peak = 15.77¢ / KWh
 - On-peak = 41.11¢ / KWh

So, it makes a big difference if you can power your hot tub using off-peak power rates. With this IQ2020 integration, we can do that by dynamically setting the hot tub temperature to use as much cheap power as possible.

First, I have 3 automations in Home Assistant that trigger at Off-Peak, Mid-Peak and On-Peak rates. I do a bunch of things with these to cut power use during the On-Peak times. I lower the temperature at mid-peak and lower again at on-peak. I also use another automation to increase the heat one hour before the mid-peak rate. Here is what the temperature graph looks like for 48 hours.

![image](https://github.com/Ylianst/ESP-IQ2020/assets/1319013/5c56901e-731b-40b2-af83-fdf881bc15ce)

The line in red is the "set temperature" that changes depending on the electricity rate. In light blue is the exaust temperature of the water heater and in darker blue the temperature of the water. This is a relatively hot day so, heat loss is low. Notice that on the second day, I automatically raised up the temperature of the tub by one degree one hour before mid-peak. This allows the tub to coast thruout the day at a higher temperature.

On weekends the electric rates are low all day so, these mid-peak and on-peak automations will not run. Overall, at over 3000 KWh a year of electricity usage, I expect to save quite a bit of money each year. Flat electricity rate here are 19.66¢ / KWh, so 589.80$ a year. With variable electric rates and this ESP-Home integration, I pay 251.70$ a year, a savings of 338.10$ a year.

One more thing I am trying is to force a cleaning cycle at 2pm each day. Hopefully, this will prevent the cleaning cycle from occuring during the on-peak time saving about 0.5 KWh during this time.

To recap, for my electric rates, the hardware costs of this integration will pay for itself is about a month.
