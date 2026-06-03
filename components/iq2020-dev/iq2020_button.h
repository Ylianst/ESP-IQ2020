#pragma once

#include "esphome/core/component.h"
#include "esphome/components/button/button.h"

namespace esphome {
namespace iq2020_button {

class IQ2020Button : public button::Button, public Component {
public:
  void setup() override;
  void dump_config() override;
  void set_button_id(unsigned int id) { this->button_id = id; }

protected:
  void press_action() override;
  unsigned int button_id;
};

}  // namespace iq2020_button
}  // namespace esphome