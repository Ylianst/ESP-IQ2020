using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AppKit;
using CoreGraphics;
using Foundation;

namespace DataViewerOSX
{
    public class MainWindowController : NSWindowController
    {
        // TCP Connection
        private TcpClient? client;
        private NetworkStream? stream;
        private byte[] readBuffer = new byte[2000];
        private int readBufferLen = 0;

        // File logging
        private StreamWriter? outputFile = null;
        private FileInfo? logFileInfo;

        // Filters
        private int Filter = 0;
        private int FilterCmd1 = 0;
        private int FilterCmd2 = 0;

        // Scanning
        private int scanValue = 0;
        private NSTimer? scanTimer;
        private NSTimer? pollTimer;

        // UI Elements
        private NSTextField addressTextField;
        private NSButton connectButton;
        private NSTextView dataPacketTextView;
        private NSTextView rawDataTextView;
        private NSTextView stateDecodeTextView;
        private NSComboBox hexComboBox;
        private NSButton sendButton;
        private NSTextField rawSendTextField;
        private NSButton rawSendButton;
        private NSTableView packetsTableView;
        private NSTableView dataTableView;
        
        private PacketsDataSource packetsDataSource;
        private DecodedDataSource decodedDataSource;
        
        private NSMenu actionsMenu;
        private NSMenu commandsMenu;

        public MainWindowController() : base()
        {
            var window = new NSWindow(
                new CGRect(0, 0, 900, 650),
                NSWindowStyle.Titled | NSWindowStyle.Closable | NSWindowStyle.Miniaturizable | NSWindowStyle.Resizable,
                NSBackingStore.Buffered,
                false)
            {
                Title = "IQ2020 Data Viewer"
            };

            window.Center();
            Window = window;

            // Initialize data sources first
            packetsDataSource = new PacketsDataSource();
            decodedDataSource = new DecodedDataSource();
            
            // Initialize UI elements
            addressTextField = new NSTextField();
            connectButton = new NSButton();
            dataPacketTextView = new NSTextView();
            rawDataTextView = new NSTextView();
            stateDecodeTextView = new NSTextView();
            hexComboBox = new NSComboBox();
            sendButton = new NSButton();
            rawSendTextField = new NSTextField();
            rawSendButton = new NSButton();
            packetsTableView = new NSTableView();
            dataTableView = new NSTableView();
            actionsMenu = new NSMenu();
            commandsMenu = new NSMenu();

            SetupUI();
            
            // Load saved address into text field
            string savedAddress = LoadSavedAddress();
            if (!string.IsNullOrEmpty(savedAddress))
            {
                addressTextField.StringValue = savedAddress;
            }
            
            UpdateInfo();
        }

