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
#define SWITCH_RETRY_COUNT 3
#define SWITCH_RETRY_TIME 200
#define SWITCH_LIGHTS 0
#define SWITCH_SPALOCK 1
#define SWITCH_TEMPLOCK 2
#define SWITCH_CLEANCYCLE 3
#define SWITCH_SUMMERTIMER 4
#define SWITCH_JETS1 5
#define SWITCH_JETS2 6
#define SWITCH_JETS3 7
#define SWITCH_JETS4 8
#define SWITCH_SALT_POWER 9  // Freshwater Salt System Power Level

class IQ2020Component : public esphome::Component {
public:
	IQ2020Component() = default;
	explicit IQ2020Component(esphome::uart::UARTComponent *stream) : stream_{ stream } {}
	void set_uart_parent(esphome::uart::UARTComponent *parent) { this->stream_ = parent; }
	void set_buffer_size(size_t size) { this->buf_size_ = size; }
	void set_flow_control_pin(esphome::GPIOPin *flow_control_pin) { this->flow_control_pin_ = flow_control_pin; }
	void set_ace_emulation(bool ace_emulation) { this->ace_emulation_ = ace_emulation; }
	void set_audio_emulation(bool audio_emulation) { this->audio_emulation_ = audio_emulation; }
	void set_polling_rate(int polling_rate) { this->polling_rate_ = polling_rate; }

#ifdef USE_BINARY_SENSOR
	void set_connected_sensor(esphome::binary_sensor::BinarySensor *connected) { this->connected_sensor_ = connected; }
	void set_connectionkit_sensor(esphome::binary_sensor::BinarySensor *present) { this->connectionkit_sensor_ = present; }
	void set_salt_boost_sensor(esphome::binary_sensor::BinarySensor *present) { this->salt_boost_sensor_ = present; }
#endif
#ifdef USE_SENSOR
	void set_current_f_temp_sensor(esphome::sensor::Sensor *temp) { this->current_f_temp_sensor_ = temp; }
	void set_target_f_temp_sensor(esphome::sensor::Sensor *temp) { this->target_f_temp_sensor_ = temp; }
	void set_outlet_f_temp_sensor(esphome::sensor::Sensor *temp) { this->outlet_f_temp_sensor_ = temp; }
	void set_current_c_temp_sensor(esphome::sensor::Sensor *temp) { this->current_c_temp_sensor_ = temp; }
	void set_target_c_temp_sensor(esphome::sensor::Sensor *temp) { this->target_c_temp_sensor_ = temp; }
	void set_outlet_c_temp_sensor(esphome::sensor::Sensor *temp) { this->outlet_c_temp_sensor_ = temp; }
	void set_connection_count_sensor(esphome::sensor::Sensor *connection_count) { this->connection_count_sensor_ = connection_count; }
	void set_heater_total_runtime_sensor(esphome::sensor::Sensor *sensor) { this->heater_total_runtime_sensor_ = sensor; }
	void set_jets1_total_runtime_sensor(esphome::sensor::Sensor *sensor) { this->jets1_total_runtime_sensor_ = sensor; }
	void set_lifetime_runtime_sensor(esphome::sensor::Sensor *sensor) { this->lifetime_runtime_sensor_ = sensor; }
	void set_jets2_total_runtime_sensor(esphome::sensor::Sensor *sensor) { this->jets2_total_runtime_sensor_ = sensor; }
	void set_jets3_total_runtime_sensor(esphome::sensor::Sensor *sensor) { this->jets3_total_runtime_sensor_ = sensor; }
	void set_lights_total_runtime_sensor(esphome::sensor::Sensor *sensor) { this->lights_total_runtime_sensor_ = sensor; }
	void set_circ_pump_total_runtime_sensor(esphome::sensor::Sensor *sensor) { this->circ_pump_total_runtime_sensor_ = sensor; }
	void set_jet1_low_total_runtime_sensor(esphome::sensor::Sensor *sensor) { this->jet1_low_total_runtime_sensor_ = sensor; }
	void set_jet2_low_total_runtime_sensor(esphome::sensor::Sensor *sensor) { this->jet2_low_total_runtime_sensor_ = sensor; }
	void set_power_on_counter_sensor(esphome::sensor::Sensor *sensor) { this->power_on_counter_sensor_ = sensor; }
	void set_salt_power_sensor(esphome::sensor::Sensor *sensor) { this->salt_power_sensor_ = sensor; }
	void set_salt_content_sensor(esphome::sensor::Sensor *sensor) { this->salt_content_sensor_ = sensor; }
	void set_voltage_l1_sensor(esphome::sensor::Sensor *sensor) { this->voltage_l1_sensor_ = sensor; }
	void set_voltage_heater_sensor(esphome::sensor::Sensor *sensor) { this->voltage_heater_sensor_ = sensor; }
	void set_voltage_l2_sensor(esphome::sensor::Sensor *sensor) { this->voltage_l2_sensor_ = sensor; }
	void set_current_l1_sensor(esphome::sensor::Sensor *sensor) { this->current_l1_sensor_ = sensor; }
	void set_current_heater_sensor(esphome::sensor::Sensor *sensor) { this->current_heater_sensor_ = sensor; }
	void set_current_l2_sensor(esphome::sensor::Sensor *sensor) { this->current_l2_sensor_ = sensor; }
	void set_power_l1_sensor(esphome::sensor::Sensor *sensor) { this->power_l1_sensor_ = sensor; }
	void set_power_heater_sensor(esphome::sensor::Sensor *sensor) { this->power_heater_sensor_ = sensor; }
	void set_power_l2_sensor(esphome::sensor::Sensor *sensor) { this->power_l2_sensor_ = sensor; }
	void set_pcb_f_temperature_sensor(esphome::sensor::Sensor *sensor) { this->pcb_f_temperature_sensor_ = sensor; }
	void set_pcb_c_temperature_sensor(esphome::sensor::Sensor *sensor) { this->pcb_c_temperature_sensor_ = sensor; }
	void set_audio_buttons_sensor(esphome::sensor::Sensor *sensor) { this->audio_buttons_sensor_ = sensor; }
	void set_audio_volume_sensor(esphome::sensor::Sensor *sensor) { this->audio_volume_sensor_ = sensor; }
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
	void switchAction(unsigned int switchid, int state);
	void setTempAction(float newtemp);

protected:
	void publish_sensor();
	void setAudioButton(int button);

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
	esphome::GPIOPin *flow_control_pin_{ nullptr };
	bool ace_emulation_;
	bool audio_emulation_;
	int polling_rate_;

#ifdef USE_BINARY_SENSOR
	esphome::binary_sensor::BinarySensor *connected_sensor_;
	esphome::binary_sensor::BinarySensor *connectionkit_sensor_;
	esphome::binary_sensor::BinarySensor *salt_boost_sensor_;
#endif
#ifdef USE_SENSOR
	esphome::sensor::Sensor *current_f_temp_sensor_;
	esphome::sensor::Sensor *target_f_temp_sensor_;
	esphome::sensor::Sensor *outlet_f_temp_sensor_;
	esphome::sensor::Sensor *current_c_temp_sensor_;
	esphome::sensor::Sensor *target_c_temp_sensor_;
	esphome::sensor::Sensor *outlet_c_temp_sensor_;
	esphome::sensor::Sensor *connection_count_sensor_;
	esphome::sensor::Sensor *heater_total_runtime_sensor_;
	esphome::sensor::Sensor *jets1_total_runtime_sensor_;
	esphome::sensor::Sensor *lifetime_runtime_sensor_;
	esphome::sensor::Sensor *jets2_total_runtime_sensor_;
	esphome::sensor::Sensor *jets3_total_runtime_sensor_;
	esphome::sensor::Sensor *lights_total_runtime_sensor_;
	esphome::sensor::Sensor *circ_pump_total_runtime_sensor_;
	esphome::sensor::Sensor *jet1_low_total_runtime_sensor_;
	esphome::sensor::Sensor *jet2_low_total_runtime_sensor_;
	esphome::sensor::Sensor *power_on_counter_sensor_;
	esphome::sensor::Sensor *salt_power_sensor_;
	esphome::sensor::Sensor *salt_content_sensor_;
	esphome::sensor::Sensor *voltage_l1_sensor_;
	esphome::sensor::Sensor *voltage_heater_sensor_;
	esphome::sensor::Sensor *voltage_l2_sensor_;
	esphome::sensor::Sensor *current_l1_sensor_;
	esphome::sensor::Sensor *current_heater_sensor_;
	esphome::sensor::Sensor *current_l2_sensor_;
	esphome::sensor::Sensor *power_l1_sensor_;
	esphome::sensor::Sensor *power_heater_sensor_;
	esphome::sensor::Sensor *power_l2_sensor_;
	esphome::sensor::Sensor *pcb_f_temperature_sensor_;
	esphome::sensor::Sensor *pcb_c_temperature_sensor_;
	esphome::sensor::Sensor *audio_buttons_sensor_;
	esphome::sensor::Sensor *audio_volume_sensor_;
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
	unsigned long next_retry = 0;
	int next_retry_count = 0;
	int salt_power = -1; // This is polled too frequently to send to HA each time.
	int salt_content = -1; // This is polled too frequently to send to HA each time.

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