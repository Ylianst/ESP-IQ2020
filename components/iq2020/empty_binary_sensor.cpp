#include "esphome/core/log.h"
#include "empty_binary_sensor.h"

namespace esphome {
namespace empty_binary_sensor {

static const char *TAG = "empty_binary_sensor.binary_sensor";

void EmptyBinarySensor::setup() {
    
}
  
void EmptyBinarySensor::update() {

}

void EmptyBinarySensor::dump_config() {
    ESP_LOGCONFIG(TAG, "Custom binary sensor");
}

} //namespace empty_binary_sensor
} //namespace esphome