import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import sensor
from esphome.const import (
    UNIT_CELSIUS,
    ICON_THERMOMETER,
    STATE_CLASS_MEASUREMENT,
    DEVICE_CLASS_TEMPERATURE,
    ENTITY_CATEGORY_DIAGNOSTIC,
)
from . import ns, IQ2020Component

UNIT_FAHRENHEIT = "°F"
CONF_TEMP_UNIT = "temp_unit"
CONF_SENSOR_CURRENT_TEMPERATURE = "current_temperature"
CONF_SENSOR_TARGET_TEMPERATURE = "target_temperature"
CONF_SENSOR_CONNECTION_COUNT = "connection_count"
CONF_IQ2020_SERVER = "iq2020_server"

CONFIG_SCHEMAF = cv.Schema(
    {
        cv.GenerateID(CONF_IQ2020_SERVER): cv.use_id(IQ2020Component),
        cv.Required(CONF_SENSOR_CURRENT_TEMPERATURE): sensor.sensor_schema(
            unit_of_measurement=UNIT_FAHRENHEIT,
            device_class=DEVICE_CLASS_TEMPERATURE,
            accuracy_decimals=0,
            state_class=STATE_CLASS_MEASUREMENT,
            icon=ICON_THERMOMETER,
        ),
        cv.Required(CONF_SENSOR_TARGET_TEMPERATURE): sensor.sensor_schema(
            unit_of_measurement=UNIT_FAHRENHEIT,
            device_class=DEVICE_CLASS_TEMPERATURE,
            accuracy_decimals=0,
            state_class=STATE_CLASS_MEASUREMENT,
            icon=ICON_THERMOMETER,
        ),
        cv.Required(CONF_SENSOR_CONNECTION_COUNT): sensor.sensor_schema(
            accuracy_decimals=0,
            state_class=STATE_CLASS_MEASUREMENT,
            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
        ),
    }
)

CONFIG_SCHEMAC = cv.Schema(
    {
        cv.GenerateID(CONF_IQ2020_SERVER): cv.use_id(IQ2020Component),
        cv.Required(CONF_SENSOR_CURRENT_TEMPERATURE): sensor.sensor_schema(
            unit_of_measurement=UNIT_CELSIUS,
            device_class=DEVICE_CLASS_TEMPERATURE,
            accuracy_decimals=1,
            state_class=STATE_CLASS_MEASUREMENT,
            icon=ICON_THERMOMETER,
        ),
        cv.Required(CONF_SENSOR_TARGET_TEMPERATURE): sensor.sensor_schema(
            unit_of_measurement=UNIT_CELSIUS,
            device_class=DEVICE_CLASS_TEMPERATURE,
            accuracy_decimals=1,
            state_class=STATE_CLASS_MEASUREMENT,
            icon=ICON_THERMOMETER,
        ),
        cv.Required(CONF_SENSOR_CONNECTION_COUNT): sensor.sensor_schema(
            accuracy_decimals=0,
            state_class=STATE_CLASS_MEASUREMENT,
            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
        ),
    }
)

async def to_code(config):
    server = await cg.get_variable(config[CONF_IQ2020_SERVER])

    if CONF_SENSOR_CURRENT_TEMPERATURE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_CURRENT_TEMPERATURE])
        cg.add(server.set_current_temp_sensor(sens))

    if CONF_SENSOR_TARGET_TEMPERATURE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_TARGET_TEMPERATURE])
        cg.add(server.set_target_temp_sensor(sens))

    if CONF_SENSOR_CONNECTION_COUNT in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_CONNECTION_COUNT])
        cg.add(server.set_connection_count_sensor(sens))

# Dynamically choose the schema based on the setting
def get_schema(config):
    if config[CONF_SENSOR_CURRENT_TEMPERATURE][CONF_TEMP_UNIT] == 'F':
        return CONFIG_SCHEMAF
    elif config[CONF_SENSOR_CURRENT_TEMPERATURE][CONF_TEMP_UNIT] == 'C':
        return CONFIG_SCHEMAC
    else:
        raise cv.Invalid(f"Unknown setting: {config}")

CONFIG_SCHEMA = cv.All(get_schema)