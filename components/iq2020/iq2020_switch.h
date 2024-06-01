#pragma once

#include "esphome/core/component.h"
#include "esphome/components/switch/switch.h"

namespace esphome {
namespace iq2020_switch {

	class IQ2020Switch : public switch_::Switch, public Component {
	public:
		void setup() override;
		void write_state(bool state) override;
		void dump_config() override;
		void set_switch_id(const std::string &switch_id) { this->switch_id_ = switch_id; }

	protected:
		std::string switch_id_;
	};

} //namespace iq2020_switch
} //namespace esphome