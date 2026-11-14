#include "esphome/core/log.h"
#include "iq2020_fan.h"
#include "../iq2020.h"

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
		ESP_LOGD(TAG, "IQ2020 fan %d control. State: %d, Speed: %d", fan_id, call.get_state().value_or(false) ? 1 : 0, call.get_speed().value_or(0));
		if ((fan_id >= FANCOUNT) || (fan_speeds < 1) || (fan_speeds > 2) || (g_iq2020_main == nullptr)) return;
		const bool requested_state = call.get_state().value_or(state);
		const int requested_speed = call.get_speed().value_or(speed > 0 ? speed : 1);
		if ((requested_speed < 1) || (requested_speed > fan_speeds)) return;
		state = requested_state;
		speed = requested_speed;
		const int xstate = state ? (fan_speeds == 1 ? 2 : speed) : 0;
		g_iq2020_main->switchAction(fan_id + SWITCH_JETS1, xstate);
	};

	void IQ2020Fan::updateState(int s) {
		ESP_LOGCONFIG(TAG, "IQ2020 fan %d update. State: %d", fan_id, s);
		switch (s) {
			case 0: { state = false; speed = 0; break; } // OFF
			case 1: { state = true; speed = 1; break; } // MEDIUM
			case 2: { state = true; speed = (fan_speeds == 2) ? 2 : 1; break; } // FULL
		}
		publish_state();
	}

}  // namespace binary
}  // namespace iq2020_fan
