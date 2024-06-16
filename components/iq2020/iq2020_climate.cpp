#include "esphome/core/defines.h"
#include "esphome/core/component.h"
#include "esphome/core/hal.h"
#include "esphome/core/log.h"
#include "esphome/components/climate/climate_traits.h"
#include "esphome/components/climate/climate_mode.h"
#include "iq2020_climate.h"
#include "iq2020.h"

extern IQ2020Component* g_iq2020_main;
extern esphome::iq2020_climate::IQ2020Climate* g_iq2020_climate;

namespace esphome {
namespace iq2020_climate {

	//float fahrenheit_to_celsius(float f) { return (f - 32) * 5 / 9; }
	//float celsius_to_fahrenheit(float c) { return c * 9 / 5 + 32; }

	static const char *TAG = "iq2020.climate";

	void IQ2020Climate::setup() {
		g_iq2020_climate = this;
		ESP_LOGD(TAG, "Climate Setup");
		mode = esphome::climate::ClimateMode::CLIMATE_MODE_HEAT;
		action = esphome::climate::CLIMATE_ACTION_OFF;
	}

	void IQ2020Climate::control(const climate::ClimateCall &call) {
		if (call.get_target_temperature().has_value()) {
			g_iq2020_main->setTempAction(call.get_target_temperature().value());
		}
	}

	void IQ2020Climate::updateTempsF(float target, float current, bool heating) {
		current_temperature = fahrenheit_to_celsius(current);
		target_temperature = fahrenheit_to_celsius(target);
		action = heating ? esphome::climate::CLIMATE_ACTION_HEATING : esphome::climate::CLIMATE_ACTION_OFF;
		publish_state();
	}

	void IQ2020Climate::updateTempsC(float target, float current, bool heating) {
		current_temperature = current;
		target_temperature = target;
		action = heating ? esphome::climate::CLIMATE_ACTION_HEATING : esphome::climate::CLIMATE_ACTION_OFF;
		publish_state();
	}

	climate::ClimateTraits IQ2020Climate::traits() {
		auto traits = climate::ClimateTraits();
		traits.set_supports_current_temperature(true);

		std::set<esphome::climate::ClimateMode> heatingModes;
		//heatingModes.insert(esphome::climate::ClimateMode::CLIMATE_MODE_OFF);
		heatingModes.insert(esphome::climate::ClimateMode::CLIMATE_MODE_HEAT);
		traits.set_supported_modes(heatingModes);
		traits.set_supports_action(true);

		if (celsius) { // Celsius setup
			traits.set_visual_min_temperature(26.5);
			traits.set_visual_max_temperature(40);
			traits.set_visual_target_temperature_step(0.5);
			traits.set_visual_current_temperature_step(0.5);
		} else { // Fahrenheit setup
			traits.set_visual_min_temperature(fahrenheit_to_celsius(80));
			traits.set_visual_max_temperature(fahrenheit_to_celsius(104));
			traits.set_visual_target_temperature_step(1);
			traits.set_visual_current_temperature_step(1);
		}

		return traits;
	}

	void IQ2020Climate::dump_config() {
		ESP_LOGCONFIG(TAG, "Empty custom climate");
	}

} //namespace iq2020_climate
} //namespace esphome