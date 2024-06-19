import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import sensor
from esphome.const import (
    UNIT_WATT,
    UNIT_SECOND,
    UNIT_CELSIUS,
    UNIT_VOLT,
    ICON_GAUGE,
    ICON_TIMER,
    ICON_COUNTER,
    ICON_CURRENT_AC,
    ICON_THERMOMETER,
    ICON_HEATING_COIL,
    STATE_CLASS_MEASUREMENT,
    DEVICE_CLASS_TEMPERATURE,
    ENTITY_CATEGORY_DIAGNOSTIC,
)
from . import ns, IQ2020Component

UNIT_FAHRENHEIT = "°F"
CONF_SENSOR_CURRENT_F_TEMPERATURE = "current_f_temperature"
CONF_SENSOR_TARGET_F_TEMPERATURE = "target_f_temperature"
CONF_SENSOR_OUTLET_F_TEMPERATURE = "outlet_f_temperature"
CONF_SENSOR_CURRENT_C_TEMPERATURE = "current_c_temperature"
CONF_SENSOR_TARGET_C_TEMPERATURE = "target_c_temperature"
CONF_SENSOR_OUTLET_C_TEMPERATURE = "outlet_c_temperature"
CONF_SENSOR_HEATER_TOTAL_RUNTIME = "heater_total_runtime"
CONF_SENSOR_JETS1_TOTAL_RUNTIME = "jets1_total_runtime"
CONF_SENSOR_LIFETIME_RUNTIME = "lifetime_runtime"
CONF_SENSOR_JETS2_TOTAL_RUNTIME = "jets2_total_runtime"
CONF_SENSOR_JETS3_TOTAL_RUNTIME = "jets3_total_runtime"
CONF_SENSOR_LIGHTS_TOTAL_RUNTIME = "lights_total_runtime"
CONF_SENSOR_POWER_ON_COUNTER = "power_on_counter"
CONF_SENSOR_SALT_POWER = "salt_power"
CONF_SENSOR_CONNECTION_COUNT = "connection_count"
CONF_SENSOR_HEATER_WATTAGE = "heater_wattage"
CONF_SENSOR_HEATER_RELAY = "heater_relay"
CONF_SENSOR_VOLTAGE_L1 = "voltage_l1"
CONF_SENSOR_VOLTAGE_HEATER = "voltage_heater"
CONF_SENSOR_VOLTAGE_L2 = "voltage_l2"
CONF_SENSOR_TESTVAL3 = "testval3"
CONF_SENSOR_PCB_TEMPERATURE = "pcb_temperature"
CONF_IQ2020_SERVER = "iq2020_server"

