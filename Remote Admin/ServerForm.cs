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

namespace Remote_Admin
{
    public partial class ServerForm : Form
    {

        private int countSockets = 0;
        private int numberOfComputer = 0;
        private Socket[] arSock = new Socket[20];
        private Socket sListener;
        private Data data = new Data();
        private double koef;
        private byte[] mouseb = new byte[10];


        public ServerForm()
        {
            InitializeComponent();

            label7.Text = Environment.MachineName;
            label6.Text = System.Net.Dns.GetHostByName(Environment.MachineName).AddressList[0].ToString();

            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 1991);

            sListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sListener.Bind(ipEndPoint);
            sListener.Listen(20);

            CheckForIllegalCrossThreadCalls = false;

            Thread RunThread = new Thread(Run);
            RunThread.IsBackground = true;
            RunThread.Start(); //запускаем поток
        }

        private void Run()
        {
            Thread DrowThread = new Thread(Drow);
            DrowThread.IsBackground = true;
            DrowThread.Start(); //запускаем поток
            while (true)
            { 
                // Программа приостанавливается, ожидая входящее соединение
                Socket handler = sListener.Accept();
                groupBox1.Visible = false;
                importToolStripMenuItem.Enabled = true;
                exportClipboardToolStripMenuItem.Enabled = true;
                arSock[countSockets] = handler;
                countSockets++;
                byte[] name = new byte[10];
                int iRx = handler.Receive(name);
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                char[] chars = new char[iRx];
                int charLen = d.GetChars(name, 0, iRx, chars, 0);
                System.String recv = new System.String(chars);
                ClientComboBox.Enabled = true;
                ClientComboBox.Items.Add(recv);
                ClientComboBox.SelectedIndex = numberOfComputer;
            }
        }

        private void Drow()
        {
            byte[] bytes = new byte[1050000];
            while (true)
            {
                try
                {
                    int bytesRec = arSock[numberOfComputer].Receive(bytes);
                    Size size = new Size(0, 0);
                    MemoryStream memoryStream = new MemoryStream(bytes);

                    Bitmap bmp = (Bitmap)Bitmap.FromStream(memoryStream);

                    koef = bmp.Width / (double)picScreen.Width;

                    if ((bmp.Height / koef) < picScreen.Height)
                    {
                        double w = bmp.Width / koef;
                        size.Width = (int)w;
                        double h = bmp.Height / koef;
                        size.Height = (int)h;
                    }
                    else
                    {
                        koef = bmp.Height / (double)picScreen.Height;
                        double h = bmp.Height / koef;
                        size.Height = (int)h;
                        double w = bmp.Width / koef;
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

        private static byte[] RawSerialize(object anything)
        {
            int rawsize = Marshal.SizeOf(anything);
            byte[] rawdata = new byte[rawsize];
            GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            Marshal.StructureToPtr(anything, handle.AddrOfPinnedObject(), false);
            handle.Free();
            return rawdata;
        }

        private void SendMyMessage()
        {
            try
            {
                mouseb = RawSerialize(data);
                arSock[numberOfComputer].Send(mouseb);
            }
            catch { }
        }

        private void picScreen_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                data.Button = 1;
            else if (e.Button == MouseButtons.Right)
                data.Button = 2;

            SendMyMessage();
        }

        private void picScreen_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                double x = e.X * koef;
                double y = e.Y * koef;

                data.x = (int)x;
                data.y = (int)y;
                data.Button = 0;

                SendMyMessage();
            }
            catch { }
        }

        private void picScreen_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                data.Button = 3;
            else if (e.Button == MouseButtons.Right)
                data.Button = 4;

            SendMyMessage();
        }

        private void ServForm_KeyDown(object sender, KeyEventArgs e)
        {
            data.Button = 5;
            data.x = e.KeyValue;
            data.y = (int)e.KeyCode;
            SendMyMessage();
        }

        private void ServForm_KeyUp(object sender, KeyEventArgs e)
        {
            data.Button = 6;
            data.x = e.KeyValue;
            data.y = (int)e.KeyCode;
            SendMyMessage();
        }

        private void ClientComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            numberOfComputer = ClientComboBox.SelectedIndex;
        }

        private void this_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                data.Button = 7;
            else
                data.Button = 8;
            SendMyMessage();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                data.Button = 10;
                SendMyMessage();

                //Создаем Listener на порт "по умолчанию"
                TcpListener Listener = new TcpListener(6999);
                //Начинаем прослушку
                Listener.Start();
                //и заведем заранее сокет
                Socket SendSocket;

                SendSocket = Listener.AcceptSocket();

                StringBuilder FileName = new StringBuilder(ofd.FileName);
                //Выделяем имя файла
                int index = FileName.Length - 1;
                while (FileName[index] != '\\' && FileName[index] != '/')
                {
                    index--;
                }
                //Получаем имя файла
                String resFileName = "";
                for (int i = index + 1; i < FileName.Length; i++)
                    resFileName += FileName[i];
                //Записываем в лист
                List<Byte> First256Bytes = Encoding.Default.GetBytes(resFileName).ToList();
                Int32 Diff = 256 - First256Bytes.Count;
                //Остаток заполняем нулями
                for (int i = 0; i < Diff; i++)
                    First256Bytes.Add(0);
                //Начинаем отправку данных
                Byte[] ReadedBytes = new Byte[256];
                using (var FileStream = new FileStream(ofd.FileName, FileMode.Open))
                {
                    using (var Reader = new BinaryReader(FileStream))
                    {
                        Int32 CurrentReadedBytesCount;
                        //Вначале отправим название файла
                        SendSocket.Send(First256Bytes.ToArray());
                        do
                        {
                            //Затем по частям - файл
                            CurrentReadedBytesCount = Reader.Read(ReadedBytes, 0, ReadedBytes.Length);
                            SendSocket.Send(ReadedBytes, CurrentReadedBytesCount, SocketFlags.None);
                        }
                        while (CurrentReadedBytesCount == ReadedBytes.Length);
                    }
                    //Завершаем передачу данных
                    SendSocket.Close();
                }
            }
        }
    }
}
