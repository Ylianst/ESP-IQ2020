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

	// The 5 coolzone modes are exposed as a combination of an HVAC mode (Heat/Cool/Auto)
	// and a "Boost" or "Saver" preset:
	//   Heat + Boost -> Heat w/Boost (0x00)   Heat + Saver -> Heat Saver (0x01)
	//   Cool         -> Chill        (0x02)   (cooling + boost is not possible)
	//   Auto + Boost -> Auto w/Boost (0x03)   Auto + Saver -> Auto Saver (0x04)
	static const char *const COOLZONE_PRESET_SAVER = "Saver";
	static const char *const COOLZONE_PRESET_BOOST = "Boost";
	static const char *const COOLZONE_PRESETS[] = { "Saver", "Boost" };

	esphome::climate::ClimateAction IQ2020Climate::mapAction(int action) {
		switch (action) {
		case CLIMATE_ACT_HEATING: return esphome::climate::CLIMATE_ACTION_HEATING;
		case CLIMATE_ACT_COOLING: return esphome::climate::CLIMATE_ACTION_COOLING;
		case CLIMATE_ACT_IDLE:    return esphome::climate::CLIMATE_ACTION_IDLE;
		default:                  return esphome::climate::CLIMATE_ACTION_OFF;
		}
	}

	// Returns true if the currently active preset is "Boost".
	bool IQ2020Climate::currentPresetBoost() {
		esphome::StringRef preset = this->get_custom_preset();
		return strcmp(preset.c_str(), COOLZONE_PRESET_BOOST) == 0;
	}

	// Maps an HVAC mode + boost flag to a coolzone mode byte (0x00 to 0x04), or -1 if not applicable.
	int IQ2020Climate::coolzoneModeFor(esphome::climate::ClimateMode mode, bool boost) {
		switch (mode) {
		case esphome::climate::CLIMATE_MODE_HEAT: return boost ? COOLZONE_MODE_HEAT_BOOST : COOLZONE_MODE_HEAT_SAVER;
		case esphome::climate::CLIMATE_MODE_COOL: return COOLZONE_MODE_CHILL; // Cooling + boost is not possible
		case esphome::climate::CLIMATE_MODE_AUTO: return boost ? COOLZONE_MODE_AUTO_BOOST : COOLZONE_MODE_AUTO_SAVER;
		default: return -1;
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
		// Register the "Saver" and "Boost" presets on the same climate control.
		this->set_supported_custom_presets(COOLZONE_PRESETS);
	}

	void IQ2020Climate::control(const climate::ClimateCall &call) {
		if (call.get_target_temperature().has_value()) {
			g_iq2020_main->setTempAction(call.get_target_temperature().value());
		}
		if (coolzone_ && (call.get_mode().has_value() || call.has_custom_preset())) {
			// Combine the requested HVAC mode with the requested (or current) Boost/Saver preset.
			esphome::climate::ClimateMode newMode = call.get_mode().has_value() ? call.get_mode().value() : this->mode;
			bool boost = currentPresetBoost();
			if (call.has_custom_preset()) {
				boost = (strcmp(call.get_custom_preset().c_str(), COOLZONE_PRESET_BOOST) == 0);
			}
			int cz = coolzoneModeFor(newMode, boost);
			if (cz >= 0) { g_iq2020_main->setCoolzoneMode(cz); }
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
		// Map the reported coolzone mode back to an HVAC mode + Boost/Saver preset.
		switch (mode) {
		case COOLZONE_MODE_HEAT_BOOST:
			this->mode = esphome::climate::CLIMATE_MODE_HEAT;
			this->set_custom_preset_(COOLZONE_PRESET_BOOST);
			break;
		case COOLZONE_MODE_HEAT_SAVER:
			this->mode = esphome::climate::CLIMATE_MODE_HEAT;
			this->set_custom_preset_(COOLZONE_PRESET_SAVER);
			break;
		case COOLZONE_MODE_CHILL:
			this->mode = esphome::climate::CLIMATE_MODE_COOL;
			this->set_custom_preset_(COOLZONE_PRESET_SAVER); // Cooling has no boost
			break;
		case COOLZONE_MODE_AUTO_BOOST:
			this->mode = esphome::climate::CLIMATE_MODE_AUTO;
			this->set_custom_preset_(COOLZONE_PRESET_BOOST);
			break;
		case COOLZONE_MODE_AUTO_SAVER:
			this->mode = esphome::climate::CLIMATE_MODE_AUTO;
			this->set_custom_preset_(COOLZONE_PRESET_SAVER);
			break;
		}
		this->action = mapAction(action);
		publish_state();
	}

	climate::ClimateTraits IQ2020Climate::traits() {
		auto traits = climate::ClimateTraits();
		traits.add_feature_flags(climate::CLIMATE_SUPPORTS_CURRENT_TEMPERATURE);

		climate::ClimateModeMask heatingModes;
		if (coolzone_) {
			// Heat, Cool and Auto map to the 5 coolzone modes together with the Boost/Saver preset.
			heatingModes.insert(climate::ClimateMode::CLIMATE_MODE_HEAT);
			heatingModes.insert(climate::ClimateMode::CLIMATE_MODE_COOL);
			heatingModes.insert(climate::ClimateMode::CLIMATE_MODE_AUTO);
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