CONFIG_SCHEMA = cv.Schema(
    {
        cv.GenerateID(CONF_IQ2020_SERVER): cv.use_id(IQ2020Component),
        cv.Optional(CONF_SENSOR_CURRENT_F_TEMPERATURE): sensor.sensor_schema(
            unit_of_measurement=UNIT_FAHRENHEIT,
            device_class=DEVICE_CLASS_TEMPERATURE,
            accuracy_decimals=0,
            state_class=STATE_CLASS_MEASUREMENT,
            icon=ICON_THERMOMETER,
        ),
        cv.Optional(CONF_SENSOR_TARGET_F_TEMPERATURE): sensor.sensor_schema(
            unit_of_measurement=UNIT_FAHRENHEIT,
            device_class=DEVICE_CLASS_TEMPERATURE,
            accuracy_decimals=0,
            state_class=STATE_CLASS_MEASUREMENT,
            icon=ICON_THERMOMETER,
        ),
        cv.Optional(CONF_SENSOR_OUTLET_F_TEMPERATURE): sensor.sensor_schema(
            unit_of_measurement=UNIT_FAHRENHEIT,
            device_class=DEVICE_CLASS_TEMPERATURE,
            accuracy_decimals=0,
            state_class=STATE_CLASS_MEASUREMENT,
            icon=ICON_THERMOMETER,
        ),
        cv.Optional(CONF_SENSOR_CURRENT_C_TEMPERATURE): sensor.sensor_schema(
            unit_of_measurement=UNIT_CELSIUS,
            device_class=DEVICE_CLASS_TEMPERATURE,
            accuracy_decimals=1,
            state_class=STATE_CLASS_MEASUREMENT,
            icon=ICON_THERMOMETER,
        ),
        cv.Optional(CONF_SENSOR_TARGET_C_TEMPERATURE): sensor.sensor_schema(
            unit_of_measurement=UNIT_CELSIUS,
            device_class=DEVICE_CLASS_TEMPERATURE,
            accuracy_decimals=1,
            state_class=STATE_CLASS_MEASUREMENT,
            icon=ICON_THERMOMETER,
        ),
        cv.Optional(CONF_SENSOR_OUTLET_C_TEMPERATURE): sensor.sensor_schema(
            unit_of_measurement=UNIT_CELSIUS,
            device_class=DEVICE_CLASS_TEMPERATURE,
            accuracy_decimals=1,
            state_class=STATE_CLASS_MEASUREMENT,
            icon=ICON_THERMOMETER,
        ),
        cv.Optional(CONF_SENSOR_CONNECTION_COUNT): sensor.sensor_schema(
            accuracy_decimals=0,
            state_class=STATE_CLASS_MEASUREMENT,
            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
        ),
        cv.Optional(CONF_SENSOR_HEATER_WATTAGE): sensor.sensor_schema(
            unit_of_measurement=UNIT_WATT,
            accuracy_decimals=0,
            state_class=STATE_CLASS_MEASUREMENT,
            icon=ICON_HEATING_COIL,
        ),
        cv.Optional(CONF_SENSOR_HEATER_RELAY): sensor.sensor_schema(
            accuracy_decimals=0,
            state_class=STATE_CLASS_MEASUREMENT,
        ),
        cv.Optional(CONF_SENSOR_HEATER_TOTAL_RUNTIME): sensor.sensor_schema(
            unit_of_measurement=UNIT_SECOND,
            accuracy_decimals=0,
            icon=ICON_TIMER
        ),
        cv.Optional(CONF_SENSOR_JETS1_TOTAL_RUNTIME): sensor.sensor_schema(
            unit_of_measurement=UNIT_SECOND,
            accuracy_decimals=0,
            icon=ICON_TIMER
        ),
        cv.Optional(CONF_SENSOR_LIFETIME_RUNTIME): sensor.sensor_schema(
            unit_of_measurement=UNIT_SECOND,
            accuracy_decimals=0,
            icon=ICON_TIMER
        ),
        cv.Optional(CONF_SENSOR_JETS2_TOTAL_RUNTIME): sensor.sensor_schema(
            unit_of_measurement=UNIT_SECOND,
            accuracy_decimals=0,
            icon=ICON_TIMER
        ),
        cv.Optional(CONF_SENSOR_JETS3_TOTAL_RUNTIME): sensor.sensor_schema(
            unit_of_measurement=UNIT_SECOND,
            accuracy_decimals=0,
            icon=ICON_TIMER
        ),
        cv.Optional(CONF_SENSOR_LIGHTS_TOTAL_RUNTIME): sensor.sensor_schema(
            unit_of_measurement=UNIT_SECOND,
            accuracy_decimals=0,
            icon=ICON_TIMER
        ),
        cv.Optional(CONF_SENSOR_POWER_ON_COUNTER): sensor.sensor_schema(
            accuracy_decimals=0,
            icon=ICON_COUNTER
        ),
        cv.Optional(CONF_SENSOR_SALT_POWER): sensor.sensor_schema(
            accuracy_decimals=0,
            icon=ICON_GAUGE
        ),
        cv.Optional(CONF_SENSOR_VOLTAGE_L1): sensor.sensor_schema(
            unit_of_measurement=UNIT_VOLT,
            state_class=STATE_CLASS_MEASUREMENT,
            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
            accuracy_decimals=0,
            icon=ICON_CURRENT_AC
        ),
        cv.Optional(CONF_SENSOR_VOLTAGE_HEATER): sensor.sensor_schema(
            unit_of_measurement=UNIT_VOLT,
            state_class=STATE_CLASS_MEASUREMENT,
            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
            accuracy_decimals=0,
            icon=ICON_CURRENT_AC
        ),
        cv.Optional(CONF_SENSOR_VOLTAGE_L2): sensor.sensor_schema(
            unit_of_measurement=UNIT_VOLT,
            state_class=STATE_CLASS_MEASUREMENT,
            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
            accuracy_decimals=0,
            icon=ICON_CURRENT_AC
        ),
        cv.Optional(CONF_SENSOR_TESTVAL3): sensor.sensor_schema(
            unit_of_measurement=UNIT_WATT,
            state_class=STATE_CLASS_MEASUREMENT,
            accuracy_decimals=0,
            icon=ICON_GAUGE
        ),
        cv.Optional(CONF_SENSOR_PCB_TEMPERATURE): sensor.sensor_schema(
            unit_of_measurement=UNIT_FAHRENHEIT,
            state_class=STATE_CLASS_MEASUREMENT,
            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
            accuracy_decimals=0,
            icon=ICON_THERMOMETER
        )
    }
)


