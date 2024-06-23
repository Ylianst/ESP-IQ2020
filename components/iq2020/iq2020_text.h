#pragma once

#include "esphome/core/component.h"
#include "esphome/components/text/text.h"

namespace esphome {
namespace iq2020_text {

	class IQ2020Text : public text::Text, public Component {
	public:
		void setup() override;
		void control(const std::string &value) override;
		void dump_config() override;
		void set_text_id(unsigned int id) { this->text_id = id; }
		void set_value(const std::string &value) {
			this->text_value = value;
			this->publish_state(value);
		}
		std::string text_value;

	protected:
		unsigned int text_id;
	};

} //namespace iq2020_text
} //namespace esphome
