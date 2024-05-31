import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import switch
from esphome.const import CONF_ID

from . import ns, IQ2020Component

CONF_IQ2020_SERVER = "IQ2020Component"
CONF_SWITCH_LIGHTS = "iq2020_switch"

#iq2020_switch_ns = cg.esphome_ns.namespace('iq2020_switch')
#EmptySwitch = iq2020_switch_ns.class_('IQ2020Switch', switch.Switch, cg.Component)
EmptySwitch = ns.class_('IQ2020Switch', switch.Switch, cg.Component)

CONFIG_SCHEMA = switch.SWITCH_SCHEMA.extend({
    cv.GenerateID(): cv.declare_id(EmptySwitch)
}).extend(cv.COMPONENT_SCHEMA)

async def to_code(config):
    server = cg.new_Pvariable(config[CONF_ID])
    await cg.register_component(server, config)
    await switch.register_switch(server, config)
