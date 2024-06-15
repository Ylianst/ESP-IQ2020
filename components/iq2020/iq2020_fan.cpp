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
		//ESP_LOGD(TAG, "Fan:%d Setup", fan_id);
		state = 0;
		speed = 1;
	}

	void IQ2020Fan::dump_config() {
		ESP_LOGCONFIG(TAG, "IQ2020 fan %d config. State: %d, Speed: %d", fan_id, state, speed);
	}

	esphome::fan::FanTraits IQ2020Fan::get_traits() {
		// bool oscillation, bool speed, bool direction, int speed_count
		auto traits = fan::FanTraits(false, true, false, fan_speeds);
		return traits;
	};

	void IQ2020Fan::control(const esphome::fan::FanCall &call) {
		ESP_LOGCONFIG(TAG, "IQ2020 fan %d control. State: %d, Speed: %d", fan_id, call.get_state(), *(call.get_speed()));
		state = call.get_state();
		speed = *(call.get_speed());
		int xstate = 0;
		if (call.get_state() == true) {
			if (call.get_speed().has_value()) { xstate = *(call.get_speed()); } else { xstate = 1; }
			if ((fan_speeds == 1) && (xstate == 1)) { xstate = 2; } // If this is a single speed jet, turn it on should send max speed command. 
		}
		g_iq2020_main->switchAction(fan_id + SWITCH_JETS1, xstate);
	};

	void IQ2020Fan::updateState(int s) {
		ESP_LOGCONFIG(TAG, "IQ2020 fan %d update. State: %d", fan_id, s);
		switch (s) {
			case 0: { state = false; break; } // OFF
			case 1: { state = true; speed = 1; break; } // MEDIUM
			case 2: { state = true; speed = (fan_speeds == 2) ? 2 : 1; break; } // FULL
		}
		publish_state();
	}

}  // namespace binary
}  // namespace iq2020_fan
