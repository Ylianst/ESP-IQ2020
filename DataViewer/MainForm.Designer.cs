namespace DataViewer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.annotateLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pollStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionKitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionKitX0256ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.freshWaterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.audioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commandsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.askLightsStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.askTempStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.askVersionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lightsOnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lightsOffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setCurrentTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.mainToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.topPanel = new System.Windows.Forms.Panel();
            this.connectButton = new System.Windows.Forms.Button();
            this.addressTextBox = new System.Windows.Forms.TextBox();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.hexComboBox = new System.Windows.Forms.ComboBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.mainDataPacketTextBox = new System.Windows.Forms.TextBox();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.dataPacketsTabPage = new System.Windows.Forms.TabPage();
            this.rawDataTabPage = new System.Windows.Forms.TabPage();
            this.mainRawDataTextBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rawSendButton = new System.Windows.Forms.Button();
            this.rawSendTextBox = new System.Windows.Forms.TextBox();
            this.storeTabPage = new System.Windows.Forms.TabPage();
            this.packetsListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dataTabPage = new System.Windows.Forms.TabPage();
            this.dataListView = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.stateDecodeTabPage = new System.Windows.Forms.TabPage();
            this.stateDecodeTextBox = new System.Windows.Forms.TextBox();
            this.scanTimer = new System.Windows.Forms.Timer(this.components);
            this.logSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.pollTimer = new System.Windows.Forms.Timer(this.components);
            this.aCEModuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.mainTabControl.SuspendLayout();
            this.dataPacketsTabPage.SuspendLayout();
            this.rawDataTabPage.SuspendLayout();
            this.panel1.SuspendLayout();
            this.storeTabPage.SuspendLayout();
            this.dataTabPage.SuspendLayout();
            this.stateDecodeTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.actionsToolStripMenuItem,
            this.filterToolStripMenuItem,
            this.commandsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(704, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logToFileToolStripMenuItem,
            this.toolStripMenuItem2,
            this.clearToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // logToFileToolStripMenuItem
            // 
            this.logToFileToolStripMenuItem.Name = "logToFileToolStripMenuItem";
            this.logToFileToolStripMenuItem.Size = new System.Drawing.Size(163, 26);
            this.logToFileToolStripMenuItem.Text = "Log to &File...";
            this.logToFileToolStripMenuItem.Click += new System.EventHandler(this.logToFileToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(160, 6);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(163, 26);
            this.clearToolStripMenuItem.Text = "C&lear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(160, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(163, 26);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.annotateLogToolStripMenuItem,
            this.scanToolStripMenuItem,
            this.pollStateToolStripMenuItem});
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            this.actionsToolStripMenuItem.Size = new System.Drawing.Size(70, 24);
            this.actionsToolStripMenuItem.Text = "&Actions";
            // 
            // annotateLogToolStripMenuItem
            // 
            this.annotateLogToolStripMenuItem.Name = "annotateLogToolStripMenuItem";
            this.annotateLogToolStripMenuItem.Size = new System.Drawing.Size(183, 26);
            this.annotateLogToolStripMenuItem.Text = "&Annotate Log...";
            this.annotateLogToolStripMenuItem.Click += new System.EventHandler(this.annotateLogToolStripMenuItem_Click);
            // 
            // scanToolStripMenuItem
            // 
            this.scanToolStripMenuItem.Name = "scanToolStripMenuItem";
            this.scanToolStripMenuItem.Size = new System.Drawing.Size(183, 26);
            this.scanToolStripMenuItem.Text = "&Scan";
            this.scanToolStripMenuItem.Click += new System.EventHandler(this.scanToolStripMenuItem_Click);
            // 
            // pollStateToolStripMenuItem
            // 
            this.pollStateToolStripMenuItem.Name = "pollStateToolStripMenuItem";
            this.pollStateToolStripMenuItem.Size = new System.Drawing.Size(183, 26);
            this.pollStateToolStripMenuItem.Text = "&Poll State";
            this.pollStateToolStripMenuItem.Click += new System.EventHandler(this.pollStateToolStripMenuItem_Click);
            // 
            // filterToolStripMenuItem
            // 
            this.filterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem,
            this.connectionKitToolStripMenuItem,
            this.connectionKitX0256ToolStripMenuItem,
            this.freshWaterToolStripMenuItem,
            this.aCEModuleToolStripMenuItem,
            this.audioToolStripMenuItem});
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.Size = new System.Drawing.Size(54, 24);
            this.filterToolStripMenuItem.Text = "F&ilter";
            this.filterToolStripMenuItem.Click += new System.EventHandler(this.filterToolStripMenuItem_Click);
            // 
            // noneToolStripMenuItem
            // 
            this.noneToolStripMenuItem.Checked = true;
            this.noneToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.noneToolStripMenuItem.Name = "noneToolStripMenuItem";
            this.noneToolStripMenuItem.Size = new System.Drawing.Size(232, 26);
            this.noneToolStripMenuItem.Text = "None";
            this.noneToolStripMenuItem.Click += new System.EventHandler(this.noneToolStripMenuItem_Click);
            // 
            // connectionKitToolStripMenuItem
            // 
            this.connectionKitToolStripMenuItem.Name = "connectionKitToolStripMenuItem";
            this.connectionKitToolStripMenuItem.Size = new System.Drawing.Size(232, 26);
            this.connectionKitToolStripMenuItem.Text = "Connection Kit";
            this.connectionKitToolStripMenuItem.Click += new System.EventHandler(this.connectionKitToolStripMenuItem_Click);
            // 
            // connectionKitX0256ToolStripMenuItem
            // 
            this.connectionKitX0256ToolStripMenuItem.Name = "connectionKitX0256ToolStripMenuItem";
            this.connectionKitX0256ToolStripMenuItem.Size = new System.Drawing.Size(232, 26);
            this.connectionKitX0256ToolStripMenuItem.Text = "Connection Kit 0x0256";
            this.connectionKitX0256ToolStripMenuItem.Click += new System.EventHandler(this.connectionKitX0256ToolStripMenuItem_Click);
            // 
            // freshWaterToolStripMenuItem
            // 
            this.freshWaterToolStripMenuItem.Name = "freshWaterToolStripMenuItem";
            this.freshWaterToolStripMenuItem.Size = new System.Drawing.Size(232, 26);
            this.freshWaterToolStripMenuItem.Text = "Freshwater Module";
            this.freshWaterToolStripMenuItem.Click += new System.EventHandler(this.freshWaterToolStripMenuItem_Click);
            // 
            // audioToolStripMenuItem
            // 
            this.audioToolStripMenuItem.Name = "audioToolStripMenuItem";
            this.audioToolStripMenuItem.Size = new System.Drawing.Size(232, 26);
            this.audioToolStripMenuItem.Text = "Audio Module";
            this.audioToolStripMenuItem.Click += new System.EventHandler(this.audioToolStripMenuItem_Click);
            // 
            // commandsToolStripMenuItem
            // 
            this.commandsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.askLightsStatusToolStripMenuItem,
            this.toolStripMenuItem3,
            this.askTempStatusToolStripMenuItem,
            this.askVersionsToolStripMenuItem,
            this.lightsOnToolStripMenuItem,
            this.lightsOffToolStripMenuItem,
            this.setCurrentTimeToolStripMenuItem});
            this.commandsToolStripMenuItem.Name = "commandsToolStripMenuItem";
            this.commandsToolStripMenuItem.Size = new System.Drawing.Size(96, 24);
            this.commandsToolStripMenuItem.Text = "&Commands";
            // 
            // askLightsStatusToolStripMenuItem
            // 
            this.askLightsStatusToolStripMenuItem.Name = "askLightsStatusToolStripMenuItem";
            this.askLightsStatusToolStripMenuItem.Size = new System.Drawing.Size(202, 26);
            this.askLightsStatusToolStripMenuItem.Text = "Ask Lights Status";
            this.askLightsStatusToolStripMenuItem.Click += new System.EventHandler(this.askLightsStatusToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(202, 26);
            this.toolStripMenuItem3.Text = "Ask Status 0x0255";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // askTempStatusToolStripMenuItem
            // 
            this.askTempStatusToolStripMenuItem.Name = "askTempStatusToolStripMenuItem";
            this.askTempStatusToolStripMenuItem.Size = new System.Drawing.Size(202, 26);
            this.askTempStatusToolStripMenuItem.Text = "Ask Status 0x0256";
            this.askTempStatusToolStripMenuItem.Click += new System.EventHandler(this.askTempStatusToolStripMenuItem_Click);
            // 
            // askVersionsToolStripMenuItem
            // 
            this.askVersionsToolStripMenuItem.Name = "askVersionsToolStripMenuItem";
            this.askVersionsToolStripMenuItem.Size = new System.Drawing.Size(202, 26);
            this.askVersionsToolStripMenuItem.Text = "Ask Version";
            this.askVersionsToolStripMenuItem.Click += new System.EventHandler(this.askVersionsToolStripMenuItem_Click);
            // 
            // lightsOnToolStripMenuItem
            // 
            this.lightsOnToolStripMenuItem.Name = "lightsOnToolStripMenuItem";
            this.lightsOnToolStripMenuItem.Size = new System.Drawing.Size(202, 26);
            this.lightsOnToolStripMenuItem.Text = "Lights On";
            this.lightsOnToolStripMenuItem.Click += new System.EventHandler(this.lightsOnToolStripMenuItem_Click);
            // 
            // lightsOffToolStripMenuItem
            // 
            this.lightsOffToolStripMenuItem.Name = "lightsOffToolStripMenuItem";
            this.lightsOffToolStripMenuItem.Size = new System.Drawing.Size(202, 26);
            this.lightsOffToolStripMenuItem.Text = "Lights Off";
            this.lightsOffToolStripMenuItem.Click += new System.EventHandler(this.lightsOffToolStripMenuItem_Click);
            // 
            // setCurrentTimeToolStripMenuItem
            // 
            this.setCurrentTimeToolStripMenuItem.Name = "setCurrentTimeToolStripMenuItem";
            this.setCurrentTimeToolStripMenuItem.Size = new System.Drawing.Size(202, 26);
            this.setCurrentTimeToolStripMenuItem.Text = "Set Current Time";
            this.setCurrentTimeToolStripMenuItem.Click += new System.EventHandler(this.setCurrentTimeToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainToolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 513);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(704, 25);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // mainToolStripStatusLabel
            // 
            this.mainToolStripStatusLabel.Name = "mainToolStripStatusLabel";
            this.mainToolStripStatusLabel.Size = new System.Drawing.Size(18, 20);
            this.mainToolStripStatusLabel.Text = "...";
            // 
            // topPanel
            // 
            this.topPanel.Controls.Add(this.connectButton);
            this.topPanel.Controls.Add(this.addressTextBox);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 28);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(704, 50);
            this.topPanel.TabIndex = 2;
            // 
            // connectButton
            // 
            this.connectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.connectButton.Location = new System.Drawing.Point(591, 10);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(101, 30);
            this.connectButton.TabIndex = 1;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // addressTextBox
            // 
            this.addressTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addressTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addressTextBox.Location = new System.Drawing.Point(12, 10);
            this.addressTextBox.Name = "addressTextBox";
            this.addressTextBox.Size = new System.Drawing.Size(573, 30);
            this.addressTextBox.TabIndex = 0;
            this.addressTextBox.Text = "192.168.2.184:1234";
            this.addressTextBox.TextChanged += new System.EventHandler(this.addressTextBox_TextChanged);
            // 
            // bottomPanel
            // 
            this.bottomPanel.Controls.Add(this.hexComboBox);
            this.bottomPanel.Controls.Add(this.sendButton);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(3, 358);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(690, 45);
            this.bottomPanel.TabIndex = 3;
            // 
            // hexComboBox
            // 
            this.hexComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexComboBox.Font = new System.Drawing.Font("Courier New", 12F);
            this.hexComboBox.FormattingEnabled = true;
            this.hexComboBox.Location = new System.Drawing.Point(5, 7);
            this.hexComboBox.Name = "hexComboBox";
            this.hexComboBox.Size = new System.Drawing.Size(573, 30);
            this.hexComboBox.TabIndex = 2;
            this.hexComboBox.Text = "29 99 40 1E010000FFFFFF00FF04FFFFFFFFFF";
            // 
            // sendButton
            // 
            this.sendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sendButton.Location = new System.Drawing.Point(584, 7);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(101, 30);
            this.sendButton.TabIndex = 1;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // mainDataPacketTextBox
            // 
            this.mainDataPacketTextBox.BackColor = System.Drawing.Color.PapayaWhip;
            this.mainDataPacketTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainDataPacketTextBox.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainDataPacketTextBox.Location = new System.Drawing.Point(3, 3);
            this.mainDataPacketTextBox.Multiline = true;
            this.mainDataPacketTextBox.Name = "mainDataPacketTextBox";
            this.mainDataPacketTextBox.ReadOnly = true;
            this.mainDataPacketTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.mainDataPacketTextBox.Size = new System.Drawing.Size(690, 355);
            this.mainDataPacketTextBox.TabIndex = 4;
            this.mainDataPacketTextBox.WordWrap = false;
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.dataPacketsTabPage);
            this.mainTabControl.Controls.Add(this.rawDataTabPage);
            this.mainTabControl.Controls.Add(this.storeTabPage);
            this.mainTabControl.Controls.Add(this.dataTabPage);
            this.mainTabControl.Controls.Add(this.stateDecodeTabPage);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 78);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(704, 435);
            this.mainTabControl.TabIndex = 5;
            // 
            // dataPacketsTabPage
            // 
            this.dataPacketsTabPage.Controls.Add(this.mainDataPacketTextBox);
            this.dataPacketsTabPage.Controls.Add(this.bottomPanel);
            this.dataPacketsTabPage.Location = new System.Drawing.Point(4, 25);
            this.dataPacketsTabPage.Name = "dataPacketsTabPage";
            this.dataPacketsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.dataPacketsTabPage.Size = new System.Drawing.Size(696, 406);
            this.dataPacketsTabPage.TabIndex = 0;
            this.dataPacketsTabPage.Text = "Data Packets";
            this.dataPacketsTabPage.UseVisualStyleBackColor = true;
            // 
            // rawDataTabPage
            // 
            this.rawDataTabPage.Controls.Add(this.mainRawDataTextBox);
            this.rawDataTabPage.Controls.Add(this.panel1);
            this.rawDataTabPage.Location = new System.Drawing.Point(4, 25);
            this.rawDataTabPage.Name = "rawDataTabPage";
            this.rawDataTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.rawDataTabPage.Size = new System.Drawing.Size(696, 406);
            this.rawDataTabPage.TabIndex = 1;
            this.rawDataTabPage.Text = "Raw Data";
            this.rawDataTabPage.UseVisualStyleBackColor = true;
            // 
            // mainRawDataTextBox
            // 
            this.mainRawDataTextBox.BackColor = System.Drawing.Color.PapayaWhip;
            this.mainRawDataTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainRawDataTextBox.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainRawDataTextBox.Location = new System.Drawing.Point(3, 3);
            this.mainRawDataTextBox.Multiline = true;
            this.mainRawDataTextBox.Name = "mainRawDataTextBox";
            this.mainRawDataTextBox.ReadOnly = true;
            this.mainRawDataTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.mainRawDataTextBox.Size = new System.Drawing.Size(690, 355);
            this.mainRawDataTextBox.TabIndex = 6;
            this.mainRawDataTextBox.WordWrap = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rawSendButton);
            this.panel1.Controls.Add(this.rawSendTextBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 358);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(690, 45);
            this.panel1.TabIndex = 5;
            // 
            // rawSendButton
            // 
            this.rawSendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rawSendButton.Location = new System.Drawing.Point(577, 9);
            this.rawSendButton.Name = "rawSendButton";
            this.rawSendButton.Size = new System.Drawing.Size(101, 30);
            this.rawSendButton.TabIndex = 1;
            this.rawSendButton.Text = "Send";
            this.rawSendButton.UseVisualStyleBackColor = true;
            this.rawSendButton.Click += new System.EventHandler(this.rawSendButton_Click);
            // 
            // rawSendTextBox
            // 
            this.rawSendTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rawSendTextBox.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rawSendTextBox.Location = new System.Drawing.Point(5, 9);
            this.rawSendTextBox.Name = "rawSendTextBox";
            this.rawSendTextBox.Size = new System.Drawing.Size(559, 30);
            this.rawSendTextBox.TabIndex = 0;
            this.rawSendTextBox.Text = "1C2101034021010078";
            // 
            // storeTabPage
            // 
            this.storeTabPage.Controls.Add(this.packetsListView);
            this.storeTabPage.Location = new System.Drawing.Point(4, 25);
            this.storeTabPage.Name = "storeTabPage";
            this.storeTabPage.Size = new System.Drawing.Size(696, 406);
            this.storeTabPage.TabIndex = 2;
            this.storeTabPage.Text = "Packet Store";
            this.storeTabPage.UseVisualStyleBackColor = true;
            // 
            // packetsListView
            // 
            this.packetsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.packetsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.packetsListView.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.packetsListView.FullRowSelect = true;
            this.packetsListView.GridLines = true;
            this.packetsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.packetsListView.Location = new System.Drawing.Point(0, 0);
            this.packetsListView.Name = "packetsListView";
            this.packetsListView.Size = new System.Drawing.Size(696, 406);
            this.packetsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.packetsListView.TabIndex = 0;
            this.packetsListView.UseCompatibleStateImageBehavior = false;
            this.packetsListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Packet";
            this.columnHeader1.Width = 651;
            // 
            // dataTabPage
            // 
            this.dataTabPage.Controls.Add(this.dataListView);
            this.dataTabPage.Location = new System.Drawing.Point(4, 25);
            this.dataTabPage.Name = "dataTabPage";
            this.dataTabPage.Size = new System.Drawing.Size(696, 406);
            this.dataTabPage.TabIndex = 3;
            this.dataTabPage.Text = "Decoded Data";
            this.dataTabPage.UseVisualStyleBackColor = true;
            // 
            // dataListView
            // 
            this.dataListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader3});
            this.dataListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataListView.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataListView.FullRowSelect = true;
            this.dataListView.GridLines = true;
            this.dataListView.Location = new System.Drawing.Point(0, 0);
            this.dataListView.Name = "dataListView";
            this.dataListView.Size = new System.Drawing.Size(696, 406);
            this.dataListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.dataListView.TabIndex = 1;
            this.dataListView.UseCompatibleStateImageBehavior = false;
            this.dataListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 321;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Value";
            this.columnHeader3.Width = 336;
            // 
            // stateDecodeTabPage
            // 
            this.stateDecodeTabPage.Controls.Add(this.stateDecodeTextBox);
            this.stateDecodeTabPage.Location = new System.Drawing.Point(4, 25);
            this.stateDecodeTabPage.Name = "stateDecodeTabPage";
            this.stateDecodeTabPage.Size = new System.Drawing.Size(696, 406);
            this.stateDecodeTabPage.TabIndex = 4;
            this.stateDecodeTabPage.Text = "State";
            this.stateDecodeTabPage.UseVisualStyleBackColor = true;
            // 
            // stateDecodeTextBox
            // 
            this.stateDecodeTextBox.BackColor = System.Drawing.Color.PapayaWhip;
            this.stateDecodeTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stateDecodeTextBox.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stateDecodeTextBox.Location = new System.Drawing.Point(0, 0);
            this.stateDecodeTextBox.Multiline = true;
            this.stateDecodeTextBox.Name = "stateDecodeTextBox";
            this.stateDecodeTextBox.ReadOnly = true;
            this.stateDecodeTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.stateDecodeTextBox.Size = new System.Drawing.Size(696, 406);
            this.stateDecodeTextBox.TabIndex = 7;
            this.stateDecodeTextBox.WordWrap = false;
            // 
            // scanTimer
            // 
            this.scanTimer.Interval = 300;
            this.scanTimer.Tick += new System.EventHandler(this.scanTimer_Tick);
            // 
            // logSaveFileDialog
            // 
            this.logSaveFileDialog.DefaultExt = "txt";
            this.logSaveFileDialog.FileName = "logfile.txt";
            this.logSaveFileDialog.Filter = "Text File|*.txt|All Files|*.*";
            this.logSaveFileDialog.OverwritePrompt = false;
            this.logSaveFileDialog.Title = "Capture Traffic";
            // 
            // pollTimer
            // 
            this.pollTimer.Interval = 2000;
            this.pollTimer.Tick += new System.EventHandler(this.pollTimer_Tick);
            // 
            // aCEModuleToolStripMenuItem
            // 
            this.aCEModuleToolStripMenuItem.Name = "aCEModuleToolStripMenuItem";
            this.aCEModuleToolStripMenuItem.Size = new System.Drawing.Size(232, 26);
            this.aCEModuleToolStripMenuItem.Text = "ACE Module";
            this.aCEModuleToolStripMenuItem.Click += new System.EventHandler(this.aCEModuleToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 538);
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.topPanel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "IQ2020 Data Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.bottomPanel.ResumeLayout(false);
            this.mainTabControl.ResumeLayout(false);
            this.dataPacketsTabPage.ResumeLayout(false);
            this.dataPacketsTabPage.PerformLayout();
            this.rawDataTabPage.ResumeLayout(false);
            this.rawDataTabPage.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.storeTabPage.ResumeLayout(false);
            this.dataTabPage.ResumeLayout(false);
            this.stateDecodeTabPage.ResumeLayout(false);
            this.stateDecodeTabPage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.TextBox addressTextBox;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.TextBox mainDataPacketTextBox;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage dataPacketsTabPage;
        private System.Windows.Forms.TabPage rawDataTabPage;
        private System.Windows.Forms.ToolStripMenuItem logToFileToolStripMenuItem;
        private System.Windows.Forms.TextBox mainRawDataTextBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button rawSendButton;
        private System.Windows.Forms.TextBox rawSendTextBox;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scanToolStripMenuItem;
        private System.Windows.Forms.Timer scanTimer;
        private System.Windows.Forms.ToolStripStatusLabel mainToolStripStatusLabel;
        private System.Windows.Forms.TabPage storeTabPage;
        private System.Windows.Forms.ListView packetsListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.SaveFileDialog logSaveFileDialog;
        private System.Windows.Forms.ToolStripMenuItem annotateLogToolStripMenuItem;
        private System.Windows.Forms.ComboBox hexComboBox;
        private System.Windows.Forms.ToolStripMenuItem filterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectionKitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem freshWaterToolStripMenuItem;
        private System.Windows.Forms.TabPage dataTabPage;
        private System.Windows.Forms.ListView dataListView;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ToolStripMenuItem commandsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem askLightsStatusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem askTempStatusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lightsOnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lightsOffToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem askVersionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectionKitX0256ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem setCurrentTimeToolStripMenuItem;
        private System.Windows.Forms.TabPage stateDecodeTabPage;
        private System.Windows.Forms.TextBox stateDecodeTextBox;
        private System.Windows.Forms.ToolStripMenuItem pollStateToolStripMenuItem;
        private System.Windows.Forms.Timer pollTimer;
        private System.Windows.Forms.ToolStripMenuItem audioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aCEModuleToolStripMenuItem;
    }
}

