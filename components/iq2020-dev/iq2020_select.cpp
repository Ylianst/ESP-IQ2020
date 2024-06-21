#include "esphome/core/log.h"
#include "iq2020_select.h"
#include "iq2020.h"

extern IQ2020Component* g_iq2020_main;
//extern esphome::iq2020_select::IQ2020Select* g_iq2020_select[SELECTCOUNT];

namespace esphome {
namespace iq2020_select {

	static const char *TAG = "iq2020.select";

	void IQ2020Select::setup() {
		//if (select_id < SELECTCOUNT) { g_iq2020_select[select_id] = this; }
		//ESP_LOGD(TAG, "Switch:%d Setup", switch_id);
	}

	void IQ2020Select::write_state(bool state) {
		ESP_LOGD(TAG, "Select:%d write state: %d", select_id, state);
		//this->publish_state(state);
		//if (g_iq2020_main != NULL) { g_iq2020_main->selectAction(select_id, state); }
	}

	void IQ2020Select::dump_config() {
		//ESP_LOGCONFIG(TAG, "Select:%d config", switch_id);
	}

} //namespace iq2020_select
} //namespace esphome