        private void SetupUI()
        {
            var contentView = new NSView(Window.ContentView!.Frame);
            Window.ContentView = contentView;

            // Menu bar
            SetupMenuBar();

            // Address field and connect button - fixed to top
            var addressLabel = new NSTextField(new CGRect(10, Window.ContentView.Frame.Height - 30, 60, 22))
            {
                StringValue = "Address:",
                Editable = false,
                Bordered = false,
                BackgroundColor = NSColor.Clear,
                TextColor = NSColor.Text,
                AutoresizingMask = NSViewResizingMask.MinYMargin
            };
            contentView.AddSubview(addressLabel);

            addressTextField = new NSTextField(new CGRect(75, Window.ContentView.Frame.Height - 30, Window.ContentView.Frame.Width - 195, 22))
            {
                AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.MinYMargin
            };
            addressTextField.Changed += (s, e) => SaveAddress(addressTextField.StringValue);
            contentView.AddSubview(addressTextField);

            connectButton = new NSButton(new CGRect(Window.ContentView.Frame.Width - 110, Window.ContentView.Frame.Height - 30, 100, 22))
            {
                Title = "Connect",
                BezelStyle = NSBezelStyle.Rounded,
                AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MinYMargin
            };
            connectButton.Activated += OnConnectClicked;
            contentView.AddSubview(connectButton);

            // Tab view
            var tabView = new NSTabView(new CGRect(10, 10, Window.ContentView.Frame.Width - 20, Window.ContentView.Frame.Height - 50));
            tabView.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;

            // Tab 1: Data Packets
            var dataPacketsTab = new NSTabViewItem { Label = "Data Packets" };
            var dataPacketsView = new NSView(tabView.Frame);
            
            var dataPacketScrollView = new NSScrollView(new CGRect(0, 35, tabView.Frame.Width, tabView.Frame.Height - 40))
            {
                AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable,
                HasVerticalScroller = true,
                HasHorizontalScroller = true,
                BorderType = NSBorderType.BezelBorder
            };
            
            dataPacketTextView = new NSTextView(new CGRect(0, 0, dataPacketScrollView.ContentSize.Width, dataPacketScrollView.ContentSize.Height))
            {
                Editable = false,
                Font = NSFont.MonospacedSystemFont(11, NSFontWeight.Regular),
                AutoresizingMask = NSViewResizingMask.WidthSizable,
                TextColor = NSColor.Text,
                BackgroundColor = NSColor.TextBackground
            };
            dataPacketScrollView.DocumentView = dataPacketTextView;
            dataPacketsView.AddSubview(dataPacketScrollView);

            hexComboBox = new NSComboBox(new CGRect(5, 5, tabView.Frame.Width - 110, 22))
            {
                AutoresizingMask = NSViewResizingMask.WidthSizable,
                Font = NSFont.MonospacedSystemFont(11, NSFontWeight.Regular)
            };
            hexComboBox.Add(new NSString("29 99 40 1E010000FFFFFF00FF04FFFFFFFFFF"));
            hexComboBox.StringValue = "29 99 40 1E010000FFFFFF00FF04FFFFFFFFFF";
            dataPacketsView.AddSubview(hexComboBox);

            sendButton = new NSButton(new CGRect(tabView.Frame.Width - 100, 5, 95, 22))
            {
                Title = "Send",
                BezelStyle = NSBezelStyle.Rounded,
                Enabled = false,
                AutoresizingMask = NSViewResizingMask.MinXMargin
            };
            sendButton.Activated += OnSendClicked;
            dataPacketsView.AddSubview(sendButton);

            dataPacketsTab.View = dataPacketsView;
            tabView.Add(dataPacketsTab);

            // Tab 2: Raw Data
            var rawDataTab = new NSTabViewItem { Label = "Raw Data" };
            var rawDataView = new NSView(tabView.Frame);

            var rawDataScrollView = new NSScrollView(new CGRect(0, 35, tabView.Frame.Width, tabView.Frame.Height - 40))
            {
                AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable,
                HasVerticalScroller = true,
                HasHorizontalScroller = true,
                BorderType = NSBorderType.BezelBorder
            };

            rawDataTextView = new NSTextView(new CGRect(0, 0, rawDataScrollView.ContentSize.Width, rawDataScrollView.ContentSize.Height))
            {
                Editable = false,
                Font = NSFont.MonospacedSystemFont(11, NSFontWeight.Regular),
                AutoresizingMask = NSViewResizingMask.WidthSizable,
                TextColor = NSColor.Text,
                BackgroundColor = NSColor.TextBackground
            };
            rawDataScrollView.DocumentView = rawDataTextView;
            rawDataView.AddSubview(rawDataScrollView);

            rawSendTextField = new NSTextField(new CGRect(5, 5, tabView.Frame.Width - 110, 22))
            {
                AutoresizingMask = NSViewResizingMask.WidthSizable,
                Font = NSFont.MonospacedSystemFont(11, NSFontWeight.Regular),
                StringValue = "1C2101034021010078",
                Enabled = false
            };
            rawDataView.AddSubview(rawSendTextField);

            rawSendButton = new NSButton(new CGRect(tabView.Frame.Width - 100, 5, 95, 22))
            {
                Title = "Send",
                BezelStyle = NSBezelStyle.Rounded,
                Enabled = false,
                AutoresizingMask = NSViewResizingMask.MinXMargin
            };
            rawSendButton.Activated += OnRawSendClicked;
            rawDataView.AddSubview(rawSendButton);

            rawDataTab.View = rawDataView;
            tabView.Add(rawDataTab);

            // Tab 3: Packet Store
            var packetsTab = new NSTabViewItem { Label = "Packet Store" };
            var packetsScrollView = new NSScrollView(new CGRect(0, 0, tabView.Frame.Width, tabView.Frame.Height))
            {
                AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable,
                HasVerticalScroller = true,
                BorderType = NSBorderType.BezelBorder
            };

            packetsTableView = new NSTableView(packetsScrollView.Frame)
            {
                AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable,
                DataSource = packetsDataSource,
                Delegate = new TableViewDelegate()
            };

            var packetsColumn = new NSTableColumn("Packet")
            {
                Title = "Packet",
                Width = tabView.Frame.Width - 20,
                ResizingMask = NSTableColumnResizing.Autoresizing
            };
            packetsTableView.AddColumn(packetsColumn);
            packetsScrollView.DocumentView = packetsTableView;
            packetsTab.View = packetsScrollView;
            tabView.Add(packetsTab);

            // Tab 4: Decoded Data
            var decodedTab = new NSTabViewItem { Label = "Decoded Data" };
            var decodedScrollView = new NSScrollView(new CGRect(0, 0, tabView.Frame.Width, tabView.Frame.Height))
            {
                AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable,
                HasVerticalScroller = true,
                BorderType = NSBorderType.BezelBorder
            };

            dataTableView = new NSTableView(decodedScrollView.Frame)
            {
                AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable,
                DataSource = decodedDataSource,
                Delegate = new TableViewDelegate()
            };

            var nameColumn = new NSTableColumn("Name")
            {
                Title = "Name",
                Width = 200,
                ResizingMask = NSTableColumnResizing.Autoresizing
            };
            var valueColumn = new NSTableColumn("Value")
            {
                Title = "Value",
                Width = tabView.Frame.Width - 220,
                ResizingMask = NSTableColumnResizing.Autoresizing
            };
            dataTableView.AddColumn(nameColumn);
            dataTableView.AddColumn(valueColumn);
            decodedScrollView.DocumentView = dataTableView;
            decodedTab.View = decodedScrollView;
            tabView.Add(decodedTab);

            // Tab 5: State
            var stateTab = new NSTabViewItem { Label = "State" };
            var stateScrollView = new NSScrollView(new CGRect(0, 0, tabView.Frame.Width, tabView.Frame.Height))
            {
                AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable,
                HasVerticalScroller = true,
                HasHorizontalScroller = true,
                BorderType = NSBorderType.BezelBorder
            };

            stateDecodeTextView = new NSTextView(new CGRect(0, 0, stateScrollView.ContentSize.Width, stateScrollView.ContentSize.Height))
            {
                Editable = false,
                Font = NSFont.MonospacedSystemFont(11, NSFontWeight.Regular),
                AutoresizingMask = NSViewResizingMask.WidthSizable,
                TextColor = NSColor.Text,
                BackgroundColor = NSColor.TextBackground
            };
            stateScrollView.DocumentView = stateDecodeTextView;
            stateTab.View = stateScrollView;
            tabView.Add(stateTab);

            contentView.AddSubview(tabView);
        }