async def to_code(config):
    server = await cg.get_variable(config[CONF_IQ2020_SERVER])

    if CONF_SENSOR_CURRENT_F_TEMPERATURE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_CURRENT_F_TEMPERATURE])
        cg.add(server.set_current_f_temp_sensor(sens))

    if CONF_SENSOR_TARGET_F_TEMPERATURE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_TARGET_F_TEMPERATURE])
        cg.add(server.set_target_f_temp_sensor(sens))

    if CONF_SENSOR_OUTLET_F_TEMPERATURE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_OUTLET_F_TEMPERATURE])
        cg.add(server.set_outlet_f_temp_sensor(sens))

    if CONF_SENSOR_CURRENT_C_TEMPERATURE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_CURRENT_C_TEMPERATURE])
        cg.add(server.set_current_c_temp_sensor(sens))

    if CONF_SENSOR_TARGET_C_TEMPERATURE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_TARGET_C_TEMPERATURE])
        cg.add(server.set_target_c_temp_sensor(sens))

    if CONF_SENSOR_OUTLET_C_TEMPERATURE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_OUTLET_C_TEMPERATURE])
        cg.add(server.set_outlet_c_temp_sensor(sens))

    if CONF_SENSOR_CONNECTION_COUNT in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_CONNECTION_COUNT])
        cg.add(server.set_connection_count_sensor(sens))

    if CONF_SENSOR_HEATER_WATTAGE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_HEATER_WATTAGE])
        cg.add(server.set_heater_wattage_sensor(sens))

    if CONF_SENSOR_HEATER_RELAY in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_HEATER_RELAY])
        cg.add(server.set_heater_relay_sensor(sens))

    if CONF_SENSOR_HEATER_TOTAL_RUNTIME in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_HEATER_TOTAL_RUNTIME])
        cg.add(server.set_heater_total_runtime_sensor(sens))

    if CONF_SENSOR_JETS1_TOTAL_RUNTIME in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_JETS1_TOTAL_RUNTIME])
        cg.add(server.set_jets1_total_runtime_sensor(sens))

    if CONF_SENSOR_LIFETIME_RUNTIME in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_LIFETIME_RUNTIME])
        cg.add(server.set_lifetime_runtime_sensor(sens))

    if CONF_SENSOR_JETS2_TOTAL_RUNTIME in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_JETS2_TOTAL_RUNTIME])
        cg.add(server.set_jets2_total_runtime_sensor(sens))

    if CONF_SENSOR_JETS3_TOTAL_RUNTIME in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_JETS3_TOTAL_RUNTIME])
        cg.add(server.set_jets3_total_runtime_sensor(sens))

    if CONF_SENSOR_LIGHTS_TOTAL_RUNTIME in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_LIGHTS_TOTAL_RUNTIME])
        cg.add(server.set_lights_total_runtime_sensor(sens))

    if CONF_SENSOR_POWER_ON_COUNTER in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_POWER_ON_COUNTER])
        cg.add(server.set_power_on_counter_sensor(sens))

    if CONF_SENSOR_SALT_POWER in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_SALT_POWER])
        cg.add(server.set_salt_power_sensor(sens))

    if CONF_SENSOR_VOLTAGE_L1 in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_VOLTAGE_L1])
        cg.add(server.set_voltage_l1_sensor(sens))

    if CONF_SENSOR_VOLTAGE_HEATER in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_VOLTAGE_HEATER])
        cg.add(server.set_voltage_heater_sensor(sens))

    if CONF_SENSOR_VOLTAGE_L2 in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_VOLTAGE_L2])
        cg.add(server.set_voltage_l2_sensor(sens))

    if CONF_SENSOR_TESTVAL3 in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_TESTVAL3])
        cg.add(server.set_testval3(sens))

    if CONF_SENSOR_PCB_TEMPERATURE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_PCB_TEMPERATURE])
        cg.add(server.set_pcb_temperature_sensor(sens))
