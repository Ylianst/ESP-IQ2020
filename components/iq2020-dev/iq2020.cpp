#include "iq2020.h"
#include "iq2020_fan.h"
#include "iq2020_switch.h"
#ifdef USE_SELECT
#include "iq2020_select.h"
#endif
#ifdef USE_TEXT
#include "iq2020_text.h"
#endif
#ifdef USE_NUMBER
#include "iq2020_number.h"
#endif
#include "iq2020_climate.h"

#include "esphome/core/helpers.h"
#include "esphome/core/log.h"
#include "esphome/core/util.h"
#include "esphome/core/version.h"

#include "esphome/components/network/util.h"
#include "esphome/components/socket/socket.h"

static const char *TAG = "iq2020";

// These globals are ugly, but I can't figure out the correct system yet.
IQ2020Component* g_iq2020_main = NULL;
esphome::iq2020_switch::IQ2020Switch* g_iq2020_switch[SWITCHCOUNT];
#ifdef USE_SELECT
esphome::iq2020_select::IQ2020Select* g_iq2020_select[SELECTCOUNT];
#endif
#ifdef USE_TEXT
esphome::iq2020_text::IQ2020Text* g_iq2020_text[TEXTCOUNT];
#endif
esphome::iq2020_fan::IQ2020Fan* g_iq2020_fan[FANCOUNT];
#ifdef USE_NUMBER
esphome::iq2020_number::IQ2020Number* g_iq2020_number[NUMBERCOUNT];
#endif
esphome::iq2020_climate::IQ2020Climate* g_iq2020_climate = NULL;

using namespace esphome;

float fahrenheit_to_celsius(float f) { return (f - 32) * 5 / 9; }
float celsius_to_fahrenheit(float c) { return c * 9 / 5 + 32; }
int readCounter(unsigned char* data, int offset) { return (data[offset]) + (data[offset + 1] << 8) + (data[offset + 2] << 16) + (data[offset + 3] << 24); }
int readCounterEx(unsigned char* data, int offset) { return (data[offset] << 24) + (data[offset + 1] << 16) + (data[offset + 2] << 8) + (data[offset + 3]); }

void IQ2020Component::setup() {
	for (int i = 0; i < SWITCHCOUNT; i++) { switch_state[i] = switch_pending[i] = NOT_SET; }
#ifdef USE_SELECT
	for (int i = 0; i < SELECTCOUNT; i++) { select_state[i] = select_pending[i] = NOT_SET; }
#endif
#ifdef USE_SELECT
	for (int i = 0; i < NUMBERCOUNT; i++) { number_state[i] = number_pending[i] = NOT_SET; }
#endif

	g_iq2020_main = this;
	if (this->flow_control_pin_ != nullptr) { this->flow_control_pin_->setup(); }
	if (this->trigger_poll_pin_ != nullptr) {
		this->trigger_poll_pin_->pin_mode(gpio::FLAG_INPUT);
		this->trigger_poll_pin_->setup();
	}
	//ESP_LOGD(TAG, "Setting up IQ2020...");

	// The make_unique() wrapper doesn't like arrays, so initialize the unique_ptr directly.
	this->buf_ = std::unique_ptr<uint8_t[]>{ new uint8_t[this->buf_size_] };

	if (this->port_ != 0) {
		struct sockaddr_storage bind_addr;
#if ESPHOME_VERSION_CODE >= VERSION_CODE(2023, 4, 0)
		socklen_t bind_addrlen = socket::set_sockaddr_any(reinterpret_cast<struct sockaddr *>(&bind_addr), sizeof(bind_addr), this->port_);
#else
		socklen_t bind_addrlen = socket::set_sockaddr_any(reinterpret_cast<struct sockaddr *>(&bind_addr), sizeof(bind_addr), htons(this->port_));
#endif
		this->socket_ = socket::socket_ip(SOCK_STREAM, PF_INET);
		this->socket_->setblocking(false);
		this->socket_->bind(reinterpret_cast<struct sockaddr *>(&bind_addr), bind_addrlen);
		this->socket_->listen(8);
	}

	this->publish_sensor();

#ifdef USE_NUMBER
	setNumberState(NUMBER_SALT_STATUS, ace_status);
#endif
	
	// Send initial polling commands
	next_poll = ::millis() + 5000;
	pollState();
}

void IQ2020Component::loop() {
	this->accept();
	this->read();
	this->flush();
	this->write();
	this->cleanup();

	unsigned long now = ::millis();
	// Check if there is any pending commands that need retry
	if ((next_retry_count > 0) && (next_retry < now)) {
		for (int switchid = 0; switchid < SWITCHCOUNT; switchid++) {
			if (switch_pending[switchid] != NOT_SET) {
				ESP_LOGE(TAG, "Retry switch %d set to %d", switchid, switch_pending[switchid]);
				switchAction(switchid, switch_pending[switchid]); // Try again
				next_retry_count--; // Setup for the next retry
				next_retry = ::millis() + SWITCH_RETRY_TIME;
				break; // Only retry one command
			}
		}
#ifdef USE_SELECT
		for (int selectid = 0; selectid <= SELECT_AUDIO_SOURCE; selectid++) {
			if (select_pending[selectid] != NOT_SET) {
				ESP_LOGE(TAG, "Retry select %d set to %d", selectid, select_pending[selectid]);
				selectAction(selectid, select_pending[selectid]); // Try again
				next_retry_count--; // Setup for the next retry
				next_retry = ::millis() + SWITCH_RETRY_TIME;
				break; // Only retry one command
			}
		}
#endif
#ifdef USE_NUMBER
		for (int numberid = 0; numberid < NUMBERCOUNT; numberid++) {
			if (number_pending[numberid] != NOT_SET) {
				ESP_LOGE(TAG, "Retry number %d set to %d", numberid, number_pending[numberid]);
				numberAction(numberid, number_pending[numberid]); // Try again
				next_retry_count--; // Setup for the next retry
				next_retry = ::millis() + SWITCH_RETRY_TIME;
				break; // Only retry one command
			}
		}
#endif
		// No commands that need retry, clear the retry state
		next_retry_count = 0;
	}

	// Check if it's time to poll for state. We poll each 10 seconds, but add 50 seconds if we get status.
	if ((connectionKit != 1) && ((connectionKit + 65000) < now)) { // If the connection kit is install, we don't poll
		ESP_LOGD(TAG, "Spa Connection Kit Removed");
#ifdef USE_BINARY_SENSOR
		if (this->connectionkit_sensor_) { this->connectionkit_sensor_->publish_state(false); }
#endif
		connectionKit = 1;
	}
	if (this->trigger_poll_pin_ != nullptr) {
		// Check if pin goes high, if it does then lets log something... for now...
		bool pinState = this->trigger_poll_pin_->digital_read();
		// Grace period if the event hasnt fired in the last second
		if (pinState && (now - last_pin_check_time >= 1000)) {
			ESP_LOGD(TAG, "Trigger Pin High, issuing poll.");
			last_pin_check_time = now; // Update the last check time
			pollState();
		}
	}

	if (next_poll < now) { next_poll = now + 5000; pollState(); }
}

void IQ2020Component::dump_config() {
	ESP_LOGCONFIG(TAG, "IQ2020:");
	ESP_LOGCONFIG(TAG, "  Address: %s:%u", esphome::network::get_use_address().c_str(), this->port_);
	if (this->flow_control_pin_ != nullptr) {
		ESP_LOGCONFIG(TAG, "  Flow Control Pin: ", this->flow_control_pin_);
	}
	if (this->trigger_poll_pin_ != nullptr) {
		ESP_LOGCONFIG(TAG, "  Trigger Poll Pin: ", this->trigger_poll_pin_);
	}
	if (this->ace_emulation_) { ESP_LOGCONFIG(TAG, "  Ace Emulation Enabled"); }
	if (this->freshwater_emulation_) { ESP_LOGCONFIG(TAG, "  Freshwater Emulation Enabled"); }
	if (this->audio_emulation_) { ESP_LOGCONFIG(TAG, "  Audio Emulation Enabled"); }
	ESP_LOGCONFIG(TAG, "  Polling Rate: %d", this->polling_rate_);
#ifdef USE_BINARY_SENSOR
	LOG_BINARY_SENSOR("  ", "Connected:", this->connected_sensor_);
#endif
#ifdef USE_SENSOR
	LOG_SENSOR("  ", "Connection count:", this->connection_count_sensor_);
#endif
}

