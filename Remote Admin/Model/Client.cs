using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Linq;
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
        private string serverIp;
        private bool isSending = false;

        public bool ConnectToServer(string IP)
        {
            serverIp = IP;
            if (LoadInfo()) // Загружаем данные и получаем размер экрана
            {
                ReceiveThread = new Thread(RunReceive);
                ReceiveThread.Start(); //запускаем поток
                ReceiveThread.Join();
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
                IPHostEntry ipHost = Dns.GetHostEntry(serverIp);

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

                            Commands.ConnectToServer(sockClient, Environment.MachineName,
                                Environment.UserName,
                                Dns.GetHostByName(Environment.MachineName).AddressList[0].ToString());
                            //MessageBox.Show("The connection to the Server was successful!", "Server Connection Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            error = true;
                            serverIp = address.ToString();
                            break;
                        }
                    }
                    catch (System.Net.Sockets.SocketException e) { }
                }

                width = Screen.PrimaryScreen.Bounds.Width;
                height = Screen.PrimaryScreen.Bounds.Height;

                Commands.StartSendingScreenEvent += StartSendingScreen;
                Commands.StopSendingScreenEvent += StopSendingScreen;

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

            while (isSending)
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
                catch (Exception) { sockClient.Close(); Thread.CurrentThread.Abort(); }
                Thread.Sleep(10);
            }
        }

        private void StartSendingScreen()
        {
            if (!isSending)
            {
                isSending = true;
                SendThread = new Thread(RunSend);
                SendThread.Start(); //запускаем поток
            }
        }

        private void StopSendingScreen()
        {
            isSending = false;
            SendThread.Abort();
        }

        private byte[] ConvertToByte(Bitmap bmp)
        {
            MemoryStream memoryStream = new MemoryStream();
            bmp.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            return memoryStream.ToArray();
        }

        private void RunReceive()
        {
            while (true)
            {
                try
                {
                    byte[] bytes = new byte[50];
                    sockClient.Receive(bytes);
                    if (bytes[0] == 100)
                    {
                        StopSendingScreen();
                        ReceiveThread.Abort();
                    }
                    Commands.ExecuteCommand(bytes, serverIp);
                }
                catch
                {
                    isSending = false;
                    ReceiveThread.Abort();
                    return;
                }
            }
        }
    }
}
