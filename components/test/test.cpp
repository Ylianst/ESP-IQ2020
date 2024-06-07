#include "test.h"
#include "test_fan.h"

#include "esphome/core/helpers.h"
#include "esphome/core/log.h"
#include "esphome/core/util.h"
#include "esphome/core/version.h"

static const char *TAG = "test";

using namespace esphome;

void TestComponent::setup() {

}

void TestComponent::loop() {

}

void TestComponent::dump_config() {
	ESP_LOGCONFIG(TAG, "Test Dump Config");
}

void TestComponent::on_shutdown() {

}