void IQ2020Component::on_shutdown() {
	for (const Client &client : this->clients_)
		client.socket->shutdown(SHUT_RDWR);
}

void IQ2020Component::publish_sensor() {
#ifdef USE_BINARY_SENSOR
	if (this->connected_sensor_)
		this->connected_sensor_->publish_state(this->clients_.size() > 0);
#endif
#ifdef USE_SENSOR
	if (this->connection_count_sensor_)
		this->connection_count_sensor_->publish_state(this->clients_.size());
#endif
}

void IQ2020Component::accept() {
	struct sockaddr_storage client_addr;
	socklen_t client_addrlen = sizeof(client_addr);
	std::unique_ptr<socket::Socket> socket = this->socket_->accept(reinterpret_cast<struct sockaddr *>(&client_addr), &client_addrlen);
	if (!socket) return;

	socket->setblocking(false);
	std::string identifier = socket->getpeername();
	this->clients_.emplace_back(std::move(socket), identifier, this->buf_head_);
	ESP_LOGD(TAG, "New client connected from %s", identifier.c_str());
	this->publish_sensor();
}

void IQ2020Component::cleanup() {
	auto discriminator = [](const Client &client) { return !client.disconnected; };
	auto last_client = std::partition(this->clients_.begin(), this->clients_.end(), discriminator);
	if (last_client != this->clients_.end()) {
		this->clients_.erase(last_client, this->clients_.end());
		this->publish_sensor();
	}
}

void IQ2020Component::read() {
	size_t len = 0;
	int available;
	while ((available = this->stream_->available()) > 0) {
		size_t free = this->buf_size_ - (this->buf_head_ - this->buf_tail_);
		if (free == 0) {
			// Only overwrite if nothing has been added yet, otherwise give flush() a chance to empty the buffer first.
			if (len > 0)
				return;

			ESP_LOGE(TAG, "Incoming bytes available, but outgoing buffer is full: stream will be corrupted!");
			free = std::min<size_t>(available, this->buf_size_);
			this->buf_tail_ += free;
			for (Client &client : this->clients_) {
				if (client.position < this->buf_tail_) {
					ESP_LOGD(TAG, "Dropped %u pending bytes for client %s", this->buf_tail_ - client.position, client.identifier.c_str());
					client.position = this->buf_tail_;
				}
			}
		}

		// Fill all available contiguous space in the ring buffer.
		len = std::min<size_t>(available, std::min<size_t>(this->buf_ahead(this->buf_head_), free));
		this->stream_->read_array(&this->buf_[this->buf_index(this->buf_head_)], len);
		processRawIQ2020Data(&this->buf_[this->buf_index(this->buf_head_)], len); // Process incoming IQ2020 data
		this->buf_head_ += len;
	}
}

void IQ2020Component::flush() {
	ssize_t written;
	this->buf_tail_ = this->buf_head_;
	for (Client &client : this->clients_) {
		if (client.disconnected || client.position == this->buf_head_)
			continue;

		// Split the write into two parts: from the current position to the end of the ring buffer, and from the start
		// of the ring buffer until the head. The second part might be zero if no wraparound is necessary.
		struct iovec iov[2];
		iov[0].iov_base = &this->buf_[this->buf_index(client.position)];
		iov[0].iov_len = std::min(this->buf_head_ - client.position, this->buf_ahead(client.position));
		iov[1].iov_base = &this->buf_[0];
		iov[1].iov_len = this->buf_head_ - (client.position + iov[0].iov_len);
		if ((written = client.socket->writev(iov, 2)) > 0) {
			client.position += written;
		}
		else if (written == 0 || errno == ECONNRESET) {
			ESP_LOGD(TAG, "Client %s disconnected", client.identifier.c_str());
			client.disconnected = true;
			continue;  // don't consider this client when calculating the tail position
		}
		else if (errno == EWOULDBLOCK || errno == EAGAIN) {
			// Expected if the (TCP) transmit buffer is full, nothing to do.
		}
		else {
			ESP_LOGE(TAG, "Failed to write to client %s with error %d!", client.identifier.c_str(), errno);
		}

		this->buf_tail_ = std::min(this->buf_tail_, client.position);
	}
}

void IQ2020Component::write() {
	uint8_t buf[128];
	ssize_t read;
	for (Client &client : this->clients_) {
		if (client.disconnected)
			continue;

		while ((read = client.socket->read(&buf, sizeof(buf))) > 0) {
			if (this->flow_control_pin_ != nullptr) { this->flow_control_pin_->digital_write(true); }
			this->stream_->write_array(buf, read);
			this->stream_->flush();
			if (this->flow_control_pin_ != nullptr) { this->flow_control_pin_->digital_write(false); }
		}

		if (read == 0 || errno == ECONNRESET) {
			ESP_LOGD(TAG, "Client %s disconnected", client.identifier.c_str());
			client.disconnected = true;
		}
		else if (errno == EWOULDBLOCK || errno == EAGAIN) {
			// Expected if the (TCP) receive buffer is empty, nothing to do.
		}
		else {
			ESP_LOGD(TAG, "Failed to read from client %s with error %d!", client.identifier.c_str(), errno);
		}
	}
}

void IQ2020Component::processRawIQ2020Data(unsigned char *data, int len) {
	if ((len > IQ202BUFLEN) || ((processingBufferLen + len) > IQ202BUFLEN)) { ESP_LOGD(TAG, "Receive buffer is overflowing!"); processingBufferLen = 0; return; }
	if (processingBufferLen == 0) { memset(processingBuffer, 0, IQ202BUFLEN); }
	memcpy(processingBuffer + processingBufferLen, data, len);
	processingBufferLen += len;
	int processedData = 0;
	while ((processedData = processIQ2020Command()) > 0) {
		// Move the remaining data to the front of the buffer
		if (processedData < processingBufferLen) { memcpy(processingBuffer, processingBuffer + processedData, processingBufferLen - processedData); }
		processingBufferLen -= processedData;
	}
}

int IQ2020Component::nextPossiblePacket() {
	for (int i = 1; i < processingBufferLen; i++) { if (processingBuffer[i] == 0x1C) return i; }
	return processingBufferLen;
}

void IQ2020Component::setAudioButton(int button) {
#ifdef USE_SENSOR
	if (this->buttons_sensor_) this->buttons_sensor_->publish_state(button);
	if (this->buttons_sensor_) this->buttons_sensor_->publish_state(0);
#endif
}

