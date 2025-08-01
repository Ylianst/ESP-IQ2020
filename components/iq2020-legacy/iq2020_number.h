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
		void set_maximum(float max) { this->maximum = max; }
		//number::NumberTraits traits();
	protected:
		unsigned int number_id;
		float maximum = 0;
	};

} //namespace iq2020_number
} //namespace esphome