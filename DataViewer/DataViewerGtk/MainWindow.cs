using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Gtk;

namespace DataViewer
{
    public class MainWindow : Window
    {
        private static CssProvider? cssProvider;
        // TCP Connection
        private TcpClient? client;
        private NetworkStream? stream;
        private byte[] readBuffer = new byte[2000];
        private int readBufferLen = 0;
        private int connectionState = 0; // 0=disconnected, 1=connecting, 2=connected

        // File logging
        private StreamWriter? outputFile = null;
        private FileInfo? logFileInfo;

        // Filters
        private int Filter = 0;
        private int FilterCmd1 = 0;
        private int FilterCmd2 = 0;

        // Scanning
        private int scanValue = 0;
        private System.Timers.Timer? scanTimer;

        // UI Widgets
        private Entry addressEntry;
        private Button connectButton;
        private TextView dataPacketTextView;
        private TextView rawDataTextView;
        private TextView stateDecodeTextView;
        private ComboBoxText hexComboBox;
        private Button sendButton;
        private Entry rawSendEntry;
        private Button rawSendButton;
        private TreeView packetsTreeView;
        private TreeView dataTreeView;
        private Label statusLabel;
        private CheckMenuItem? logToFileMenuItem;
        private MenuItem? annotateLogMenuItem;
        private CheckMenuItem? pollStateMenuItem;
        private CheckMenuItem? scanMenuItem;
        
        private System.Timers.Timer? pollTimer;

        public MainWindow() : base("IQ2020 Data Viewer")
        {
            SetDefaultSize(800, 600);
            SetPosition(WindowPosition.Center);
            DeleteEvent += OnDeleteEvent;

            // Initialize CSS for monospace font
            InitializeCSS();

            // Create main layout
            var mainVBox = new Box(Orientation.Vertical, 0);

            // Menu bar
            var menuBar = CreateMenuBar();
            mainVBox.PackStart(menuBar, false, false, 0);

            // Top panel - Address and Connect
            var topHBox = new Box(Orientation.Horizontal, 5);
            topHBox.BorderWidth = 5;
            
            addressEntry = new Entry();
            addressEntry.Text = LoadSavedAddress();
            addressEntry.Changed += OnAddressChanged;
            topHBox.PackStart(addressEntry, true, true, 0);

            connectButton = new Button("Connect");
            connectButton.Clicked += OnConnectClicked;
            topHBox.PackStart(connectButton, false, false, 0);

            mainVBox.PackStart(topHBox, false, false, 0);

            // Notebook (tabs)
            var notebook = new Notebook();

            // Tab 1: Data Packets
            var dataPacketsVBox = new Box(Orientation.Vertical, 0);
            var dataPacketScrolled = new ScrolledWindow();
            dataPacketTextView = new TextView();
            dataPacketTextView.Editable = false;
            dataPacketTextView.Name = "monospace-text";
            dataPacketScrolled.Add(dataPacketTextView);
            dataPacketsVBox.PackStart(dataPacketScrolled, true, true, 0);

            var sendHBox = new Box(Orientation.Horizontal, 5);
            sendHBox.BorderWidth = 5;
            hexComboBox = ComboBoxText.NewWithEntry();
            hexComboBox.Entry.Text = "29 99 40 1E010000FFFFFF00FF04FFFFFFFFFF";
            hexComboBox.Entry.Name = "monospace-entry";
            sendHBox.PackStart(hexComboBox, true, true, 0);

            sendButton = new Button("Send");
            sendButton.Sensitive = false;
            sendButton.Clicked += OnSendClicked;
            sendHBox.PackStart(sendButton, false, false, 0);

            dataPacketsVBox.PackStart(sendHBox, false, false, 0);
            notebook.AppendPage(dataPacketsVBox, new Label("Data Packets"));

            // Tab 2: Raw Data
            var rawDataVBox = new Box(Orientation.Vertical, 0);
            var rawDataScrolled = new ScrolledWindow();
            rawDataTextView = new TextView();
            rawDataTextView.Editable = false;
            rawDataTextView.Name = "monospace-text";
            rawDataScrolled.Add(rawDataTextView);
            rawDataVBox.PackStart(rawDataScrolled, true, true, 0);

            var rawSendHBox = new Box(Orientation.Horizontal, 5);
            rawSendHBox.BorderWidth = 5;
            rawSendEntry = new Entry();
            rawSendEntry.Text = "1C2101034021010078";
            rawSendEntry.Name = "monospace-entry";
            rawSendEntry.Sensitive = false;
            rawSendHBox.PackStart(rawSendEntry, true, true, 0);

            rawSendButton = new Button("Send");
            rawSendButton.Sensitive = false;
            rawSendButton.Clicked += OnRawSendClicked;
            rawSendHBox.PackStart(rawSendButton, false, false, 0);

            rawDataVBox.PackStart(rawSendHBox, false, false, 0);
            notebook.AppendPage(rawDataVBox, new Label("Raw Data"));

            // Tab 3: Packet Store
            var packetsScrolled = new ScrolledWindow();
            packetsTreeView = CreatePacketsTreeView();
            packetsScrolled.Add(packetsTreeView);
            notebook.AppendPage(packetsScrolled, new Label("Packet Store"));

            // Tab 4: Decoded Data
            var dataScrolled = new ScrolledWindow();
            dataTreeView = CreateDataTreeView();
            dataScrolled.Add(dataTreeView);
            notebook.AppendPage(dataScrolled, new Label("Decoded Data"));

            // Tab 5: State Decode
            var stateScrolled = new ScrolledWindow();
            stateDecodeTextView = new TextView();
            stateDecodeTextView.Editable = false;
            stateDecodeTextView.Name = "monospace-text";
            stateScrolled.Add(stateDecodeTextView);
            notebook.AppendPage(stateScrolled, new Label("State"));

            mainVBox.PackStart(notebook, true, true, 0);

            // Status bar
            var statusBar = new Box(Orientation.Horizontal, 0);
            statusBar.BorderWidth = 2;
            statusLabel = new Label("Disconnected");
            statusLabel.Xalign = 0;
            statusLabel.Yalign = 0.5f;
            statusBar.PackStart(statusLabel, true, true, 5);
            mainVBox.PackStart(statusBar, false, false, 0);

            Add(mainVBox);
            ShowAll();
            
            UpdateInfo();
        }