int IQ2020Component::processIQ2020Command() {
	if (processingBufferLen < 6) return 0; // Need more data
	if ((processingBuffer[0] != 0x1C) || ((processingBuffer[4] != 0x40) && (processingBuffer[4] != 0x80))) { ESP_LOGD(TAG, "Receive buffer out of sync!"); return nextPossiblePacket(); } // Out of sync
	int cmdlen = 6 + processingBuffer[3];
	if (processingBufferLen < cmdlen) return 0; // Need more data
	unsigned char checksum = 0; // Compute the checksum
	for (int i = 1; i < (cmdlen - 1); i++) { checksum += processingBuffer[i]; }
	if (processingBuffer[cmdlen - 1] != (checksum ^ 0xFF)) { ESP_LOGD(TAG, "Invalid checksum. Got 0x%02x, expected 0x%02x.", processingBuffer[cmdlen - 1], (checksum ^ 0xFF)); return nextPossiblePacket(); }
	//ESP_LOGD(TAG, "IQ2020 data, dst:%02x src:%02x op:%02x datalen:%d", processingBuffer[1], processingBuffer[2], processingBuffer[4], processingBuffer[3]);

	if ((processingBuffer[1] == 0x01) && (processingBuffer[2] == 0x1F) && (processingBuffer[4] == 0x40)) {
		// This is a request command from the SPA connection kit, take note of this.
		// Presence of this device will cause us to no longer poll for state since this device will do it for us.
		if (connectionKit <= 1) {
			ESP_LOGD(TAG, "Spa Connection Kit Detected");
#ifdef USE_BINARY_SENSOR
			if (this->connectionkit_sensor_) { this->connectionkit_sensor_->publish_state(true); }
#endif
		}
		connectionKit = ::millis();

		//ESP_LOGD(TAG, "SCK CMD Data, len=%d, cmd=%02x%02x", cmdlen, processingBuffer[5], processingBuffer[6]);
	}

	if (((processingBuffer[1] == 0x33) || (processingBuffer[1] == 0x1D)) && (processingBuffer[2] == 0x01) && (processingBuffer[4] == 0x40) && (cmdlen >= 8)) {
		// This is a command from IQ2020 to the audio module
		//ESP_LOGD(TAG, "Audio REQ Data, len=%d, cmd=%02x%02x", cmdlen, processingBuffer[5], processingBuffer[6]);
		audio_module_address = processingBuffer[1]; // There are two audio modules at 0x33 or 0x1D.

		int responded = 0;
		if ((processingBuffer[5] == 0x19) && (cmdlen >= 9)) {
			if ((processingBuffer[6] == 0x01) && (cmdlen == 9)) { // Audio controls
				if (processingBuffer[7] > 0) { setAudioButton(processingBuffer[7]); }
				switch (processingBuffer[7]) {
				case 1: ESP_LOGD(TAG, "AUDIO - PLAY"); break;
				case 2: ESP_LOGD(TAG, "AUDIO - PAUSE"); break;
				case 3: ESP_LOGD(TAG, "AUDIO - NEXT"); break;
				case 4: ESP_LOGD(TAG, "AUDIO - BACK"); break;
				}
			}
			else if ((processingBuffer[6] == 0x03) && (cmdlen == 9)) { // Audio source
#ifdef USE_SELECT
				setSelectState(SELECT_AUDIO_SOURCE, processingBuffer[7]);
#endif
			}
			else if ((processingBuffer[6] == 0x00) && (processingBuffer[7] == 0x01) && (cmdlen == 14)) { // Audio settings
#ifdef USE_NUMBER
				ESP_LOGD(TAG, "AUDIO - Volume=%d, Treble=%d, Bass=%d, Balance=%d, Subwoofer=%d", processingBuffer[8], processingBuffer[9], processingBuffer[10], processingBuffer[11], processingBuffer[12]);
				if (audio_module_address == 0x33) { // 0x33
					setNumberState(NUMBER_AUDIO_VOLUME, (processingBuffer[8] - 15) << 2);
				} else { // 0x1D
					setNumberState(NUMBER_AUDIO_VOLUME, (((int)processingBuffer[8]) * 100) / 40);
				}
				setNumberState(NUMBER_AUDIO_TREBLE, (signed char)(processingBuffer[9]));
				setNumberState(NUMBER_AUDIO_BASS, (signed char)(processingBuffer[10]));
				setNumberState(NUMBER_AUDIO_BALANCE, (signed char)(processingBuffer[11]));
				setNumberState(NUMBER_AUDIO_SUBWOOFER, processingBuffer[12]);
#endif
			}
			else if (processingBuffer[6] == 0x06) { // Song title
				int text_len = 2;
				char text[22];
				text[0] = 0x19;
				text[1] = 0x06;
#ifdef USE_TEXT
				if (g_iq2020_text[TEXT_SONG_TITLE] != NULL) {
					text_len += g_iq2020_text[TEXT_SONG_TITLE]->text_value.size();
					memcpy(text + 2, g_iq2020_text[TEXT_SONG_TITLE]->text_value.c_str(), text_len - 2);
				}
#endif
				sendIQ2020Command(0x01, audio_module_address, 0x80, (unsigned char*)text, text_len);
				responded = 1;
			}
			else if (processingBuffer[6] == 0x07) { // Artist name
				int text_len = 2;
				char text[22];
				text[0] = 0x19;
				text[1] = 0x07;
#ifdef USE_TEXT
				if (g_iq2020_text[TEXT_ARTIST_NAME] != NULL) {
					text_len += g_iq2020_text[TEXT_ARTIST_NAME]->text_value.size();
					memcpy(text + 2, g_iq2020_text[TEXT_ARTIST_NAME]->text_value.c_str(), text_len - 2);
				}
#endif
				sendIQ2020Command(0x01, audio_module_address, 0x80, (unsigned char*)text, text_len);
				responded = 1;
			}
		}

		// Audio emulation
		if (audio_emulation_ && (responded == 0)) {
			//ESP_LOGD(TAG, "AUDIO - Enumlate");
			// These numbers below are a complete guess, I need real ones.
			// They likely tell the IQ2020 controller what source inputs are available and other things.
			unsigned char cmd[] = { 0x19, 0x01, 0x00, 0x19, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x04, 0x01, 0x00, 0x00 };
			//unsigned char cmd[] = { processingBuffer[5], processingBuffer[6] }; // Echo back the command with no data
			sendIQ2020Command(0x01, audio_module_address, 0x80, cmd, sizeof(cmd));
		}
	}

	if ((processingBuffer[2] == 0x37) && (processingBuffer[4] == 0x80) && (cmdlen == 36) && (processingBuffer[5] == 0x23) && (processingBuffer[6] == 0xD1)) {
		// This is a status command from Freshwater IQ
		if (got_iq_data < 5) { got_iq_data = 5; pollState(); }
		int iq_va = readCounterEx(processingBuffer, 7);  // Unknown value, always zero
		int iq_vb = readCounterEx(processingBuffer, 11); // Unknown value, always zero
		int iq_vc = readCounterEx(processingBuffer, 15); // Unknown value
		int iq_vd = readCounterEx(processingBuffer, 19); // Unknown value
		int iq_ve = readCounterEx(processingBuffer, 23); // Chlorine level
		int iq_vf = readCounterEx(processingBuffer, 27); // Ph Level
		int iq_vg = readCounterEx(processingBuffer, 31); // Cartridge life countdown.

#ifdef USE_SENSOR
		if (this->iq_va_sensor_) this->iq_va_sensor_->publish_state((float)iq_va);
		if (this->iq_vb_sensor_) this->iq_vb_sensor_->publish_state((float)iq_vb);
		if (this->iq_vc_sensor_) this->iq_vc_sensor_->publish_state((float)iq_vc);
		if (this->iq_vd_sensor_) this->iq_vd_sensor_->publish_state((float)iq_vd);
		if (this->iq_chlorine_sensor_) this->iq_chlorine_sensor_->publish_state(((float)iq_ve) / 10);
		if (this->iq_ph_sensor_) this->iq_ph_sensor_->publish_state(((float)iq_vf) / 10);
		if (this->iq_hoursleft_sensor_) this->iq_hoursleft_sensor_->publish_state((float)iq_vg);
#endif
	}

	if (((processingBuffer[1] == 0x24) || (processingBuffer[1] == 0x29)) && (processingBuffer[2] == 0x01) && (processingBuffer[4] == 0x40) && (cmdlen == 21) && (processingBuffer[5] == 0x1E) && (processingBuffer[6] == 0x01)) {
		// This is a command from IQ2020 to the Salt System
		//ESP_LOGD(TAG, "Salt REQ Data, len=%d, cmd=%02x%02x", cmdlen, processingBuffer[5], processingBuffer[6]);
		if (processingBuffer[7] <= 10) { setNumberState(NUMBER_SALT_POWER, processingBuffer[7]); }

#ifdef USE_BINARY_SENSOR
		if (this->salt_boost_sensor_) {
			if (processingBuffer[11] == 1) { this->salt_boost_sensor_->publish_state(true); }
			if (processingBuffer[11] == 2) { this->salt_boost_sensor_->publish_state(false); }
		}
#endif

		// ACE emulation
		if (ace_emulation_ && (processingBuffer[1] == 0x24)) {
			if (processingBuffer[7] <= 10) {
				salt_power = processingBuffer[7];
				setNumberState(NUMBER_SALT_POWER, salt_power);
			}
			ESP_LOGD(TAG, "ACE Emulation, power = %d", processingBuffer[7]);
			if (processingBuffer[16] == 1) { /*if ((ace_flags & 8) == 0) { ace_flags += 8; } else { ace_flags -= 8; }*/ setAudioButton(6); } // Salt Test
			if (processingBuffer[11] == 1) { if ((ace_flags & 4) == 0) { ace_flags += 4; } setAudioButton(7); } // Salt Boost on
			if (processingBuffer[11] == 2) { if ((ace_flags & 4) != 0) { ace_flags -= 4; } setAudioButton(8); } // Salt Boost off

			setSwitchState(SWITCH_SALT_BOOST, ((ace_flags & 4) != 0));

			// ace_flags : 0x01 = Functioning, 0x04 = Boosting, 0x08 = Testing
			// ace_status : Upper 4 bits is level 0x0 = min, 0x4 = green, 0x8 = max. Lower 4 bits status = 0x08 = ok, 0x0F = error.
			//unsigned char cmd[] = { 0x1E, 0x01, processingBuffer[7], 0x00, 0x38, 0x00, 0x00, ace_flags, 0x00, 0x00, 0x8C, 0x1B, 0x00, 0x00, 0x90 }; // 1/3 green
			//unsigned char cmd[] = { 0x1E, 0x01, processingBuffer[7], 0x00, 0x10, 0x00, 0x00, ace_flags, 0x00, 0x00, 0x8C, 0x1B, 0x00, 0x00, 0xA0 }; // Half Yellow
			//unsigned char cmd[] = { 0x1E, 0x01, processingBuffer[7], 0x00, 0x80, 0x00, 0x00, ace_flags, 0x00, 0x00, 0x8C, 0x1B, 0x00, 0x00, 0x90 }; // All way down
			//unsigned char cmd[] = { 0x1E, 0x01, processingBuffer[7], 0x00, 0x70, 0x00, 0x00, ace_flags, 0x00, 0x00, 0x8C, 0x1B, 0x00, 0x00, 0x90 }; // Mid Red
			//unsigned char cmd[] = { 0x1E, 0x01, processingBuffer[7], 0x00, 0x7F, 0x00, 0x00, ace_flags, 0x00, 0x00, 0x8C, 0x1B, 0x00, 0x00, 0x90 }; // Max - Inspect Cell Check Salt
			unsigned char cmd[] = { 0x1E, 0x01, processingBuffer[7], 0x00, (unsigned char)((ace_status << 4) + 8), 0x00, 0x00, ace_flags, 0x00, 0x00, 0x8C, 0x1B, 0x00, 0x00, 0x90 };
			sendIQ2020Command(0x01, 0x24, 0x80, cmd, sizeof(cmd));
		}
		else if (freshwater_emulation_ && (processingBuffer[1] == 0x29)) {
			if (processingBuffer[7] <= 10) {
				salt_power = processingBuffer[7];
				setNumberState(NUMBER_SALT_POWER, salt_power);
			}
			ESP_LOGD(TAG, "Freshwater Emulation, power = %d", processingBuffer[7]);
			unsigned char cmd[] = { 0x1E, 0x01, processingBuffer[7], 0x00, 0x30, 0x00, 0x00, 0x05, 0x00, 0x00, 0x8C, 0x1B, 0x00, 0x00, 0x50 };
			sendIQ2020Command(0x01, 0x29, 0x80, cmd, sizeof(cmd));
		}
	}

	if ((processingBuffer[1] == 0x01) && ((processingBuffer[2] == 0x24) || (processingBuffer[2] == 0x29)) && (processingBuffer[4] == 0x80) && (cmdlen == 21) && (processingBuffer[5] == 0x1E) && (processingBuffer[6] == 0x01)) {
		// This is a reply command from the Salt System to the IQ2020
		//ESP_LOGD(TAG, "Salt RSP Data, len=%d, cmd=%02x%02x, power=%d", cmdlen, processingBuffer[5], processingBuffer[6], processingBuffer[7]);
		if ((processingBuffer[7] <= 10) && (salt_power != processingBuffer[7])) {
			salt_power = processingBuffer[7];
			setNumberState(NUMBER_SALT_POWER, salt_power);
		}
		if (salt_content != processingBuffer[9]) {
			salt_content = processingBuffer[9] >> 4;
#ifdef USE_SENSOR
			if (this->salt_content_sensor_) this->salt_content_sensor_->publish_state((float)salt_content);
#endif
		}
		setSwitchState(SWITCH_SALT_BOOST, (processingBuffer[12] & 0x04) != 0);
	}

	if ((processingBuffer[1] == 0x1F) && (processingBuffer[2] == 0x01) && (processingBuffer[4] == 0x80)) {
		// This is response data going towards the SPA connection kit.
		//ESP_LOGD(TAG, "SCK RSP Data, len=%d, cmd=%02x%02x", cmdlen, processingBuffer[5], processingBuffer[6]);

		if ((cmdlen == 9) && (processingBuffer[5] == 0xE1) && (processingBuffer[6] == 0x02) && (processingBuffer[7] == 0x06)) {
			// Confirmation that the ACE/Freshwater salt system has changed state (not sure what state however)
			setNumberState(NUMBER_SALT_POWER, NOT_SET);
			setSwitchState(SWITCH_SALT_BOOST, NOT_SET);
		}

		if ((cmdlen == 9) && (processingBuffer[5] == 0x19) && (processingBuffer[6] == 0x00) && (processingBuffer[7] == 0x06)) {
			// Confirmation that the audio command was received
#ifdef USE_SELECT
			setSelectState(SELECT_AUDIO_SOURCE, NOT_SET);
#endif
		}

		if ((cmdlen == 19) && (processingBuffer[5] == 0x19) && (processingBuffer[6] == 0x01)) {
			if (got_audio_data < 5) { got_audio_data = 5; pollState(); }

			// Status of audio module
#ifdef USE_SELECT
			setSelectState(SELECT_AUDIO_SOURCE, processingBuffer[14]); // Audio Source
#endif
#ifdef USE_NUMBER
			if (audio_module_address == 0x33) { // 0x33
				setNumberState(NUMBER_AUDIO_VOLUME, (processingBuffer[8] - 15) << 2);
			} else { // 0x1D
				setNumberState(NUMBER_AUDIO_VOLUME, (((int)processingBuffer[8]) * 100) / 40);
			}
			setNumberState(NUMBER_AUDIO_TREBLE, (signed char)processingBuffer[9]);
			setNumberState(NUMBER_AUDIO_BASS, (signed char)processingBuffer[10]);
			setNumberState(NUMBER_AUDIO_BALANCE, (signed char)processingBuffer[11]);
			setNumberState(NUMBER_AUDIO_SUBWOOFER, processingBuffer[12]);
#endif
		}

		if ((cmdlen == 26) && (processingBuffer[5] == 0x1E) && (processingBuffer[6] == 0x03)) {
			// Status of the Freshwater Salt System
			if (salt_power != processingBuffer[7]) {
				salt_power = processingBuffer[7];
				setNumberState(NUMBER_SALT_POWER, salt_power);
			}
		}

		if ((cmdlen == 9) && (processingBuffer[5] == 0x17) && (processingBuffer[6] == 0x02) && (processingBuffer[7] == 0x06)) {
			// Confirmation that the pending light command was received
			setSwitchState(SWITCH_LIGHTS, NOT_SET);
		}

		if ((cmdlen == 9) && (processingBuffer[5] == 0x0B) && (processingBuffer[6] == 0x1D)) {
			// Confirmation that the pending spa lock command was received, includes new state
			setSwitchState(SWITCH_SPALOCK, (processingBuffer[7] & 1));
		}

		if ((cmdlen == 9) && (processingBuffer[5] == 0x0B) && (processingBuffer[6] == 0x1E)) {
			// Confirmation that the pending temp lock command was received, includes new state
			setSwitchState(SWITCH_TEMPLOCK, (processingBuffer[7] & 1));
		}

		if ((cmdlen == 9) && (processingBuffer[5] == 0x0B) && (processingBuffer[6] == 0x1F)) {
			// Confirmation that the pending cleaning cycle command was received, includes new state
			setSwitchState(SWITCH_CLEANCYCLE, (processingBuffer[7] & 1));
		}

		if ((cmdlen == 9) && (processingBuffer[5] == 0x0B) && (processingBuffer[6] == 0x1C)) {
			// Confirmation that the pending summer timer command was received, includes new state
			setSwitchState(SWITCH_SUMMERTIMER, (processingBuffer[7] & 1));
		}

		// Confirmation that a Jets command was received which includes new state
		if ((cmdlen == 9) && (processingBuffer[5] == 0x0B) && (processingBuffer[6] >= 0x02) && (processingBuffer[6] <= 0x05)) {
			setSwitchState(processingBuffer[6] + 3, processingBuffer[7]);
		}

		if ((cmdlen == 28) && (processingBuffer[5] == 0x17) && (processingBuffer[6] == 0x05)) {
			// This is an update on the status of the spa lights (enabled, intensity, color)
			setSwitchState(SWITCH_LIGHTS, (processingBuffer[24] & 1));
#ifdef USE_SENSOR
			if (this->lights_intensity_underwater_sensor_) this->lights_intensity_underwater_sensor_->publish_state((float)processingBuffer[8]);
			if (this->lights_intensity_bartop_sensor_) this->lights_intensity_bartop_sensor_->publish_state((float)processingBuffer[9]);
			if (this->lights_intensity_pillow_sensor_) this->lights_intensity_pillow_sensor_->publish_state((float)processingBuffer[10]);
			if (this->lights_intensity_exterior_sensor_) this->lights_intensity_exterior_sensor_->publish_state((float)processingBuffer[11]);
			if (this->lights_main_loop_speed_sensor_) this->lights_main_loop_speed_sensor_->publish_state((float)processingBuffer[16]);
			if (this->lights_color_underwater_sensor_) this->lights_color_underwater_sensor_->publish_state((float)processingBuffer[20]);
			if (this->lights_color_bartop_sensor_) this->lights_color_bartop_sensor_->publish_state((float)processingBuffer[21]);
			if (this->lights_color_pillow_sensor_) this->lights_color_pillow_sensor_->publish_state((float)processingBuffer[22]);
			if (this->lights_color_exterior_sensor_) this->lights_color_exterior_sensor_->publish_state((float)processingBuffer[23]);
#endif

#ifdef USE_NUMBER
			for (int i = 0; i < 4; i++) {
				int val = processingBuffer[8 + i];
				if ((number_pending[NUMBER_LIGHTS1_INTENSITY + i] != NOT_SET) && (val != number_pending[NUMBER_LIGHTS1_INTENSITY + i])) {
					number_state[NUMBER_LIGHTS1_INTENSITY + i] = val;
					numberAction(NUMBER_LIGHTS1_INTENSITY + i, number_pending[i]);
				} else {
					setNumberState(NUMBER_LIGHTS1_INTENSITY + i, val);
				}
			}
#endif

			// Fix the lights cycle speed if needed
			if (select_pending[SELECT_LIGHTS_CYCLE_SPEED] != NOT_SET) {
				int changes = 0;
				for (int i = 0; i < 4; i++) {
					if (g_iq2020_select[i + 1] == NULL) continue;
					int cycleColor = (g_iq2020_select[i + 1]->traits.get_options().size() == 8); // SELECT_LIGHTS1_COLOR
					if (!cycleColor || (processingBuffer[20 + i] == 8)) { // If cycling is enabled for these lights, turn off or fix the speed if needed
						// Check if we need to fix cycling speed
						int c = processingBuffer[16 + i]; // This is the speed of the cycling, c = 0 for pause, c = 3 for fast
						while (c != select_pending[SELECT_LIGHTS_CYCLE_SPEED]) {
							if ((c < select_pending[SELECT_LIGHTS_CYCLE_SPEED])) {
								//ESP_LOGD(TAG, "** MOVE CYCLE UP %d from %d to %d", i, c, select_pending[SELECT_LIGHTS_CYCLE_SPEED]);
								unsigned char cmd[] = { 0x17, 0x02, (unsigned char)i, 0x07 };
								sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd)); // Faster cycle
								next_poll = ::millis() + 100;
								changes++;
								c++;
							}
							else if ((c > select_pending[SELECT_LIGHTS_CYCLE_SPEED])) {
								//ESP_LOGD(TAG, "** MOVE CYCLE DOWN %d from %d to %d", i, c, select_pending[SELECT_LIGHTS_CYCLE_SPEED]);
								unsigned char cmd[] = { 0x17, 0x02, (unsigned char)i, 0x06 };
								sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd)); // Slower cycle
								next_poll = ::millis() + 100;
								changes++;
								c--;
							}
						}
					}
				}
				// If no changes where made, we are done.
				if (changes == 0) { select_pending[SELECT_LIGHTS_CYCLE_SPEED] = NOT_SET; }
			}

			if (select_pending[SELECT_LIGHTS_CYCLE_SPEED] == NOT_SET) {
				// Figure out the current cycling speed
				// Finish with i = 0 since that is the main control
				int speed = 0;
				for (int i = 3; i >= 0; i--) {
					if (processingBuffer[12 + i] != 0) { speed = processingBuffer[16 + i]; }
				}
				setSelectState(SELECT_LIGHTS_CYCLE_SPEED, speed);
			}

