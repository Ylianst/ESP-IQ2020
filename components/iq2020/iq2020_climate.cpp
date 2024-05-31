#include "esphome/core/defines.h"
#include "esphome/core/component.h"
#include "esphome/core/hal.h"
#include "esphome/core/log.h"
#include "esphome/components/climate/climate_traits.h"

//#include "esphome/core/log.h"
#include "iq2020_climate.h"
//#include "iq2020.h"

//extern IQ2020Component* g_iq2020_main;
//extern esphome::iq2020_climate::IQ2020Climate* g_iq2020_light_climate;

namespace esphome {
namespace iq2020_climate {

	static const char *TAG = "iq2020.switch";

	void IQ2020Climate::setup() {
		//g_iq2020_light_climate = this;
		ESP_LOGD(TAG, "Climate Setup");
	}

	void IQ2020Climate::write_state(int state) {
		ESP_LOGD(TAG, "Climate write state: %d", state);
		//this->publish_state(state);
		//if (g_iq2020_main != NULL) { g_iq2020_main->ClimateAction(state); }
	}

	void IQ2020Climate::dump_config() {
		ESP_LOGCONFIG(TAG, "Empty custom climate");
	}

} //namespace iq2020_climate
} //namespace esphome