import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import text_sensor
from . import ns, IQ2020Component

CONF_IQ2020_SERVER = "iq2020_server"
CONF_SENSOR_VERSION = "versionstr"
CONF_SENSOR_RTC_DATETIME = "rtc_datetime"

CONFIG_SCHEMA = cv.Schema(
    {
        cv.GenerateID(CONF_IQ2020_SERVER): cv.use_id(IQ2020Component),
        cv.Required(CONF_SENSOR_VERSION): text_sensor.text_sensor_schema(
            icon="mdi:information"
        ),
        cv.Optional(CONF_SENSOR_RTC_DATETIME): text_sensor.text_sensor_schema(
            icon="mdi:clock-outline"
        )
    }
)

async def to_code(config):
    server = await cg.get_variable(config[CONF_IQ2020_SERVER])
    
    if CONF_SENSOR_VERSION in config:
        sens = await text_sensor.new_text_sensor(config[CONF_SENSOR_VERSION])
        cg.add(server.set_version_sensor(sens))
    
    if CONF_SENSOR_RTC_DATETIME in config:
        sens = await text_sensor.new_text_sensor(config[CONF_SENSOR_RTC_DATETIME])
        cg.add(server.set_rtc_datetime_sensor(sens))