#ifdef USE_SELECT
			for (int i = SELECT_LIGHTS1_COLOR; i <= SELECT_LIGHTS4_COLOR; i++) {
				int val = processingBuffer[19 + i];
				if ((select_pending[i] != NOT_SET) && (val != select_pending[i])) {
					select_state[i] = val;
					selectAction(i, select_pending[i]);
				} else {
					setSelectState(i, val);
				}
			}
#endif
		}

		if ((cmdlen > 10) && (processingBuffer[5] == 0x01) && (processingBuffer[6] == 0x00)) {
			// Version string
			processingBuffer[cmdlen - 1] = 0;
			std::string vstr((char*)(processingBuffer + 7));
			versionstr = vstr;
#ifdef USE_TEXT_SENSOR
			if (this->version_sensor_) this->version_sensor_->publish_state(versionstr);
#endif
			ESP_LOGD(TAG, "Version: %d", versionstr.c_str());
			pollState();
		}

		if ((pending_temp != NOT_SET) && (cmdlen == 9) && (processingBuffer[5] == 0x01) && (processingBuffer[6] == 0x09) && (processingBuffer[7] == 0x06)) {
			// Confirmation that the pending temperature change command was received
			target_temp = pending_temp_cmd;
			if (pending_temp == pending_temp_cmd) { pending_temp = NOT_SET; pending_temp_retry = 0; }
			pending_temp = NOT_SET;
			pending_temp_cmd = NOT_SET;
			ESP_LOGD(TAG, "Temp Set Confirmation, Target Temp: %.1f", target_temp);
#ifdef USE_SENSOR
			if (temp_celsius) {
				if (this->target_c_temp_sensor_) this->target_c_temp_sensor_->publish_state(target_temp);
			} else {
				if (this->target_f_temp_sensor_) this->target_f_temp_sensor_->publish_state(target_temp);
			}
#endif
			if (g_iq2020_climate != NULL) {
				if (temp_celsius) { g_iq2020_climate->updateTempsC(target_temp, current_temp, temp_action); }
				else { g_iq2020_climate->updateTempsF(target_temp, current_temp, temp_action); }
			}
			next_poll = ::millis() + 5000; // Perform state polling in the next 5 seconds to update heater status.
		}

		if ((cmdlen == 140) && (processingBuffer[5] == 0x02) && (processingBuffer[6] == 0x56)) {
			// This is the main status data (jets, temperature)
#ifdef USE_SELECT
			if (!versionstr.empty() && (select_state[SELECT_AUDIO_SOURCE] != NOT_SET)) { next_poll = ::millis() + (this->polling_rate_ * 1000); } // Next poll
#else
			if (!versionstr.empty()) { next_poll = ::millis() + (this->polling_rate_ * 1000); } // Next poll
#endif

			// Read state flags
			unsigned char flags1 = processingBuffer[9];
			unsigned char flags2 = processingBuffer[10];
			setSwitchState(SWITCH_TEMPLOCK, flags1 & 0x01);
			setSwitchState(SWITCH_SPALOCK, (flags1 & 0x02) != 0);
			setSwitchState(SWITCH_CLEANCYCLE, (flags1 & 0x10) != 0);
			setSwitchState(SWITCH_SUMMERTIMER, (flags1 & 0x20) != 0);

#ifdef USE_BINARY_SENSOR
			if (this->salt_confirmed_sensor_) {
				this->salt_confirmed_sensor_->publish_state((flags1 & 0x40) != 0);
			}
#endif

			// Water heater status
			float heaterActive = (processingBuffer[110] + (processingBuffer[111] << 8));

			// Update JETS status. I don't currently know all of the JET status flags.
			int jetState = 0; // JETS1 OFF
			if (flags2 & 0x01) { jetState = 1; } // JETS1 MEDIUM
			if (flags1 & 0x04) { jetState = 2; } // JETS1 FULL
			setSwitchState(SWITCH_JETS1, jetState);
			
			jetState = 0; // JETS2 OFF
			if (flags2 & 0x02) { jetState = 1; } // JETS2 MEDIUM
			if (flags1 & 0x08) { jetState = 2; } // JETS2 FULL
			setSwitchState(SWITCH_JETS2, jetState);

			// Read temperatures
			float _target_temp = 0, _current_temp = 0;
			if (processingBuffer[93] == 'F') { // Fahrenheit
				temp_celsius = false;
				_target_temp = ((processingBuffer[91] - '0') * 10) + (processingBuffer[92] - '0') + ((processingBuffer[90] == '1') ? 100 : 0);
				_current_temp = ((processingBuffer[95] - '0') * 10) + (processingBuffer[96] - '0') + ((processingBuffer[94] == '1') ? 100 : 0);
				outlet_temp = ((processingBuffer[37] - '0') * 10) + (processingBuffer[38] - '0') + ((processingBuffer[36] == '1') ? 100 : 0);
			} else if (processingBuffer[92] == '.') { // Celcius
				temp_celsius = true;
				_target_temp = ((processingBuffer[90] - '0') * 10) + (processingBuffer[91] - '0') + ((processingBuffer[93] - '0') * 0.1);
				_current_temp = ((processingBuffer[94] - '0') * 10) + (processingBuffer[95] - '0') + ((processingBuffer[97] - '0') * 0.1);
				outlet_temp = ((processingBuffer[36] - '0') * 10) + (processingBuffer[37] - '0') + ((processingBuffer[39] - '0') * 0.1);
			}
			ESP_LOGD(TAG, "Reported Current Temp: %.1f, Target Temp: %.1f, Outlet Temp: %.1f", _current_temp, _target_temp, outlet_temp);

			// Publish the temperature values even if they don't change.
			if ((_target_temp != target_temp) || (_current_temp != current_temp) || (temp_action != (heaterActive != 0))) {
				target_temp = _target_temp;
				current_temp = _current_temp;
				temp_action = (heaterActive != 0);
				if (g_iq2020_climate != NULL) {
					if (temp_celsius) { g_iq2020_climate->updateTempsC(target_temp, current_temp, temp_action); }
					else { g_iq2020_climate->updateTempsF(target_temp, current_temp, temp_action); }
				}
				ESP_LOGD(TAG, "Changed Current Temp: %.1f, Target Temp: %.1f, Action: %d", current_temp, target_temp, temp_action);
			}

#ifdef USE_SENSOR
			// Temperature sensors
			if (temp_celsius) {
				if (this->target_c_temp_sensor_) this->target_c_temp_sensor_->publish_state(target_temp);
				if (this->current_c_temp_sensor_) this->current_c_temp_sensor_->publish_state(current_temp);
				if (this->outlet_c_temp_sensor_) this->outlet_c_temp_sensor_->publish_state(outlet_temp);
			} else {
				if (this->target_f_temp_sensor_) this->target_f_temp_sensor_->publish_state(target_temp);
				if (this->current_f_temp_sensor_) this->current_f_temp_sensor_->publish_state(current_temp);
				if (this->outlet_f_temp_sensor_) this->outlet_f_temp_sensor_->publish_state(outlet_temp);
			}

			// Runtime sensors
			if (this->heater_total_runtime_sensor_) this->heater_total_runtime_sensor_->publish_state(readCounter(processingBuffer, 40));
			if (this->jets1_total_runtime_sensor_) this->jets1_total_runtime_sensor_->publish_state(readCounter(processingBuffer, 44));
			if (this->lifetime_runtime_sensor_) this->lifetime_runtime_sensor_->publish_state(readCounter(processingBuffer, 48));
			if (this->power_on_counter_sensor_) this->power_on_counter_sensor_->publish_state(readCounter(processingBuffer, 52));
			if (this->jets2_total_runtime_sensor_) this->jets2_total_runtime_sensor_->publish_state(readCounter(processingBuffer, 60));
			if (this->jets3_total_runtime_sensor_) this->jets3_total_runtime_sensor_->publish_state(readCounter(processingBuffer, 64));
			if (this->lights_total_runtime_sensor_) this->lights_total_runtime_sensor_->publish_state(readCounter(processingBuffer, 72));
			if (this->circ_pump_total_runtime_sensor_) this->circ_pump_total_runtime_sensor_->publish_state(readCounter(processingBuffer, 78));
			if (this->jet1_low_total_runtime_sensor_) this->jet1_low_total_runtime_sensor_->publish_state(readCounter(processingBuffer, 82));
			if (this->jet2_low_total_runtime_sensor_) this->jet2_low_total_runtime_sensor_->publish_state(readCounter(processingBuffer, 86));

			// Voltage sensors
			int voltage_l1 = (processingBuffer[98] + (processingBuffer[99] << 8));
			int voltage_heater = (processingBuffer[100] + (processingBuffer[101] << 8));
			int voltage_l2 = (processingBuffer[102] + (processingBuffer[103] << 8));
			if (this->voltage_l1_sensor_) this->voltage_l1_sensor_->publish_state((float)voltage_l1);
			if (this->voltage_heater_sensor_) this->voltage_heater_sensor_->publish_state((float)voltage_heater);
			if (this->voltage_l2_sensor_) this->voltage_l2_sensor_->publish_state((float)voltage_l2);

			// Current sensors
			int current_l1 = (processingBuffer[106] + (processingBuffer[107] << 8));
			int current_heater = (processingBuffer[108] + (processingBuffer[109] << 8));
			int current_l2 = (processingBuffer[110] + (processingBuffer[111] << 8));
			if (this->current_l1_sensor_) this->current_l1_sensor_->publish_state((float)current_l1);
			if (this->current_heater_sensor_) this->current_heater_sensor_->publish_state((float)current_heater);
			if (this->current_l2_sensor_) this->current_l2_sensor_->publish_state((float)current_l2);

			// Power sensors
			if (this->power_l1_sensor_) {
				int power_l1 = (processingBuffer[114] + (processingBuffer[115] << 8));
				if ((power_l1 == 0) && (current_l1 != 0) && (voltage_l1 != 0)) { power_l1 = current_l1 * voltage_l1; } // If there is now power sensor, use volts * amps.
				this->power_l1_sensor_->publish_state((float)power_l1);
			}
			if (this->power_heater_sensor_) {
				int power_heater = (processingBuffer[116] + (processingBuffer[117] << 8));
				if ((power_heater == 0) && (current_heater != 0) && (voltage_heater != 0)) { power_heater = current_heater * voltage_heater; } // If there is now power sensor, use volts * amps.
				this->power_heater_sensor_->publish_state((float)power_heater);
			}
			if (this->power_l2_sensor_) {
				int power_l2 = (processingBuffer[118] + (processingBuffer[119] << 8));
				if ((power_l2 == 0) && (current_l2 != 0) && (voltage_l2 != 0)) { power_l2 = current_l2 * voltage_l2; } // If there is now power sensor, use volts * amps.
				this->power_l2_sensor_->publish_state((float)power_l2);
			}

			// Misc sensors
			if (this->pcb_f_temperature_sensor_) this->pcb_f_temperature_sensor_->publish_state((float)processingBuffer[128]);
			if (this->pcb_c_temperature_sensor_) this->pcb_c_temperature_sensor_->publish_state((float)esphome::fahrenheit_to_celsius((float)processingBuffer[128]));
			if (this->salt_content_sensor_ && (salt_content >= 0)) this->salt_content_sensor_->publish_state((float)salt_content);
			if (this->lights_intensity_sensor_) this->lights_intensity_sensor_->publish_state((float)(flags2 >> 4));

			int logo_lights = 0;
			if (processingBuffer[16] == 0x02) logo_lights = 1; // Power on
			if (processingBuffer[16] == 0x0A) logo_lights = 2; // Power & ready on
			if (processingBuffer[16] == 0x06) logo_lights = 3; // Ready flash
			if (processingBuffer[16] == 0x09) logo_lights = 4; // Power flash
			if (processingBuffer[16] == 0x05) logo_lights = 5; // Power and ready flash
			if ((processingBuffer[16] & 0x20) == 0x20) logo_lights = 6; // Power & ready alternate
			if ((processingBuffer[16] & 0x1C) == 0x10) logo_lights = 7; // Power & ready salt error
			if (this->logo_lights_sensor_) this->logo_lights_sensor_->publish_state(logo_lights);
#endif

			if (pending_temp != NOT_SET) {
				if (pending_temp == target_temp) {
					pending_temp = NOT_SET;
				} else {
					if (pending_temp_retry > 0) { // Try to adjust the temperature again
						unsigned char deltaSteps = ((temp_celsius ? 2 : 1) * (pending_temp - target_temp));
						ESP_LOGD(TAG, "Retry setTempAction: new=%f, target=%f, deltasteps=%d", pending_temp, target_temp, deltaSteps);
						if (deltaSteps != 0) {
							unsigned char changeTempCmd[] = { 0x01, 0x09, 0xFF, deltaSteps };
							sendIQ2020Command(0x01, 0x1F, 0x40, changeTempCmd, 4); // Adjust temp
							pending_temp_retry--;
						} else {
							pending_temp = NOT_SET;
						}
					} else {
						pending_temp = NOT_SET; // Give up
					}
				}
			}

			if (connectionKit <= 1) {
				// Poll lights state
				unsigned char lightPollCmd[] = { 0x17, 0x05 };
				sendIQ2020Command(0x01, 0x1F, 0x40, lightPollCmd, 2);
			}
		}
	}

	return cmdlen;
}

