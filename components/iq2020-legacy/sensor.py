import esphome.codegen as cg
import esphome.config_validation as cv
from esphome.components import sensor
from esphome.const import (
    UNIT_WATT,
    UNIT_SECOND,
    UNIT_CELSIUS,
    UNIT_AMPERE,
    UNIT_VOLT,
    ICON_GAUGE,
    ICON_TIMER,
    ICON_COUNTER,
    ICON_LIGHTBULB,
    ICON_CURRENT_AC,
    ICON_THERMOMETER,
    ICON_HEATING_COIL,
    STATE_CLASS_MEASUREMENT,
    STATE_CLASS_TOTAL_INCREASING,
    DEVICE_CLASS_VOLUME,
    DEVICE_CLASS_TEMPERATURE,
    DEVICE_CLASS_DURATION,
    ENTITY_CATEGORY_DIAGNOSTIC
)
from . import ns, IQ2020Component


UNIT_FAHRENHEIT = "°F"
CONF_SENSOR_CURRENT_F_TEMPERATURE = "current_f_temperature"
CONF_SENSOR_TARGET_F_TEMPERATURE = "target_f_temperature"
CONF_SENSOR_OUTLET_F_TEMPERATURE = "outlet_f_temperature"
CONF_SENSOR_CURRENT_C_TEMPERATURE = "current_c_temperature"
CONF_SENSOR_TARGET_C_TEMPERATURE = "target_c_temperature"
CONF_SENSOR_OUTLET_C_TEMPERATURE = "outlet_c_temperature"
CONF_SENSOR_HEATER_TOTAL_RUNTIME = "heater_total_runtime"
CONF_SENSOR_JETS1_TOTAL_RUNTIME = "jets1_total_runtime"
CONF_SENSOR_LIFETIME_RUNTIME = "lifetime_runtime"
CONF_SENSOR_JETS2_TOTAL_RUNTIME = "jets2_total_runtime"
CONF_SENSOR_JETS3_TOTAL_RUNTIME = "jets3_total_runtime"
CONF_SENSOR_LIGHTS_TOTAL_RUNTIME = "lights_total_runtime"
CONF_SENSOR_CIRCULATION_PUMP_TOTAL_RUNTIME = "circulation_pump_total_runtime"
CONF_SENSOR_JETS1_LOW_RUNTIME = "jet1_low_total_runtime"
CONF_SENSOR_JETS2_LOW_RUNTIME = "jet2_low_total_runtime"
CONF_SENSOR_POWER_ON_COUNTER = "power_on_counter"
CONF_SENSOR_SALT_POWER = "salt_power"
CONF_SENSOR_SALT_CONTENT = "salt_content"
CONF_SENSOR_CONNECTION_COUNT = "connection_count"
CONF_SENSOR_VOLTAGE_L1 = "voltage_l1"
CONF_SENSOR_VOLTAGE_HEATER = "voltage_heater"
CONF_SENSOR_VOLTAGE_L2 = "voltage_l2"
CONF_SENSOR_CURRENT_L1 = "current_l1"
CONF_SENSOR_CURRENT_HEATER = "current_heater"
CONF_SENSOR_CURRENT_L2 = "current_l2"
CONF_SENSOR_POWER_L1 = "power_l1"
CONF_SENSOR_POWER_HEATER = "power_heater"
CONF_SENSOR_POWER_L2 = "power_l2"
CONF_SENSOR_PCB_F_TEMPERATURE = "pcb_f_temperature"
CONF_SENSOR_PCB_C_TEMPERATURE = "pcb_c_temperature"
CONF_SENSOR_BUTTONS = "buttons"
CONF_SENSOR_LOGO_LIGHTS = "logo_lights"
CONF_SENSOR_LIGHTS_INTENSITY = "lights_intensity"
CONF_SENSOR_LIGHTS_INTENSITY_UNDERWATER = "lights_underwater_intensity"
CONF_SENSOR_LIGHTS_INTENSITY_BARTOP = "lights_bartop_intensity"
CONF_SENSOR_LIGHTS_INTENSITY_PILLOW = "lights_pillow_intensity"
CONF_SENSOR_LIGHTS_INTENSITY_EXTERIOR = "lights_exterior_intensity"
CONF_SENSOR_LIGHTS_COLOR_UNDERWATER = "lights_underwater_color"
CONF_SENSOR_LIGHTS_COLOR_BARTOP = "lights_bartop_color"
CONF_SENSOR_LIGHTS_COLOR_PILLOW = "lights_pillow_color"
CONF_SENSOR_LIGHTS_COLOR_EXTERIOR = "lights_exterior_color"
CONF_SENSOR_LIGHTS_MAIN_LOOP_SPEED = "lights_main_loop_speed"
CONF_SENSOR_IQ_VA = "iq_va"
CONF_SENSOR_IQ_VB = "iq_vb"
CONF_SENSOR_IQ_VC = "iq_vc"
CONF_SENSOR_IQ_VD = "iq_vd"
CONF_SENSOR_IQ_CHLORINE = "iq_chlorine"
CONF_SENSOR_IQ_PH = "iq_ph"
CONF_SENSOR_IQ_HOURSLEFT = "iq_hoursleft"

