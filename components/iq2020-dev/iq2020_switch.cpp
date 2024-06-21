#include "esphome/core/log.h"
#include "iq2020_switch.h"
#include "iq2020.h"

extern IQ2020Component* g_iq2020_main;
extern esphome::iq2020_switch::IQ2020Switch* g_iq2020_switch[SWITCHCOUNT];

namespace esphome {
namespace iq2020_switch {

	static const char *TAG = "iq2020.switch";

	void IQ2020Switch::setup() {
		if (switch_id < SWITCHCOUNT) { g_iq2020_switch[switch_id] = this; }
		//ESP_LOGD(TAG, "Switch:%d Setup", switch_id);
	}

	void IQ2020Switch::write_state(bool state) {
		ESP_LOGD(TAG, "Switch:%d write state: %d", switch_id, state);
		//this->publish_state(state);
		if (g_iq2020_main != NULL) { g_iq2020_main->switchAction(switch_id, state); }
	}

	void IQ2020Switch::dump_config() {
		//ESP_LOGCONFIG(TAG, "Switch:%d config", switch_id);
	}

} //namespace iq2020_switch
} //namespace esphome