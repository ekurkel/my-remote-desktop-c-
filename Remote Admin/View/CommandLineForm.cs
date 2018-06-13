using Remote_Admin.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Remote_Admin.View
{
    public partial class CommandLineForm : Form
    {
        private RemoteComputer computer;

        public CommandLineForm(RemoteComputer _computer)
        {
            computer = _computer;

            InitializeComponent();
        }

        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            if (CommandLineTextBox.Text != "")
                computer.RunCommandLine(CommandLineTextBox.Text);
        }
    }
}
