import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import sensor
from . import ns, IQ2020Component

CONF_IQ2020_SERVER = "iq2020_server"

CONFIG_SCHEMA = cv.Schema(
    {
        cv.GenerateID(CONF_IQ2020_SERVER): cv.use_id(IQ2020Component),
    }
)

async def to_code(config):
    server = await cg.get_variable(config[CONF_IQ2020_SERVER])
