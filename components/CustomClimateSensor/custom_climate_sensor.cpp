#include "custom_climate_sensor.h"

namespace esphome {
	namespace custom_climate {

		void CustomClimateSensor::setup() {
			// Call the setup method of the parent class
			PollingComponent::setup();
			//dht_.setup(23, dht::DHT_TYPE_DHT22);
		}

		void CustomClimateSensor::update() {
			// Call the update method of the parent class
			PollingComponent::update();
			float temperature = 55; // dht_.read_temperature();
			float humidity = 66; // dht_.read_humidity();
			if (!isnan(temperature)) {
				publish_state(temperature);
			}
			if (!isnan(humidity)) {
				publish_state(humidity);
			}
		}

	}  // namespace custom_climate
}  // namespace esphome