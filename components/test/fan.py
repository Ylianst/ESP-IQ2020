import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import fan
from esphome.const import CONF_ID

from.import ns, TestComponent

CONF_FAN_SPEEDS = "speeds"

test_fan_ns = cg.esphome_ns.namespace('test_fan')
TestFan = test_fan_ns.class_('TestFan', fan.Fan, cg.Component)

CONFIG_SCHEMA = fan.FAN_SCHEMA.extend({
	cv.GenerateID() : cv.declare_id(TestFan)
}).extend({
	cv.Required(CONF_FAN_SPEEDS) : cv.positive_int
}).extend(cv.COMPONENT_SCHEMA)

async def to_code(config) :
	server = cg.new_Pvariable(config[CONF_ID])
	await cg.register_component(server, config)
	await fan.register_fan(server, config)
	cg.add(server.set_fan_speeds(config[CONF_FAN_SPEEDS]))
