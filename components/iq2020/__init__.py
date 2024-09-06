import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import uart
from esphome.const import CONF_ID, CONF_PORT, CONF_FLOW_CONTROL_PIN, CONF_BUFFER_SIZE
from esphome.cpp_helpers import gpio_pin_expression
from esphome import pins

# ESPHome doesn't know the Stream abstraction yet, so hardcode to use a UART for now.

AUTO_LOAD = ["socket"]

DEPENDENCIES = ["uart", "network"]
CONF_ACE_EMULATION = "ace_emulation"
CONF_FRESHWATER_EMULATION = "freshwater_emulation"
CONF_AUDIO_EMULATION = "audio_emulation"
CONF_POLLING_RATE = "polling_rate"
CONF_TRIGGER_POLL_PIN = "trigger_poll_pin"
CONF_TEMP_UNIT = "temp_unit"
MULTI_CONF = False

ns = cg.global_ns
IQ2020Component = ns.class_("IQ2020Component", cg.Component)

def validate_buffer_size(buffer_size):
    if buffer_size & (buffer_size - 1) != 0:
        raise cv.Invalid("Buffer size must be a power of two.")
    return buffer_size

def validate_polling_rate(polling_rate):
    if polling_rate < 5:
        raise cv.Invalid("Polling rate must be 5 or above.")
    return polling_rate

CONFIG_SCHEMA = cv.All(
    cv.require_esphome_version(2022, 3, 0),
    cv.Schema(
        {
            cv.GenerateID(): cv.declare_id(IQ2020Component),
            cv.Optional(CONF_PORT, default = 0): cv.port,
            cv.Optional(CONF_BUFFER_SIZE, default = 128): cv.All(cv.positive_int, validate_buffer_size),
            cv.Optional(CONF_FLOW_CONTROL_PIN): pins.gpio_output_pin_schema,
            cv.Optional(CONF_POLLING_RATE, default = 65): cv.All(cv.positive_int, validate_polling_rate),
            cv.Optional(CONF_ACE_EMULATION, default = 'false'): cv.boolean,
            cv.Optional(CONF_FRESHWATER_EMULATION, default = 'false'): cv.boolean,
            cv.Optional(CONF_AUDIO_EMULATION, default = 'false'): cv.boolean,

            cv.Optional(CONF_TRIGGER_POLL_PIN): pins.gpio_input_pin_schema,
        }
    )
    .extend(cv.COMPONENT_SCHEMA)
    .extend(uart.UART_DEVICE_SCHEMA),
)

async def to_code(config):
    var = cg.new_Pvariable(config[CONF_ID])
    cg.add(var.set_port(config[CONF_PORT]))
    cg.add(var.set_buffer_size(config[CONF_BUFFER_SIZE]))
    cg.add(var.set_polling_rate(config[CONF_POLLING_RATE]))
    cg.add(var.set_ace_emulation(config[CONF_ACE_EMULATION]))
    cg.add(var.set_freshwater_emulation(config[CONF_FRESHWATER_EMULATION]))
    cg.add(var.set_audio_emulation(config[CONF_AUDIO_EMULATION]))

    if CONF_FLOW_CONTROL_PIN in config:
        pin = await gpio_pin_expression(config[CONF_FLOW_CONTROL_PIN])
        cg.add(var.set_flow_control_pin(pin))

    if CONF_TRIGGER_POLL_PIN in config:
        trigPin = await gpio_pin_expression(config[CONF_TRIGGER_POLL_PIN])
        cg.add(var.set_trigger_poll_pin(trigPin))

    await cg.register_component(var, config)
    await uart.register_uart_device(var, config)
