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
		void set_iq2020_parent(IQ2020Component *parent) { this->parent_ = parent; }

	protected:
		IQ2020Component *parent_;
	};

} //namespace iq2020_switch
} //namespace esphome