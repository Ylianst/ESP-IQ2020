import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import text_sensor
from esphome.const import CONF_SENSOR_DATAPOINT

from .. import (
    CONF_IQ2020_ID,
    CONF_REQUEST_MOD,
    CONF_REQUEST_ONCE,
    CONF_SRC_ADDRESS,
    IQ2020_CLIENT_SCHEMA,
    IQ2020Client,
    iq2020_ns,
)

DEPENDENCIES = ["iq2020"]

IQ2020TextSensor = iq2020_ns.class_(
    "IQ2020TextSensor", text_sensor.TextSensor, cg.Component, IQ2020Client
)

CONFIG_SCHEMA = (
    text_sensor.text_sensor_schema()
    .extend(
        {
            cv.GenerateID(): cv.declare_id(IQ2020TextSensor),
            cv.Required(CONF_SENSOR_DATAPOINT): cv.string,
        }
    )
    .extend(IQ2020_CLIENT_SCHEMA)
    .extend(cv.COMPONENT_SCHEMA)
)


async def to_code(config):
    var = await text_sensor.new_text_sensor(config)
    await cg.register_component(var, config)

    paren = await cg.get_variable(config[CONF_IQ2020_ID])
    cg.add(var.set_iq2020_parent(paren))
    cg.add(var.set_request_mod(config[CONF_REQUEST_MOD]))
    cg.add(var.set_request_once(config[CONF_REQUEST_ONCE]))
    cg.add(var.set_sensor_id(config[CONF_SENSOR_DATAPOINT]))
    cg.add(var.set_src_adr(config[CONF_SRC_ADDRESS]))
