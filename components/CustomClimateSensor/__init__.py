import esphome.codegen as cg
import esphome.config_validation as cv
from esphome import pins
from esphome.components import sensor
from esphome.const import CONF_PIN, CONF_TEMPERATURE, CONF_HUMIDITY, UNIT_CELSIUS, UNIT_PERCENT, ICON_THERMOMETER, ICON_WATER_PERCENT

DEPENDENCIES = ['dht']

CONF_DHT_PIN = 'pin'

custom_climate_ns = cg.esphome_ns.namespace('custom_climate')
CustomClimateSensor = custom_climate_ns.class_('CustomClimateSensor', cg.PollingComponent, sensor.Sensor)

CONFIG_SCHEMA = cv.Schema({
    cv.GenerateID(): cv.declare_id(CustomClimateSensor),
    cv.Required(CONF_DHT_PIN): pins.gpio_input_pin_schema,
    cv.Optional(CONF_TEMPERATURE): sensor.sensor_schema(UNIT_CELSIUS, ICON_THERMOMETER, 1),
    cv.Optional(CONF_HUMIDITY): sensor.sensor_schema(UNIT_PERCENT, ICON_WATER_PERCENT, 1),
}).extend(cv.polling_component_schema('15s'))

def to_code(config):
    var = cg.new_Pvariable(config[CONF_ID])
    yield cg.register_component(var, config)
    yield sensor.register_sensor(var, config[CONF_TEMPERATURE])
    yield sensor.register_sensor(var, config[CONF_HUMIDITY])
    cg.add(var.set_pin(config[CONF_DHT_PIN]))