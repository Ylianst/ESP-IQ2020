#pragma once

#include "esphome/core/component.h"
#include "esphome/components/text/text.h"

namespace esphome {
namespace iq2020_text {

	class IQ2020Text : public text::Text, public Component {
	public:
		void setup() override;
		//void write_state(bool state) override;
		void dump_config() override;
		void set_text_id(unsigned int id) { this->text_id = id; }

	protected:
		unsigned int text_id;
	};

} //namespace iq2020_text
} //namespace esphome