        private MenuBar CreateMenuBar()
        {
            var menuBar = new MenuBar();

            // File Menu
            var fileMenu = new Menu();
            var fileMenuItem = new MenuItem("File");
            fileMenuItem.Submenu = fileMenu;

            logToFileMenuItem = new CheckMenuItem("Log to File...");
            logToFileMenuItem.Activated += OnLogToFileClicked;
            fileMenu.Append(logToFileMenuItem);

            annotateLogMenuItem = new MenuItem("Annotate Log...");
            annotateLogMenuItem.Activated += OnAnnotateLogClicked;
            fileMenu.Append(annotateLogMenuItem);

            fileMenu.Append(new SeparatorMenuItem());

            var clearMenuItem = new MenuItem("Clear");
            clearMenuItem.Activated += OnClearClicked;
            fileMenu.Append(clearMenuItem);

            fileMenu.Append(new SeparatorMenuItem());

            var exitMenuItem = new MenuItem("Exit");
            exitMenuItem.Activated += (s, e) => Application.Quit();
            fileMenu.Append(exitMenuItem);

            menuBar.Append(fileMenuItem);

            // Actions Menu
            var actionsMenu = new Menu();
            var actionsMenuItem = new MenuItem("Actions");
            actionsMenuItem.Submenu = actionsMenu;

            pollStateMenuItem = new CheckMenuItem("Poll State");
            pollStateMenuItem.Activated += OnPollStateClicked;
            actionsMenu.Append(pollStateMenuItem);

            scanMenuItem = new CheckMenuItem("Scan");
            scanMenuItem.Activated += OnScanClicked;
            actionsMenu.Append(scanMenuItem);

            actionsMenu.Append(new SeparatorMenuItem());

            var fiqTest1Item = new MenuItem("FIQ Test 1");
            fiqTest1Item.Activated += OnFiqTest1Clicked;
            actionsMenu.Append(fiqTest1Item);

            var fiqTest2Item = new MenuItem("FIQ Test 2");
            fiqTest2Item.Activated += OnFiqTest2Clicked;
            actionsMenu.Append(fiqTest2Item);

            menuBar.Append(actionsMenuItem);

            // Filter Menu
            var filterMenu = new Menu();
            var filterMenuItem = new MenuItem("Filter");
            filterMenuItem.Submenu = filterMenu;

            var noneFilterItem = new RadioMenuItem("None");
            noneFilterItem.Active = true;
            noneFilterItem.Activated += (s, e) => { Filter = 0; FilterCmd1 = 0; FilterCmd2 = 0; };
            filterMenu.Append(noneFilterItem);

            var connectionKitItem = new RadioMenuItem(noneFilterItem, "Connection Kit");
            connectionKitItem.Activated += (s, e) => { Filter = 0x1F; FilterCmd1 = 0; FilterCmd2 = 0; };
            filterMenu.Append(connectionKitItem);

            var connectionKit256Item = new RadioMenuItem(noneFilterItem, "Connection Kit 0x0256");
            connectionKit256Item.Activated += (s, e) => { Filter = 0x1F; FilterCmd1 = 0x02; FilterCmd2 = 0x56; };
            filterMenu.Append(connectionKit256Item);

            var freshWaterItem = new RadioMenuItem(noneFilterItem, "Fresh Water");
            freshWaterItem.Activated += (s, e) => { Filter = 0x29; FilterCmd1 = 0; FilterCmd2 = 0; };
            filterMenu.Append(freshWaterItem);

            var freshwaterIqModuleItem = new RadioMenuItem(noneFilterItem, "Freshwater IQ Module");
            freshwaterIqModuleItem.Activated += (s, e) => { Filter = 0x37; FilterCmd1 = 0; FilterCmd2 = 0; };
            filterMenu.Append(freshwaterIqModuleItem);

            var audioItem = new RadioMenuItem(noneFilterItem, "Audio");
            audioItem.Activated += (s, e) => { Filter = 0x33; FilterCmd1 = 0; FilterCmd2 = 0; };
            filterMenu.Append(audioItem);

            var aceModuleItem = new RadioMenuItem(noneFilterItem, "ACE Module");
            aceModuleItem.Activated += (s, e) => { Filter = 0x24; FilterCmd1 = 0; FilterCmd2 = 0; };
            filterMenu.Append(aceModuleItem);

            menuBar.Append(filterMenuItem);

            // Commands Menu
            var commandsMenu = new Menu();
            var commandsMenuItem = new MenuItem("Commands");
            commandsMenuItem.Submenu = commandsMenu;

            var askLightsItem = new MenuItem("Ask Lights Status");
            askLightsItem.Activated += (s, e) => SendPacket("01 1F 40 1705");
            commandsMenu.Append(askLightsItem);

            var askStatus255Item = new MenuItem("Ask Status 0x0255");
            askStatus255Item.Activated += (s, e) => SendPacket("01 1F 40 0255");
            commandsMenu.Append(askStatus255Item);

            var askStatus256Item = new MenuItem("Ask Status 0x0256");
            askStatus256Item.Activated += (s, e) => SendPacket("01 1F 40 0256");
            commandsMenu.Append(askStatus256Item);

            commandsMenu.Append(new SeparatorMenuItem());

            var lightsOnItem = new MenuItem("Lights On");
            lightsOnItem.Activated += (s, e) => { SendPacket("01 1F 40 1702041100"); SendPacket("01 1F 40 1705"); };
            commandsMenu.Append(lightsOnItem);

            var lightsOffItem = new MenuItem("Lights Off");
            lightsOffItem.Activated += (s, e) => { SendPacket("01 1F 40 1702041000"); SendPacket("01 1F 40 1705"); };
            commandsMenu.Append(lightsOffItem);

            commandsMenu.Append(new SeparatorMenuItem());

            var askVersionsItem = new MenuItem("Ask Versions");
            askVersionsItem.Activated += (s, e) => SendPacket("01 1F 40 0100");
            commandsMenu.Append(askVersionsItem);

            var setTimeItem = new MenuItem("Set Current Time");
            setTimeItem.Activated += OnSetCurrentTimeClicked;
            commandsMenu.Append(setTimeItem);

            menuBar.Append(commandsMenuItem);

            return menuBar;
        }

