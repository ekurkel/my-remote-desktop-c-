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

namespace Remote_Admin
{
    public partial class RemoteDesctopForm : Form
    {
        private double koef;
        private int selectedComputer;
        private RemoteComputer clientComp;

        public RemoteDesctopForm(RemoteComputer r)
        {
            clientComp = r;
            InitializeComponent();

            clientComp.RemoteComputerScreenHasChangedEvent += Drow;

            Commands.StartSendingScreen(clientComp.clientSocket);
        }

        private void Drow()
        {
                 double w, h;

                 try
                 {
                     Bitmap bmp = clientComp.ComputerScreen;

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

        private void picScreen_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                double x = e.X * koef;
                double y = e.Y * koef;

                Commands.MouseMove(clientComp.clientSocket, (int)x, (int)y);
            }
            catch { }
        }

        private void picScreen_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                    Commands.LeftMouseBtnClick(clientComp.clientSocket);
                else if (e.Button == MouseButtons.Right)
                    Commands.RightMouseBtnClick(clientComp.clientSocket);
            }
            catch { }
        }

        private void picScreen_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                    Commands.LeftMouseBtnUp(clientComp.clientSocket);
                else if (e.Button == MouseButtons.Right)
                    Commands.RightMouseBtnUp(clientComp.clientSocket);
            }
            catch { }
        }

        private void ServForm_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Commands.KeyDown(clientComp.clientSocket, e.KeyValue, (int)e.KeyCode);
            }
            catch { }
        }

        private void ServForm_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                Commands.KeyUp(clientComp.clientSocket, e.KeyValue, (int)e.KeyCode);
            }
            catch { }
        }

        private void this_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                Commands.MouseWheel(clientComp.clientSocket, e.Delta);
            }
            catch { }
        }

    }
}
