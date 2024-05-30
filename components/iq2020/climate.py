import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import climate
from esphome.const import (
    DEVICE_CLASS_CONNECTIVITY,
    ENTITY_CATEGORY_DIAGNOSTIC,
)
from . import ns, IQ2020Component

CONF_WATERTEMP = "watertemp"
CONF_IQ2020_SERVER = "iq2020_server"
CONF_CURRENT_TEMPERATURE_DATAPOINT = "current_temperature_datapoint"
CONF_TARGET_TEMPERATURE_DATAPOINT = "target_temperature_datapoint"
CONF_TARGET_TEMPERATURE_LOW_DATAPOINT = "target_temperature_low_datapoint"
CONF_TARGET_TEMPERATURE_HIGH_DATAPOINT = "target_temperature_high_datapoint"

CONFIG_SCHEMA = cv.Schema(
    {
        cv.GenerateID(CONF_IQ2020_SERVER): cv.use_id(IQ2020Component),
		cv.Optional(CONF_CURRENT_TEMPERATURE_DATAPOINT, default=""): cv.string,
        cv.Optional(CONF_TARGET_TEMPERATURE_DATAPOINT, default=""): cv.string,
        cv.Optional(CONF_TARGET_TEMPERATURE_LOW_DATAPOINT, default=""): cv.string,
        cv.Optional(CONF_TARGET_TEMPERATURE_HIGH_DATAPOINT, default=""): cv.string,
        cv.Required(CONF_WATERTEMP): climate.climate_schema(
            device_class=DEVICE_CLASS_CLIMATE,
            entity_category=ENTITY_CATEGORY_SENSORS,
        ),
    }
)

async def to_code(config):
    server = await cg.get_variable(config[CONF_IQ2020_SERVER])
    sens = await climate.new_climate(config[CONF_WATERTEMP])
	cg.add(server.set_current_temperature_id(config[CONF_CURRENT_TEMPERATURE_DATAPOINT]))
    cg.add(server.set_target_temperature_id(config[CONF_TARGET_TEMPERATURE_DATAPOINT]))
    cg.add(server.set_target_temperature_low_id(config[CONF_TARGET_TEMPERATURE_LOW_DATAPOINT]))
    cg.add(server.set_target_temperature_high_id(config[CONF_TARGET_TEMPERATURE_HIGH_DATAPOINT]))
