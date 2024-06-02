﻿import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import text_sensor
from . import ns, IQ2020Component

CONF_IQ2020_SERVER = "iq2020_server"
CONF_SENSOR_VERSION = "version"

CONFIG_SCHEMA = cv.Schema(
    {
        cv.GenerateID(CONF_IQ2020_SERVER): cv.use_id(IQ2020Component),
        cv.Optional(CONF_SENSOR_VERSION): text_sensor.text_sensor_schema(
#            device_class=DEVICE_CLASS_CONNECTIVITY,
#            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
        )
    }
)

async def to_code(config):
    server = await cg.get_variable(config[CONF_IQ2020_SERVER])
    
    if CONF_SENSOR_VERSION in config:
        sens = await text_sensor.new_text_sensor(config[CONF_SENSOR_VERSION])
#        cg.add(server.set_connected_sensor(sens))