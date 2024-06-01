#include "empty_fan.h"
#include "esphome/core/log.h"

namespace esphome {
namespace empty_fan {

static const char *TAG = "empty_fan.fan";

void EmptyFan::setup() {
  auto traits = fan::FanTraits();
  traits.set_direction(false);
  traits.set_oscillation(false);
  traits.set_speed(false);
  
  this->fan_->set_traits(traits);
  
  this->fan_->add_on_state_callback([this]() { this->next_update_ = true; });
}

void EmptyFan::loop() {
  if (!this->next_update_) {
    return; //no state change, nothing to do
  }
  this->next_update_ = false;
  
  //there was a state change, do something here.
}

void EmptyFan::dump_config() {
  ESP_LOGCONFIG(TAG, "Empty fan");
}

}  // namespace binary
}  // namespace empty_fan
