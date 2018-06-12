using System;
using System.Net;
using System.Windows.Forms;
using Remote_Admin.Model;
using MaterialSkin.Controls;
using MaterialSkin;

namespace Remote_Admin
{
    public partial class ServerForm : MaterialForm
    {
        public Server server { get; private set; }

        public ServerForm()
        {
            InitializeComponent();

            server = new Server();
            server.RemoteComputersListChangedEvent += UpdateRemoteComputers;


            MaterialSkinManager skinManager = MaterialSkinManager.Instance;
            skinManager.AddFormToManage(this);
            skinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            skinManager.ColorScheme = new ColorScheme(Primary.Blue800, Primary.BlueGrey700, Primary.Blue500, Accent.Orange700, TextShade.WHITE);


            Label7.Text = Environment.MachineName;
            Label6.Text = Dns.GetHostByName(Environment.MachineName).AddressList[0].ToString();
        }

        private void UpdateRemoteComputers()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(delegate ()
                {
                    listViewClients.Items.Clear();
                    for (int i = 0; i < server.RemoteComputers.Count; i++)
                    {
                        listViewClients.Items.Add(new ListViewItem(new string[] { i.ToString(), server.RemoteComputers[i].ComputerName, server.RemoteComputers[i].ComputerUser, server.RemoteComputers[i].ClientIP }));
                    }

                }));
        }


        private void closeAllConnectionsButton_Click(object sender, EventArgs e)
        {
            server.CloseAllConnections();
        }

        private void remoteDesctopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewClients.SelectedItems.Count < 1)
            {
                MessageBox.Show("You have to select a client in order to access this function!",
                   "ERROR : Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var rdf = new RemoteDesctopForm(server.RemoteComputers[listViewClients.Items.IndexOf(listViewClients.SelectedItems[0])]);
                rdf.ShowDialog();
            }
        }

        private void sendFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewClients.SelectedItems.Count < 1)
            {
                MessageBox.Show("You have to select a client in order to access this function!",
                    "ERROR : Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    server.RemoteComputers[listViewClients.FocusedItem.Index].SendFile();
                }
                catch { }
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewClients.SelectedItems.Count < 1)
            {
                MessageBox.Show("You have to select a client in order to access this function!",
                    "ERROR : Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                server.RemoteComputerConnectionClose(listViewClients.FocusedItem.Index);
            }
        }

        private void SendToAllButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (RemoteComputer r in server.RemoteComputers)
                {
                    r.SendFile(ofd.FileName);
                }
            }
        }

        private void RunAtAllButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (RemoteComputer r in server.RemoteComputers)
                {
                    r.SendFile(ofd.FileName);
                    r.RunFile(ofd.FileName);
                }
            }
        }

        private void runFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                server.RemoteComputers[listViewClients.FocusedItem.Index].SendFile(ofd.FileName);
                server.RemoteComputers[listViewClients.FocusedItem.Index].RunFile(ofd.FileName);
            }
        }



    }
}
