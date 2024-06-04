#pragma once
#include "esphome/core/component.h"
#include "esphome/components/socket/socket.h"
#include "esphome/components/uart/uart.h"

#ifdef USE_BINARY_SENSOR
#include "esphome/components/binary_sensor/binary_sensor.h"
#endif
#ifdef USE_SENSOR
#include "esphome/components/sensor/sensor.h"
#endif
#ifdef USE_TEXT_SENSOR
#include "esphome/components/text_sensor/text_sensor.h"
#endif

#include <memory>
#include <string>
#include <vector>

#define IQ202BUFLEN 512
#define FANCOUNT 4
#define FAN_JETS1 0
#define FAN_JETS2 1
#define FAN_JETS3 2
#define FAN_JETS4 3
#define SWITCHCOUNT 10
#define SWITCH_LIGHTS 0
#define SWITCH_SPALOCK 1
#define SWITCH_TEMPLOCK 2
#define SWITCH_CLEANCYCLE 3
#define SWITCH_SUMMERTIMER 4
#define SWITCH_JETS1 5
#define SWITCH_JETS2 6
#define SWITCH_JETS3 7
#define SWITCH_JETS4 8

class IQ2020Component : public esphome::Component {
public:
	IQ2020Component() = default;
	explicit IQ2020Component(esphome::uart::UARTComponent *stream) : stream_{ stream } {}
	void set_uart_parent(esphome::uart::UARTComponent *parent) { this->stream_ = parent; }
	void set_buffer_size(size_t size) { this->buf_size_ = size; }

#ifdef USE_BINARY_SENSOR
	void set_connected_sensor(esphome::binary_sensor::BinarySensor *connected) { this->connected_sensor_ = connected; }
	void set_connectionkit_sensor(esphome::binary_sensor::BinarySensor *present) { this->connectionkit_sensor_ = present; }
#endif
#ifdef USE_SENSOR
	void set_current_f_temp_sensor(esphome::sensor::Sensor *temp) { this->current_f_temp_sensor_ = temp; }
	void set_target_f_temp_sensor(esphome::sensor::Sensor *temp) { this->target_f_temp_sensor_ = temp; }
	void set_outlet_f_temp_sensor(esphome::sensor::Sensor *temp) { this->outlet_f_temp_sensor_ = temp; }
	void set_current_c_temp_sensor(esphome::sensor::Sensor *temp) { this->current_c_temp_sensor_ = temp; }
	void set_target_c_temp_sensor(esphome::sensor::Sensor *temp) { this->target_c_temp_sensor_ = temp; }
	void set_outlet_c_temp_sensor(esphome::sensor::Sensor *temp) { this->outlet_c_temp_sensor_ = temp; }
	void set_connection_count_sensor(esphome::sensor::Sensor *connection_count) { this->connection_count_sensor_ = connection_count; }
	void set_heater_wattage_sensor(esphome::sensor::Sensor *wattage) { this->wattage_sensor_ = wattage; }
	void set_heater_relay_sensor(esphome::sensor::Sensor *relay) { this->relay_sensor_ = relay; }
	void set_heater_total_runtime_sensor(esphome::sensor::Sensor *sensor) { this->heater_total_runtime_sensor_ = sensor; }
	void set_jets1_total_runtime_sensor(esphome::sensor::Sensor *sensor) { this->jets1_total_runtime_sensor_ = sensor; }
	void set_lifetime_runtime_sensor(esphome::sensor::Sensor *sensor) { this->lifetime_runtime_sensor_ = sensor; }
	void set_jets2_total_runtime_sensor(esphome::sensor::Sensor *sensor) { this->jets2_total_runtime_sensor_ = sensor; }
	void set_jets3_total_runtime_sensor(esphome::sensor::Sensor *sensor) { this->jets3_total_runtime_sensor_ = sensor; }
	void set_lights_total_runtime_sensor(esphome::sensor::Sensor *sensor) { this->lights_total_runtime_sensor_ = sensor; }
#endif
#ifdef USE_TEXT_SENSOR
	void set_version_sensor(esphome::text_sensor::TextSensor *text) { this->version_sensor_ = text; }
#endif

