import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import sensor
from esphome.const import (
    UNIT_WATT,
    UNIT_CELSIUS,
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
CONF_SENSOR_CURRENT_C_TEMPERATURE = "current_c_temperature"
CONF_SENSOR_TARGET_C_TEMPERATURE = "target_c_temperature"
CONF_SENSOR_CONNECTION_COUNT = "connection_count"
CONF_SENSOR_HEATER_WATTAGE = "heater_wattage"
CONF_SENSOR_HEATER_RELAY = "heater_relay"
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

    if CONF_SENSOR_CURRENT_C_TEMPERATURE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_CURRENT_C_TEMPERATURE])
        cg.add(server.set_current_c_temp_sensor(sens))

    if CONF_SENSOR_TARGET_C_TEMPERATURE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_TARGET_C_TEMPERATURE])
        cg.add(server.set_target_c_temp_sensor(sens))

    if CONF_SENSOR_CONNECTION_COUNT in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_CONNECTION_COUNT])
        cg.add(server.set_connection_count_sensor(sens))

    if CONF_SENSOR_HEATER_WATTAGE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_HEATER_WATTAGE])
        cg.add(server.set_heater_wattage_sensor(sens))

    if CONF_SENSOR_HEATER_RELAY in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_HEATER_RELAY])
        cg.add(server.set_heater_relay_sensor(sens))