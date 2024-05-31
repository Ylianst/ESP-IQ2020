#include "iq2020.h"
#include "iq2020_switch.h"

#include "esphome/core/helpers.h"
#include "esphome/core/log.h"
#include "esphome/core/util.h"
#include "esphome/core/version.h"

#include "esphome/components/network/util.h"
#include "esphome/components/socket/socket.h"

static const char *TAG = "iq2020";

IQ2020Component* g_iq2020_main = NULL;
esphome::iq2020_switch::IQ2020Switch* g_iq2020_light_switch = NULL;

using namespace esphome;

void IQ2020Component::setup() {
	g_iq2020_main = this;
	ESP_LOGD(TAG, "Setting up IQ2020...");

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

	// Send initial polling commands
	unsigned char lightPollCmd[] = { 0x17, 0x05 };
	sendIQ2020Command(0x01, 0x1F, 0x40, lightOffCmd, 2); // Poll lights state
	unsigned char generalPollCmd[] = { 0x02, 0x56 };
	sendIQ2020Command(0x01, 0x1F, 0x40, lightOffCmd, 2); // Poll general state
}

void IQ2020Component::loop() {
	this->accept();
	this->read();
	this->flush();
	this->write();
	this->cleanup();
}

void IQ2020Component::dump_config() {
	ESP_LOGCONFIG(TAG, "IQ2020:");
	ESP_LOGCONFIG(TAG, "  Address: %s:%u", esphome::network::get_use_address().c_str(), this->port_);
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
#ifdef USE_TEXT_SENSOR
	if (this->sample_text_sensor_)
		this->sample_text_sensor_->publish_state("Sample123");
#endif
}

void IQ2020Component::accept() {
	struct sockaddr_storage client_addr;
	socklen_t client_addrlen = sizeof(client_addr);
	std::unique_ptr<socket::Socket> socket = this->socket_->accept(reinterpret_cast<struct sockaddr *>(&client_addr), &client_addrlen);
	if (!socket)
		return;

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
					ESP_LOGW(TAG, "Dropped %u pending bytes for client %s", this->buf_tail_ - client.position, client.identifier.c_str());
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

		while ((read = client.socket->read(&buf, sizeof(buf))) > 0)
			this->stream_->write_array(buf, read);

		if (read == 0 || errno == ECONNRESET) {
			ESP_LOGD(TAG, "Client %s disconnected", client.identifier.c_str());
			client.disconnected = true;
		}
		else if (errno == EWOULDBLOCK || errno == EAGAIN) {
			// Expected if the (TCP) receive buffer is empty, nothing to do.
		}
		else {
			ESP_LOGW(TAG, "Failed to read from client %s with error %d!", client.identifier.c_str(), errno);
		}
	}
}

