#include "esphome/core/log.h"
#include "iq2020_button.h"
#include "iq2020.h"

extern IQ2020Component *g_iq2020_main;
extern esphome::iq2020_button::IQ2020Button *g_iq2020_button[BUTTONCOUNT];

namespace esphome {
namespace iq2020_button {

static const char *TAG = "iq2020.button";

void IQ2020Button::setup() {
  if (button_id < BUTTONCOUNT) {
    g_iq2020_button[button_id] = this;
  }
}

void IQ2020Button::press_action() {
  ESP_LOGD(TAG, "Button:%d press", button_id);
  if (g_iq2020_main != nullptr) {
    g_iq2020_main->buttonAction(button_id);
  }
}

void IQ2020Button::dump_config() {
}

}  // namespace iq2020_button
}  // namespace esphome