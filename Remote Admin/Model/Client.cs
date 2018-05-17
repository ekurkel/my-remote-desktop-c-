using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Remote_Admin.Model
{
    class Client
    {
        private Socket sockClient;
        private Thread SendThread;
        private Thread ReceiveThread;
        private int width;
        private int height;
        private string ip;

        public bool ConnectToServer(string IP)
        {
            ip = IP;
            if (LoadInfo()) // Загружаем данные и получаем размер экрана
            {
                SendThread = new Thread(RunSend);
                SendThread.Start(); //запускаем поток
                ReceiveThread = new Thread(RunReceive);
                ReceiveThread.Start(); //запускаем поток
                SendThread.Join();
                return true;
            }
            return false;
        }

        private bool LoadInfo()
        {
            // Загружаем IP адресс и порт
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
           // Data d = new Data();

            while (true)
            {
                try
                {
                    sockClient.Receive(bytes);
                    Commands.ExecuteCommand(bytes, ip);
                }
                catch
                {
                        sockClient.Shutdown(SocketShutdown.Both);
                        sockClient.Close();
                        return;
                }
            }
        }
    }
}
