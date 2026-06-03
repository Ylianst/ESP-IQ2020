import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import button
from esphome.const import CONF_ID

from .. import ns

CONF_BUTTON_DATAPOINT = "datapoint"

iq2020_button_ns = cg.esphome_ns.namespace('iq2020_button')
IQ2020Button = iq2020_button_ns.class_('IQ2020Button', button.Button, cg.Component)

CONFIG_SCHEMA = button.button_schema(IQ2020Button).extend({
    cv.GenerateID(): cv.declare_id(IQ2020Button),
    cv.Required(CONF_BUTTON_DATAPOINT): cv.positive_int,
}).extend(cv.COMPONENT_SCHEMA)

async def to_code(config):
    cg.add_define("USE_IQ2020_BUTTON")
    var = cg.new_Pvariable(config[CONF_ID])
    await cg.register_component(var, config)
    await button.register_button(var, config)
    cg.add(var.set_button_id(config[CONF_BUTTON_DATAPOINT]))
