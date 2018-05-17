using System;
using System.Drawing;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Linq;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Remote_Admin.Model;

namespace Remote_Admin
{
    public partial class ServerForm : Form
    {
        private double koef;
        private Server server;
        private int selectedComputer;
        private Thread DrowImageThread;

        public ServerForm()
        {
            InitializeComponent();

            server = new Server();

            label7.Text = Environment.MachineName;
            label6.Text = Dns.GetHostByName(Environment.MachineName).AddressList[0].ToString();
        }

        private void Drow()
        {
            double w, h;

            while (true)
            {
                try
                {
                    
                    Bitmap bmp = server.RemoteComputers[selectedComputer].ComputerScreen;

                    Size size = new Size(0, 0);
                    koef = bmp.Width / (double)picScreen.Width;

                    if ((bmp.Height / koef) < picScreen.Height)
                    {
                        w = bmp.Width / koef;
                        size.Width = (int)w;
                        h = bmp.Height / koef;
                        size.Height = (int)h;
                    }
                    else
                    {
                        koef = bmp.Height / (double)picScreen.Height;
                        h = bmp.Height / koef;
                        size.Height = (int)h;
                        w = bmp.Width / koef;
                        size.Width = (int)w;
                    }

                    Bitmap bitmp = new Bitmap(bmp, size);
                    picScreen.Image = bitmp;
                }
                catch
                {
                }
            }
        }

        private void ClientComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedComputer = ClientComboBox.SelectedIndex;

            if (!DrowImageThread.IsAlive)
            {
                DrowImageThread = new Thread(Drow);
                DrowImageThread.Start(); //запускаем поток
            }
        }

        private void picScreen_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                double x = e.X * koef;
                double y = e.Y * koef;

                Commands.MouseMove(server.RemoteComputers[selectedComputer].clientSocket, (int)x, (int)y);
            }
            catch { }
        }

        private void picScreen_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Commands.LeftMouseBtnClick(server.RemoteComputers[selectedComputer].clientSocket);
            else if (e.Button == MouseButtons.Right)
                Commands.RightMouseBtnClick(server.RemoteComputers[selectedComputer].clientSocket);
        }

        private void picScreen_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Commands.LeftMouseBtnUp(server.RemoteComputers[selectedComputer].clientSocket);
            else if (e.Button == MouseButtons.Right)
                Commands.RightMouseBtnUp(server.RemoteComputers[selectedComputer].clientSocket);
        }

        private void ServForm_KeyDown(object sender, KeyEventArgs e)
        {
            Commands.KeyDown(server.RemoteComputers[selectedComputer].clientSocket, e.KeyValue, (int)e.KeyCode);
        }

        private void ServForm_KeyUp(object sender, KeyEventArgs e)
        {
            Commands.KeyUp(server.RemoteComputers[selectedComputer].clientSocket, e.KeyValue, (int)e.KeyCode);
        }

        private void this_MouseWheel(object sender, MouseEventArgs e)
        {
            Commands.MouseWheel(server.RemoteComputers[selectedComputer].clientSocket, e.Delta);
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.SendFile(server.RemoteComputers[selectedComputer].clientSocket);
        }
    }
}
