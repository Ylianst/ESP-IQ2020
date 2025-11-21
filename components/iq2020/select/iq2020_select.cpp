#include "esphome/core/log.h"
#include "iq2020_select.h"
#include "../iq2020.h"

extern IQ2020Component* g_iq2020_main;
extern esphome::iq2020_select::IQ2020Select* g_iq2020_select[SELECTCOUNT];

namespace esphome {
namespace iq2020_select {

	static const char *TAG = "iq2020.select";

	void IQ2020Select::setup() {
		if (select_id < SELECTCOUNT) { g_iq2020_select[select_id] = this; }
		ESP_LOGD(TAG, "Select:%d Setup", select_id);

		switch (select_id) {
		case SELECT_AUDIO_SOURCE:
			this->traits.set_options({ "iPOD", "TV", "Aux", "Bluetooth" });
			break;
		case SELECT_LIGHTS1_COLOR:
		case SELECT_LIGHTS2_COLOR:
		case SELECT_LIGHTS3_COLOR:
		case SELECT_LIGHTS4_COLOR:
			if ((this->traits.get_options().size() < 7) || (this->traits.get_options().size() > 9)) {
				this->traits.set_options({ "Violet", "Blue", "Cyan", "Green", "White", "Yellow", "Red", "Cycle" });
			}
			break;
		case SELECT_LIGHTS_CYCLE_SPEED:
			if (this->traits.get_options().size() != 4) {
				this->traits.set_options({ "Off", "Slow", "Normal", "Fast" });
			}
			// If there is no off state for this control, set it to "normal"
			//if (this->traits.get_options().size() != 4) { this->publish_state("Normal"); }
			break;
		}
	}

	void IQ2020Select::control(const std::string &value) {
		ESP_LOGD(TAG, "Select:%d control state: %s", select_id, value.c_str());
		this->publish_state(value);
		if (g_iq2020_main == NULL) return;
		if (select_id == SELECT_AUDIO_SOURCE) {
			// Audio source: iPOD = 1, TV = 2, Aux = 3, Bluetooth = 4
			if (value.compare("iPOD") == 0) { g_iq2020_main->selectAction(select_id, 1); }
			else if (value.compare("TV") == 0) { g_iq2020_main->selectAction(select_id, 2); }
			else if (value.compare("Aux") == 0) { g_iq2020_main->selectAction(select_id, 3); }
			else if (value.compare("Bluetooth") == 0) { g_iq2020_main->selectAction(select_id, 4); }
		} else if (select_id == SELECT_LIGHTS_CYCLE_SPEED) {
			// Lights cycle speed
			for (int i = 0; i < this->traits.get_options().size(); i++) {
				if (value.compare(this->traits.get_options()[i]) == 0) { g_iq2020_main->selectAction(select_id, i); }
			}
		} else {
			// Lights color
			for (int i = 0; i < this->traits.get_options().size(); i++) {
				if (value.compare(this->traits.get_options()[i]) == 0) { g_iq2020_main->selectAction(select_id, i + 1); }
			}
		}
	}

	void IQ2020Select::publish_state_ex(int value) {
		ESP_LOGD(TAG, "Select:%d publish_state_ex: %d", select_id, value);
		if (select_id == SELECT_AUDIO_SOURCE) { // Audio Source, iPOD = 1, TV = 2, Aux = 3, Bluetooth = 4
			if (value == 1) { this->publish_state("iPOD"); }
			if (value == 2) { this->publish_state("TV"); }
			if (value == 3) { this->publish_state("Aux"); }
			if (value == 4) { this->publish_state("Bluetooth"); }
		} if (select_id == SELECT_LIGHTS_CYCLE_SPEED) {
			if (this->traits.get_options().size() > value) {
				this->publish_state(this->traits.get_options()[value]);
			}
		} else {
			if (this->traits.get_options().size() >= value) {
				if (value == 0) { value = 8; } // Attempt to keep from going to negative index (??)
				this->publish_state(this->traits.get_options()[value - 1]);
			}
		}
	}

	void IQ2020Select::dump_config() {
		//ESP_LOGCONFIG(TAG, "Select:%d config", switch_id);
	}

} //namespace iq2020_select
} //namespace esphome
