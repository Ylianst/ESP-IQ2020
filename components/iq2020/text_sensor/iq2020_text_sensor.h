#pragma once

#include "esphome/core/component.h"
#include "esphome/components/text_sensor/text_sensor.h"
#include "../iq2020.h"

namespace esphome {
namespace iq2020 {

class IQ2020TextSensor : public text_sensor::TextSensor, public Component, public IQ2020Client {
 public:
  void setup() override;
  void dump_config() override;
  void set_sensor_id(const std::string &sensor_id) { this->sensor_id_ = sensor_id; }

 protected:
  std::string sensor_id_{""};
};

}  // namespace iq2020
}  // namespace esphome
