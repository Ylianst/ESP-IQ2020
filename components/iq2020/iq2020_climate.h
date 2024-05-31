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
		void updateTempsF(float target, float current);
		void updateTempsC(float target, float current);
		climate::ClimateTraits traits() override;
		void dump_config() override;
	private:
		template<class K, class V> std::set<V> map_values_as_set(std::map<K, V> map);
	};

} //namespace iq2020_climate
} //namespace esphome