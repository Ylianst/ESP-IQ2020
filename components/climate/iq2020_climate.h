#pragma once

#include "esphome/core/component.h"
#include "esphome/components/climate/climate.h"

namespace esphome {
namespace iq2020 {

class IQ2020Climate : public climate::Climate, public Component {
 public:
  void setup() override;
  void dump_config() override;
  void set_current_temperature_id(const std::string &current_temperature_id) {
    current_temperature_id_ = current_temperature_id;
  }
  void set_target_temperature_id(const std::string &target_temperature_id) {
    target_temperature_id_ = target_temperature_id;
  }
  void set_target_temperature_low_id(const std::string &target_temperature_low_id) {
    target_temperature_low_id_ = target_temperature_low_id;
  }
  void set_target_temperature_high_id(const std::string &target_temperature_high_id) {
    target_temperature_high_id_ = target_temperature_high_id;
  }

 protected:
  std::string current_temperature_id_{""};
  std::string target_temperature_id_{""};
  std::string target_temperature_low_id_{""};
  std::string target_temperature_high_id_{""};
  void control(const climate::ClimateCall &call) override;
  climate::ClimateTraits traits() override;
};

}  // namespace iq2020
}  // namespace esphome
