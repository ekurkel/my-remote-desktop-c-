using System;
using System.Drawing;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Linq;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Remote_Admin
{
    [StructLayout(LayoutKind.Sequential)]
    struct Data
    {
        public int Button;
        public int x;
        public int y;
    }
    public partial class RemAdmForm : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;


        private Socket sockClient;
        private Thread SendThread;
        private Thread ReceiveThread;
        private int width;
        private int height;
        private bool awd = false;
        string ip;

        public RemAdmForm()
        {
            InitializeComponent();
            label5.Text = Environment.MachineName;
            label6.Text = System.Net.Dns.GetHostByName(Environment.MachineName).AddressList[0].ToString();
        }

        private bool LoadInfo()
        {
            // Загружаем IP адресс и порт
            ip = textBoxIP.Text;
            int port = 1991;
            bool error = false;
            try
            {
                // Устанавливаем удаленную точку для сокета
                IPHostEntry ipHost = Dns.GetHostEntry(ip);

                foreach (var address in ipHost.AddressList)
                {
                    try
                    {
                        var ipe = new IPEndPoint(address, port);
                        var tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        tempSocket.Connect(ipe);
                        if (tempSocket.Connected)
                        {
                            sockClient = tempSocket;
                            sockClient.Send(System.Text.Encoding.ASCII.GetBytes(Environment.MachineName));
                            error = true;
                            ip = address.ToString();
                            break;
                        }
                    }
                    catch (Exception) { }
                }

                width = Screen.PrimaryScreen.Bounds.Width;
                height = Screen.PrimaryScreen.Bounds.Height;

                return error;
            }
            catch
            {
                MessageBox.Show("Error! Server not found", "Error",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
                textBoxIP.Text = "Error!!!";
                return false;
            }
        }


        private void RunSend()
        {

            Bitmap BackGround = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(BackGround);

            while (true)
            {
                // Получаем снимок экрана
                graphics.CopyFromScreen(0, 0, 0, 0, new Size(width, height));

                // Получаем изображение в виде массива байтов
                byte[] bytes = ConvertToByte(BackGround);
                try
                {
                    // Отправляем картинку клиенту
                    sockClient.Send(bytes, bytes.Length, 0);
                }
                catch (Exception) { sockClient.Close(); return; }
                Thread.Sleep(10);
            }
        }

        private byte[] ConvertToByte(Bitmap bmp)
        {
            MemoryStream memoryStream = new MemoryStream();
            bmp.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            return memoryStream.ToArray();
        }

        private void RunReceive()
        {
            byte[] bytes = new byte[30];
            Data d = new Data();

            while (true)
            {
                try
                {
                    sockClient.Receive(bytes);

                    d.Button = BitConverter.ToInt32(bytes, 0);
                    d.x = BitConverter.ToInt32(bytes, 4) * 65536 / Screen.PrimaryScreen.Bounds.Width;
                    d.y = BitConverter.ToInt32(bytes, 8) * 65536 / Screen.PrimaryScreen.Bounds.Height;

                    switch (d.Button)
                    {
                        case 0:
                            mouse_event((uint)(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE | /*MOUSEEVENTF_MOVE_NOCOALESCE*/ 0x2000), (uint)d.x, (uint)d.y, 0, 0);
                            break;
                        case 1:
                            mouse_event((uint)(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_ABSOLUTE), (uint)d.x, (uint)d.y, 0, 0);
                            break;
                        case 2:
                            mouse_event((uint)(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_ABSOLUTE), (uint)d.x, (uint)d.y, 0, 0);
                            break;
                        case 3:
                            mouse_event((uint)(MOUSEEVENTF_LEFTUP | MOUSEEVENTF_ABSOLUTE), (uint)d.x, (uint)d.y, 0, 0);
                            break;
                        case 4:
                            mouse_event((uint)(MOUSEEVENTF_RIGHTUP | MOUSEEVENTF_ABSOLUTE), (uint)d.x, (uint)d.y, 0, 0);
                            break;
                        case 5:
                            keybd_event(bytes[4], bytes[8], 1 | 0, (UIntPtr)0);
                            break;
                        case 6:
                            keybd_event(bytes[4], bytes[8], 1 | 2, (UIntPtr)0);
                            break;
                        case 7:
                            mouse_event(0x0800, 0, 0, unchecked((uint)150), 0);
                            break;
                        case 8:
                            mouse_event(0x0800, 0, 0, unchecked((uint)-150), 0);
                            break;
                        case 10:
                            {
                                //Коннектимся
                                IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse(ip), 6999);
                                Socket Connector = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                                Connector.Connect(EndPoint);
                                awd = true;
                                ReceiveFile(Connector);
                                Connector.Close();
                            }
                            break;
                    }
                }
                catch
                {
                    if (!awd)
                    {
                        sockClient.Shutdown(SocketShutdown.Both);
                        sockClient.Close();
                        return;
                    }
                }
            }
        }

        protected void ReceiveFile(Socket Connector)
        {
            try
            {
                Byte[] Receive = new Byte[256];
                //Читать сообщение будем в поток
                using (MemoryStream MessageR = new MemoryStream())
                {

                    //Количество считанных байт
                    Int32 ReceivedBytes;
                    Int32 Firest256Bytes = 0;
                    String FilePath = "";
                    do
                    {//Собственно читаем
                        ReceivedBytes = Connector.Receive(Receive, Receive.Length, 0);
                        //Разбираем первые 256 байт
                        if (Firest256Bytes < 256)
                        {
                            Firest256Bytes += ReceivedBytes;
                            Byte[] ToStr = Receive;
                            //Учтем, что может возникнуть ситуация, когда они не могу передаться "сразу" все
                            if (Firest256Bytes > 256)
                            {
                                Int32 Start = Firest256Bytes - ReceivedBytes;
                                Int32 CountToGet = 256 - Start;
                                Firest256Bytes = 256;
                                //В случае если было принято >256 байт (двумя сообщениями к примеру)
                                //Остаток (до 256) записываем в "путь файла"
                                ToStr = Receive.Take(CountToGet).ToArray();
                                //А остальную часть - в будующий файл
                                Receive = Receive.Skip(CountToGet).ToArray();
                                MessageR.Write(Receive, 0, ReceivedBytes);
                            }
                            //Накапливаем имя файла
                            FilePath += Encoding.Default.GetString(ToStr);
                        }
                        else

                            //и записываем в поток
                            MessageR.Write(Receive, 0, ReceivedBytes);
                        //Читаем до тех пор, пока в очереди не останется данных
                    } while (ReceivedBytes == Receive.Length);
                    //Убираем лишние байты
                    String resFilePath = FilePath.Substring(0, FilePath.IndexOf('\0'));
                    using (var File = new FileStream(resFilePath, FileMode.Create))
                    {//Записываем в файл
                        File.Write(MessageR.ToArray(), 0, MessageR.ToArray().Length);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void bConnect_Click(object sender, EventArgs e)
        {
            if (LoadInfo()) // Загружаем данные и получаем размер экрана
            {
                SendThread = new Thread(RunSend);
                SendThread.Start(); //запускаем поток
                this.Hide();
                ReceiveThread = new Thread(RunReceive);
                ReceiveThread.Start(); //запускаем поток
                SendThread.Join();
                this.Show();
            }
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
