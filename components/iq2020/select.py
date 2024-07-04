import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import select
from esphome.const import CONF_ID

from . import ns, IQ2020Component

CONF_IQ2020_ID = "IQ2020Component";
CONF_IQ2020_SERVER = "iq2020_server"
CONF_SELECT_DATAPOINT = "datapoint"
CONF_OPTIONS = "options"

iq2020_select_ns = cg.esphome_ns.namespace('iq2020_select')
IQ2020Select = iq2020_select_ns.class_('IQ2020Select', select.Select, cg.Component)

CONFIG_SCHEMA = select.SELECT_SCHEMA.extend({
    cv.GenerateID(): cv.declare_id(IQ2020Select),
    cv.Required(CONF_SELECT_DATAPOINT): cv.positive_int,
    cv.Optional(CONF_OPTIONS): cv.ensure_list(cv.string)
}).extend(cv.COMPONENT_SCHEMA)

async def to_code(config):
    server = cg.new_Pvariable(config[CONF_ID])
    await cg.register_component(server, config)

    options = []
    if CONF_OPTIONS in config:
        options = config[CONF_OPTIONS]

    await select.register_select(server, config, options=options)
    cg.add(server.set_select_id(config[CONF_SELECT_DATAPOINT]))