#pragma once

#include "esphome/core/component.h"
#include "esphome/components/output/binary_output.h"
#include "esphome/components/fan/fan_state.h"

DEPENDENCIES = ["binary_output"]

namespace esphome {
namespace iq2020_fan {

class IQ2020Fan : public Component {
 public:
  void set_fan(fan::FanState *fan) { fan_ = fan; }
  void set_output(output::BinaryOutput *output) { output_ = output; }
  void setup() override;
  void loop() override;
  void dump_config() override;
  
 protected:
  fan::FanState *fan_;
  output::BinaryOutput *output_;
  bool next_update_{true};
};

}  // namespace iq2020_fan
}  // namespace esphome