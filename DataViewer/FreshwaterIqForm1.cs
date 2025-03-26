﻿using System;
using System.Windows.Forms;

namespace DataViewer
{
    public partial class FreshwaterIqForm1: Form
    {
        private MainForm parent;

        public FreshwaterIqForm1(MainForm parent)
        {
            this.parent = parent;
            InitializeComponent();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            string r = "01 37 80 23D1";
            r += ((int)numericUpDown1.Value).ToString("X8");
            r += ((int)numericUpDown2.Value).ToString("X8");
            r += ((int)numericUpDown3.Value).ToString("X8");
            r += ((int)numericUpDown4.Value).ToString("X8");
            r += ((int)numericUpDown5.Value).ToString("X8");
            r += ((int)numericUpDown6.Value).ToString("X8");
            r += ((int)numericUpDown7.Value).ToString("X8");
            parent.SetSendText(r);
            parent.HitSend();
        }
    }
}