        private void SetupMenuBar()
        {
            var mainMenu = new NSMenu();

            // App menu
            var appMenuItem = new NSMenuItem();
            mainMenu.AddItem(appMenuItem);
            var appMenu = new NSMenu();
            appMenuItem.Submenu = appMenu;
            appMenu.AddItem(new NSMenuItem("Quit", "q", (s, e) => NSApplication.SharedApplication.Terminate(this)));

            // File menu
            var fileMenuItem = new NSMenuItem { Title = "File" };
            mainMenu.AddItem(fileMenuItem);
            var fileMenu = new NSMenu("File");
            fileMenu.AutoEnablesItems = false;
            fileMenuItem.Submenu = fileMenu;
            
            fileMenu.AddItem(new NSMenuItem("Log to File...", "", OnLogToFileClicked));
            fileMenu.AddItem(new NSMenuItem("Annotate Log...", "", OnAnnotateLogClicked) { Enabled = false, Tag = 100 });
            fileMenu.AddItem(NSMenuItem.SeparatorItem);
            fileMenu.AddItem(new NSMenuItem("Clear", "", OnClearClicked));

            // Actions menu
            var actionsMenuItem = new NSMenuItem { Title = "Actions" };
            mainMenu.AddItem(actionsMenuItem);
            actionsMenu = new NSMenu("Actions");
            actionsMenu.AutoEnablesItems = false;
            actionsMenuItem.Submenu = actionsMenu;
            
            actionsMenu.AddItem(new NSMenuItem("Poll State", "", OnPollStateClicked) { Tag = 101, Enabled = false });
            actionsMenu.AddItem(new NSMenuItem("Scan", "", OnScanClicked) { Tag = 102, Enabled = false });
            actionsMenu.AddItem(NSMenuItem.SeparatorItem);
            actionsMenu.AddItem(new NSMenuItem("FIQ Test 1", "", OnFiqTest1Clicked) { Enabled = false });
            actionsMenu.AddItem(new NSMenuItem("FIQ Test 2", "", OnFiqTest2Clicked) { Enabled = false });

            // Filter menu
            var filterMenuItem = new NSMenuItem { Title = "Filter" };
            mainMenu.AddItem(filterMenuItem);
            var filterMenu = new NSMenu("Filter");
            filterMenuItem.Submenu = filterMenu;

            filterMenu.AddItem(new NSMenuItem("None", "", (s, e) => OnFilterSelected(filterMenu, s, 0, 0, 0)) { State = NSCellStateValue.On });
            filterMenu.AddItem(new NSMenuItem("Connection Kit", "", (s, e) => OnFilterSelected(filterMenu, s, 0x1F, 0, 0)));
            filterMenu.AddItem(new NSMenuItem("Connection Kit 0x0256", "", (s, e) => OnFilterSelected(filterMenu, s, 0x1F, 0x02, 0x56)));
            filterMenu.AddItem(new NSMenuItem("Fresh Water", "", (s, e) => OnFilterSelected(filterMenu, s, 0x29, 0, 0)));
            filterMenu.AddItem(new NSMenuItem("Freshwater IQ Module", "", (s, e) => OnFilterSelected(filterMenu, s, 0x37, 0, 0)));
            filterMenu.AddItem(new NSMenuItem("Audio", "", (s, e) => OnFilterSelected(filterMenu, s, 0x33, 0, 0)));
            filterMenu.AddItem(new NSMenuItem("ACE Module", "", (s, e) => OnFilterSelected(filterMenu, s, 0x24, 0, 0)));

            // Commands menu
            var commandsMenuItem = new NSMenuItem { Title = "Commands" };
            mainMenu.AddItem(commandsMenuItem);
            commandsMenu = new NSMenu("Commands");
            commandsMenu.AutoEnablesItems = false;
            commandsMenuItem.Submenu = commandsMenu;

            commandsMenu.AddItem(new NSMenuItem("Ask Lights Status", "", (s, e) => SendPacket("01 1F 40 1705")) { Enabled = false });
            commandsMenu.AddItem(new NSMenuItem("Ask Status 0x0255", "", (s, e) => SendPacket("01 1F 40 0255")) { Enabled = false });
            commandsMenu.AddItem(new NSMenuItem("Ask Status 0x0256", "", (s, e) => SendPacket("01 1F 40 0256")) { Enabled = false });
            commandsMenu.AddItem(NSMenuItem.SeparatorItem);
            commandsMenu.AddItem(new NSMenuItem("Lights On", "", (s, e) => { SendPacket("01 1F 40 1702041100"); SendPacket("01 1F 40 1705"); }) { Enabled = false });
            commandsMenu.AddItem(new NSMenuItem("Lights Off", "", (s, e) => { SendPacket("01 1F 40 1702041000"); SendPacket("01 1F 40 1705"); }) { Enabled = false });
            commandsMenu.AddItem(NSMenuItem.SeparatorItem);
            commandsMenu.AddItem(new NSMenuItem("Ask Versions", "", (s, e) => SendPacket("01 1F 40 0100")) { Enabled = false });
            commandsMenu.AddItem(new NSMenuItem("Set Current Time", "", OnSetCurrentTimeClicked) { Enabled = false });

            NSApplication.SharedApplication.MainMenu = mainMenu;
        }

