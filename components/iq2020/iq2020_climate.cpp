#include "esphome/core/defines.h"
#include "esphome/core/component.h"
#include "esphome/core/hal.h"
#include "esphome/core/log.h"
#include "esphome/components/climate/climate_traits.h"
//#include "esphome/components/climate/climate_mode.h"
#include "iq2020_climate.h"
#include "iq2020.h"

extern IQ2020Component* g_iq2020_main;
extern esphome::iq2020_climate::IQ2020Climate* g_iq2020_climate;

namespace esphome {
namespace iq2020_climate {

	static const char *TAG = "iq2020.climate";

	void IQ2020Climate::setup() {
		g_iq2020_climate = this;
		ESP_LOGD(TAG, "Climate Setup");
	}

	void IQ2020Climate::control(const climate::ClimateCall &call) {
		if (call.get_target_temperature().has_value()) {
			g_iq2020_main->SetTempAction(call.get_target_temperature().value());
		}
	}

	climate::ClimateTraits IQ2020Climate::traits() {
		auto traits = climate::ClimateTraits();
		traits.set_supports_current_temperature(true);

		std::set<uint8_t> heatingModes;
		heatingModes.insert(6); // ClimateMode::CLIMATE_MODE_AUTO
		traits.set_supported_modes(heatingModes);

		/*
		traits.set_supports_current_temperature(!current_temperature_id_.empty());
		traits.set_supports_current_humidity(!current_humidity_id_.empty());
		traits.set_supports_target_humidity(!target_dehumidification_level_id_.empty());
		traits.set_supports_two_point_target_temperature(!target_temperature_high_id_.empty());
		if (!mode_id_.empty()) {traits.set_supported_modes(map_values_as_set(modes_));}
		if (!custom_preset_id_.empty()) {traits.set_supported_custom_presets(map_values_as_set(custom_presets_));}
		if (!custom_fan_mode_id_.empty()) {traits.set_supported_custom_fan_modes(map_values_as_set(custom_fan_modes_));}
		*/
		return traits;
	}

	void IQ2020Climate::dump_config() {
		ESP_LOGCONFIG(TAG, "Empty custom climate");
	}

} //namespace iq2020_climate
} //namespace esphome