#pragma once

#include "esphome/core/component.h"
#include "esphome/components/select/select.h"

namespace esphome {
namespace iq2020_select {

	class IQ2020Select : public select::Select, public Component {
	public:
		void setup() override;
		void write_state(bool state) override;
		void dump_config() override;
		void set_select_id(unsigned int id) { this->select_id = id; }

	protected:
		unsigned int select_id;
	};

} //namespace iq2020_select
} //namespace esphome