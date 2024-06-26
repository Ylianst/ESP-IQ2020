using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DataViewer
{
    public partial class MainForm : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private byte[] readBuffer = new byte[2000];
        private int readBufferLen = 0;
        private StreamWriter outputFile = null;
        private int scanValue = 0;
        private int connectionState = 0;
        private FileInfo logFileInfo;
        private int Filter = 0;
        private int FilterCmd1 = 0;
        private int FilterCmd2 = 0;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            addressTextBox.Text = (string)Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Open Source\IQ2020DataViewer", "address", "1.2.3.4:1234");
            if (addressTextBox.Text == "") { addressTextBox.Text = "1.2.3.4:1234"; }
            UpdateInfo();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainDataPacketTextBox.Clear();
            mainRawDataTextBox.Clear();
            packetsListView.Items.Clear();
            dataListView.Items.Clear();
        }

        private void addPacketToStore(string packet)
        {
            if (InvokeRequired) { try { Invoke(new AppendTextHandler(addPacketToStore), packet); } catch (Exception) { } return; }
            foreach (ListViewItem i in packetsListView.Items) { if (i.Text == packet) { return; } }
            ListViewItem l = new ListViewItem(packet);
            packetsListView.Items.Add(l);
        }

        public delegate void SetStateDecodedHandler(string value);
        private void setStateDecoded(string value)
        {
            if (InvokeRequired) { try { Invoke(new SetStateDecodedHandler(setStateDecoded), value); } catch (Exception) { } return; }
            stateDecodeTextBox.Text = value;
        }

        public delegate void AddDecodedDataHandler(string name, string value);
        private void addDecodedData(string name, string value)
        {
            if (InvokeRequired) { try { Invoke(new AddDecodedDataHandler(addDecodedData), name, value); } catch (Exception) { } return; }
            foreach (ListViewItem i in dataListView.Items) {
                if (i.Text == name) { i.SubItems[1].Text = value; return; }
            }
            string[] x = new string[2];
            x[0] = name;
            x[1] = value;
            ListViewItem l = new ListViewItem(x);
            dataListView.Items.Add(l);
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (client == null)
            {
                string[] split = addressTextBox.Text.Split(':');
                if (split.Length != 2) return;

                ushort port = 0;
                if (ushort.TryParse(split[1], out port) == false) return;

                client = new TcpClient();
                client.BeginConnect(IPAddress.Parse(split[0]), port, OnConnectSink, null);
                connectionState = 1;
                UpdateInfo();
            }
            else
            {
                Disconnect();
            }
            UpdateInfo();
        }

        public delegate void AppendTextHandler(string msg);

        private void AppendRawDataText(string msg)
        {
            if (InvokeRequired) { try { Invoke(new AppendTextHandler(AppendRawDataText), msg); } catch (Exception) { } return; }
            mainRawDataTextBox.AppendText(msg + "\r\n");
            //if (outputFile != null) { outputFile.WriteLine(DateTime.Now.ToString() + " " + msg); outputFile.Flush(); }
        }

        public void AppendPacketDataText(string msg)
        {
            if (InvokeRequired) { try { Invoke(new AppendTextHandler(AppendPacketDataText), msg); } catch (Exception) { } return; }
            mainDataPacketTextBox.AppendText(msg + "\r\n");
            if (outputFile != null) { outputFile.WriteLine(DateTime.Now.ToString() + " " + msg); outputFile.Flush(); }
        }

        public delegate void UpdateInfoHandler();
        private void UpdateInfo()
        {
            if (InvokeRequired) { Invoke(new UpdateInfoHandler(UpdateInfo)); return; }
            connectButton.Text = (stream == null) ? "Connect" : "Disconnect";
            addressTextBox.Enabled = (stream == null);
            hexComboBox.Enabled = sendButton.Enabled = (stream != null);
            rawSendTextBox.Enabled = rawSendButton.Enabled = (stream != null);
            string extra = "";
            if (outputFile != null) { extra = ", Capturing to " + logFileInfo.Name; }
            if (connectionState == 0) { mainToolStripStatusLabel.Text = "Disconnected" + extra; }
            if (connectionState == 1) { mainToolStripStatusLabel.Text = "Connecting" + extra; }
            if (connectionState == 2) { mainToolStripStatusLabel.Text = "Connected" + extra; }
            logToFileToolStripMenuItem.Checked = (outputFile != null);
            annotateLogToolStripMenuItem.Enabled = (outputFile != null);
        }

        public delegate void DisconnectHandler();
        private void Disconnect()
        {
            if (InvokeRequired) { Invoke(new DisconnectHandler(Disconnect)); return; }
            if (stream != null) { stream.Close(); stream.Dispose(); stream = null; }
            client = null;
            readBufferLen = 0;
            connectionState = 0;
            UpdateInfo();
        }

        private void OnConnectSink(IAsyncResult ar)
        {
            if (client == null) return;

            // Accept the connection
            try { client.EndConnect(ar); } catch (Exception) { Disconnect(); return; }

            connectionState = 2;
            stream = client.GetStream();
            stream.BeginRead(readBuffer, readBufferLen, readBuffer.Length - readBufferLen, new AsyncCallback(ResponseSink), this);
            UpdateInfo();
        }

        private void ResponseSink(IAsyncResult ar)
        {
            if (client == null) return;
            if (stream == null) return;

            // Read the data
            int len = 0;
            try { len = stream.EndRead(ar); } catch (Exception) { }
            if (len == 0) { Disconnect(); return; }
            AppendRawDataText(" <-- " + ConvertByteArrayToHexString(readBuffer, readBufferLen, len));
            readBufferLen += len;

            // Process the data
            int consumed = 0;
            while ((consumed = processData(readBuffer, readBufferLen)) > 0) {
                if ((consumed > 0) && (consumed < readBufferLen))
                {
                    for (var i = 0; i < (readBufferLen - consumed); i++) { readBuffer[i] = readBuffer[i + consumed]; }
                }
                readBufferLen -= consumed;
            }

            // Read again
            try
            {
                stream.BeginRead(readBuffer, readBufferLen, readBuffer.Length - readBufferLen, new AsyncCallback(ResponseSink), this);
            }
            catch (Exception) { Disconnect(); }
        }

        private int processData(byte[] data, int len)
        {
            //AppendText(" <-- " + ConvertByteArrayToHexString(data, 0, len));
            if (len < 5) return 0;

            if ((data[0] != 0x1c) && ((data[1] != 0x01) || (data[2] != 0x01)) && ((data[4] != 0x40) || (data[4] != 0x80))) {
                AppendPacketDataText("Out of sync: " + ConvertByteArrayToHexString(data, 0, len));
                for (var i = 1; i < len; i++) { if (data[i] == 0x1c) { return i; } }
                return len;
            }

            int datalen = data[3];
            int totallen = datalen + 6;
            if (len < (datalen + 6)) { /*AppendPacketDataText("Incomplete data: " + ConvertByteArrayToHexString(data, 0, len));*/ return 0; }
            byte checksum = ComputeChecksum(data, 1, datalen + 5);
            if (checksum != data[datalen + 5]) {
                AppendPacketDataText("Invalid checksum: " + ConvertByteArrayToHexString(data, 0, len));
                for (var i = 1; i < len; i++) { if (data[i] == 0x1c) { return i; } }
                return len;
            }
            //AppendText("Checksum " + checksum + " / " + data[datalenpadded + 4] + ", len: " + datalenpadded);
            //AppendText(" <-- " + ConvertByteArrayToHexString(data, 1, datalen + 3));
            if ((Filter != 0) && (data[1] != Filter) && (data[2] != Filter)) return totallen;
            if ((FilterCmd1 != 0) && ((datalen < 2) || (data[5] != FilterCmd1))) return totallen;
            if ((FilterCmd2 != 0) && ((datalen < 2) || (data[6] != FilterCmd2))) return totallen;

            try
            {
                string t = ConvertByteArrayToHexString(data, 1, 1) + " " + ConvertByteArrayToHexString(data, 2, 1) + " " + ConvertByteArrayToHexString(data, 4, 1) + " " + ConvertByteArrayToHexString(data, 5, datalen);
                addPacketToStore(t);
                AppendPacketDataText(" <-- " + t);

                // Responses directed at the connection kit
                if ((data[1] == 0x1F) && (data[2] == 0x01) && (data[4] == 0x80))
                {
                    // Lights status
                    if ((data[5] == 0x17) && (data[6] == 0x05))
                    {
                        addDecodedData("Lights", (data[24] == 0x01) ? "Enabled" : "Disabled");
                    }

                    // Temprature status
                    if ((data[5] == 0x02) && (data[6] == 0x56))
                    {
                        addDecodedData("Temp Set", UTF8Encoding.Default.GetString(data, 90, 4));
                        addDecodedData("Temp Current", UTF8Encoding.Default.GetString(data, 94, 4));
                        addDecodedData("Heater Outlet", UTF8Encoding.Default.GetString(data, 36, 4)); // Heater outlet temp
                        addDecodedData("JetInfo1", Convert.ToString(data[9], 2));
                        addDecodedData("JetInfo2", Convert.ToString(data[10], 2));

                        // Decode the time
                        int seconds = data[131];
                        int minutes = data[132];
                        int hours = data[133];
                        int day = data[134];
                        int month = data[135] + 1;
                        int year = data[136] + (data[137] << 8);
                        try { addDecodedData("Time", new DateTime(year, month, day, hours, minutes, seconds).ToString()); } catch (Exception) { }

                        int HeaterTotalRuntime = getIntFromByteArray(data, 40);
                        int Jets1TotalRuntime = getIntFromByteArray(data, 44);
                        int LifetimeRuntimeSeconds1 = getIntFromByteArray(data, 48);
                        int UnknownCounter1 = getIntFromByteArray(data, 52);
                        int Jets2TotalRuntime = getIntFromByteArray(data, 60);
                        int Jets3TotalRuntime = getIntFromByteArray(data, 64);
                        int UnknownCounter2 = getIntFromByteArray(data, 68);
                        int LightsTotalRuntime = getIntFromByteArray(data, 72);
                        int LifetimeRuntimeSeconds2 = getIntFromByteArray(data, 78);
                        int UnknownCounter3 = getIntFromByteArray(data, 82);
                        int UnknownCounter4 = getIntFromByteArray(data, 86);

                        int voltage_l1 = getShortFromByteArray(data, 98);
                        int voltage_heater = getShortFromByteArray(data, 100);
                        int voltage_l2 = getShortFromByteArray(data, 102);

                        int current_l1 = getShortFromByteArray(data, 106);
                        int current_heater = getShortFromByteArray(data, 108);
                        int current_l2 = getShortFromByteArray(data, 110);

                        int power_l1 = getShortFromByteArray(data, 114);
                        int power_heater = getShortFromByteArray(data, 116);
                        int power_l2 = getShortFromByteArray(data, 118);

                        addDecodedData("Heater Total Runtime", HeaterTotalRuntime.ToString());
                        addDecodedData("Jets 1 Total Runtime", Jets1TotalRuntime.ToString());
                        addDecodedData("Lifetime Runtime 1", LifetimeRuntimeSeconds1.ToString());
                        addDecodedData("Unknown Counter 1", UnknownCounter1.ToString());
                        addDecodedData("Jets 2 Total Runtime", Jets2TotalRuntime.ToString());
                        addDecodedData("Jets 3 Total Runtime", Jets3TotalRuntime.ToString());
                        addDecodedData("Unknown Counter 2", UnknownCounter2.ToString());
                        addDecodedData("Lights Total Runtime", LightsTotalRuntime.ToString());
                        addDecodedData("Lifetime Runtime 2", LifetimeRuntimeSeconds2.ToString());
                        addDecodedData("Unknown Counter 3", UnknownCounter3.ToString());
                        addDecodedData("Unknown Counter 4", UnknownCounter4.ToString());

                        addDecodedData("Voltage L1", voltage_l1.ToString());
                        addDecodedData("Voltage Heater", voltage_heater.ToString());
                        addDecodedData("Voltage L2", voltage_l2.ToString());
                        addDecodedData("Current L1", current_l1.ToString());
                        addDecodedData("Current Heater", current_heater.ToString());
                        addDecodedData("Current L2", current_l2.ToString());
                        addDecodedData("Power L1", power_l1.ToString());
                        addDecodedData("Power Heater", power_heater.ToString());
                        addDecodedData("Power L2", power_l2.ToString());

                        string hex = ConvertByteArrayToHexString(data, 1, totallen);
                        string decodeHex = string.Format("Hex: {0}\r\n", hex);
                        decodeHex += string.Format("{0}     - Unknown\r\n", hex.Substring(12, 4));
                        decodeHex += string.Format("{0}       - Flags\r\n", hex.Substring(16, 2));
                        decodeHex += string.Format("{0}       - Flags\r\n", hex.Substring(18, 2));
                        decodeHex += string.Format("{0} - Unknown\r\n", hex.Substring(20, 18));
                        decodeHex += string.Format("{0}     - Flags, 0x4000 is Celsius\r\n", hex.Substring(38, 4));
                        decodeHex += string.Format("{0} - Unknown\r\n", hex.Substring(42, 12));
                        //                    decodeHex += string.Format("{0}     - Unknown\r\n", hex.Substring(46, 4));
                        //                    decodeHex += string.Format("{0}     - Unknown\r\n", hex.Substring(50, 4));
                        decodeHex += string.Format("{0} - Unknown\r\n", hex.Substring(54, 8));
                        decodeHex += string.Format("{0} - Unknown\r\n", hex.Substring(62, 8));
                        decodeHex += string.Format("{0} - Exaust Temperature string, \"{1}\"\r\n", hex.Substring(70, 8), UTF8Encoding.Default.GetString(data, 36, 4));
                        decodeHex += string.Format("{0} - Heater total runtime in seconds, {1}\r\n", hex.Substring(78, 8), getIntFromByteArray(data, 40));
                        decodeHex += string.Format("{0} - Jets 1 total runtime in seconds, {1}\r\n", hex.Substring(86, 8), getIntFromByteArray(data, 44));
                        decodeHex += string.Format("{0} - Lifetime in seconds, {1}\r\n", hex.Substring(94, 8), getIntFromByteArray(data, 48));
                        decodeHex += string.Format("{0} - Power on / Boot counter\r\n", hex.Substring(102, 8));
                        decodeHex += string.Format("{0} - Flags\r\n", hex.Substring(110, 8));
                        decodeHex += string.Format("{0} - Jet 2 runtime\r\n", hex.Substring(118, 8));
                        decodeHex += string.Format("{0} - Jet 3 runtime\r\n", hex.Substring(126, 8));
                        decodeHex += string.Format("{0} - Blower runtime\r\n", hex.Substring(134, 8));
                        decodeHex += string.Format("{0} - Lights runtime\r\n", hex.Substring(142, 8));
                        decodeHex += string.Format("{0}     - SPA state & Light state\r\n", hex.Substring(150, 4));
                        decodeHex += string.Format("{0} - Circulation pump Lifetime\r\n", hex.Substring(154, 8));
                        decodeHex += string.Format("{0} - Jets 1 low operation Lifetime\r\n", hex.Substring(162, 8));
                        decodeHex += string.Format("{0} - Jets 2 low operation Lifetime\r\n", hex.Substring(170, 8));
                        decodeHex += string.Format("{0} - Target Temperature String, \"{1}\"\r\n", hex.Substring(178, 8), UTF8Encoding.Default.GetString(data, 90, 4));
                        decodeHex += string.Format("{0} - Current Temperature String, \"{1}\"\r\n", hex.Substring(186, 8), UTF8Encoding.Default.GetString(data, 94, 4));
                        decodeHex += string.Format("{0}     - Voltage L1\r\n", hex.Substring(194, 4));
                        decodeHex += string.Format("{0}     - Voltage Heater\r\n", hex.Substring(198, 4));
                        decodeHex += string.Format("{0}     - Voltage L2\r\n", hex.Substring(202, 4));
                        decodeHex += string.Format("{0}     - Voltage Jet3\r\n", hex.Substring(206, 4));
                        decodeHex += string.Format("{0}     - Current L1\r\n", hex.Substring(210, 4));
                        decodeHex += string.Format("{0}     - Current Heater\r\n", hex.Substring(214, 4));
                        decodeHex += string.Format("{0}     - Current L2 (Heater)\r\n", hex.Substring(218, 4));
                        decodeHex += string.Format("{0}     - Current Jet3\r\n", hex.Substring(222, 4));
                        decodeHex += string.Format("{0}     - Power L1\r\n", hex.Substring(226, 4));
                        decodeHex += string.Format("{0}     - Power Heater\r\n", hex.Substring(230, 4));
                        decodeHex += string.Format("{0}     - Power L2 (Heater)\r\n", hex.Substring(234, 4));
                        decodeHex += string.Format("{0}     - Power Jet3\r\n", hex.Substring(238, 4));
                        decodeHex += string.Format("{0} - Unknown\r\n", hex.Substring(242, 16));
                        decodeHex += string.Format("{0} - Time hh:mm:ss, {1}:{2}:{3}\r\n", hex.Substring(258, 8), hours, minutes, seconds);
                        decodeHex += string.Format("{0} - Date, {1}-{2}-{3}\r\n", hex.Substring(266, 8), day, month, year);
                        decodeHex += string.Format("{0}       - Clock Status\r\n", hex.Substring(274, 2));

                        setStateDecoded(decodeHex);
                    }

                    // Version string
                    if ((data[5] == 0x01) && (data[6] == 0x0))
                    {
                        addDecodedData("Version", UTF8Encoding.Default.GetString(data, 7, datalen - 3));
                    }
                }

                //if (data[1] == 0x33) { SendPacket("01 33 80 190100190000000B0004010000"); } // Audio emulation
                //if (data[1] == 0x33) { SendPacket("01 33 80 1901"); } // Audio emulation

            }
            catch (Exception ex)
            {
                AppendPacketDataText(ex.ToString());
            }

            return totallen;
        }

        private int getIntFromByteArray(byte[] data, int offset)
        {
            return data[offset] + (data[offset + 1] << 8) + (data[offset + 2] << 16) + (data[offset + 3] << 24);
        }

        private int getShortFromByteArray(byte[] data, int offset)
        {
            return data[offset] + (data[offset + 1] << 8);
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (stream == null) return;
            if (InvokeRequired) { try { Invoke(new EventHandler(sendButton_Click), sender, e); } catch (Exception) { } return; }
            SendPacket(hexComboBox.Text);
        }

        public delegate void SendPacketHandler(string textPacket);

        private void SendPacket(string textPacket)
        {
            if (InvokeRequired) { Invoke(new SendPacketHandler(SendPacket), textPacket); return; }
            textPacket = textPacket.Replace("<", "").Replace("-", "").Replace(":", "").Replace(" ", "").Replace(",", "");
            byte[] raw = ConvertHexStringToByteArray(textPacket);
            if (raw.Length < 4) return;
            byte dst = raw[0];
            byte src = raw[1];
            byte op = raw[2];
            byte[] data = new byte[raw.Length - 3];
            Array.Copy(raw, 3, data, 0, raw.Length - 3);
            int totallen = 6 + data.Length;
            byte[] packet = new byte[totallen];
            packet[0] = 0x1C;
            packet[1] = dst;
            packet[2] = src;
            packet[3] = (byte)(data.Length);
            packet[4] = op;
            data.CopyTo(packet, 5);
            packet[packet.Length - 1] = ComputeChecksum(packet, 1, packet.Length - 1);
            //AppendText(" RAW --> " + ConvertByteArrayToHexString(packet, 0, packet.Length));
            AppendPacketDataText(" --> " + ConvertByteArrayToHexString(packet, 1, 1) + " " + ConvertByteArrayToHexString(packet, 2, 1) + " " + ConvertByteArrayToHexString(packet, 4, 1) + " " + ConvertByteArrayToHexString(packet, 5, data.Length));
            AppendRawDataText(" --> " + ConvertByteArrayToHexString(packet, 0, packet.Length));
            try { stream.WriteAsync(packet, 0, packet.Length); } catch (Exception) { Disconnect(); return; }

            // Add this packet to the combo box
            var exist = false;
            foreach (string i in hexComboBox.Items) { if (i == hexComboBox.Text) { exist = true; return; } }
            if (exist == false) { hexComboBox.Items.Add(hexComboBox.Text); }
        }

        private byte[] ConvertHexStringToByteArray(string hex)
        {
            if (hex.Length % 2 != 0) return new byte[0];
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                string byteValue = hex.Substring(i, 2);
                bytes[i / 2] = Convert.ToByte(byteValue, 16);
            }
            return bytes;
        }

        private string ConvertByteArrayToHexString(byte[] bytes, int offset, int len)
        {
            char[] c = new char[len * 2];
            byte b;
            for (int i = 0; i < len; i++)
            {
                b = ((byte)(bytes[i + offset] >> 4));
                c[i * 2] = (char)(b > 9 ? b + 0x37 : b + 0x30);
                b = ((byte)(bytes[i + offset] & 0xF));
                c[i * 2 + 1] = (char)(b > 9 ? b + 0x37 : b + 0x30);
            }
            return new string(c);
        }

        private byte ComputeChecksum(byte[] bytes, int start, int end)
        {
            //AppendText(string.Format("ComputeChecksum: {0}, {1}", start, end));
            uint sum = 0;
            for (var i = start; i < end; i++) { sum += bytes[i]; }
            sum ^= 0xFFFFFFFF;
            return (byte)(sum & 0xFF);
        }

        private void logToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (outputFile == null)
            {
                if (logSaveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    logFileInfo = new FileInfo(logSaveFileDialog.FileName);
                    outputFile = new StreamWriter(logSaveFileDialog.FileName, true);
                }
            }
            else
            {
                outputFile.Close();
                outputFile.Dispose();
                outputFile = null;
            }
            UpdateInfo();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (outputFile != null)
            {
                outputFile.Close();
                outputFile.Dispose();
                outputFile = null;
            }
        }

        private void rawSendButton_Click(object sender, EventArgs e)
        {
            if (stream == null) return;

            string t = rawSendTextBox.Text;
            t = t.Replace("<", "").Replace("-", "").Replace(":", "").Replace(" ", "").Replace(",", "");
            byte[] packet = ConvertHexStringToByteArray(t);
            AppendRawDataText(" --> " + ConvertByteArrayToHexString(packet, 0, packet.Length));
            stream.WriteAsync(packet, 0, packet.Length);
        }

        private void scanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scanTimer.Enabled)
            {
                scanToolStripMenuItem.Checked = false;
                scanTimer.Enabled = false;
            }
            else
            {
                scanToolStripMenuItem.Checked = true;
                scanValue = 0;
                scanTimer.Enabled = true;
            }
        }

        private void scanTimer_Tick(object sender, EventArgs e)
        {
            if ((scanValue > 65535) || (stream == null)) { scanToolStripMenuItem.Checked = false; scanTimer.Enabled = false; return; }

            /*
            byte[] data = ConvertHexStringToByteArray(hexTextBox.Text);
            AppendText(" --> " + ConvertByteArrayToHexString(data, 0, data.Length));
            stream.WriteAsync(data, 0, data.Length);
            */

            string t = hexComboBox.Text;
            t = t.Replace("<", "").Replace("-", "").Replace(":", "").Replace(" ", "").Replace(",", "");
            byte[] raw = ConvertHexStringToByteArray(t);
            if (raw.Length < 4) return;
            byte dst = raw[0];
            byte src = raw[1];
            byte op = raw[2];
            byte[] data = new byte[raw.Length - 3];
            Array.Copy(raw, 3, data, 0, raw.Length - 3);
            int totallen = 6 + data.Length;
            byte[] packet = new byte[totallen];
            packet[0] = 0x1C;
            packet[1] = dst;
            packet[2] = src;
            packet[3] = (byte)(data.Length);
            packet[4] = op;
            data.CopyTo(packet, 5);

            // Scan all commands
            packet[5] = (byte)(scanValue & 0xFF);
            packet[6] = (byte)(scanValue >> 8);

            packet[packet.Length - 1] = ComputeChecksum(packet, 1, packet.Length - 1);
            //AppendText(" RAW --> " + ConvertByteArrayToHexString(packet, 0, packet.Length));
            AppendPacketDataText(" --> " + ConvertByteArrayToHexString(packet, 1, 1) + " " + ConvertByteArrayToHexString(packet, 2, 1) + " " + ConvertByteArrayToHexString(packet, 4, 1) + " " + ConvertByteArrayToHexString(packet, 5, data.Length));
            AppendRawDataText(" --> " + ConvertByteArrayToHexString(packet, 0, packet.Length));
            stream.WriteAsync(packet, 0, packet.Length);

            scanValue++;
        }

        private void annotateLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new LogAnnotationForm(this).ShowDialog(this);
        }

        private void addressTextBox_TextChanged(object sender, EventArgs e)
        {
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Open Source\IQ2020DataViewer", "address", addressTextBox.Text);
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filter = 0;
            FilterCmd1 = 0;
            FilterCmd2 = 0;
            noneToolStripMenuItem.Checked = true;
            connectionKitToolStripMenuItem.Checked = false;
            connectionKitX0256ToolStripMenuItem.Checked = false;
            freshWaterToolStripMenuItem.Checked = false;
            audioToolStripMenuItem.Checked = false;
            aCEModuleToolStripMenuItem.Checked = false;
        }

        private void connectionKitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filter = 0x1F;
            FilterCmd1 = 0;
            FilterCmd2 = 0;
            noneToolStripMenuItem.Checked = false;
            connectionKitToolStripMenuItem.Checked = true;
            connectionKitX0256ToolStripMenuItem.Checked = false;
            freshWaterToolStripMenuItem.Checked = false;
            audioToolStripMenuItem.Checked = false;
            aCEModuleToolStripMenuItem.Checked = false;
        }

        private void connectionKitX0256ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filter = 0x1F;
            FilterCmd1 = 0x02;
            FilterCmd2 = 0x56;
            noneToolStripMenuItem.Checked = false;
            connectionKitToolStripMenuItem.Checked = false;
            connectionKitX0256ToolStripMenuItem.Checked = true;
            freshWaterToolStripMenuItem.Checked = false;
            audioToolStripMenuItem.Checked = false;
            aCEModuleToolStripMenuItem.Checked = false;
        }

        private void freshWaterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filter = 0x29;
            FilterCmd1 = 0;
            FilterCmd2 = 0;
            noneToolStripMenuItem.Checked = false;
            connectionKitToolStripMenuItem.Checked = false;
            connectionKitX0256ToolStripMenuItem.Checked = false;
            freshWaterToolStripMenuItem.Checked = true;
            audioToolStripMenuItem.Checked = false;
            aCEModuleToolStripMenuItem.Checked = false;
        }

        private void audioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filter = 0x33;
            FilterCmd1 = 0;
            FilterCmd2 = 0;
            noneToolStripMenuItem.Checked = false;
            connectionKitToolStripMenuItem.Checked = false;
            connectionKitX0256ToolStripMenuItem.Checked = false;
            freshWaterToolStripMenuItem.Checked = false;
            audioToolStripMenuItem.Checked = true;
            aCEModuleToolStripMenuItem.Checked = false;
        }

        private void aCEModuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filter = 0x24;
            FilterCmd1 = 0;
            FilterCmd2 = 0;
            noneToolStripMenuItem.Checked = false;
            connectionKitToolStripMenuItem.Checked = false;
            connectionKitX0256ToolStripMenuItem.Checked = false;
            freshWaterToolStripMenuItem.Checked = false;
            audioToolStripMenuItem.Checked = false;
            aCEModuleToolStripMenuItem.Checked = true;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            packetsListView.Columns[0].Width = this.Width - 45;
        }

        private void askLightsStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendPacket("01 1F 40 1705"); // Ask lights status
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SendPacket("01 1F 40 0255"); // Ask status
        }

        private void askTempStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendPacket("01 1F 40 0256"); // Ask status
        }

        private void lightsOnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendPacket("01 1F 40 1702041100"); // Turn on lights
            SendPacket("01 1F 40 1705"); // Ask lights status
        }

        private void lightsOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendPacket("01 1F 40 1702041000"); // Turn on lights
            SendPacket("01 1F 40 1705"); // Ask lights status
        }

        private void askVersionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendPacket("01 1F 40 0100"); // Ask version strings
        }

        private void setCurrentTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime n = DateTime.Now;
            //DateTime n = new DateTime(2023, 12, 31, 23, 59, 30);
            byte[] b = new byte[7];
            b[0] = (byte)n.Second;
            b[1] = (byte)n.Minute;
            b[2] = (byte)n.Hour;
            b[3] = (byte)n.Day;
            b[4] = (byte)(n.Month - 1);
            b[5] = (byte)(n.Year & 0xFF);
            b[6] = (byte)(n.Year >> 8);
            SendPacket("01 1F 40 024C " + ConvertByteArrayToHexString(b , 0 , 7));
        }

        private void filterToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pollStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pollTimer.Enabled)
            {
                pollStateToolStripMenuItem.Checked = false;
                pollTimer.Enabled = false;
            }
            else
            {
                pollStateToolStripMenuItem.Checked = true;
                pollTimer.Enabled = true;
            }
        }

        private void pollTimer_Tick(object sender, EventArgs e)
        {
            SendPacket("01 1F 40 0256"); // Ask status
        }

    }
}
