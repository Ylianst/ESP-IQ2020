#include "iq2020.h"

#include "esphome/core/helpers.h"
#include "esphome/core/log.h"
#include "esphome/core/util.h"
#include "esphome/core/version.h"

#include "esphome/components/network/util.h"
#include "esphome/components/socket/socket.h"

static const char *TAG = "iq2020";

using namespace esphome;

void IQ2020Component::setup() {
    ESP_LOGCONFIG(TAG, "Setting up IQ2020...");

    // The make_unique() wrapper doesn't like arrays, so initialize the unique_ptr directly.
    this->buf_ = std::unique_ptr<uint8_t[]>{new uint8_t[this->buf_size_]};

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
        } else if (written == 0 || errno == ECONNRESET) {
            ESP_LOGD(TAG, "Client %s disconnected", client.identifier.c_str());
            client.disconnected = true;
            continue;  // don't consider this client when calculating the tail position
        } else if (errno == EWOULDBLOCK || errno == EAGAIN) {
            // Expected if the (TCP) transmit buffer is full, nothing to do.
        } else {
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
        } else if (errno == EWOULDBLOCK || errno == EAGAIN) {
            // Expected if the (TCP) receive buffer is empty, nothing to do.
        } else {
            ESP_LOGW(TAG, "Failed to read from client %s with error %d!", client.identifier.c_str(), errno);
        }
    }
}

void IQ2020Component::processRawIQ2020Data(unsigned char *data, int len) {
	if ((len > IQ202BUFLEN) || ((processingBufferLen + len) > IQ202BUFLEN)) { ESP_LOGW(TAG, "Receive buffer is overflowing!"); processingBufferLen = 0; return; }
	if (processingBufferLen == 0) { memset(processingBuffer, 0, IQ202BUFLEN);  }
	memcpy(processingBuffer + processingBufferLen, data, len);
	processingBufferLen += len;
	int processedData = 0;
	while ((processedData = processIQ2020Command()) > 0) {
		// Move the remaining data to the front of the buffer
		if (processedData < processingBufferLen) { memcpy(processingBuffer, processingBuffer + processedData, processingBufferLen - processedData); }
		processingBufferLen -= processedData;
	}
}

int IQ2020Component::processIQ2020Command() {
	if (processingBufferLen < 6) return 0; // Need more data
	if (processingBuffer[0] != 0x1C) { ESP_LOGW(TAG, "Receive buffer out of sync!"); return processingBufferLen; } // Out of sync
	int cmdlen = 6 + processingBuffer[3];
	if (processingBufferLen < cmdlen) return 0; // Need more data
	unsigned char checksum = 0; // Compute the checksum
	for (int i = 1; i < (cmdlen - 1); i++) { checksum += processingBuffer[i]; }
	if (processingBuffer[cmdlen - 1] != (checksum ^ 0xFF)) { ESP_LOGW(TAG, "Invalid checksum. Got 0x%02x, expected 0x%02x.", processingBuffer[cmdlen - 1], (checksum ^ 0xFF)); return processingBufferLen; }
	ESP_LOGW(TAG, "IQ2020 data, dst:%02x src:%02x op:%02x datalen:%d", processingBuffer[1], processingBuffer[2], processingBuffer[4], processingBuffer[3]);
	if (processingBuffer[1] = 0x33) { unsigned char senddata[1]; sendIQ2020Command(0x29, 0x99, 1, 0x40, senddata, 1); }
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
	for (int i = 1; i < (cmdlen - 1); i++) { checksum += processingBuffer[i]; }
	outboundBuffer[len + 5] = (checksum ^ 0xFF);
	this->stream_->write_array(outboundBuffer, len + 6);
	ESP_LOGW(TAG, "IQ2020 transmit, dst:%02x src:%02x op:%02x datalen:%d", outboundBuffer[1], outboundBuffer[2], outboundBuffer[4], outboundBuffer[3]);
}

IQ2020Component::Client::Client(std::unique_ptr<esphome::socket::Socket> socket, std::string identifier, size_t position)
    : socket(std::move(socket)), identifier{identifier}, position{position} {}
