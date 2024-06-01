#pragma once

#include "esphome/core/component.h"
#include "esphome/components/fan/fan.h"
#include "esphome/components/fan/fan_traits.h"

namespace esphome {
namespace iq2020_fan {

	class IQ2020Fan : public fan::Fan, public Component {
	public:
//		void set_fan(fan::Fan *fan) { fan_ = fan; }
		//void set_output(output::BinaryOutput *output) { output_ = output; }
		void setup() override;
//		void loop() override;
		void dump_config() override;
		void set_fan_id(unsigned int id) { this->fan_id = id; }

		esphome::fan::FanTraits get_traits() override;
		void control(const esphome::fan::FanCall &call) override;

	protected:
//		fan::Fan *fan_;
		//  output::BinaryOutput *output_;
		bool next_update_{ true };
		unsigned int fan_id;
	};

}  // namespace iq2020_fan
}  // namespace esphome