void IQ2020Component::processRawIQ2020Data(unsigned char *data, int len) {
	if ((len > IQ202BUFLEN) || ((processingBufferLen + len) > IQ202BUFLEN)) { ESP_LOGW(TAG, "Receive buffer is overflowing!"); processingBufferLen = 0; return; }
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

int IQ2020Component::processIQ2020Command() {
	if (processingBufferLen < 6) return 0; // Need more data
	if ((processingBuffer[0] != 0x1C) || ((processingBuffer[4] != 0x40) && (processingBuffer[4] != 0x80))) { ESP_LOGW(TAG, "Receive buffer out of sync!"); return nextPossiblePacket(); } // Out of sync
	int cmdlen = 6 + processingBuffer[3];
	if (processingBufferLen < cmdlen) return 0; // Need more data
	unsigned char checksum = 0; // Compute the checksum
	for (int i = 1; i < (cmdlen - 1); i++) { checksum += processingBuffer[i]; }
	if (processingBuffer[cmdlen - 1] != (checksum ^ 0xFF)) { ESP_LOGW(TAG, "Invalid checksum. Got 0x%02x, expected 0x%02x.", processingBuffer[cmdlen - 1], (checksum ^ 0xFF)); return nextPossiblePacket(); }
	//ESP_LOGW(TAG, "IQ2020 data, dst:%02x src:%02x op:%02x datalen:%d", processingBuffer[1], processingBuffer[2], processingBuffer[4], processingBuffer[3]);

	if ((processingBuffer[1] == 0x01) && (processingBuffer[2] == 0x1F) && (processingBuffer[4] == 0x40)) {
		// This is a request command from the SPA connection kit, take note of this.
		// Presence of this device will cause us to no longer poll for state since this device will do it for us.
		if (connectionKit == 0) {
			//ESP_LOGW(TAG, "Spa Connection Kit Detected");
#ifdef USE_BINARY_SENSOR
			if (this->connectionkit_sensor_) { this->connectionkit_sensor_->publish_state(true); }
#endif
			connectionKit = 1;
		}

		ESP_LOGW(TAG, "SCK CMD Data, len=%d, cmd=%02x%02x", cmdlen, processingBuffer[5], processingBuffer[6]);

		if ((cmdlen == 11) && (processingBuffer[5] == 0x17) && (processingBuffer[6] == 0x02)) {
			// This is the SPA connection kit command to turn the lights on/off
			if ((processingBuffer[8] & 1) != lights) {
				lights = (processingBuffer[8] & 1);
				if (g_iq2020_light_switch != NULL) { g_iq2020_light_switch->publish_state(lights); }
			}
		}

	}

	if ((processingBuffer[1] == 0x1F) && (processingBuffer[2] == 0x01) && (processingBuffer[4] == 0x80)) {
		// This is response data going towards the SPA connection kit.

		ESP_LOGW(TAG, "SCK RSP Data, len=%d, cmd=%02x%02x", cmdlen, processingBuffer[5], processingBuffer[6]);

		if ((lights_pending != -1) && (cmdlen == 9) && (processingBuffer[5] == 0x17) && (processingBuffer[6] == 0x02) && (processingBuffer[7] == 0x06)) {
			// Confirmation that the pending light command we received
			lights = lights_pending;
			lights_pending = -1;
			if (g_iq2020_light_switch != NULL) { g_iq2020_light_switch->publish_state(lights); }
		}

		if ((cmdlen == 28) && (processingBuffer[5] == 0x17) && (processingBuffer[6] == 0x05)) {
			// This is an update on the status of the spa lights (enabled, intensity, color)
			lights_pending = -1;
			if (lights != (processingBuffer[24] & 1)) {
				lights = (processingBuffer[24] & 1);
				if (g_iq2020_light_switch != NULL) { g_iq2020_light_switch->publish_state(lights); }
			}
		}

		if ((cmdlen == 140) && (processingBuffer[5] == 0x02) && (processingBuffer[6] == 0x56)) {
			// This is the main status data (jets, temperature)

			// Read target temperature
			float temp = 0;
			if (processingBuffer[93] == 'F') { // Fahrenheit
				temp = ((processingBuffer[91] - '0') * 10) + (processingBuffer[92] - '0') + ((processingBuffer[90] == '1') ? 100 : 0);
#ifdef USE_SENSOR
				if ((target_temp != temp) && (this->target_f_temp_sensor_)) this->target_f_temp_sensor_->publish_state(temp);
#endif
			} else if (processingBuffer[92] == '.') { // Celcius
				temp = ((processingBuffer[90] - '0') * 10) + (processingBuffer[91] - '0') + ((processingBuffer[92] - '0') * 0.1);
#ifdef USE_SENSOR
				if ((target_temp != temp) && (this->target_c_temp_sensor_)) this->target_c_temp_sensor_->publish_state(temp);
#endif
			}
			//ESP_LOGW(TAG, "Target Temp: %.1f", temp);
			target_temp = temp;

			// Read current temperature
			if (processingBuffer[97] == 'F') { // Fahrenheit
				temp = ((processingBuffer[95] - '0') * 10) + (processingBuffer[96] - '0') + ((processingBuffer[94] == '1') ? 100 : 0);
#ifdef USE_SENSOR
				if ((current_temp != temp) && (this->current_f_temp_sensor_)) this->current_f_temp_sensor_->publish_state(temp);
#endif
			}
			else if (processingBuffer[96] == '.') { // Celcius
				temp = ((processingBuffer[94] - '0') * 10) + (processingBuffer[95] - '0') + ((processingBuffer[96] - '0') * 0.1);
#ifdef USE_SENSOR
				if ((current_temp != temp) && (this->current_c_temp_sensor_)) this->current_c_temp_sensor_->publish_state(temp);
#endif
			}
			//ESP_LOGW(TAG, "Current Temp: %.1f", temp);
			current_temp = temp;

			ESP_LOGW(TAG, "Current Temp: %.1f, Target Temp: %.1f", current_temp, target_temp);
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
	this->stream_->write_array(outboundBuffer, len + 6);
	ESP_LOGW(TAG, "IQ2020 transmit, dst:%02x src:%02x op:%02x datalen:%d", outboundBuffer[1], outboundBuffer[2], outboundBuffer[4], outboundBuffer[3]);
}

void IQ2020Component::LightSwitchAction(int state) {
	lights_pending = state;
	if (state != 0) {
		unsigned char lightOnCmd[] = { 0x17, 0x02, 0x04, 0x11, 0x00 };
		sendIQ2020Command(0x01, 0x1F, 0x40, lightOnCmd, 5); // Turn on lights
	} else {
		unsigned char lightOffCmd[] = { 0x17, 0x02, 0x04, 0x10, 0x00 };
		sendIQ2020Command(0x01, 0x1F, 0x40, lightOffCmd, 5); // Turn off lights
	}
}

IQ2020Component::Client::Client(std::unique_ptr<esphome::socket::Socket> socket, std::string identifier, size_t position)
	: socket(std::move(socket)), identifier{ identifier }, position{ position } {}
