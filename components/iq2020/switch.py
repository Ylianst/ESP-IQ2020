import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import switch
from esphome.const import CONF_ID

from . import ns, IQ2020Component

CONF_IQ2020_ID = "IQ2020Component";
CONF_IQ2020_SERVER = "iq2020_server"
CONF_SWITCH_DATAPOINT = "datapoint"

iq2020_switch_ns = cg.esphome_ns.namespace('iq2020_switch')
IQ2020Switch = iq2020_switch_ns.class_('IQ2020Switch', switch.Switch, cg.Component)

CONFIG_SCHEMA = switch.SWITCH_SCHEMA.extend({
    cv.GenerateID(): cv.declare_id(IQ2020Switch)
}).extend({ cv.Required(CONF_SWITCH_DATAPOINT): cv.positive_int }).extend(cv.COMPONENT_SCHEMA)

async def to_code(config):
    server = cg.new_Pvariable(config[CONF_ID])
    await cg.register_component(server, config)
    await switch.register_switch(server, config)

#    paren = await cg.get_variable(config[CONF_IQ2020_ID])
#    cg.add(server.set_iq2020_parent(paren))
#   cg.add(server.set_switch_id(config[CONF_ID]))
    cg.add(server.set_switch_id(config[CONF_SWITCH_DATAPOINT]))