void IQ2020Component::sendIQ2020Command(unsigned char dst, unsigned char src, unsigned char op, unsigned char *data, int len) {
	outboundBuffer[0] = 0x1C;
	outboundBuffer[1] = dst;
	outboundBuffer[2] = src;
	outboundBuffer[3] = len;
	outboundBuffer[4] = op;
	memcpy(outboundBuffer + 5, data, len);
	unsigned char checksum = 0; // Compute the checksum
	for (int i = 1; i < (len + 5); i++) { checksum += outboundBuffer[i]; }
	outboundBuffer[len + 5] = (checksum ^ 0xFF);
	if (this->flow_control_pin_ != nullptr) { this->flow_control_pin_->digital_write(true); }
	this->stream_->write_array(outboundBuffer, len + 6);
	this->stream_->flush();
	if (this->flow_control_pin_ != nullptr) { this->flow_control_pin_->digital_write(false); }
	//ESP_LOGD(TAG, "IQ2020 transmit, dst:%02x src:%02x op:%02x datalen:%d", outboundBuffer[1], outboundBuffer[2], outboundBuffer[4], outboundBuffer[3]);
}

void IQ2020Component::switchAction(unsigned int switchid, int state) {
	ESP_LOGD(TAG, "switchAction, switchid = %d, status = %d", switchid, state);
	switch (switchid) {
	case SWITCH_LIGHTS: { // Spa Lights Switch
		switch_pending[SWITCH_LIGHTS] = state;
		unsigned char cmd[] = { 0x17, 0x02, 0x04, (state != 0) ? (unsigned char)0x11 : (unsigned char)0x10, 0x00 };
		sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd)); // Turn on/off lights
		next_poll = ::millis() + 100;
		break;
	}
	case SWITCH_SPALOCK: { // Spa Lock Switch
		switch_pending[SWITCH_SPALOCK] = state;
		unsigned char cmd[] = { 0x0B, 0x1D, (state != 0) ? (unsigned char)0x02 : (unsigned char)0x01 };
		sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd));
		break;
	}
	case SWITCH_TEMPLOCK: { // Temp Lock Switch
		switch_pending[SWITCH_TEMPLOCK] = state;
		unsigned char cmd[] = { 0x0B, 0x1E, (state != 0) ? (unsigned char)0x02 : (unsigned char)0x01 };
		sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd));
		break;
	}
	case SWITCH_CLEANCYCLE: { // Clean Cycle Switch
		switch_pending[SWITCH_CLEANCYCLE] = state;
		unsigned char cmd[] = { 0x0B, 0x1F, (state != 0) ? (unsigned char)0x02 : (unsigned char)0x01 };
		sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd));
		break;
	}
	case SWITCH_SUMMERTIMER: { // Summer Timer Switch
		switch_pending[SWITCH_SUMMERTIMER] = state;
		unsigned char cmd[] = { 0x0B, 0x1C, (state != 0) ? (unsigned char)0x02 : (unsigned char)0x01 };
		sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd));
		break;
	}
	case SWITCH_JETS1: // CMD 0x02 (Tested)
	case SWITCH_JETS2: // CMD 0x03 (Tested)
	case SWITCH_JETS3: // CMD 0x04 (I never testing this, but I assume this is the correct command)
	{
		if ((state < 0) || (state > 2)) break;
		switch_pending[switchid] = state; // 0 = OFF, 1 = MEDIUM, 2 = HIGH
		unsigned char cmd[] = { 0x0B, (unsigned char)(switchid - 3), (unsigned char)(state + 1) };
		sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd));
		break;
	}
	case SWITCH_SALT_BOOST: // ACE Boost
	{
		if (ace_emulation_)
		{   // ACE emulation
			if ((state == 0) && ((ace_flags & 4) != 0)) { ace_flags -= 4; }
			if ((state != 0) && ((ace_flags & 4) == 0)) { ace_flags += 4; }
			switch_state[switchid] = switch_pending[switchid] = state;
		}
		else
		{   // Control a real ACE module
			switch_pending[switchid] = state; // 0 = OFF, 1 = ON
			unsigned char cmd[] = { 0x1E, 0x02, 0x03, (unsigned char)(state ? 0x08 : 0x00), 0x00 };
			sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd));
		}
		break;
	}
	default: { return; }
	}
	// If the command does not get confirmed, setup to try again
	next_retry_count += SWITCH_RETRY_COUNT;
	next_retry = ::millis() + SWITCH_RETRY_TIME;
}

