#include "esphome/core/defines.h"
#include "esphome/core/component.h"
#include "esphome/core/hal.h"
#include "esphome/core/log.h"
#include "esphome/components/climate/climate_traits.h"
#include "esphome/components/climate/climate_mode.h"
#include "iq2020_climate.h"
#include "../iq2020.h"
#include <cstring>

extern IQ2020Component* g_iq2020_main;
extern esphome::iq2020_climate::IQ2020Climate* g_iq2020_climate;

namespace esphome {
namespace iq2020_climate {

	//float fahrenheit_to_celsius(float f) { return (f - 32) * 5 / 9; }
	//float celsius_to_fahrenheit(float c) { return c * 9 / 5 + 32; }

	static const char *TAG = "iq2020.climate";

	// Coolzone mode names, indexed by the mode byte (0x00 to 0x04) of command 0x1D07.
	static const char *const COOLZONE_PRESETS[] = {
		"Heat w/Boost", // 0x00
		"Heat Saver",   // 0x01
		"Chill",        // 0x02
		"Auto w/Boost", // 0x03
		"Auto Saver",   // 0x04
	};
	static const int COOLZONE_PRESET_COUNT = 5;

	esphome::climate::ClimateAction IQ2020Climate::mapAction(int action) {
		switch (action) {
		case CLIMATE_ACT_HEATING: return esphome::climate::CLIMATE_ACTION_HEATING;
		case CLIMATE_ACT_COOLING: return esphome::climate::CLIMATE_ACTION_COOLING;
		case CLIMATE_ACT_IDLE:    return esphome::climate::CLIMATE_ACTION_IDLE;
		default:                  return esphome::climate::CLIMATE_ACTION_OFF;
		}
	}

	void IQ2020Climate::setup() {
		g_iq2020_climate = this;
		ESP_LOGD(TAG, "Climate Setup");
		mode = esphome::climate::CLIMATE_MODE_HEAT;
		action = esphome::climate::CLIMATE_ACTION_OFF;
	}

	// Enable the coolzone modes on this climate control. Called by the main component during its
	// setup (which runs after the climate setup), so the coolzone config flag is known.
	void IQ2020Climate::enableCoolzone() {
		coolzone_ = true;
		// Use HEAT_COOL so Home Assistant reliably renders both heating (red) and cooling (blue) actions.
		mode = esphome::climate::CLIMATE_MODE_HEAT_COOL;
		// Register the 5 coolzone modes as custom presets on the same climate control.
		this->set_supported_custom_presets(COOLZONE_PRESETS);
	}

	void IQ2020Climate::control(const climate::ClimateCall &call) {
		if (call.get_target_temperature().has_value()) {
			g_iq2020_main->setTempAction(call.get_target_temperature().value());
		}
		if (call.has_custom_preset()) {
			esphome::StringRef preset = call.get_custom_preset();
			for (int i = 0; i < COOLZONE_PRESET_COUNT; i++) {
				if (strcmp(preset.c_str(), COOLZONE_PRESETS[i]) == 0) {
					g_iq2020_main->setCoolzoneMode(i);
					break;
				}
			}
		}
	}

	void IQ2020Climate::updateTempsF(float target, float current, int action) {
		current_temperature = fahrenheit_to_celsius(current);
		target_temperature = fahrenheit_to_celsius(target);
		this->action = mapAction(action);
		publish_state();
	}

	void IQ2020Climate::updateTempsC(float target, float current, int action) {
		current_temperature = current;
		target_temperature = target;
		this->action = mapAction(action);
		publish_state();
	}

	void IQ2020Climate::updateCoolzone(int mode, int action) {
		if ((mode >= 0) && (mode < COOLZONE_PRESET_COUNT)) {
			this->set_custom_preset_(COOLZONE_PRESETS[mode]);
		}
		this->action = mapAction(action);
		publish_state();
	}

	climate::ClimateTraits IQ2020Climate::traits() {
		auto traits = climate::ClimateTraits();
		traits.add_feature_flags(climate::CLIMATE_SUPPORTS_CURRENT_TEMPERATURE);

		climate::ClimateModeMask heatingModes;
		if (coolzone_) {
			// Use HEAT_COOL so Home Assistant reliably renders both heating (red) and cooling (blue) actions.
			heatingModes.insert(climate::ClimateMode::CLIMATE_MODE_HEAT_COOL);
		} else {
			heatingModes.insert(climate::ClimateMode::CLIMATE_MODE_HEAT);
		}
		traits.set_supported_modes(heatingModes);
		traits.add_feature_flags(climate::CLIMATE_SUPPORTS_ACTION);

		if (celsius) { // Celsius setup
			traits.set_visual_min_temperature(26);
			traits.set_visual_max_temperature(40);
			traits.set_visual_target_temperature_step(0.5);
			traits.set_visual_current_temperature_step(0.5);
		} else { // Fahrenheit setup
			traits.set_visual_min_temperature(26); // fahrenheit_to_celsius(80)
			traits.set_visual_max_temperature(40); // fahrenheit_to_celsius(104)
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