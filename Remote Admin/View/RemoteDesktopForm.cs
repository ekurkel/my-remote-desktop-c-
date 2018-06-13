using Remote_Admin.Model;
using System.Drawing;
using System.Windows.Forms;

namespace Remote_Admin
{
    public partial class RemoteDesktopForm : Form
    {
        private double koef;
        private RemoteComputer clientComp;

        public RemoteDesktopForm(RemoteComputer r)
        {
            clientComp = r;
            InitializeComponent();

            clientComp.RemoteComputerScreenHasChangedEvent += Drow;

            clientComp.SendMessage(new Model.CommandMessage(NetworkCommands.START_SENDING));
        }

        private void Drow()
        {
            double w, h;

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

        private void picScreen_MouseMove(object sender, MouseEventArgs e)
        {
            double x = e.X * koef;
            double y = e.Y * koef;

           clientComp.SendMessage(new CommandMessage(NetworkCommands.MOUSE_MOVE, (int)x, (int)y));
        }

        private void picScreen_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                clientComp.SendMessage(new CommandMessage(NetworkCommands.MOUSE_LEFTDOWN, (int)(e.X * koef), (int)(e.Y * koef)));
            else if (e.Button == MouseButtons.Right)
                clientComp.SendMessage(new CommandMessage(NetworkCommands.MOUSE_RIGHTDOWN, (int)(e.X * koef), (int)(e.Y * koef)));
        }

        private void picScreen_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                clientComp.SendMessage(new CommandMessage(NetworkCommands.MOUSE_LEFTUP, (int)(e.X * koef), (int)(e.Y * koef)));
            else if (e.Button == MouseButtons.Right)
                clientComp.SendMessage(new CommandMessage(NetworkCommands.MOUSE_RIGHTUP, (int)(e.X * koef), (int)(e.Y * koef)));
        }

        private void ServForm_KeyDown(object sender, KeyEventArgs e)
        {
            clientComp.SendMessage(new CommandMessage(NetworkCommands.KEYBOARD_DOWN, e.KeyValue, (int)e.KeyCode));
        }

        private void ServForm_KeyUp(object sender, KeyEventArgs e)
        {
            clientComp.SendMessage(new CommandMessage(NetworkCommands.KEYBOARD_UP, e.KeyValue, (int)e.KeyCode));
        }

        private void this_MouseWheel(object sender, MouseEventArgs e)
        {
            clientComp.SendMessage(new CommandMessage(NetworkCommands.MOUSE_WHEEL_ROTATED, e.Delta));
        }

    }
}