#ifdef USE_SELECT
void IQ2020Component::selectAction(unsigned int selectid, int state) {
	ESP_LOGD(TAG, "selectAction, selectid = %d, state = %d", selectid, state);
	switch (selectid) {
	case SELECT_AUDIO_SOURCE: // Audio Source
	{
		select_pending[SELECT_AUDIO_SOURCE] = state;
		unsigned char cmd[] = { 0x19, 0x00, 0x03, (unsigned char)state, 0x00 };
		sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd)); // Change audio source
		break;
	}
	case SELECT_LIGHTS_CYCLE_SPEED: // Lights cycle speed
	{
		select_pending[selectid] = state;
		next_poll = ::millis() + 100;
		return;
	}
	case SELECT_LIGHTS1_COLOR:
	case SELECT_LIGHTS2_COLOR:
	case SELECT_LIGHTS3_COLOR:
	case SELECT_LIGHTS4_COLOR:
	{
		// We have to move forward or back to get to the right color
		if (select_state[selectid] == NOT_SET) return;
		select_pending[selectid] = state;
		int current = select_state[selectid];
		int cmdsent = 0;
		while (current != state) {
			if ((state == 8) && (current < 8)) {
				//ESP_LOGD(TAG, "** MOVE TO CYCLE %d", selectid);
				unsigned char cmd[] = { 0x17, 0x02, (unsigned char)(selectid - 1), 0x08 };
				sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd)); // Enable cycle state
				cmdsent = 1;
				current = state;
				// If the cycle speed if off, change it to normal
				if (select_state[SELECT_LIGHTS_CYCLE_SPEED] == 0) { select_pending[SELECT_LIGHTS_CYCLE_SPEED] = 2; }
			} else if (current > state) {
				//ESP_LOGD(TAG, "** MOVE DOWN %d from %d to %d", selectid, current, select_pending[selectid]);
				unsigned char cmd[] = { 0x17, 0x02, (unsigned char)(selectid - 1), 0x04 };
				sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd)); // Previous color
				cmdsent = 1;
				current--;
			} else if (current < state) {
				//ESP_LOGD(TAG, "** MOVE UP %d from %d to %d", selectid, current, select_pending[selectid]);
				unsigned char cmd[] = { 0x17, 0x02, (unsigned char)(selectid - 1), 0x05 };
				sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd)); // Next color
				cmdsent = 1;
				current++;
			}
		}
		if (cmdsent == 1) { next_poll = ::millis() + 100; }
		return; // Don't do the normal retry logic.
	}
	}
	// If the command does not get confirmed, setup to try again
	next_retry_count += SWITCH_RETRY_COUNT;
	next_retry = ::millis() + SWITCH_RETRY_TIME;
}
#endif

