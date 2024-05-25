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
            this.scanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.mainToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.topPanel = new System.Windows.Forms.Panel();
            this.connectButton = new System.Windows.Forms.Button();
            this.addressTextBox = new System.Windows.Forms.TextBox();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.sendButton = new System.Windows.Forms.Button();
            this.hexTextBox = new System.Windows.Forms.TextBox();
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
            this.scanTimer = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.mainTabControl.SuspendLayout();
            this.dataPacketsTabPage.SuspendLayout();
            this.rawDataTabPage.SuspendLayout();
            this.panel1.SuspendLayout();
            this.storeTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.actionsToolStripMenuItem});
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
            this.logToFileToolStripMenuItem.Size = new System.Drawing.Size(154, 26);
            this.logToFileToolStripMenuItem.Text = "Log to &File";
            this.logToFileToolStripMenuItem.Click += new System.EventHandler(this.logToFileToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(151, 6);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(154, 26);
            this.clearToolStripMenuItem.Text = "C&lear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(151, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(154, 26);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scanToolStripMenuItem});
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            this.actionsToolStripMenuItem.Size = new System.Drawing.Size(70, 24);
            this.actionsToolStripMenuItem.Text = "&Actions";
            // 
            // scanToolStripMenuItem
            // 
            this.scanToolStripMenuItem.Name = "scanToolStripMenuItem";
            this.scanToolStripMenuItem.Size = new System.Drawing.Size(115, 26);
            this.scanToolStripMenuItem.Text = "&Scan";
            this.scanToolStripMenuItem.Click += new System.EventHandler(this.scanToolStripMenuItem_Click);
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
            this.connectButton.Location = new System.Drawing.Point(591, 13);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(101, 23);
            this.connectButton.TabIndex = 1;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // addressTextBox
            // 
            this.addressTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addressTextBox.Location = new System.Drawing.Point(12, 13);
            this.addressTextBox.Name = "addressTextBox";
            this.addressTextBox.Size = new System.Drawing.Size(573, 22);
            this.addressTextBox.TabIndex = 0;
            this.addressTextBox.Text = "192.168.2.184:1234";
            // 
            // bottomPanel
            // 
            this.bottomPanel.Controls.Add(this.sendButton);
            this.bottomPanel.Controls.Add(this.hexTextBox);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(3, 358);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(690, 45);
            this.bottomPanel.TabIndex = 3;
            // 
            // sendButton
            // 
            this.sendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sendButton.Location = new System.Drawing.Point(577, 9);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(101, 30);
            this.sendButton.TabIndex = 1;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // hexTextBox
            // 
            this.hexTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexTextBox.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexTextBox.Location = new System.Drawing.Point(5, 9);
            this.hexTextBox.Name = "hexTextBox";
            this.hexTextBox.Size = new System.Drawing.Size(559, 30);
            this.hexTextBox.TabIndex = 0;
            this.hexTextBox.Text = "29 99 40 1E010000FFFFFF00FF04FFFFFFFFFF";
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
            // scanTimer
            // 
            this.scanTimer.Interval = 500;
            this.scanTimer.Tick += new System.EventHandler(this.scanTimer_Tick);
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
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.bottomPanel.ResumeLayout(false);
            this.bottomPanel.PerformLayout();
            this.mainTabControl.ResumeLayout(false);
            this.dataPacketsTabPage.ResumeLayout(false);
            this.dataPacketsTabPage.PerformLayout();
            this.rawDataTabPage.ResumeLayout(false);
            this.rawDataTabPage.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.storeTabPage.ResumeLayout(false);
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
        private System.Windows.Forms.TextBox hexTextBox;
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
    }
}

