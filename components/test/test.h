#pragma once
#include "esphome/core/component.h"

#include <memory>
#include <string>
#include <vector>

class TestComponent : public esphome::Component {
public:
	TestComponent() = default;

	void setup() override;
	void loop() override;
	void dump_config() override;
	void on_shutdown() override;

	float get_setup_priority() const override { return esphome::setup_priority::AFTER_WIFI; }

protected:

};