#ifdef USE_NUMBER
void IQ2020Component::numberAction(unsigned int numberid, int value) {
	ESP_LOGD(TAG, "numberAction, numberid = %d, value = %d", numberid, value);
	number_pending[numberid] = value;
	switch (numberid) {
	case NUMBER_AUDIO_VOLUME:
	{
		number_pending[NUMBER_AUDIO_VOLUME] = value;
		if (audio_module_address == 0x33) { // 0x33
			unsigned char cmd[] = { 0x19, 0x00, 0x01, (unsigned char)((value >> 2) + 15) };
			sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd)); // Change volume
		}
		else { // 0x1D
			unsigned char cmd[] = { 0x19, 0x00, 0x01, (unsigned char)((value * 40) / 100) };
			sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd)); // Change volume
		}
		break;
	}
	case NUMBER_AUDIO_TREBLE:
	{
		number_pending[NUMBER_AUDIO_TREBLE] = value;
		unsigned char cmd[] = { 0x19, 0x00, 0x05, (unsigned char)value };
		sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd)); // Change treble
		break;
	}
	case NUMBER_AUDIO_BASS:
	{
		number_pending[NUMBER_AUDIO_BASS] = value;
		unsigned char cmd[] = { 0x19, 0x00, 0x06, (unsigned char)value };
		sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd)); // Change bass
		break;
	}
	case NUMBER_AUDIO_BALANCE:
	{
		number_pending[NUMBER_AUDIO_BALANCE] = value;
		unsigned char cmd[] = { 0x19, 0x00, 0x07, (unsigned char)value };
		sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd)); // Change balance
		break;
	}
	case NUMBER_AUDIO_SUBWOOFER:
	{
		number_pending[NUMBER_AUDIO_SUBWOOFER] = value;
		unsigned char cmd[] = { 0x19, 0x00, 0x08, (unsigned char)value };
		sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd)); // Change subwoofer
		break;
	}
	case NUMBER_SALT_POWER:
	{
		number_pending[NUMBER_SALT_POWER] = value;
		unsigned char cmd[] = { 0x1E, 0x02, 0x01, (unsigned char)value, 0x00 };
		sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd)); // Change salt power
		break;
	}
	case NUMBER_SALT_STATUS:
	{
		ace_status = value;
		break;
	}
	case NUMBER_LIGHTS1_INTENSITY:
	case NUMBER_LIGHTS2_INTENSITY:
	case NUMBER_LIGHTS3_INTENSITY:
	case NUMBER_LIGHTS4_INTENSITY:
	{
		// We have to move forward or back to get to the right intensity
		if (number_state[numberid] == NOT_SET) return;
		number_pending[numberid] = value;
		int current = number_state[numberid];
		int cmdsent = 0;
		while (current != value) {
			if (current > value) {
				//ESP_LOGD(TAG, "** MOVE BRIGHTNESS DOWN %d from %d to %d", selectid, current, number_pending[selectid]);
				unsigned char cmd[] = { 0x17, 0x02, (unsigned char)(numberid - NUMBER_LIGHTS1_INTENSITY), 0x02 };
				sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd)); // Less intensity
				cmdsent = 1;
				current--;
			}
			else if (current < value) {
				//ESP_LOGD(TAG, "** MOVE BRIGHTNESS UP %d from %d to %d", selectid, current, number_pending[selectid]);
				unsigned char cmd[] = { 0x17, 0x02, (unsigned char)(numberid - NUMBER_LIGHTS1_INTENSITY), 0x03 };
				sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd)); // More intensity
				cmdsent = 1;
				current++;
			}
		}
		if (cmdsent == 1) { next_poll = ::millis() + 100; }
		return; // Don't do the normal retry logic.
	}
	}

	// If the command does not get confirmed, setup to try again
	next_retry_count += SWITCH_RETRY_COUNT;
	next_retry = ::millis() + SWITCH_RETRY_TIME;
}
#endif

