#include "esphome/core/log.h"
#include "iq2020_switch.h"
#include "iq2020.h"

namespace esphome {
namespace iq2020_switch {

	static const char *TAG = "iq2020.switch";

	void IQ2020Switch::setup() {
		ESP_LOGD(TAG, "Switch Setup");
	}

	void IQ2020Switch::write_state(bool state) {
		ESP_LOGD(TAG, "Switch write state: %d", state);
		this->publish_state(state);
		g_iq2020_main->testcall("Hello!");
	}

	void IQ2020Switch::dump_config() {
		ESP_LOGCONFIG(TAG, "Empty custom switch");
	}

} //namespace iq2020_switch
} //namespace esphome