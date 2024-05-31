import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import switch
from esphome.const import CONF_ID

CONF_IQ2020_SERVER = "IQ2020Component"
CONF_CLIMATE = "iq2020_climate"

iq2020_switch_ns = cg.esphome_ns.namespace('iq2020_climate')
IQ2020Climate = iq2020_switch_ns.class_('IQ2020Climate', switch.Switch, cg.Component)

CONFIG_SCHEMA = switch.CLIMATE_SCHEMA.extend({
    cv.GenerateID(): cv.declare_id(IQ2020Climate)
}).extend(cv.COMPONENT_SCHEMA)

async def to_code(config):
    server = cg.new_Pvariable(config[CONF_ID])
    await cg.register_component(server, config)
    await switch.register_switch(server, config)
