import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import climate
from esphome.const import CONF_ID

CONF_IQ2020_SERVER = "IQ2020Component"
CONF_CLIMATE = "iq2020_climate"
CONF_CELSIUS = "celsius"

iq2020_climate_ns = cg.esphome_ns.namespace('iq2020_climate')
IQ2020Climate = iq2020_climate_ns.class_('IQ2020Climate', climate.Climate, cg.Component)

CONFIG_SCHEMA = climate.CLIMATE_SCHEMA.extend({
    cv.GenerateID(): cv.declare_id(IQ2020Climate)
}).extend({
	cv.Optional(CONF_CELSIUS) : cv.boolean,
}).extend(cv.COMPONENT_SCHEMA)

async def to_code(config):
    server = cg.new_Pvariable(config[CONF_ID])
    await cg.register_component(server, config)
    await climate.register_climate(server, config)

    if CONF_CELSIUS in config:
        cg.add(server.set_celsius(config[CONF_CELSIUS]))