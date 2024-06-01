import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import switch
from esphome.const import CONF_ID

#CONF_IQ2020_SERVER = "IQ2020Component"
#CONF_SWITCH_LIGHTS = "iq2020_switch"

CONF_SWITCH_LIGHTS = "lights_switch"
CONF_SWITCH_SPALOCK = "spa_lock_switch"
CONF_IQ2020_SERVER = "iq2020_server"

CONFIG_SCHEMA = cv.Schema(
    {
        cv.GenerateID(CONF_IQ2020_SERVER): cv.use_id(IQ2020Component),
        cv.Optional(CONF_SWITCH_LIGHTS): switch.switch_schema(
            device_class=DEVICE_CLASS_CONNECTIVITY,
            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
        ),
        cv.Optional(CONF_SWITCH_SPALOCK): switch.switch_schema(
            device_class=DEVICE_CLASS_CONNECTIVITY,
            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
        ),
    }
)

async def to_code(config):
    server = await cg.get_variable(config[CONF_IQ2020_SERVER])

    if CONF_SWITCH_LIGHTS in config:
        server = cg.new_Pvariable(config[CONF_SWITCH_LIGHTS])
        await cg.register_component(server, config)
        await switch.register_switch(server, config)

    if CONF_SWITCH_SPALOCK in config:
        server = cg.new_Pvariable(config[CONF_SWITCH_SPALOCK])
        await cg.register_component(server, config)
        await switch.register_switch(server, config)


#iq2020_switch_ns = cg.esphome_ns.namespace('iq2020_switch')
#IQ2020Switch = iq2020_switch_ns.class_('IQ2020Switch', switch.Switch, cg.Component)

#CONFIG_SCHEMA = switch.SWITCH_SCHEMA.extend(
#    {
#        cv.GenerateID(): cv.declare_id(IQ2020Switch)
#    }
#).extend(cv.COMPONENT_SCHEMA)

#async def to_code(config):
#    server = cg.new_Pvariable(config[CONF_ID])
#    await cg.register_component(server, config)
#    await switch.register_switch(server, config)
