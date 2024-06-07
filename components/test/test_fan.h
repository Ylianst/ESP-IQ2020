#pragma once

#include "esphome/core/component.h"
#include "esphome/components/fan/fan.h"
#include "esphome/components/fan/fan_traits.h"

namespace esphome {
namespace test_fan {

	class TestFan : public fan::Fan, public Component {
	public:
		void setup() override;
		void loop() override;
		void dump_config() override;
		void set_fan_speeds(unsigned int speeds) { this->fan_speeds = speeds; }

		esphome::fan::FanTraits get_traits() override;
		void control(const esphome::fan::FanCall &call) override;

	protected:
		unsigned int fan_speeds;
	};

}  // namespace test_fan
}  // namespace esphome