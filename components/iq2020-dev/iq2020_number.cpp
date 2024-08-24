#include "esphome/core/log.h"
#include "iq2020_number.h"
#include "iq2020.h"

extern IQ2020Component* g_iq2020_main;
extern esphome::iq2020_number::IQ2020Number* g_iq2020_number[NUMBERCOUNT];

namespace esphome {
namespace iq2020_number {

	static const char *TAG = "iq2020.number";

	void IQ2020Number::setup() {
		if (number_id < NUMBERCOUNT) { g_iq2020_number[number_id] = this; }
		//ESP_LOGD(TAG, "Number:%d Setup", switch_id);

		switch (number_id) {
		case NUMBER_AUDIO_VOLUME:
			this->traits.set_min_value(0);
			this->traits.set_max_value(100);
			//this->traits.set_step(4);
			break;
		case NUMBER_AUDIO_TREBLE:
			this->traits.set_min_value(-5);
			this->traits.set_max_value(5);
			break;
		case NUMBER_AUDIO_BASS:
			this->traits.set_min_value(-5);
			this->traits.set_max_value(5);
			break;
		case NUMBER_AUDIO_BALANCE:
			this->traits.set_min_value(-5);
			this->traits.set_max_value(5);
			break;
		case NUMBER_AUDIO_SUBWOOFER:
			this->traits.set_min_value(0);
			this->traits.set_max_value(11);
			break;
		case NUMBER_SALT_POWER:
			this->traits.set_min_value(0);
			this->traits.set_max_value(10);
			break;
		case NUMBER_SALT_STATUS:
			this->traits.set_min_value(0);
			this->traits.set_max_value(7);
			break;
		case NUMBER_LIGHTS1_INTENSITY:
		case NUMBER_LIGHTS2_INTENSITY:
		case NUMBER_LIGHTS3_INTENSITY:
		case NUMBER_LIGHTS4_INTENSITY:
			this->traits.set_min_value(0);
			this->traits.set_max_value(maximum ? maximum : 5);
			break;
		}
	}

	void IQ2020Number::control(float value) {
		ESP_LOGD(TAG, "Number:%d write value: %d", number_id, value);
		if ((maximum != 0) && (value > maximum)) { value = maximum; }
		this->publish_state(value);
		if (g_iq2020_main != NULL) { g_iq2020_main->numberAction(number_id, value); }
	}

	void IQ2020Number::dump_config() {
		//ESP_LOGCONFIG(TAG, "Number:%d config", switch_id);
	}

} //namespace iq2020_number
} //namespace esphome