CONF_IQ2020_SERVER = "iq2020_server"

CONFIG_SCHEMA = cv.Schema(
    {
        cv.GenerateID(CONF_IQ2020_SERVER): cv.use_id(IQ2020Component),
        cv.Optional(CONF_SENSOR_CURRENT_F_TEMPERATURE): sensor.sensor_schema(
            unit_of_measurement=UNIT_FAHRENHEIT,
            device_class=DEVICE_CLASS_TEMPERATURE,
            accuracy_decimals=0,
            state_class=STATE_CLASS_MEASUREMENT,
            icon=ICON_THERMOMETER,
        ),
        cv.Optional(CONF_SENSOR_TARGET_F_TEMPERATURE): sensor.sensor_schema(
            unit_of_measurement=UNIT_FAHRENHEIT,
            device_class=DEVICE_CLASS_TEMPERATURE,
            accuracy_decimals=0,
            state_class=STATE_CLASS_MEASUREMENT,
            icon=ICON_THERMOMETER,
        ),
        cv.Optional(CONF_SENSOR_OUTLET_F_TEMPERATURE): sensor.sensor_schema(
            unit_of_measurement=UNIT_FAHRENHEIT,
            device_class=DEVICE_CLASS_TEMPERATURE,
            accuracy_decimals=0,
            state_class=STATE_CLASS_MEASUREMENT,
            icon=ICON_THERMOMETER,
        ),
        cv.Optional(CONF_SENSOR_CURRENT_C_TEMPERATURE): sensor.sensor_schema(
            unit_of_measurement=UNIT_CELSIUS,
            device_class=DEVICE_CLASS_TEMPERATURE,
            accuracy_decimals=1,
            state_class=STATE_CLASS_MEASUREMENT,
            icon=ICON_THERMOMETER,
        ),
        cv.Optional(CONF_SENSOR_TARGET_C_TEMPERATURE): sensor.sensor_schema(
            unit_of_measurement=UNIT_CELSIUS,
            device_class=DEVICE_CLASS_TEMPERATURE,
            accuracy_decimals=1,
            state_class=STATE_CLASS_MEASUREMENT,
            icon=ICON_THERMOMETER,
        ),
        cv.Optional(CONF_SENSOR_OUTLET_C_TEMPERATURE): sensor.sensor_schema(
            unit_of_measurement=UNIT_CELSIUS,
            device_class=DEVICE_CLASS_TEMPERATURE,
            accuracy_decimals=1,
            state_class=STATE_CLASS_MEASUREMENT,
            icon=ICON_THERMOMETER,
        ),
        cv.Optional(CONF_SENSOR_CONNECTION_COUNT): sensor.sensor_schema(
            accuracy_decimals=0,
            state_class=STATE_CLASS_MEASUREMENT,
            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
        ),
        cv.Optional(CONF_SENSOR_HEATER_TOTAL_RUNTIME): sensor.sensor_schema(
            unit_of_measurement=UNIT_SECOND,
            accuracy_decimals=0,
            icon=ICON_TIMER,
            state_class=STATE_CLASS_TOTAL_INCREASING,
            device_class=DEVICE_CLASS_DURATION
        ),
        cv.Optional(CONF_SENSOR_JETS1_TOTAL_RUNTIME): sensor.sensor_schema(
            unit_of_measurement=UNIT_SECOND,
            accuracy_decimals=0,
            icon=ICON_TIMER,
            state_class=STATE_CLASS_TOTAL_INCREASING,
            device_class=DEVICE_CLASS_DURATION
        ),
        cv.Optional(CONF_SENSOR_LIFETIME_RUNTIME): sensor.sensor_schema(
            unit_of_measurement=UNIT_SECOND,
            accuracy_decimals=0,
            icon=ICON_TIMER,
            state_class=STATE_CLASS_TOTAL_INCREASING,
            device_class=DEVICE_CLASS_DURATION
        ),
        cv.Optional(CONF_SENSOR_JETS2_TOTAL_RUNTIME): sensor.sensor_schema(
            unit_of_measurement=UNIT_SECOND,
            accuracy_decimals=0,
            icon=ICON_TIMER,
            state_class=STATE_CLASS_TOTAL_INCREASING,
            device_class=DEVICE_CLASS_DURATION
        ),
        cv.Optional(CONF_SENSOR_JETS3_TOTAL_RUNTIME): sensor.sensor_schema(
            unit_of_measurement=UNIT_SECOND,
            accuracy_decimals=0,
            icon=ICON_TIMER,
            state_class=STATE_CLASS_TOTAL_INCREASING,
            device_class=DEVICE_CLASS_DURATION
        ),
        cv.Optional(CONF_SENSOR_LIGHTS_TOTAL_RUNTIME): sensor.sensor_schema(
            unit_of_measurement=UNIT_SECOND,
            accuracy_decimals=0,
            icon=ICON_TIMER,
            state_class=STATE_CLASS_TOTAL_INCREASING,
            device_class=DEVICE_CLASS_DURATION
        ),
        cv.Optional(CONF_SENSOR_CIRCULATION_PUMP_TOTAL_RUNTIME): sensor.sensor_schema(
            unit_of_measurement=UNIT_SECOND,
            accuracy_decimals=0,
            icon=ICON_TIMER,
            state_class=STATE_CLASS_TOTAL_INCREASING,
            device_class=DEVICE_CLASS_DURATION
        ),
        cv.Optional(CONF_SENSOR_JETS1_LOW_RUNTIME): sensor.sensor_schema(
            unit_of_measurement=UNIT_SECOND,
            accuracy_decimals=0,
            icon=ICON_TIMER,
            state_class=STATE_CLASS_TOTAL_INCREASING,
            device_class=DEVICE_CLASS_DURATION
        ),
        cv.Optional(CONF_SENSOR_JETS2_LOW_RUNTIME): sensor.sensor_schema(
            unit_of_measurement=UNIT_SECOND,
            accuracy_decimals=0,
            icon=ICON_TIMER,
            state_class=STATE_CLASS_TOTAL_INCREASING,
            device_class=DEVICE_CLASS_DURATION
        ),
        cv.Optional(CONF_SENSOR_POWER_ON_COUNTER): sensor.sensor_schema(
            accuracy_decimals=0,
            icon=ICON_COUNTER
        ),
        cv.Optional(CONF_SENSOR_SALT_POWER): sensor.sensor_schema(
            accuracy_decimals=0,
            icon=ICON_GAUGE
        ),
        cv.Optional(CONF_SENSOR_SALT_CONTENT): sensor.sensor_schema(
            accuracy_decimals=0,
            state_class=STATE_CLASS_MEASUREMENT,
            icon=ICON_GAUGE
        ),
        cv.Optional(CONF_SENSOR_VOLTAGE_L1): sensor.sensor_schema(
            unit_of_measurement=UNIT_VOLT,
            state_class=STATE_CLASS_MEASUREMENT,
            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
            accuracy_decimals=0,
            icon=ICON_CURRENT_AC
        ), 
        cv.Optional(CONF_SENSOR_VOLTAGE_HEATER): sensor.sensor_schema(
            unit_of_measurement=UNIT_VOLT,
            state_class=STATE_CLASS_MEASUREMENT,
            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
            accuracy_decimals=0,
            icon=ICON_CURRENT_AC
        ),
        cv.Optional(CONF_SENSOR_VOLTAGE_L2): sensor.sensor_schema(
            unit_of_measurement=UNIT_VOLT,
            state_class=STATE_CLASS_MEASUREMENT,
            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
            accuracy_decimals=0,
            icon=ICON_CURRENT_AC
        ),
        cv.Optional(CONF_SENSOR_CURRENT_L1): sensor.sensor_schema(
            unit_of_measurement=UNIT_AMPERE,
            state_class=STATE_CLASS_MEASUREMENT,
            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
            accuracy_decimals=0,
            icon=ICON_CURRENT_AC
        ),
        cv.Optional(CONF_SENSOR_CURRENT_HEATER): sensor.sensor_schema(
            unit_of_measurement=UNIT_AMPERE,
            state_class=STATE_CLASS_MEASUREMENT,
            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
            accuracy_decimals=0,
            icon=ICON_CURRENT_AC
        ),
        cv.Optional(CONF_SENSOR_CURRENT_L2): sensor.sensor_schema(
            unit_of_measurement=UNIT_AMPERE,
            state_class=STATE_CLASS_MEASUREMENT,
            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
            accuracy_decimals=0,
            icon=ICON_CURRENT_AC
        ),
        cv.Optional(CONF_SENSOR_POWER_L1): sensor.sensor_schema(
            unit_of_measurement=UNIT_WATT,
            state_class=STATE_CLASS_MEASUREMENT,
            accuracy_decimals=0,
            icon="mdi:turbine"
        ),
        cv.Optional(CONF_SENSOR_POWER_HEATER): sensor.sensor_schema(
            unit_of_measurement=UNIT_WATT,
            state_class=STATE_CLASS_MEASUREMENT,
            accuracy_decimals=0,
            icon=ICON_CURRENT_AC
        ),
        cv.Optional(CONF_SENSOR_POWER_L2): sensor.sensor_schema(
            unit_of_measurement=UNIT_WATT,
            state_class=STATE_CLASS_MEASUREMENT,
            accuracy_decimals=0,
            icon=ICON_HEATING_COIL
        ),
        cv.Optional(CONF_SENSOR_PCB_F_TEMPERATURE): sensor.sensor_schema(
            unit_of_measurement=UNIT_FAHRENHEIT,
            state_class=STATE_CLASS_MEASUREMENT,
            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
            accuracy_decimals=0,
            icon=ICON_THERMOMETER
        ),
        cv.Optional(CONF_SENSOR_PCB_C_TEMPERATURE): sensor.sensor_schema(
            unit_of_measurement=UNIT_CELSIUS,
            state_class=STATE_CLASS_MEASUREMENT,
            entity_category=ENTITY_CATEGORY_DIAGNOSTIC,
            accuracy_decimals=1,
            icon=ICON_THERMOMETER
        ),
        cv.Optional(CONF_SENSOR_BUTTONS): sensor.sensor_schema(
            accuracy_decimals=0
        ),
        cv.Optional(CONF_SENSOR_LOGO_LIGHTS): sensor.sensor_schema(
            accuracy_decimals=0,
            icon=ICON_LIGHTBULB
        ),
        cv.Optional(CONF_SENSOR_LIGHTS_INTENSITY): sensor.sensor_schema(
            accuracy_decimals=0,
            icon=ICON_LIGHTBULB
        ),
        cv.Optional(CONF_SENSOR_LIGHTS_INTENSITY_UNDERWATER): sensor.sensor_schema(
            accuracy_decimals=0,
            icon=ICON_LIGHTBULB
        ),
        cv.Optional(CONF_SENSOR_LIGHTS_INTENSITY_BARTOP): sensor.sensor_schema(
            accuracy_decimals=0,
            icon=ICON_LIGHTBULB
        ),
        cv.Optional(CONF_SENSOR_LIGHTS_INTENSITY_PILLOW): sensor.sensor_schema(
            accuracy_decimals=0,
            icon=ICON_LIGHTBULB
        ),
        cv.Optional(CONF_SENSOR_LIGHTS_INTENSITY_EXTERIOR): sensor.sensor_schema(
            accuracy_decimals=0,
            icon=ICON_LIGHTBULB
        ),
        cv.Optional(CONF_SENSOR_LIGHTS_COLOR_UNDERWATER): sensor.sensor_schema(
            accuracy_decimals=0,
            icon=ICON_LIGHTBULB
        ),
        cv.Optional(CONF_SENSOR_LIGHTS_COLOR_BARTOP): sensor.sensor_schema(
            accuracy_decimals=0,
            icon=ICON_LIGHTBULB
        ),
        cv.Optional(CONF_SENSOR_LIGHTS_COLOR_PILLOW): sensor.sensor_schema(
            accuracy_decimals=0,
            icon=ICON_LIGHTBULB
        ),
        cv.Optional(CONF_SENSOR_LIGHTS_COLOR_EXTERIOR): sensor.sensor_schema(
            accuracy_decimals=0,
            icon=ICON_LIGHTBULB
        ),
        cv.Optional(CONF_SENSOR_LIGHTS_MAIN_LOOP_SPEED): sensor.sensor_schema(
            accuracy_decimals=0,
            icon=ICON_LIGHTBULB
        ),
        cv.Optional(CONF_SENSOR_IQ_VA): sensor.sensor_schema(
            accuracy_decimals=0
        ),
        cv.Optional(CONF_SENSOR_IQ_VB): sensor.sensor_schema(
            accuracy_decimals=0
        ),
        cv.Optional(CONF_SENSOR_IQ_VC): sensor.sensor_schema(
            accuracy_decimals=0
        ),
        cv.Optional(CONF_SENSOR_IQ_VD): sensor.sensor_schema(
            accuracy_decimals=0
        ),
        cv.Optional(CONF_SENSOR_IQ_CHLORINE): sensor.sensor_schema(
            accuracy_decimals=1
        ),
        cv.Optional(CONF_SENSOR_IQ_PH): sensor.sensor_schema(
            accuracy_decimals=1
        ),
        cv.Optional(CONF_SENSOR_IQ_HOURSLEFT): sensor.sensor_schema(
            accuracy_decimals=0
        )
    }
)

