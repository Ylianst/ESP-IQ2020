#pragma once

#include "esphome/core/component.h"
#include "esphome/components/binary_sensor/binary_sensor.h"

namespace esphome {
namespace empty_binary_sensor {

class EmptyBinarySensor : public binary_sensor::BinarySensor, public PollingComponent {
 public:
  void setup() override;
  void update() override;
  void dump_config() override;
};

} //namespace empty_binary_sensor
} //namespace esphome