import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import binary_sensor
from esphome.const import (
    DEVICE_CLASS_CONNECTIVITY,
    ENTITY_CATEGORY_DIAGNOSTIC,
)
from . import ns, IQ2020Component

CONF_SENSOR_CONNECTED = "connected"
CONF_SENSOR_CONNECTIONKIT = "connectionkit"
CONF_SENSOR_SALT_BOOST = "salt_boost"
CONF_SENSOR_SALT_CONFIRMED = "salt_level_confirmed"
CONF_IQ2020_SERVER = "iq2020_server"

CONFIG_SCHEMA = cv.Schema(
    {
        cv.GenerateID(CONF_IQ2020_SERVER): cv.use_id(IQ2020Component),
        cv.Optional(CONF_SENSOR_CONNECTED): binary_sensor.binary_sensor_schema(
            device_class=DEVICE_CLASS_CONNECTIVITY,
            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
        ),
        cv.Optional(CONF_SENSOR_CONNECTIONKIT): binary_sensor.binary_sensor_schema(
            device_class=DEVICE_CLASS_CONNECTIVITY,
            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
        ),
        cv.Optional(CONF_SENSOR_SALT_BOOST): binary_sensor.binary_sensor_schema(),
        cv.Optional(CONF_SENSOR_SALT_CONFIRMED): binary_sensor.binary_sensor_schema()
    }
)

async def to_code(config):
    server = await cg.get_variable(config[CONF_IQ2020_SERVER])

    if CONF_SENSOR_CONNECTED in config:
        sens = await binary_sensor.new_binary_sensor(config[CONF_SENSOR_CONNECTED])
        cg.add(server.set_connected_sensor(sens))

    if CONF_SENSOR_CONNECTIONKIT in config:
        sens = await binary_sensor.new_binary_sensor(config[CONF_SENSOR_CONNECTIONKIT])
        cg.add(server.set_connectionkit_sensor(sens))

    if CONF_SENSOR_SALT_BOOST in config:
        sens = await binary_sensor.new_binary_sensor(config[CONF_SENSOR_SALT_BOOST])
        cg.add(server.set_salt_boost_sensor(sens))

    if CONF_SENSOR_SALT_CONFIRMED in config:
        sens = await binary_sensor.new_binary_sensor(config[CONF_SENSOR_SALT_CONFIRMED])
        cg.add(server.set_salt_confirmed_sensor(sens))