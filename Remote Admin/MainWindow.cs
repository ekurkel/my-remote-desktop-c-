using System;
using System.Windows.Forms;
using MaterialSkin.Controls;
using Remote_Admin.Model;
using System.Threading;

namespace Remote_Admin
{

    public partial class MainWindow : MaterialForm
    {
        private Client client;
        private Thread clientThread;

        public MainWindow()
        {
            InitializeComponent();

            MaterialSkin.MaterialSkinManager skinManager = MaterialSkin.MaterialSkinManager.Instance;
            skinManager.AddFormToManage(this);
            skinManager.Theme = MaterialSkin.MaterialSkinManager.Themes.LIGHT;
            skinManager.ColorScheme = new MaterialSkin.ColorScheme(MaterialSkin.Primary.Blue800, MaterialSkin.Primary.BlueGrey700, MaterialSkin.Primary.Blue500, MaterialSkin.Accent.Orange700, MaterialSkin.TextShade.WHITE);

            label5.Text = Environment.MachineName;
            label6.Text = System.Net.Dns.GetHostByName(Environment.MachineName).AddressList[0].ToString();
        }

        private void StartClient(string ip)
        {
            if (!client.ConnectToServer(ip))
            {
                clientThread.Abort();
            }
        }

        private void bConnect_Click(object sender, EventArgs e)
        {
            client = new Client();
            clientThread = new Thread(delegate () { StartClient(textBoxIP.Text); } );
            clientThread.Start();
            this.Hide();
            clientThread.Join();
            textBoxIP.Text = "Error!!!";
            this.Show();
        }

        private void bServStart_Click(object sender, EventArgs e)
        {
            Form fServ = new ServerForm();
            this.Hide();
            fServ.ShowDialog();
            this.Show();
        }
    }
}