        private string LoadSavedAddress()
        {
            try
            {
                var configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".iq2020dataviewer");
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
                var configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".iq2020dataviewer");
                File.WriteAllText(configPath, address);
            }
            catch { }
        }

        private void OnClearClicked(object? sender, EventArgs e)
        {
            dataPacketTextView.Value = "";
            rawDataTextView.Value = "";
            stateDecodeTextView.Value = "";
            packetsDataSource.Clear();
            packetsTableView.ReloadData();
            decodedDataSource.Clear();
            dataTableView.ReloadData();
        }

        private void OnConnectClicked(object? sender, EventArgs e)
        {
            if (client == null)
            {
                string[] split = addressTextField.StringValue.Split(':');
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
                UpdateInfo();

                Task.Run(async () =>
                {
                    try
                    {
                        await client.ConnectAsync(host, port);
                        NSApplication.SharedApplication.InvokeOnMainThread(OnConnected);
                    }
                    catch (Exception ex)
                    {
                        NSApplication.SharedApplication.InvokeOnMainThread(() =>
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
                            NSApplication.SharedApplication.InvokeOnMainThread(Disconnect);
                            break;
                        }

                        NSApplication.SharedApplication.InvokeOnMainThread(() => 
                            AppendRawDataText(" <-- " + ConvertByteArrayToHexString(readBuffer, readBufferLen, len)));
                        readBufferLen += len;

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
                    NSApplication.SharedApplication.InvokeOnMainThread(() =>
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
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            connectButton.Title = (stream == null) ? "Connect" : "Disconnect";
            addressTextField.Enabled = (stream == null);
            hexComboBox.Enabled = sendButton.Enabled = (stream != null);
            rawSendTextField.Enabled = rawSendButton.Enabled = (stream != null);

            // Enable/disable menu items based on connection state
            bool isConnected = (stream != null);
            
            // Enable/disable all Actions menu items (skip separators)
            for (nint i = 0; i < actionsMenu.Count; i++)
            {
                var item = actionsMenu.ItemAt(i);
                if (item != null && !item.IsSeparatorItem)
                {
                    item.Enabled = isConnected;
                }
            }
            
            // Enable/disable all Commands menu items (skip separators)
            for (nint i = 0; i < commandsMenu.Count; i++)
            {
                var item = commandsMenu.ItemAt(i);
                if (item != null && !item.IsSeparatorItem)
                {
                    item.Enabled = isConnected;
                }
            }

            // Update File menu items
            var fileMenu = NSApplication.SharedApplication.MainMenu?.ItemAt(1)?.Submenu;
            var annotateItem = fileMenu?.ItemWithTag(100);
            if (annotateItem != null)
            {
                annotateItem.Enabled = (outputFile != null);
            }
        }

        private void AppendRawDataText(string msg)
        {
            var storage = rawDataTextView.TextStorage;
            if (storage != null)
            {
                storage.Append(new NSAttributedString(msg + "\n", new NSStringAttributes
                {
                    Font = NSFont.MonospacedSystemFont(11, NSFontWeight.Regular),
                    ForegroundColor = NSColor.Text
                }));
                rawDataTextView.ScrollRangeToVisible(new NSRange(storage.Length, 0));
            }
        }

        private void AppendPacketDataText(string msg)
        {
            var storage = dataPacketTextView.TextStorage;
            if (storage != null)
            {
                storage.Append(new NSAttributedString(msg + "\n", new NSStringAttributes
                {
                    Font = NSFont.MonospacedSystemFont(11, NSFontWeight.Regular),
                    ForegroundColor = NSColor.Text
                }));
                dataPacketTextView.ScrollRangeToVisible(new NSRange(storage.Length, 0));
            }

            if (outputFile != null)
            {
                outputFile.WriteLine(DateTime.Now.ToString() + " " + msg);
                outputFile.Flush();
            }
        }

        private void SetStateDecoded(string value)
        {
            stateDecodeTextView.Value = value;
        }

        private void AddPacketToStore(string packet)
        {
            if (!packetsDataSource.Contains(packet))
            {
                packetsDataSource.Add(packet);
                packetsTableView.ReloadData();
            }
        }

        private void AddDecodedData(string name, string value)
        {
            decodedDataSource.AddOrUpdate(name, value);
            dataTableView.ReloadData();
        }

        private int ProcessData(byte[] data, int len)
        {
            if (len < 5) return 0;

            if ((data[0] != 0x1c) && ((data[1] != 0x01) || (data[2] != 0x01)) && ((data[4] != 0x40) || (data[4] != 0x80)))
            {
                NSApplication.SharedApplication.InvokeOnMainThread(() =>
                    AppendPacketDataText("Out of sync: " + ConvertByteArrayToHexString(data, 0, len)));
                for (var i = 1; i < len; i++) { if (data[i] == 0x1c) { return i; } }
                return len;
            }

            int datalen = data[3];
            int totallen = datalen + 6;
            if (len < totallen) return 0;

            byte checksum = ComputeChecksum(data, 1, datalen + 5);
            if (checksum != data[datalen + 5])
            {
                NSApplication.SharedApplication.InvokeOnMainThread(() =>
                    AppendPacketDataText("Invalid checksum: " + ConvertByteArrayToHexString(data, 0, len)));
                for (var i = 1; i < len; i++) { if (data[i] == 0x1c) { return i; } }
                return len;
            }

            if ((Filter != 0) && (data[1] != Filter) && (data[2] != Filter)) return totallen;
            if ((FilterCmd1 != 0) && ((datalen < 2) || (data[5] != FilterCmd1))) return totallen;
            if ((FilterCmd2 != 0) && ((datalen < 2) || (data[6] != FilterCmd2))) return totallen;

            try
            {
                string t = ConvertByteArrayToHexString(data, 1, 1) + " " + ConvertByteArrayToHexString(data, 2, 1) + " " + ConvertByteArrayToHexString(data, 4, 1) + " " + ConvertByteArrayToHexString(data, 5, datalen);
                NSApplication.SharedApplication.InvokeOnMainThread(() =>
                {
                    AddPacketToStore(t);
                    AppendPacketDataText(" <-- " + t);
                });

                if ((data[1] == 0x1F) && (data[2] == 0x01) && (data[4] == 0x80))
                {
                    if ((data[5] == 0x17) && (data[6] == 0x05))
                    {
                        NSApplication.SharedApplication.InvokeOnMainThread(() =>
                            AddDecodedData("Lights", (data[24] == 0x01) ? "Enabled" : "Disabled"));
                    }

                    if ((data[5] == 0x02) && (data[6] == 0x56) && datalen > 100)
                    {
                        NSApplication.SharedApplication.InvokeOnMainThread(() =>
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
                            int month = data[135] + 1;
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
                        });
                    }

                    if ((data[5] == 0x01) && (data[6] == 0x0))
                    {
                        NSApplication.SharedApplication.InvokeOnMainThread(() =>
                            AddDecodedData("Version", Encoding.UTF8.GetString(data, 7, datalen - 3)));
                    }
                }
            }
            catch (Exception ex)
            {
                NSApplication.SharedApplication.InvokeOnMainThread(() =>
                    AppendPacketDataText(ex.ToString()));
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
            SendPacket(hexComboBox.StringValue);
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
            bool found = false;
            for (nint i = 0; i < hexComboBox.Count; i++)
            {
                if (hexComboBox.GetItemObject(i).ToString() == hexComboBox.StringValue)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                hexComboBox.Add(new NSString(hexComboBox.StringValue));
            }
        }

        private void OnRawSendClicked(object? sender, EventArgs e)
        {
            if (stream == null) return;

            string t = rawSendTextField.StringValue;
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
                var savePanel = NSSavePanel.SavePanel;
                savePanel.Title = "Save Log File";
                savePanel.NameFieldStringValue = "logfile.txt";

                if (savePanel.RunModal() == 1 && savePanel.Url?.Path != null)
                {
                    logFileInfo = new FileInfo(savePanel.Url.Path);
                    outputFile = new StreamWriter(savePanel.Url.Path, true);
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

        private void OnAnnotateLogClicked(object? sender, EventArgs e)
        {
            if (outputFile == null) return;

            var alert = new NSAlert
            {
                MessageText = "Annotate Log",
                InformativeText = "Enter annotation:",
                AlertStyle = NSAlertStyle.Informational
            };

            var input = new NSTextField(new CGRect(0, 0, 300, 24));
            alert.AccessoryView = input;
            alert.AddButton("OK");
            alert.AddButton("Cancel");
            alert.Window.InitialFirstResponder = input;

            if (alert.RunModal() == 1000 && input.StringValue.Length > 0)
            {
                AppendPacketDataText(" --- " + input.StringValue);
            }
        }

        private void OnPollStateClicked(object? sender, EventArgs e)
        {
            if (pollTimer == null)
            {
                pollTimer = NSTimer.CreateRepeatingScheduledTimer(2.0, (timer) =>
                {
                    SendPacket("01 1F 40 0256");
                });
            }
            else
            {
                pollTimer.Invalidate();
                pollTimer = null;
            }
        }

        private void OnScanClicked(object? sender, EventArgs e)
        {
            if (scanTimer == null)
            {
                scanValue = 0;
                scanTimer = NSTimer.CreateRepeatingScheduledTimer(0.1, (timer) =>
                {
                    if (scanValue > 65535 || stream == null)
                    {
                        scanTimer?.Invalidate();
                        scanTimer = null;
                        return;
                    }

                    string t = hexComboBox.StringValue;
                    t = t.Replace("<", "").Replace("-", "").Replace(":", "").Replace(" ", "").Replace(",", "");
                    byte[] raw = ConvertHexStringToByteArray(t);
                    if (raw.Length < 4)
                    {
                        scanTimer?.Invalidate();
                        scanTimer = null;
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
                        scanTimer?.Invalidate();
                        scanTimer = null;
                    }

                    scanValue++;
                });
            }
            else
            {
                scanTimer.Invalidate();
                scanTimer = null;
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
            b[4] = (byte)(n.Month - 1);
            b[5] = (byte)(n.Year & 0xFF);
            b[6] = (byte)(n.Year >> 8);
            SendPacket("01 1F 40 024C " + ConvertByteArrayToHexString(b, 0, 7));
        }

        private void OnFiqTest1Clicked(object? sender, EventArgs e)
        {
            var alert = new NSAlert
            {
                MessageText = "FIQ Test 1",
                InformativeText = "Enter 7 values:",
                AlertStyle = NSAlertStyle.Informational
            };

            var stackView = new NSStackView(new CGRect(0, 0, 300, 200))
            {
                Orientation = NSUserInterfaceLayoutOrientation.Vertical,
                Spacing = 5
            };

            NSTextField[] textFields = new NSTextField[7];
            for (int i = 0; i < 7; i++)
            {
                var hStack = new NSStackView
                {
                    Orientation = NSUserInterfaceLayoutOrientation.Horizontal,
                    Spacing = 5
                };
                hStack.AddView(new NSTextField { StringValue = $"Value {i + 1}:", Editable = false, Bordered = false, BackgroundColor = NSColor.Clear }, NSStackViewGravity.Leading);
                textFields[i] = new NSTextField { StringValue = "0" };
                hStack.AddView(textFields[i], NSStackViewGravity.Trailing);
                stackView.AddView(hStack, NSStackViewGravity.Leading);
            }

            alert.AccessoryView = stackView;
            alert.AddButton("Send");
            alert.AddButton("Close");

            if (alert.RunModal() == 1000)
            {
                string r = "01 37 80 23D1";
                for (int i = 0; i < 7; i++)
                {
                    if (int.TryParse(textFields[i].StringValue, out int val))
                    {
                        r += val.ToString("X8");
                    }
                }
                hexComboBox.StringValue = r;
                SendPacket(r);
            }
        }

        private void OnFilterSelected(NSMenu filterMenu, object? sender, int filter, int cmd1, int cmd2)
        {
            Filter = filter;
            FilterCmd1 = cmd1;
            FilterCmd2 = cmd2;

            // Update menu item states - uncheck all, then check the selected one
            for (nint i = 0; i < filterMenu.Count; i++)
            {
                var item = filterMenu.ItemAt(i);
                if (item != null)
                {
                    item.State = (item == sender) ? NSCellStateValue.On : NSCellStateValue.Off;
                }
            }
        }

        private void OnFiqTest2Clicked(object? sender, EventArgs e)
        {
            var alert = new NSAlert
            {
                MessageText = "FIQ Test 2",
                InformativeText = "Enter 9 values:",
                AlertStyle = NSAlertStyle.Informational
            };

            var stackView = new NSStackView(new CGRect(0, 0, 300, 250))
            {
                Orientation = NSUserInterfaceLayoutOrientation.Vertical,
                Spacing = 5
            };

            NSTextField[] textFields = new NSTextField[9];
            for (int i = 0; i < 9; i++)
            {
                var hStack = new NSStackView
                {
                    Orientation = NSUserInterfaceLayoutOrientation.Horizontal,
                    Spacing = 5
                };
                hStack.AddView(new NSTextField { StringValue = $"Value {i + 1}:", Editable = false, Bordered = false, BackgroundColor = NSColor.Clear }, NSStackViewGravity.Leading);
                textFields[i] = new NSTextField { StringValue = "0" };
                hStack.AddView(textFields[i], NSStackViewGravity.Trailing);
                stackView.AddView(hStack, NSStackViewGravity.Leading);
            }

            alert.AccessoryView = stackView;
            alert.AddButton("Send");
            alert.AddButton("Close");

            if (alert.RunModal() == 1000)
            {
                string r = "01 37 80 23DC";
                for (int i = 0; i < 9; i++)
                {
                    if (int.TryParse(textFields[i].StringValue, out int val))
                    {
                        r += val.ToString("X8");
                    }
                }
                hexComboBox.StringValue = r;
                SendPacket(r);
            }
        }

    }

    // Data sources for table views
    public class PacketsDataSource : NSTableViewDataSource
    {
        private List<string> packets = new List<string>();

        public void Add(string packet)
        {
            packets.Add(packet);
        }

        public bool Contains(string packet)
        {
            return packets.Contains(packet);
        }

        public void Clear()
        {
            packets.Clear();
        }

        public override nint GetRowCount(NSTableView tableView)
        {
            return packets.Count;
        }

        public override NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            return new NSString(packets[(int)row]);
        }
    }

    public class DecodedDataSource : NSTableViewDataSource
    {
        private Dictionary<string, string> data = new Dictionary<string, string>();
        private List<string> keys = new List<string>();

        public void AddOrUpdate(string name, string value)
        {
            if (!data.ContainsKey(name))
            {
                keys.Add(name);
            }
            data[name] = value;
        }

        public void Clear()
        {
            data.Clear();
            keys.Clear();
        }

        public override nint GetRowCount(NSTableView tableView)
        {
            return keys.Count;
        }

        public override NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            string key = keys[(int)row];
            if (tableColumn.Identifier == "Name")
            {
                return new NSString(key);
            }
            else
            {
                return new NSString(data[key]);
            }
        }
    }

    public class TableViewDelegate : NSTableViewDelegate
    {
        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            var cellIdentifier = tableColumn.Identifier;
            var view = (NSTextField?)tableView.MakeView(cellIdentifier, this);

            if (view == null)
            {
                view = new NSTextField
                {
                    Identifier = cellIdentifier,
                    BackgroundColor = NSColor.Clear,
                    Bordered = false,
                    Editable = false,
                    Font = NSFont.MonospacedSystemFont(11, NSFontWeight.Regular)
                };
            }

            var dataSource = tableView.DataSource;
            if (dataSource != null)
            {
                var obj = dataSource.GetObjectValue(tableView, tableColumn, row);
                view.StringValue = obj?.ToString() ?? "";
            }

            return view;
        }
    }
}
