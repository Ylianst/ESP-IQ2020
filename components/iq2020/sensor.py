import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import sensor
from esphome.const import (
    STATE_CLASS_MEASUREMENT,
    ENTITY_CATEGORY_DIAGNOSTIC,
)
from . import ns, IQ2020Component

CONF_SENSOR_CURRENT_TEMPERATURE = "current_temperature"
CONF_SENSOR_TARGET_TEMPERATURE = "target_temperature"
CONF_SENSOR_CONNECTION_COUNT = "connection_count"
CONF_IQ2020_SERVER = "iq2020_server"

CONFIG_SCHEMA = cv.Schema(
    {
        cv.GenerateID(CONF_IQ2020_SERVER): cv.use_id(IQ2020Component),
        cv.Required(CONF_SENSOR_CURRENT_TEMPERATURE): sensor.sensor_schema(
            accuracy_decimals=1,
            state_class=STATE_CLASS_MEASUREMENT,
        ),
        cv.Required(CONF_SENSOR_TARGET_TEMPERATURE): sensor.sensor_schema(
            accuracy_decimals=1,
            state_class=STATE_CLASS_MEASUREMENT,
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
        cg.add(server.set_current_temperature_sensor(sens))

    if CONF_SENSOR_TARGET_TEMPERATURE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_TARGET_TEMPERATURE])
        cg.add(server.set_target_temperature_sensor(sens))

    if CONF_SENSOR_CONNECTION_COUNT in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_CONNECTION_COUNT])
        cg.add(server.set_connection_count_sensor(sens))
