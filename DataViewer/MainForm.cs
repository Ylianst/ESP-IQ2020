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
        private int scanAddress = 0;
        private int connectionState = 0;
        private FileInfo logFileInfo;
        private int Filter = 0;

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

            //if ((data[5] != 0x02) || (data[6] != 0x56) || (totallen < 20)) return totallen; // DEBUG

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
                    addDecodedData("Time", data[133].ToString("D2") + ":" + data[132].ToString("D2") + ":" + data[131].ToString("D2"));
                    addDecodedData("JetInfo1", Convert.ToString(data[9], 2));
                    addDecodedData("JetInfo2", Convert.ToString(data[10], 2));
                    int timer1 = data[48] + (data[49] << 8) + (data[50] << 16) + (data[51] << 24);
                    addDecodedData("Timer1", timer1.ToString());
                    int timer2 = data[78] + (data[79] << 8) + (data[80] << 16) + (data[81] << 24);
                    addDecodedData("Timer2", timer2.ToString());
                }

                // Version string
                if ((data[5] == 0x01) && (data[6] == 0x0))
                {
                    addDecodedData("Version", UTF8Encoding.Default.GetString(data, 7, datalen - 3));
                }
            }

            //if (data[1] == 0x29) { sendButton_Click(this, null); }
            return totallen;
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (stream == null) return;
            if (InvokeRequired) { try { Invoke(new EventHandler(sendButton_Click), sender, e); } catch (Exception) { } return; }
            SendPacket(hexComboBox.Text);
        }

        private void SendPacket(string textPacket)
        {
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
            scanAddress = 0;
            scanTimer.Enabled = true;
        }

        private void scanTimer_Tick(object sender, EventArgs e)
        {
            if ((scanAddress > 255) || (stream == null)) { scanTimer.Enabled = false; return; }

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
            //packet[4] = (byte)scanAddress;
            data.CopyTo(packet, 5);
            packet[5] = (byte)scanAddress;
            packet[packet.Length - 1] = ComputeChecksum(packet, 1, packet.Length - 1);
            //AppendText(" RAW --> " + ConvertByteArrayToHexString(packet, 0, packet.Length));
            AppendPacketDataText(" --> " + ConvertByteArrayToHexString(packet, 1, 1) + " " + ConvertByteArrayToHexString(packet, 2, 1) + " " + ConvertByteArrayToHexString(packet, 4, 1) + " " + ConvertByteArrayToHexString(packet, 5, data.Length));
            AppendRawDataText(" --> " + ConvertByteArrayToHexString(packet, 0, packet.Length));
            stream.WriteAsync(packet, 0, packet.Length);

            scanAddress++;
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
            noneToolStripMenuItem.Checked = true;
            connectionKitToolStripMenuItem.Checked = false;
            freshWaterToolStripMenuItem.Checked = false;
        }

        private void connectionKitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filter = 0x1F;
            noneToolStripMenuItem.Checked = false;
            connectionKitToolStripMenuItem.Checked = true;
            freshWaterToolStripMenuItem.Checked = false;
        }

        private void freshWaterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filter = 0x29;
            noneToolStripMenuItem.Checked = false;
            connectionKitToolStripMenuItem.Checked = false;
            freshWaterToolStripMenuItem.Checked = true;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            packetsListView.Columns[0].Width = this.Width - 45;
        }

        private void askLightsStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendPacket("01 1F 40 1705"); // Ask lights status
        }

        private void askTempStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendPacket("01 1F 40 0256"); // Ask temp status
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
    }
}