void IQ2020Component::setTempAction(float newtemp) {
	//ESP_LOGD(TAG, "setTempAction: new=%f", newtemp);

	if (temp_celsius) {
		pending_temp = (std::round(newtemp * 2) / 2); // Round to the nearest .5
	} else {
		pending_temp = std::round(esphome::celsius_to_fahrenheit(newtemp)); // Convert and round to the nearest integer
	}
	pending_temp_retry = 2;
	
	if (pending_temp_cmd == NOT_SET) { // If there are no outstanding temp commands in-flight, send one now.
		unsigned char deltaSteps = ((temp_celsius ? 2 : 1) * (pending_temp - target_temp));
		ESP_LOGD(TAG, "setTempAction: new=%f, target=%f, deltasteps=%d", pending_temp, target_temp, deltaSteps);
		if (deltaSteps == 0) { pending_temp = NOT_SET; return; }
		pending_temp_cmd = pending_temp;
		unsigned char changeTempCmd[] = { 0x01, 0x09, 0xFF, deltaSteps };
		sendIQ2020Command(0x01, 0x1F, 0x40, changeTempCmd, 4); // Adjust temp
	}
}

// Update the state of a switch or jet from the hot tub.
// If you set state to NOT_SET, that indicates that whatever state we wanted to go to, we got a confirmation.
void IQ2020Component::setSwitchState(unsigned int switchid, int state) {
	ESP_LOGD(TAG, "setSwitchState, switchid = %d, status = %d", switchid, state);
	if (state == NOT_SET) {
		if (switch_pending[switchid] == NOT_SET) return;
		state = switch_pending[switchid];
		switch_pending[switchid] = NOT_SET;
	}
	if (state != switch_state[switchid]) {
		switch_state[switchid] = state;
		switch_pending[switchid] = NOT_SET;
		if (g_iq2020_switch[switchid] != NULL) { g_iq2020_switch[switchid]->publish_state(state != 0); }
		if ((switchid >= SWITCH_JETS1) && (switchid <= SWITCH_JETS3) && (g_iq2020_fan[switchid - SWITCH_JETS1] != NULL)) { g_iq2020_fan[switchid - SWITCH_JETS1]->updateState(state); }
	}
}

// Update the state of a selector
// If you set state to NOT_SET, that indicates that whatever state we wanted to go to, we got a confirmation.
#ifdef USE_SELECT
void IQ2020Component::setSelectState(unsigned int selectid, int state) {
	ESP_LOGD(TAG, "setSelectState, selectid = %d, status = %d", selectid, state);
	if (state == NOT_SET) {
		if (select_pending[selectid] == NOT_SET) return;
		state = select_pending[selectid];
		select_pending[selectid] = NOT_SET;
	}
	if (state != select_state[selectid]) {
		select_state[selectid] = state;
		select_pending[selectid] = NOT_SET;
		if (g_iq2020_select[selectid] != NULL) { g_iq2020_select[selectid]->publish_state_ex(state); }
	}
}
#endif

// Update the state of a number
// If you set state to NOT_SET, that indicates that whatever state we wanted to go to, we got a confirmation.
#ifdef USE_NUMBER
void IQ2020Component::setNumberState(unsigned int numberid, int value) {
	ESP_LOGD(TAG, "setNumberState, numberid = %d, value = %d", numberid, value);
	if (value == NOT_SET) {
		if (number_pending[numberid] == NOT_SET) return;
		value = number_pending[numberid];
		number_pending[numberid] = NOT_SET;
	}
	if (value != number_state[numberid]) {
		number_state[numberid] = value;
		number_pending[numberid] = NOT_SET;
		if (g_iq2020_number[numberid] != NULL) { g_iq2020_number[numberid]->publish_state(value); }
	}
}
#endif

void IQ2020Component::pollState() {
	// If we don't have the version string, fetch it now.
	if (versionstr.empty()) {
		ESP_LOGD(TAG, "Poll Version");
		unsigned char cmd[] = { 0x01, 0x00 };
		sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd)); // Get version string
		return;
	}

#ifdef USE_SELECT
	// If we don't have the audio status, fetch it now.
	if ((got_audio_data < 3) && (select_state[SELECT_AUDIO_SOURCE] == NOT_SET)) {
		ESP_LOGD(TAG, "Poll Audio");
		got_audio_data++;
		unsigned char cmd[] = { 0x19, 0x01 };
		sendIQ2020Command(0x01, 0x1F, 0x40, cmd, sizeof(cmd)); // Get audio status
		return;
	}
#endif

	if ((got_iq_data < 3) && (this->iq_chlorine_sensor_)) {
		ESP_LOGD(TAG, "Poll Freshwater IQ");
		got_iq_data++;
		// Get Freshwater IQ status. No command can be sent after this one since this module will
		// respond right away even of more data is being sent.
		unsigned char cmd[] = { 0x23, 0xD1, 0x00 };
		sendIQ2020Command(0x37, 0x1F, 0x40, cmd, sizeof(cmd));
		return;
	}

	ESP_LOGD(TAG, "Poll");
	unsigned char generalPollCmd[] = { 0x02, 0x56 };
	sendIQ2020Command(0x01, 0x1F, 0x40, generalPollCmd, 2); // Poll general state
}

IQ2020Component::Client::Client(std::unique_ptr<esphome::socket::Socket> socket, std::string identifier, size_t position)
	: socket(std::move(socket)), identifier{ identifier }, position{ position } {}