	void setup() override;
	void loop() override;
	void dump_config() override;
	void on_shutdown() override;

	float get_setup_priority() const override { return esphome::setup_priority::AFTER_WIFI; }

	void set_port(uint16_t port) { this->port_ = port; }
	void SwitchAction(unsigned int switchid, int state);
	void SetTempAction(float newtemp);

protected:
	void publish_sensor();

	void accept();
	void cleanup();
	void read();
	void flush();
	void write();

	size_t buf_index(size_t pos) { return pos & (this->buf_size_ - 1); }
	/// Return the number of consecutive elements that are ahead of @p pos in memory.
	size_t buf_ahead(size_t pos) { return (pos | (this->buf_size_ - 1)) - pos + 1; }

	struct Client {
		Client(std::unique_ptr<esphome::socket::Socket> socket, std::string identifier, size_t position);

		std::unique_ptr<esphome::socket::Socket> socket{ nullptr };
		std::string identifier{};
		bool disconnected{ false };
		size_t position{ 0 };
	};

	esphome::uart::UARTComponent *stream_{ nullptr };
	uint16_t port_;
	size_t buf_size_;

#ifdef USE_BINARY_SENSOR
	esphome::binary_sensor::BinarySensor *connected_sensor_;
	esphome::binary_sensor::BinarySensor *connectionkit_sensor_;
#endif
#ifdef USE_SENSOR
	esphome::sensor::Sensor *current_f_temp_sensor_;
	esphome::sensor::Sensor *target_f_temp_sensor_;
	esphome::sensor::Sensor *outlet_f_temp_sensor_;
	esphome::sensor::Sensor *current_c_temp_sensor_;
	esphome::sensor::Sensor *target_c_temp_sensor_;
	esphome::sensor::Sensor *outlet_c_temp_sensor_;
	esphome::sensor::Sensor *connection_count_sensor_;
	esphome::sensor::Sensor *wattage_sensor_;
	esphome::sensor::Sensor *relay_sensor_;
	esphome::sensor::Sensor *heater_total_runtime_sensor_;
	esphome::sensor::Sensor *jets1_total_runtime_sensor_;
	esphome::sensor::Sensor *lifetime_runtime_sensor_;
	esphome::sensor::Sensor *jets2_total_runtime_sensor_;
	esphome::sensor::Sensor *jets3_total_runtime_sensor_;
	esphome::sensor::Sensor *lights_total_runtime_sensor_;
#endif
#ifdef USE_TEXT_SENSOR
	esphome::text_sensor::TextSensor *version_sensor_;
#endif

	std::unique_ptr<uint8_t[]> buf_{};
	size_t buf_head_{ 0 };
	size_t buf_tail_{ 0 };

	std::unique_ptr<esphome::socket::Socket> socket_{};
	std::vector<Client> clients_{};
	std::string versionstr;
	int switch_state[SWITCHCOUNT];   // Current state of all switches
	int switch_pending[SWITCHCOUNT]; // Desired state of all switches
	unsigned long connectionKit = 0; // The time the spa connection kit was last seen
	bool temp_celsius = false;
	int temp_action = -1;
	float target_temp = -1;
	float current_temp = -1;
	float outlet_temp = -1;
	float pending_temp = -1;
	float pending_temp_cmd = -1;
	int pending_temp_retry = 0;
	unsigned long next_poll = 0;

	// IQ2020 processing
	int nextPossiblePacket();
	unsigned char processingBuffer[IQ202BUFLEN];
	unsigned char outboundBuffer[64];
	int processingBufferLen = 0;
	void processRawIQ2020Data(unsigned char *data, int len);
	int processIQ2020Command();
	void sendIQ2020Command(unsigned char dst, unsigned char src, unsigned char op, unsigned char *data, int len);
	void setSwitchState(unsigned int switchid, int state);
	void pollState();
};