async def to_code(config):
    server = await cg.get_variable(config[CONF_IQ2020_SERVER])

    if CONF_SENSOR_CURRENT_F_TEMPERATURE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_CURRENT_F_TEMPERATURE])
        cg.add(server.set_current_f_temp_sensor(sens))

    if CONF_SENSOR_TARGET_F_TEMPERATURE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_TARGET_F_TEMPERATURE])
        cg.add(server.set_target_f_temp_sensor(sens))

    if CONF_SENSOR_OUTLET_F_TEMPERATURE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_OUTLET_F_TEMPERATURE])
        cg.add(server.set_outlet_f_temp_sensor(sens))

    if CONF_SENSOR_CURRENT_C_TEMPERATURE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_CURRENT_C_TEMPERATURE])
        cg.add(server.set_current_c_temp_sensor(sens))

    if CONF_SENSOR_TARGET_C_TEMPERATURE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_TARGET_C_TEMPERATURE])
        cg.add(server.set_target_c_temp_sensor(sens))

    if CONF_SENSOR_OUTLET_C_TEMPERATURE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_OUTLET_C_TEMPERATURE])
        cg.add(server.set_outlet_c_temp_sensor(sens))

    if CONF_SENSOR_CONNECTION_COUNT in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_CONNECTION_COUNT])
        cg.add(server.set_connection_count_sensor(sens))

    if CONF_SENSOR_HEATER_TOTAL_RUNTIME in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_HEATER_TOTAL_RUNTIME])
        cg.add(server.set_heater_total_runtime_sensor(sens))

    if CONF_SENSOR_JETS1_TOTAL_RUNTIME in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_JETS1_TOTAL_RUNTIME])
        cg.add(server.set_jets1_total_runtime_sensor(sens))

    if CONF_SENSOR_LIFETIME_RUNTIME in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_LIFETIME_RUNTIME])
        cg.add(server.set_lifetime_runtime_sensor(sens))

    if CONF_SENSOR_JETS2_TOTAL_RUNTIME in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_JETS2_TOTAL_RUNTIME])
        cg.add(server.set_jets2_total_runtime_sensor(sens))

    if CONF_SENSOR_JETS3_TOTAL_RUNTIME in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_JETS3_TOTAL_RUNTIME])
        cg.add(server.set_jets3_total_runtime_sensor(sens))

    if CONF_SENSOR_LIGHTS_TOTAL_RUNTIME in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_LIGHTS_TOTAL_RUNTIME])
        cg.add(server.set_lights_total_runtime_sensor(sens))

    if CONF_SENSOR_CIRCULATION_PUMP_TOTAL_RUNTIME in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_CIRCULATION_PUMP_TOTAL_RUNTIME])
        cg.add(server.set_circ_pump_total_runtime_sensor(sens))

    if CONF_SENSOR_JETS1_LOW_RUNTIME in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_JETS1_LOW_RUNTIME])
        cg.add(server.set_jet1_low_total_runtime_sensor(sens))

    if CONF_SENSOR_JETS2_LOW_RUNTIME in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_JETS2_LOW_RUNTIME])
        cg.add(server.set_jet2_low_total_runtime_sensor(sens))

    if CONF_SENSOR_POWER_ON_COUNTER in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_POWER_ON_COUNTER])
        cg.add(server.set_power_on_counter_sensor(sens))

    if CONF_SENSOR_SALT_POWER in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_SALT_POWER])
        cg.add(server.set_salt_power_sensor(sens))

    if CONF_SENSOR_SALT_CONTENT in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_SALT_CONTENT])
        cg.add(server.set_salt_content_sensor(sens))

    if CONF_SENSOR_VOLTAGE_L1 in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_VOLTAGE_L1])
        cg.add(server.set_voltage_l1_sensor(sens))

    if CONF_SENSOR_VOLTAGE_HEATER in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_VOLTAGE_HEATER])
        cg.add(server.set_voltage_heater_sensor(sens))

    if CONF_SENSOR_VOLTAGE_L2 in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_VOLTAGE_L2])
        cg.add(server.set_voltage_l2_sensor(sens))

    if CONF_SENSOR_CURRENT_L1 in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_CURRENT_L1])
        cg.add(server.set_current_l1_sensor(sens))

    if CONF_SENSOR_CURRENT_HEATER in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_CURRENT_HEATER])
        cg.add(server.set_current_heater_sensor(sens))

    if CONF_SENSOR_CURRENT_L2 in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_CURRENT_L2])
        cg.add(server.set_current_l2_sensor(sens))

    if CONF_SENSOR_POWER_L1 in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_POWER_L1])
        cg.add(server.set_power_l1_sensor(sens))

    if CONF_SENSOR_POWER_HEATER in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_POWER_HEATER])
        cg.add(server.set_power_heater_sensor(sens))

    if CONF_SENSOR_POWER_L2 in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_POWER_L2])
        cg.add(server.set_power_l2_sensor(sens))

    if CONF_SENSOR_PCB_F_TEMPERATURE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_PCB_F_TEMPERATURE])
        cg.add(server.set_pcb_f_temperature_sensor(sens))

    if CONF_SENSOR_PCB_C_TEMPERATURE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_PCB_C_TEMPERATURE])
        cg.add(server.set_pcb_c_temperature_sensor(sens))

    if CONF_SENSOR_BUTTONS in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_BUTTONS])
        cg.add(server.set_buttons_sensor(sens))

    if CONF_SENSOR_LOGO_LIGHTS in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_LOGO_LIGHTS])
        cg.add(server.set_logo_lights_sensor(sens))

    if CONF_SENSOR_LIGHTS_INTENSITY in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_LIGHTS_INTENSITY])
        cg.add(server.set_lights_intensity_sensor(sens))

    if CONF_SENSOR_LIGHTS_INTENSITY_UNDERWATER in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_LIGHTS_INTENSITY_UNDERWATER])
        cg.add(server.set_lights_intensity_underwater_sensor(sens))

    if CONF_SENSOR_LIGHTS_INTENSITY_BARTOP in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_LIGHTS_INTENSITY_BARTOP])
        cg.add(server.set_lights_intensity_bartop_sensor(sens))

    if CONF_SENSOR_LIGHTS_INTENSITY_PILLOW in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_LIGHTS_INTENSITY_PILLOW])
        cg.add(server.set_lights_intensity_pillow_sensor(sens))

    if CONF_SENSOR_LIGHTS_INTENSITY_EXTERIOR in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_LIGHTS_INTENSITY_EXTERIOR])
        cg.add(server.set_lights_intensity_exterior_sensor(sens))

    if CONF_SENSOR_LIGHTS_COLOR_UNDERWATER in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_LIGHTS_COLOR_UNDERWATER])
        cg.add(server.set_lights_color_underwater_sensor(sens))

    if CONF_SENSOR_LIGHTS_COLOR_BARTOP in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_LIGHTS_COLOR_BARTOP])
        cg.add(server.set_lights_color_bartop_sensor(sens))

    if CONF_SENSOR_LIGHTS_COLOR_PILLOW in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_LIGHTS_COLOR_PILLOW])
        cg.add(server.set_lights_color_pillow_sensor(sens))

    if CONF_SENSOR_LIGHTS_COLOR_EXTERIOR in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_LIGHTS_COLOR_EXTERIOR])
        cg.add(server.set_lights_color_exterior_sensor(sens))

    if CONF_SENSOR_LIGHTS_MAIN_LOOP_SPEED in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_LIGHTS_MAIN_LOOP_SPEED])
        cg.add(server.set_lights_main_loop_speed_sensor(sens))

    if CONF_SENSOR_IQ_VA in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_IQ_VA])
        cg.add(server.set_iq_va_sensor(sens))

    if CONF_SENSOR_IQ_VB in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_IQ_VB])
        cg.add(server.set_iq_vb_sensor(sens))

    if CONF_SENSOR_IQ_VC in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_IQ_VC])
        cg.add(server.set_iq_vc_sensor(sens))

    if CONF_SENSOR_IQ_VD in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_IQ_VD])
        cg.add(server.set_iq_vd_sensor(sens))

    if CONF_SENSOR_IQ_CHLORINE in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_IQ_CHLORINE])
        cg.add(server.set_iq_chlorine_sensor(sens))

    if CONF_SENSOR_IQ_PH in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_IQ_PH])
        cg.add(server.set_iq_ph_sensor(sens))

    if CONF_SENSOR_IQ_HOURSLEFT in config:
        sens = await sensor.new_sensor(config[CONF_SENSOR_IQ_HOURSLEFT])
        cg.add(server.set_iq_hoursleft_sensor(sens))
