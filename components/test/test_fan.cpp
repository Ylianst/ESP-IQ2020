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

	void TestFan::loop() {
		//ESP_LOGCONFIG(TAG, "Fan:%d loop", fan_id);
	}

	esphome::fan::FanTraits TestFan::get_traits() {
		// bool oscillation, bool speed, bool direction, int speed_count
		auto traits = fan::FanTraits(true, true, true, fan_speeds);
		return traits;
	};

	void TestFan::control(const esphome::fan::FanCall &call) {
		ESP_LOGCONFIG(TAG, "Test control. State: %d, Speed: %d", call.get_state(), *(call.get_speed()));
		state = call.get_state();
		speed = *(call.get_speed());
		publish_state();
	};

}  // namespace binary
}  // namespace test_fan
