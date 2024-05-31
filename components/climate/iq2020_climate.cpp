#include "esphome/core/defines.h"
#include "esphome/core/component.h"
#include "esphome/core/hal.h"
#include "esphome/core/log.h"
#include "esphome/components/climate/climate_traits.h"
#include "iq2020_climate.h"

using namespace esphome;

namespace esphome {
namespace iq2020 {

namespace {

float fahrenheit_to_celsius(float f) { return (f - 32) * 5 / 9; }
float celsius_to_fahrenheit(float c) { return c * 9 / 5 + 32; }

}  // namespace

static const char *const TAG = "iq2020.climate";

void IQ202Climate::dump_config() {
  LOG_CLIMATE("", "IQ2020 Climate", this);
  dump_traits_(TAG);
}

climate::ClimateTraits IQ2020Climate::traits() {
  auto traits = climate::ClimateTraits();
  traits.set_supports_current_temperature(!current_temperature_id_.empty());
  return traits;
}

void IQ2020Climate::setup() {

}

void IQ2020Climate::control(const climate::ClimateCall &call) {

}

}  // namespace econet
}  // namespace esphome
