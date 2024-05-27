using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataViewer
{
    public partial class LogAnnotationForm : Form
    {
        MainForm parent;

        public LogAnnotationForm(MainForm parent)
        {
            this.parent = parent;
            InitializeComponent();
        }

        private void LogAnnotationForm_Load(object sender, EventArgs e)
        {
            okButton.Enabled = (mainTextBox.Text.Length > 0);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            parent.AppendPacketDataText(" --- " + mainTextBox.Text);
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void mainTextBox_TextChanged(object sender, EventArgs e)
        {
            okButton.Enabled = (mainTextBox.Text.Length > 0);
        }

        private void LogAnnotationForm_Activated(object sender, EventArgs e)
        {
            mainTextBox.Focus();
        }

        private void mainTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) { okButton.PerformClick(); }
        }
    }
}
