#include "esphome/core/log.h"
#include "iq2020_text.h"
#include "iq2020.h"

extern IQ2020Component* g_iq2020_main;
extern esphome::iq2020_text::IQ2020Text* g_iq2020_text[TEXTCOUNT];

namespace esphome {
namespace iq2020_text {

	static const char *TAG = "iq2020.text";

	void IQ2020Text::setup() {
		if (text_id < TEXTCOUNT) { g_iq2020_text[text_id] = this; }
		//ESP_LOGD(TAG, "Text:%d Setup", text_id);
	}

	void IQ2020Text::control(const std::string &value) {
		ESP_LOGD(TAG, "Text:%d write state: %d", text_id, value.c_str());
		text_value = value;
		this->publish_state(value);
	}

	void IQ2020Text::dump_config() {
		//ESP_LOGCONFIG(TAG, "Text:%d config", text_id);
	}

} //namespace iq2020_text
} //namespace esphome