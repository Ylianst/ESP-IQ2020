#include "esphome/core/log.h"
#include "iq2020_fan.h"
#include "iq2020.h"

extern IQ2020Component* g_iq2020_main;
extern esphome::iq2020_fan::IQ2020Fan* g_iq2020_fan[FANCOUNT];

namespace esphome {
namespace iq2020_fan {

	static const char *TAG = "iq2020_fan.fan";

	void IQ2020Fan::setup() {
		if (fan_id < FANCOUNT) { g_iq2020_fan[fan_id] = this; }
		ESP_LOGD(TAG, "Fan:%d Setup", fan_id);
	}

	void IQ2020Fan::dump_config() {
		ESP_LOGCONFIG(TAG, "IQ2020 fan");
	}

	esphome::fan::FanTraits IQ2020Fan::get_traits() {
		// bool oscillation, bool speed, bool direction, int speed_count
		auto traits = fan::FanTraits(false, true, false, fan_speeds);
		return traits;
	};

	void IQ2020Fan::control(const esphome::fan::FanCall &call) {
		ESP_LOGCONFIG(TAG, "IQ2020 fan %d control, state: %d, speed: %d", fan_id, call.get_state(), *(call.get_speed()));
		int state = 0;
		if (call.get_state() == true) {
			if (call.get_speed().has_value()) { state = *(call.get_speed()); } else { state = 1; }
			if ((fan_speeds == 1) && (state == 1)) { state = 2; } // If this is a single speed jet, turn it on should send max speed command. 
		}
		g_iq2020_main->SwitchAction(fan_id + SWITCH_JETS1, state);
	};

	void IQ2020Fan::updateState(int s) {
		ESP_LOGCONFIG(TAG, "IQ2020 fan %d updateState, state: %d", fan_id, s);
		switch (s) {
			case 0: { state = false; break; } // OFF
			case 1: { state = true; speed = 1; break; } // MEDIUM
			case 2: { state = true; speed = (fan_speeds == 2) ? 2 : 1; break; } // FULL
		}
		publish_state();
	}

}  // namespace binary
}  // namespace iq2020_fan
