#include "esphome/core/log.h"
#include "test_fan.h"
#include "test.h"

namespace esphome {
namespace test_fan {

	static const char *TAG = "test_fan.fan";

	void TestFan::setup() {
		//ESP_LOGD(TAG, "Fan:%d Setup", fan_id);
	}

	void TestFan::dump_config() {
		//ESP_LOGCONFIG(TAG, "Fan:%d config", fan_id);
	}

	esphome::fan::FanTraits TestFan::get_traits() {
		// bool oscillation, bool speed, bool direction, int speed_count
		auto traits = fan::FanTraits(false, true, false, fan_speeds);
		return traits;
	};

	void TestFan::control(const esphome::fan::FanCall &call) {
		ESP_LOGCONFIG(TAG, "Test fan %d control. State: %d, Speed: %d", fan_id, call.get_state(), *(call.get_speed()));
	};

	void TestFan::updateState(int s) {
		ESP_LOGCONFIG(TAG, "Test fan %d update. State: %d", fan_id, s);
	}

}  // namespace binary
}  // namespace test_fan
