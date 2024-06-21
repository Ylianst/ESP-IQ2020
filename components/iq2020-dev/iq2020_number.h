#pragma once

#include "esphome/core/component.h"
#include "esphome/components/number/number.h"

namespace esphome {
namespace iq2020_number {

	class IQ2020Number : public number::Number, public Component {
	public:
		void setup() override;
		void control(float value) override;
		void dump_config() override;
		void set_number_id(unsigned int id) { this->number_id = id; }
		number::NumberTraits traits() override;
	protected:
		unsigned int number_id;
	};

} //namespace iq2020_number
} //namespace esphome