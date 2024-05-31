import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import switch
from esphome.const import CONF_ID

from . import ns, IQ2020Component

CONF_IQ2020_SERVER = "iq2020_server"
CONF_SWITCH_LIGHTS = "iq2020_switch"

iq2020_switch_ns = cg.esphome_ns.namespace('iq2020_switch')
EmptySwitch = iq2020_switch_ns.class_('IQ2020Switch', switch.Switch, cg.Component)

CONFIG_SCHEMA = switch.SWITCH_SCHEMA.extend({
    cv.GenerateID(): cv.declare_id(EmptySwitch)
}).extend(cv.COMPONENT_SCHEMA)

#async def to_code(config):
#    server = cg.new_Pvariable(config[CONF_ID])
#    await cg.register_component(server, config)
#    await switch.register_switch(server, config)

async def to_code(config):
    var = await switch.new_switch(config)
    await cg.register_component(var, config)

    paren = await cg.get_variable(config[CONF_ECONET_ID])
    cg.add(var.set_econet_parent(paren))
    cg.add(var.set_request_mod(config[CONF_REQUEST_MOD]))
    cg.add(var.set_request_once(config[CONF_REQUEST_ONCE]))
    cg.add(var.set_switch_id(config[CONF_SWITCH_DATAPOINT]))
    cg.add(var.set_src_adr(config[CONF_SRC_ADDRESS]))