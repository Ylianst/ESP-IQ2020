import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import number
from esphome.const import CONF_ID

from . import ns, IQ2020Component

CONF_IQ2020_ID = "IQ2020Component";
CONF_IQ2020_SERVER = "iq2020_server"
CONF_NUMBER_DATAPOINT = "datapoint"
CONF_MAXIMUM = "maximum"

iq2020_number_ns = cg.esphome_ns.namespace('iq2020_number')
IQ2020Number = iq2020_number_ns.class_('IQ2020Number', number.Number, cg.Component)

#CONFIG_SCHEMA = number.NUMBER_SCHEMA.extend({
#    cv.GenerateID(): cv.declare_id(IQ2020Number)
#}).extend({ cv.Required(CONF_NUMBER_DATAPOINT): cv.positive_int }).extend(cv.COMPONENT_SCHEMA)

CONFIG_SCHEMA = number.NUMBER_SCHEMA.extend({
    cv.GenerateID(): cv.declare_id(IQ2020Number),
    cv.Required(CONF_NUMBER_DATAPOINT): cv.positive_int,
    cv.Optional(CONF_MAXIMUM): cv.positive_int
}).extend(cv.COMPONENT_SCHEMA)

async def to_code(config):
    server = cg.new_Pvariable(config[CONF_ID])
    await cg.register_component(server, config)
    await number.register_number(server, config, min_value=0, max_value=3, step=1)

    if CONF_MAXIMUM in config:
        cg.add(server.set_maximum(config[CONF_MAXIMUM]))

    cg.add(server.set_number_id(config[CONF_NUMBER_DATAPOINT]))