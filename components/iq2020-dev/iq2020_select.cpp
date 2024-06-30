#include "esphome/core/log.h"
#include "iq2020_select.h"
#include "iq2020.h"

extern IQ2020Component* g_iq2020_main;
extern esphome::iq2020_select::IQ2020Select* g_iq2020_select[SELECTCOUNT];

std::vector<std::string> audio_source_values = { "TV", "Aux", "Bluetooth" };
std::vector<std::string> lights_colors_values = { "Violet", "Blue", "Cyan", "Green", "White", "Yellow", "Red", "Cycle" };
std::vector<std::string> lights_cycle_speed = { "Pause", "Slow", "Normal", "Fast" };

namespace esphome {
namespace iq2020_select {

	static const char *TAG = "iq2020.select";

	void IQ2020Select::setup() {
		if (select_id < SELECTCOUNT) { g_iq2020_select[select_id] = this; }
		ESP_LOGD(TAG, "Select:%d Setup", select_id);

		switch (select_id) {
		case SELECT_AUDIO_SOURCE:
			this->traits.set_options(audio_source_values);
			break;
		case SELECT_LIGHTS1_COLOR:
		case SELECT_LIGHTS2_COLOR:
		case SELECT_LIGHTS3_COLOR:
		case SELECT_LIGHTS4_COLOR:
			this->traits.set_options(lights_colors_values);
			break; 
		case SELECT_LIGHTS_CYCLE_SPEED:
			this->publish_state("Normal");
			this->traits.set_options(lights_cycle_speed);
			break;
		}
	}

	void IQ2020Select::control(const std::string &value) {
		ESP_LOGD(TAG, "Select:%d control state: %s", select_id, value.c_str());
		this->publish_state(value);
		if (g_iq2020_main == NULL) return;
		if (select_id == SELECT_AUDIO_SOURCE) { // Audio Source: TV = 2, Aux = 3, Bluetooth = 4
			if (value.compare("TV") == 0) { g_iq2020_main->selectAction(select_id, 2); }
			else if (value.compare("Aux") == 0) { g_iq2020_main->selectAction(select_id, 3); }
			else if (value.compare("Bluetooth") == 0) { g_iq2020_main->selectAction(select_id, 4); }
		} else if (select_id == SELECT_LIGHTS_CYCLE_SPEED) { // Lights cycle Speed
			if (value.compare("Pause") == 0) { g_iq2020_main->selectAction(select_id, 0); }
			else if (value.compare("Slow") == 0) { g_iq2020_main->selectAction(select_id, 1); }
			else if (value.compare("Normal") == 0) { g_iq2020_main->selectAction(select_id, 2); }
			else if (value.compare("Fast") == 0) { g_iq2020_main->selectAction(select_id, 3); }
		} else { // Lights color
			if (value.compare("Violet") == 0) { g_iq2020_main->selectAction(select_id, 1); }
			else if (value.compare("Blue") == 0) { g_iq2020_main->selectAction(select_id, 2); }
			else if (value.compare("Cyan") == 0) { g_iq2020_main->selectAction(select_id, 3); }
			else if (value.compare("Green") == 0) { g_iq2020_main->selectAction(select_id, 4); }
			else if (value.compare("White") == 0) { g_iq2020_main->selectAction(select_id, 5); }
			else if (value.compare("Yellow") == 0) { g_iq2020_main->selectAction(select_id, 6); }
			else if (value.compare("Red") == 0) { g_iq2020_main->selectAction(select_id, 7); }
			else if (value.compare("Cycle") == 0) { g_iq2020_main->selectAction(select_id, 8); }
		}
	}

	void IQ2020Select::publish_state_ex(int value) {
		ESP_LOGD(TAG, "Select:%d publish_state_ex: %d", select_id, value);
		if (select_id == SELECT_AUDIO_SOURCE) { // Audio Source, TV = 2, Aux = 3, Bluetooth = 4
			if (value == 2) { this->publish_state("TV"); }
			if (value == 3) { this->publish_state("Aux"); }
			if (value == 4) { this->publish_state("Bluetooth"); }
		} if (select_id == SELECT_LIGHTS_CYCLE_SPEED) {
			if (value == 0) { this->publish_state("Pause"); }
			if (value == 1) { this->publish_state("Slow"); }
			if (value == 2) { this->publish_state("Normal"); }
			if (value == 3) { this->publish_state("Fast"); }
		} else {
			if (value == 1) { this->publish_state("Violet"); }
			if (value == 2) { this->publish_state("Blue"); }
			if (value == 3) { this->publish_state("Cyan"); }
			if (value == 4) { this->publish_state("Green"); }
			if (value == 5) { this->publish_state("White"); }
			if (value == 6) { this->publish_state("Yellow"); }
			if (value == 7) { this->publish_state("Red"); }
			if (value == 8) { this->publish_state("Cycle"); }
		}
	}

	void IQ2020Select::dump_config() {
		//ESP_LOGCONFIG(TAG, "Select:%d config", switch_id);
	}

} //namespace iq2020_select
} //namespace esphome
