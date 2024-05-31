#pragma once

#include "esphome.h"

namespace esphome {
	namespace custom_climate {

		class CustomClimateSensor : public PollingComponent, public Sensor {
		public:
			CustomClimateSensor() : PollingComponent(15000) {} // Poll every 15 seconds
			void setup() override;
			void update() override;

		protected:
			//dht::DHTComponent dht_;
		};

	}  // namespace custom_climate
}  // namespace esphome