        private TreeView CreatePacketsTreeView()
        {
            var store = new ListStore(typeof(string));
            var treeView = new TreeView(store);
            treeView.Name = "monospace-treeview";
            treeView.HeadersVisible = false;

            var column = new TreeViewColumn();
            var cell = new CellRendererText();
            column.PackStart(cell, true);
            column.AddAttribute(cell, "text", 0);
            treeView.AppendColumn(column);

            return treeView;
        }

        private TreeView CreateDataTreeView()
        {
            var store = new ListStore(typeof(string), typeof(string));
            var treeView = new TreeView(store);
            treeView.Name = "monospace-treeview";

            var nameColumn = new TreeViewColumn();
            nameColumn.Title = "Name";
            var nameCell = new CellRendererText();
            nameColumn.PackStart(nameCell, true);
            nameColumn.AddAttribute(nameCell, "text", 0);
            treeView.AppendColumn(nameColumn);

            var valueColumn = new TreeViewColumn();
            valueColumn.Title = "Value";
            var valueCell = new CellRendererText();
            valueColumn.PackStart(valueCell, true);
            valueColumn.AddAttribute(valueCell, "text", 1);
            treeView.AppendColumn(valueColumn);

            return treeView;
        }

        private static void InitializeCSS()
        {
            if (cssProvider == null)
            {
                cssProvider = new CssProvider();
                var css = @"
                    #monospace-text text {
                        font-family: monospace;
                        font-size: 10pt;
                    }
                    #monospace-entry {
                        font-family: monospace;
                        font-size: 10pt;
                    }
                    #monospace-treeview {
                        font-family: monospace;
                        font-size: 10pt;
                    }
                ";
                cssProvider.LoadFromData(css);
                StyleContext.AddProviderForScreen(
                    Gdk.Screen.Default,
                    cssProvider,
                    StyleProviderPriority.Application);
            }
        }

        private string LoadSavedAddress()
        {
            try
            {
                var configPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".iq2020dataviewer");
                if (File.Exists(configPath))
                {
                    return File.ReadAllText(configPath).Trim();
                }
            }
            catch { }
            return "192.168.2.184:1234";
        }

        private void SaveAddress(string address)
        {
            try
            {
                var configPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".iq2020dataviewer");
                File.WriteAllText(configPath, address);
            }
            catch { }
        }

        private void OnAddressChanged(object? sender, EventArgs e)
        {
            SaveAddress(addressEntry.Text);
        }

        private void OnDeleteEvent(object sender, DeleteEventArgs args)
        {
            if (outputFile != null)
            {
                outputFile.Close();
                outputFile.Dispose();
                outputFile = null;
            }
            Application.Quit();
            args.RetVal = true;
        }

        private void OnClearClicked(object? sender, EventArgs e)
        {
            dataPacketTextView.Buffer.Text = "";
            rawDataTextView.Buffer.Text = "";
            stateDecodeTextView.Buffer.Text = "";
            (packetsTreeView.Model as ListStore)?.Clear();
            (dataTreeView.Model as ListStore)?.Clear();
        }

        private void OnConnectClicked(object? sender, EventArgs e)
        {
            if (client == null)
            {
                string[] split = addressEntry.Text.Split(':');
                if (split.Length != 2)
                {
                    AppendPacketDataText("Invalid address format. Use: host:port");
                    return;
                }

                if (!ushort.TryParse(split[1], out ushort port))
                {
                    AppendPacketDataText("Invalid port number");
                    return;
                }

                string host = split[0].Trim();
                client = new TcpClient();
                connectionState = 1;
                UpdateInfo();

                Task.Run(async () =>
                {
                    try
                    {
                        // Use ConnectAsync with hostname string instead of IPAddress.Parse
                        // This allows DNS resolution and handles both IP addresses and hostnames
                        await client.ConnectAsync(host, port);
                        Application.Invoke((s, a) => OnConnected());
                    }
                    catch (Exception ex)
                    {
                        Application.Invoke((s, a) => 
                        {
                            AppendPacketDataText($"Connection failed: {ex.Message}");
                            Disconnect();
                        });
                    }
                });
            }
            else
            {
                Disconnect();
            }
        }

        private void OnConnected()
        {
            if (client == null) return;

            connectionState = 2;
            stream = client.GetStream();
            UpdateInfo();

            Task.Run(async () =>
            {
                try
                {
                    while (stream != null && client != null && client.Connected)
                    {
                        int len = await stream.ReadAsync(readBuffer, readBufferLen, readBuffer.Length - readBufferLen);
                        if (len == 0)
                        {
                            Application.Invoke((s, a) => Disconnect());
                            break;
                        }

                        Application.Invoke((s, a) => AppendRawDataText(" <-- " + ConvertByteArrayToHexString(readBuffer, readBufferLen, len)));
                        readBufferLen += len;

                        // Process the data
                        int consumed;
                        while ((consumed = ProcessData(readBuffer, readBufferLen)) > 0)
                        {
                            if (consumed < readBufferLen)
                            {
                                Array.Copy(readBuffer, consumed, readBuffer, 0, readBufferLen - consumed);
                            }
                            readBufferLen -= consumed;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Application.Invoke((s, a) =>
                    {
                        AppendPacketDataText($"Read error: {ex.Message}");
                        Disconnect();
                    });
                }
            });
        }

        private void Disconnect()
        {
            if (stream != null)
            {
                stream.Close();
                stream.Dispose();
                stream = null;
            }
            client = null;
            readBufferLen = 0;
            connectionState = 0;
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            connectButton.Label = (stream == null) ? "Connect" : "Disconnect";
            addressEntry.Sensitive = (stream == null);
            hexComboBox.Sensitive = sendButton.Sensitive = (stream != null);
            rawSendEntry.Sensitive = rawSendButton.Sensitive = (stream != null);

            string extra = "";
            if (outputFile != null && logFileInfo != null)
            {
                extra = $", Capturing to {logFileInfo.Name}";
            }

            if (connectionState == 0) statusLabel.Text = "Disconnected" + extra;
            else if (connectionState == 1) statusLabel.Text = "Connecting" + extra;
            else if (connectionState == 2) statusLabel.Text = "Connected" + extra;

            if (logToFileMenuItem != null)
            {
                logToFileMenuItem.Active = (outputFile != null);
            }

            if (annotateLogMenuItem != null)
            {
                annotateLogMenuItem.Sensitive = (outputFile != null);
            }
        }

        private void AppendRawDataText(string msg)
        {
            var buffer = rawDataTextView.Buffer;
            var endIter = buffer.EndIter;
            buffer.Insert(ref endIter, msg + "\n");
            rawDataTextView.ScrollToIter(buffer.EndIter, 0, false, 0, 0);
        }

        private void AppendPacketDataText(string msg)
        {
            var buffer = dataPacketTextView.Buffer;
            var endIter = buffer.EndIter;
            buffer.Insert(ref endIter, msg + "\n");
            dataPacketTextView.ScrollToIter(buffer.EndIter, 0, false, 0, 0);

            if (outputFile != null)
            {
                outputFile.WriteLine(DateTime.Now.ToString() + " " + msg);
                outputFile.Flush();
            }
        }

        private void SetStateDecoded(string value)
        {
            stateDecodeTextView.Buffer.Text = value;
        }

        private void AddPacketToStore(string packet)
        {
            var store = packetsTreeView.Model as ListStore;
            if (store == null) return;

            // Check if packet already exists
            TreeIter iter;
            if (store.GetIterFirst(out iter))
            {
                do
                {
                    string existingPacket = (string)store.GetValue(iter, 0);
                    if (existingPacket == packet) return;
                } while (store.IterNext(ref iter));
            }

            store.AppendValues(packet);
        }

        private void AddDecodedData(string name, string value)
        {
            var store = dataTreeView.Model as ListStore;
            if (store == null) return;

            // Update existing or add new
            TreeIter iter;
            if (store.GetIterFirst(out iter))
            {
                do
                {
                    string existingName = (string)store.GetValue(iter, 0);
                    if (existingName == name)
                    {
                        store.SetValue(iter, 1, value);
                        return;
                    }
                } while (store.IterNext(ref iter));
            }

            store.AppendValues(name, value);
        }

        private int ProcessData(byte[] data, int len)
        {
            if (len < 5) return 0;

            if ((data[0] != 0x1c) && ((data[1] != 0x01) || (data[2] != 0x01)) && ((data[4] != 0x40) || (data[4] != 0x80)))
            {
                AppendPacketDataText("Out of sync: " + ConvertByteArrayToHexString(data, 0, len));
                for (var i = 1; i < len; i++) { if (data[i] == 0x1c) { return i; } }
                return len;
            }

            int datalen = data[3];
            int totallen = datalen + 6;
            if (len < totallen) return 0;

            byte checksum = ComputeChecksum(data, 1, datalen + 5);
            if (checksum != data[datalen + 5])
            {
                AppendPacketDataText("Invalid checksum: " + ConvertByteArrayToHexString(data, 0, len));
                for (var i = 1; i < len; i++) { if (data[i] == 0x1c) { return i; } }
                return len;
            }

            if ((Filter != 0) && (data[1] != Filter) && (data[2] != Filter)) return totallen;
            if ((FilterCmd1 != 0) && ((datalen < 2) || (data[5] != FilterCmd1))) return totallen;
            if ((FilterCmd2 != 0) && ((datalen < 2) || (data[6] != FilterCmd2))) return totallen;

            try
            {
                string t = ConvertByteArrayToHexString(data, 1, 1) + " " + ConvertByteArrayToHexString(data, 2, 1) + " " + ConvertByteArrayToHexString(data, 4, 1) + " " + ConvertByteArrayToHexString(data, 5, datalen);
                AddPacketToStore(t);
                AppendPacketDataText(" <-- " + t);

                // Decode specific packets
                if ((data[1] == 0x1F) && (data[2] == 0x01) && (data[4] == 0x80))
                {
                    if ((data[5] == 0x17) && (data[6] == 0x05))
                    {
                        AddDecodedData("Lights", (data[24] == 0x01) ? "Enabled" : "Disabled");
                    }

                    if ((data[5] == 0x02) && (data[6] == 0x56) && datalen > 100)
                    {
                        AddDecodedData("Temp Set", Encoding.UTF8.GetString(data, 90, 4));
                        AddDecodedData("Temp Current", Encoding.UTF8.GetString(data, 94, 4));
                        AddDecodedData("Heater Outlet", Encoding.UTF8.GetString(data, 36, 4));
                        AddDecodedData("JetInfo1", Convert.ToString(data[9], 2));
                        AddDecodedData("JetInfo2", Convert.ToString(data[10], 2));

                        int seconds = data[131];
                        int minutes = data[132];
                        int hours = data[133];
                        int day = data[134];
                        int month = data[135];
                        int year = data[136] + (data[137] << 8);
                        try
                        {
                            AddDecodedData("Time", new DateTime(year, month, day, hours, minutes, seconds).ToString());
                        }
                        catch { }

                        int HeaterTotalRuntime = GetIntFromByteArray(data, 40);
                        int Jets1TotalRuntime = GetIntFromByteArray(data, 44);
                        int LifetimeRuntimeSeconds1 = GetIntFromByteArray(data, 48);
                        int UnknownCounter1 = GetIntFromByteArray(data, 52);
                        int Jets2TotalRuntime = GetIntFromByteArray(data, 60);
                        int Jets3TotalRuntime = GetIntFromByteArray(data, 64);
                        int UnknownCounter2 = GetIntFromByteArray(data, 68);
                        int LightsTotalRuntime = GetIntFromByteArray(data, 72);
                        int LifetimeRuntimeSeconds2 = GetIntFromByteArray(data, 78);
                        int UnknownCounter3 = GetIntFromByteArray(data, 82);
                        int UnknownCounter4 = GetIntFromByteArray(data, 86);

                        int voltage_l1 = GetShortFromByteArray(data, 98);
                        int voltage_heater = GetShortFromByteArray(data, 100);
                        int voltage_l2 = GetShortFromByteArray(data, 102);

                        int current_l1 = GetShortFromByteArray(data, 106);
                        int current_heater = GetShortFromByteArray(data, 108);
                        int current_l2 = GetShortFromByteArray(data, 110);

                        int power_l1 = GetShortFromByteArray(data, 114);
                        int power_heater = GetShortFromByteArray(data, 116);
                        int power_l2 = GetShortFromByteArray(data, 118);

                        AddDecodedData("Heater Total Runtime", HeaterTotalRuntime.ToString());
                        AddDecodedData("Jets 1 Total Runtime", Jets1TotalRuntime.ToString());
                        AddDecodedData("Lifetime Runtime 1", LifetimeRuntimeSeconds1.ToString());
                        AddDecodedData("Unknown Counter 1", UnknownCounter1.ToString());
                        AddDecodedData("Jets 2 Total Runtime", Jets2TotalRuntime.ToString());
                        AddDecodedData("Jets 3 Total Runtime", Jets3TotalRuntime.ToString());
                        AddDecodedData("Unknown Counter 2", UnknownCounter2.ToString());
                        AddDecodedData("Lights Total Runtime", LightsTotalRuntime.ToString());
                        AddDecodedData("Lifetime Runtime 2", LifetimeRuntimeSeconds2.ToString());
                        AddDecodedData("Unknown Counter 3", UnknownCounter3.ToString());
                        AddDecodedData("Unknown Counter 4", UnknownCounter4.ToString());

                        AddDecodedData("Voltage L1", voltage_l1.ToString());
                        AddDecodedData("Voltage Heater", voltage_heater.ToString());
                        AddDecodedData("Voltage L2", voltage_l2.ToString());
                        AddDecodedData("Current L1", current_l1.ToString());
                        AddDecodedData("Current Heater", current_heater.ToString());
                        AddDecodedData("Current L2", current_l2.ToString());
                        AddDecodedData("Power L1", power_l1.ToString());
                        AddDecodedData("Power Heater", power_heater.ToString());
                        AddDecodedData("Power L2", power_l2.ToString());

                        string hex = ConvertByteArrayToHexString(data, 1, totallen);
                        string decodeHex = string.Format("Hex: {0}\r\n", hex);
                        decodeHex += string.Format("{0}     - Unknown\r\n", hex.Substring(12, 4));
                        decodeHex += string.Format("{0}       - Flags\r\n", hex.Substring(16, 2));
                        decodeHex += string.Format("{0}       - Flags\r\n", hex.Substring(18, 2));
                        decodeHex += string.Format("{0} - Unknown\r\n", hex.Substring(20, 18));
                        decodeHex += string.Format("{0}     - Flags, 0x4000 is Celsius\r\n", hex.Substring(38, 4));
                        decodeHex += string.Format("{0} - Unknown\r\n", hex.Substring(42, 12));
                        decodeHex += string.Format("{0} - Unknown\r\n", hex.Substring(54, 8));
                        decodeHex += string.Format("{0} - Unknown\r\n", hex.Substring(62, 8));
                        decodeHex += string.Format("{0} - Exaust Temperature string, \"{1}\"\r\n", hex.Substring(70, 8), Encoding.UTF8.GetString(data, 36, 4));
                        decodeHex += string.Format("{0} - Heater total runtime in seconds, {1}\r\n", hex.Substring(78, 8), GetIntFromByteArray(data, 40));
                        decodeHex += string.Format("{0} - Jets 1 total runtime in seconds, {1}\r\n", hex.Substring(86, 8), GetIntFromByteArray(data, 44));
                        decodeHex += string.Format("{0} - Lifetime in seconds, {1}\r\n", hex.Substring(94, 8), GetIntFromByteArray(data, 48));
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
                        decodeHex += string.Format("{0} - Target Temperature String, \"{1}\"\r\n", hex.Substring(178, 8), Encoding.UTF8.GetString(data, 90, 4));
                        decodeHex += string.Format("{0} - Current Temperature String, \"{1}\"\r\n", hex.Substring(186, 8), Encoding.UTF8.GetString(data, 94, 4));
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

                        SetStateDecoded(decodeHex);
                    }

                    if ((data[5] == 0x01) && (data[6] == 0x0))
                    {
                        AddDecodedData("Version", Encoding.UTF8.GetString(data, 7, datalen - 3));
                    }
                }
            }
            catch (Exception ex)
            {
                AppendPacketDataText(ex.ToString());
            }

            return totallen;
        }

        private int GetIntFromByteArray(byte[] data, int offset)
        {
            return data[offset] + (data[offset + 1] << 8) + (data[offset + 2] << 16) + (data[offset + 3] << 24);
        }

        private int GetShortFromByteArray(byte[] data, int offset)
        {
            return data[offset] + (data[offset + 1] << 8);
        }

        private void OnSendClicked(object? sender, EventArgs e)
        {
            if (stream == null) return;
            SendPacket(hexComboBox.Entry.Text);
        }

        private void SendPacket(string textPacket)
        {
            if (stream == null) return;

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

            AppendPacketDataText(" --> " + ConvertByteArrayToHexString(packet, 1, 1) + " " + ConvertByteArrayToHexString(packet, 2, 1) + " " + ConvertByteArrayToHexString(packet, 4, 1) + " " + ConvertByteArrayToHexString(packet, 5, data.Length));
            AppendRawDataText(" --> " + ConvertByteArrayToHexString(packet, 0, packet.Length));

            try
            {
                stream.WriteAsync(packet, 0, packet.Length);
            }
            catch (Exception ex)
            {
                AppendPacketDataText($"Send error: {ex.Message}");
                Disconnect();
            }

            // Add to combo box if not exists
            bool exists = false;
            TreeIter iter;
            var model = hexComboBox.Model;
            if (model.GetIterFirst(out iter))
            {
                do
                {
                    string item = (string)model.GetValue(iter, 0);
                    if (item == hexComboBox.Entry.Text)
                    {
                        exists = true;
                        break;
                    }
                } while (model.IterNext(ref iter));
            }
            if (!exists)
            {
                hexComboBox.AppendText(hexComboBox.Entry.Text);
            }
        }

        private void OnRawSendClicked(object? sender, EventArgs e)
        {
            if (stream == null) return;

            string t = rawSendEntry.Text;
            t = t.Replace("<", "").Replace("-", "").Replace(":", "").Replace(" ", "").Replace(",", "");
            byte[] packet = ConvertHexStringToByteArray(t);
            AppendRawDataText(" --> " + ConvertByteArrayToHexString(packet, 0, packet.Length));

            try
            {
                stream.WriteAsync(packet, 0, packet.Length);
            }
            catch (Exception ex)
            {
                AppendPacketDataText($"Send error: {ex.Message}");
                Disconnect();
            }
        }

        private byte[] ConvertHexStringToByteArray(string hex)
        {
            if (hex.Length % 2 != 0) return Array.Empty<byte>();
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
            uint sum = 0;
            for (var i = start; i < end; i++) { sum += bytes[i]; }
            sum ^= 0xFFFFFFFF;
            return (byte)(sum & 0xFF);
        }

        private void OnLogToFileClicked(object? sender, EventArgs e)
        {
            if (outputFile == null)
            {
                var fileChooser = new FileChooserDialog(
                    "Save Log File",
                    this,
                    FileChooserAction.Save,
                    "Cancel", ResponseType.Cancel,
                    "Save", ResponseType.Accept);

                fileChooser.CurrentName = "logfile.txt";

                if (fileChooser.Run() == (int)ResponseType.Accept)
                {
                    logFileInfo = new FileInfo(fileChooser.Filename);
                    outputFile = new StreamWriter(fileChooser.Filename, true);
                }

                fileChooser.Destroy();
            }
            else
            {
                outputFile.Close();
                outputFile.Dispose();
                outputFile = null;
            }
            UpdateInfo();
        }

        private void OnPollStateClicked(object? sender, EventArgs e)
        {
            if (pollTimer == null)
            {
                pollTimer = new System.Timers.Timer(2000);
                pollTimer.Elapsed += (s, args) =>
                {
                    Application.Invoke((_, __) => SendPacket("01 1F 40 0256"));
                };
                pollTimer.Start();
                if (pollStateMenuItem != null)
                {
                    pollStateMenuItem.Active = true;
                }
            }
            else
            {
                pollTimer.Stop();
                pollTimer.Dispose();
                pollTimer = null;
                if (pollStateMenuItem != null)
                {
                    pollStateMenuItem.Active = false;
                }
            }
        }

        private void OnSetCurrentTimeClicked(object? sender, EventArgs e)
        {
            DateTime n = DateTime.Now;
            byte[] b = new byte[7];
            b[0] = (byte)n.Second;
            b[1] = (byte)n.Minute;
            b[2] = (byte)n.Hour;
            b[3] = (byte)n.Day;
            b[4] = (byte)(n.Month);
            b[5] = (byte)(n.Year & 0xFF);
            b[6] = (byte)(n.Year >> 8);
            SendPacket("01 1F 40 024C " + ConvertByteArrayToHexString(b, 0, 7));
        }

        private void OnAnnotateLogClicked(object? sender, EventArgs e)
        {
            if (outputFile == null) return;

            var dialog = new Dialog("Annotate Log", this, DialogFlags.Modal);
            dialog.SetDefaultSize(400, 150);

            var contentArea = dialog.ContentArea;
            contentArea.Spacing = 10;
            contentArea.BorderWidth = 10;

            var label = new Label("Enter annotation:");
            label.Xalign = 0;
            contentArea.PackStart(label, false, false, 0);

            var entry = new Entry();
            entry.ActivatesDefault = true;
            contentArea.PackStart(entry, false, false, 0);

            dialog.AddButton("Cancel", ResponseType.Cancel);
            var okButton = dialog.AddButton("OK", ResponseType.Ok);
            okButton.Sensitive = false;
            dialog.DefaultResponse = ResponseType.Ok;

            entry.Changed += (s, args) => { okButton.Sensitive = entry.Text.Length > 0; };

            dialog.ShowAll();
            entry.GrabFocus();

            if (dialog.Run() == (int)ResponseType.Ok && entry.Text.Length > 0)
            {
                AppendPacketDataText(" --- " + entry.Text);
            }

            dialog.Destroy();
        }

        private void OnScanClicked(object? sender, EventArgs e)
        {
            if (scanTimer == null)
            {
                scanValue = 0;
                scanTimer = new System.Timers.Timer(100);
                scanTimer.Elapsed += (s, args) =>
                {
                    Application.Invoke((_, __) =>
                    {
                        if (scanValue > 65535 || stream == null)
                        {
                            StopScan();
                            return;
                        }

                        string t = hexComboBox.Entry.Text;
                        t = t.Replace("<", "").Replace("-", "").Replace(":", "").Replace(" ", "").Replace(",", "");
                        byte[] raw = ConvertHexStringToByteArray(t);
                        if (raw.Length < 4)
                        {
                            StopScan();
                            return;
                        }

                        byte dst = raw[0];
                        byte src = raw[1];
                        byte op = raw[2];
                        byte[] cmdData = new byte[raw.Length - 3];
                        Array.Copy(raw, 3, cmdData, 0, raw.Length - 3);
                        int totallen = 6 + cmdData.Length;
                        byte[] packet = new byte[totallen];
                        packet[0] = 0x1C;
                        packet[1] = dst;
                        packet[2] = src;
                        packet[3] = (byte)(cmdData.Length);
                        packet[4] = op;
                        cmdData.CopyTo(packet, 5);

                        // Scan all commands
                        packet[5] = (byte)(scanValue & 0xFF);
                        packet[6] = (byte)(scanValue >> 8);

                        packet[packet.Length - 1] = ComputeChecksum(packet, 1, packet.Length - 1);

                        AppendPacketDataText(" --> " + ConvertByteArrayToHexString(packet, 1, 1) + " " + ConvertByteArrayToHexString(packet, 2, 1) + " " + ConvertByteArrayToHexString(packet, 4, 1) + " " + ConvertByteArrayToHexString(packet, 5, cmdData.Length));
                        AppendRawDataText(" --> " + ConvertByteArrayToHexString(packet, 0, packet.Length));

                        try
                        {
                            stream?.WriteAsync(packet, 0, packet.Length);
                        }
                        catch
                        {
                            Disconnect();
                            StopScan();
                        }

                        scanValue++;
                    });
                };
                scanTimer.Start();
                if (scanMenuItem != null)
                {
                    scanMenuItem.Active = true;
                }
            }
            else
            {
                StopScan();
            }
        }

        private void StopScan()
        {
            if (scanTimer != null)
            {
                scanTimer.Stop();
                scanTimer.Dispose();
                scanTimer = null;
            }
            if (scanMenuItem != null)
            {
                scanMenuItem.Active = false;
            }
        }

        private void OnFiqTest1Clicked(object? sender, EventArgs e)
        {
            var dialog = new Dialog("FIQ Test 1", this, DialogFlags.Modal);
            dialog.SetDefaultSize(300, 400);

            var contentArea = dialog.ContentArea;
            contentArea.Spacing = 5;
            contentArea.BorderWidth = 10;

            SpinButton[] spinButtons = new SpinButton[7];
            for (int i = 0; i < 7; i++)
            {
                var hbox = new Box(Orientation.Horizontal, 5);
                hbox.PackStart(new Label($"Value {i + 1}:"), false, false, 0);
                spinButtons[i] = new SpinButton(0, int.MaxValue, 1);
                spinButtons[i].Value = 0;
                hbox.PackStart(spinButtons[i], true, true, 0);
                contentArea.PackStart(hbox, false, false, 0);
            }

            dialog.AddButton("Close", ResponseType.Close);
            dialog.AddButton("Send", ResponseType.Accept);

            dialog.ShowAll();

            if (dialog.Run() == (int)ResponseType.Accept)
            {
                string r = "01 37 80 23D1";
                for (int i = 0; i < 7; i++)
                {
                    r += ((int)spinButtons[i].Value).ToString("X8");
                }
                hexComboBox.Entry.Text = r;
                SendPacket(r);
            }

            dialog.Destroy();
        }

        private void OnFiqTest2Clicked(object? sender, EventArgs e)
        {
            var dialog = new Dialog("FIQ Test 2", this, DialogFlags.Modal);
            dialog.SetDefaultSize(300, 450);

            var contentArea = dialog.ContentArea;
            contentArea.Spacing = 5;
            contentArea.BorderWidth = 10;

            SpinButton[] spinButtons = new SpinButton[9];
            for (int i = 0; i < 9; i++)
            {
                var hbox = new Box(Orientation.Horizontal, 5);
                hbox.PackStart(new Label($"Value {i + 1}:"), false, false, 0);
                spinButtons[i] = new SpinButton(0, int.MaxValue, 1);
                spinButtons[i].Value = 0;
                hbox.PackStart(spinButtons[i], true, true, 0);
                contentArea.PackStart(hbox, false, false, 0);
            }

            dialog.AddButton("Close", ResponseType.Close);
            dialog.AddButton("Send", ResponseType.Accept);

            dialog.ShowAll();

            if (dialog.Run() == (int)ResponseType.Accept)
            {
                string r = "01 37 80 23DC";
                for (int i = 0; i < 9; i++)
                {
                    r += ((int)spinButtons[i].Value).ToString("X8");
                }
                hexComboBox.Entry.Text = r;
                SendPacket(r);
            }

            dialog.Destroy();
        }
    }
}
