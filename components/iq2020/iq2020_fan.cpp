#include "empty_fan.h"
#include "esphome/core/log.h"

namespace esphome {
namespace iq2020_fan {

static const char *TAG = "iq2020_fan.fan";

void IQ2020Fan::setup() {
  auto traits = fan::FanTraits();
  traits.set_direction(false);
  traits.set_oscillation(false);
  traits.set_speed(false);
  
  this->fan_->set_traits(traits);
  
  this->fan_->add_on_state_callback([this]() { this->next_update_ = true; });
}

void IQ2020Fan::loop() {
  if (!this->next_update_) {
    return; //no state change, nothing to do
  }
  this->next_update_ = false;
  
  //there was a state change, do something here.
}

void IQ2020Fan::dump_config() {
  ESP_LOGCONFIG(TAG, "IQ2020 fan");
}

}  // namespace binary
}  // namespace iq2020_fan
