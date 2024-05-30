#include "esphome/core/log.h"
#include "iq2020_text_sensor.h"

namespace esphome {
namespace iq2020 {

static const char *const TAG = "iq2020.text_sensor";

void IQ2020TextSensor::setup() {
  this->parent_->register_listener(
      this->sensor_id_, this->request_mod_, this->request_once_,
      [this](const IQ2020Datapoint &datapoint) {
        ESP_LOGD(TAG, "MCU reported text sensor %s is: %s", this->sensor_id_.c_str(), datapoint.value_string.c_str());
        this->publish_state(datapoint.value_string);
      },
      false, this->src_adr_);
}

void IQ2020TextSensor::dump_config() {
  ESP_LOGCONFIG(TAG, "IQ2020 Text Sensor:");
  ESP_LOGCONFIG(TAG, "  Text Sensor has datapoint ID %s", this->sensor_id_.c_str());
}

}  // namespace econet
}  // namespace esphome
