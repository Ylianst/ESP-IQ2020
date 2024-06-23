#include "esphome/core/log.h"
#include "iq2020_select.h"
#include "iq2020.h"

extern IQ2020Component* g_iq2020_main;
extern esphome::iq2020_select::IQ2020Select* g_iq2020_select[SELECTCOUNT];

namespace esphome {
namespace iq2020_select {

	static const char *TAG = "iq2020.select";

	void IQ2020Select::setup() {
		if (select_id < SELECTCOUNT) { g_iq2020_select[select_id] = this; }
		ESP_LOGD(TAG, "Select:%d Setup", select_id);
	}

	void IQ2020Select::control(const std::string &value) {
		//ESP_LOGD(TAG, "Select:%d control state: %s", select_id, value);
		this->publish_state(value);
		if (g_iq2020_main == NULL) return;
		if (select_id == 0) { // Audio Source, TV = 2, Aux = 3, Bluetooth = 4
			if (value.compare("TV")) { g_iq2020_main->selectAction(select_id, 2); }
			if (value.compare("Aux")) { g_iq2020_main->selectAction(select_id, 3); }
			if (value.compare("Bluetooth")) { g_iq2020_main->selectAction(select_id, 4); }
		}
	}

	void IQ2020Select::publish_state_ex(int value) {
		//ESP_LOGD(TAG, "Select:%d publish_state_ex: %d", select_id, value);
		if (select_id == 0) { // Audio Source, TV = 2, Aux = 3, Bluetooth = 4
			if (value == 2) { this->publish_state("TV"); }
			if (value == 3) { this->publish_state("Aux"); }
			if (value == 4) { this->publish_state("Bluetooth"); }
		}
	}

	void IQ2020Select::dump_config() {
		//ESP_LOGCONFIG(TAG, "Select:%d config", switch_id);
	}

} //namespace iq2020_select
} //namespace esphome
