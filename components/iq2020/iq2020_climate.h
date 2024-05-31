#pragma once

#include "esphome/core/component.h"
#include "esphome/components/climate/climate.h"

namespace esphome {
namespace iq2020_climate {

	class IQ2020Switch : public climate_::Climate, public Component {
	public:
		void setup() override;
		void write_state(bool state) override;
		void dump_config() override;
	};

} //namespace iq2020_climate
} //namespace esphome