#pragma once

#include <map>
#include "esphome/core/component.h"
#include "esphome/components/climate/climate.h"

namespace esphome {
namespace iq2020_climate {

	class IQ2020Climate : public climate::Climate, public Component {
	public:
		void setup() override;
		void control(const climate::ClimateCall &call) override;
		void updateTempsF(float target, float current, bool heating);
		void updateTempsC(float target, float current, bool heating);
		void set_celsius(int celsius) { this->celsius = celsius; }
		climate::ClimateTraits traits() override;
		void dump_config() override;
	private:
		int celsius = 0;
		//template<class K, class V> std::set<V> map_values_as_set(std::map<K, V> map);
	};

} //namespace iq2020_climate
} //namespace esphome