import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import switch
from esphome.const import CONF_ID

CONF_SWITCH_LIGHTS = "iq2020_switch"

iq2020_switch_ns = cg.esphome_ns.namespace('iq2020_switch')
EmptySwitch = iq2020_switch_ns.class_('IQ2020Switch', switch.Switch, cg.Component)

CONFIG_SCHEMA = switch.SWITCH_SCHEMA.extend({
    cv.GenerateID(): cv.declare_id(EmptySwitch)
}).extend(cv.COMPONENT_SCHEMA)

def to_code(config):
    server = cg.new_Pvariable(config[CONF_ID])
    yield cg.register_component(server, config)
    yield switch.register_switch(server, config)

    var = await cg.get_variable(config[CONF_IQ2020_SERVER])
    cg.add(server.set_request_mod(config[CONF_SWITCH_LIGHTS]))
