using Remote_Admin.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Remote_Admin.View
{
    public partial class TaskManagerForm : Form
    {
        private RemoteComputer computer;

        public TaskManagerForm(RemoteComputer _comp)
        {
            computer = _comp;
            InitializeComponent();
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            foreach (Process process in System.Diagnostics.Process.GetProcesses())
            {
                ProcessListView.Items.Add(process.ProcessName);
            }
        }
    }
}
