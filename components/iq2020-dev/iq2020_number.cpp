#include "esphome/core/log.h"
#include "iq2020_number.h"
#include "iq2020.h"

extern IQ2020Component* g_iq2020_main;
extern esphome::iq2020_number::IQ2020Switch* g_iq2020_number[NUMBERCOUNT];

namespace esphome {
namespace iq2020_number {

	static const char *TAG = "iq2020.switch";

	void IQ2020Number::setup() {
		if (switch_id < NUMBERCOUNT) { g_iq2020_number[number_id] = this; }
		//ESP_LOGD(TAG, "Number:%d Setup", switch_id);
	}

	void IQ2020Number::write_state(bool state) {
		ESP_LOGD(TAG, "Number:%d write state: %d", number_id, state);
		//this->publish_state(state);
		if (g_iq2020_main != NULL) { g_iq2020_main->numberAction(number_id, state); }
	}

	void IQ2020Number::dump_config() {
		//ESP_LOGCONFIG(TAG, "Number:%d config", switch_id);
	}

} //namespace iq2020_number
} //namespace esphome