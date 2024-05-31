import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import climate
from esphome.const import CONF_CUSTOM_FAN_MODES, CONF_CUSTOM_PRESETS, CONF_ID

#from .. import (
#    CONF_IQ2020_ID,
#    CONF_REQUEST_MOD,
#    CONF_REQUEST_ONCE,
#    CONF_SRC_ADDRESS,
#    IQ2020_CLIENT_SCHEMA,
#    EconetClient,
#    econet_ns,
#)

#DEPENDENCIES = ["econet"]

CONF_CURRENT_TEMPERATURE_DATAPOINT = "current_temperature_datapoint"
CONF_TARGET_TEMPERATURE_DATAPOINT = "target_temperature_datapoint"
CONF_TARGET_TEMPERATURE_LOW_DATAPOINT = "target_temperature_low_datapoint"
CONF_TARGET_TEMPERATURE_HIGH_DATAPOINT = "target_temperature_high_datapoint"

#IQ2020Climate = econet_ns.class_(
#    "IQ2020Climate", climate.Climate, cg.Component, EconetClient
#)

CONFIG_SCHEMA = cv.All(
    climate.CLIMATE_SCHEMA.extend(
        {
#            cv.GenerateID(): cv.declare_id(IQ2020Climate),
            cv.Optional(CONF_CURRENT_TEMPERATURE_DATAPOINT, default=""): cv.string,
            cv.Optional(CONF_TARGET_TEMPERATURE_DATAPOINT, default=""): cv.string,
            cv.Optional(CONF_TARGET_TEMPERATURE_LOW_DATAPOINT, default=""): cv.string,
            cv.Optional(CONF_TARGET_TEMPERATURE_HIGH_DATAPOINT, default=""): cv.string,
        }
    )
    .extend(cv.COMPONENT_SCHEMA)
    .extend(CLIENT_SCHEMA)
)

async def to_code(config):
    var = cg.new_Pvariable(config[CONF_ID])
    await cg.register_component(var, config)
    await climate.register_climate(var, config)

    paren = await cg.get_variable(config[CONF_ECONET_ID])
#    cg.add(var.set_econet_parent(paren))
#    cg.add(var.set_request_mod(config[CONF_REQUEST_MOD]))
#    cg.add(var.set_request_once(config[CONF_REQUEST_ONCE]))
#    cg.add(var.set_src_adr(config[CONF_SRC_ADDRESS]))
    cg.add(var.set_current_temperature_id(config[CONF_CURRENT_TEMPERATURE_DATAPOINT]))
    cg.add(var.set_target_temperature_id(config[CONF_TARGET_TEMPERATURE_DATAPOINT]))
    cg.add(var.set_target_temperature_low_id(config[CONF_TARGET_TEMPERATURE_LOW_DATAPOINT]))
    cg.add(var.set_target_temperature_high_id(config[CONF_TARGET_TEMPERATURE_HIGH_DATAPOINT]))
