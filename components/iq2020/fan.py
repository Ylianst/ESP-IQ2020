import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import fan
from esphome.const import CONF_ID

from.import ns, IQ2020Component

CONF_IQ2020_ID = "IQ2020Component";
CONF_IQ2020_SERVER = "iq2020_server"
CONF_FAN_DATAPOINT = "datapoint"
CONF_FAN_SPEEDS = "speeds"

iq2020_fan_ns = cg.esphome_ns.namespace('iq2020_fan')
IQ2020Fan = iq2020_fan_ns.class_('IQ2020Fan', fan.Fan, cg.Component)

CONFIG_SCHEMA = fan.FAN_SCHEMA.extend({
	cv.GenerateID() : cv.declare_id(IQ2020Fan)
}).extend({
	cv.Required(CONF_FAN_DATAPOINT) : cv.positive_int,
	cv.Required(CONF_FAN_SPEEDS) : cv.positive_int
}).extend(cv.COMPONENT_SCHEMA)

async def to_code(config) :
	server = cg.new_Pvariable(config[CONF_ID])
	await cg.register_component(server, config)
	await fan.register_fan(server, config)

	#    paren = await cg.get_variable(config[CONF_IQ2020_ID])
	#    cg.add(server.set_iq2020_parent(paren))
	#   cg.add(server.set_fan_id(config[CONF_ID]))
	cg.add(server.set_fan_id(config[CONF_FAN_DATAPOINT]))
	cg.add(server.set_fan_speeds(config[CONF_FAN_SPEEDS]))
