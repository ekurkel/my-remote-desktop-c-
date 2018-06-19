using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Remote_Admin.Model
{
    public class Client
    {
        private Socket sockClient;
        private Thread SendThread;
        private Thread ReceiveThread;
        private AesCryptoServiceProvider AES;
        private int width;
        private int height;
        private string serverIp;
        private bool isSending = false;

        public bool ConnectToServer(string IP)
        {
            serverIp = IP;
            if (LoadInfo()) // Загружаем данные и получаем размер экрана
            {
                SendInformation();
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
                            error = true;
                            serverIp = address.ToString();
                            break;
                        }
                    }
                    catch (SocketException) { }
                }

                width = Screen.PrimaryScreen.Bounds.Width;
                height = Screen.PrimaryScreen.Bounds.Height;

                AES = new AesCryptoServiceProvider();

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
                catch
                {
                    Thread.CurrentThread.Abort();
                }
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
            byte[] bytes = new byte[500];
            while (true)
            {
                try
               { 
                    int length = sockClient.Receive(bytes);
                    ExecuteCommand(bytes, length);
                }
                catch (Exception)
                {
                    isSending = false;
                    ReceiveThread.Abort();
                    return;
                }
            }
        }

        private void SendInformation()
        {
            AES.Key = MessageEncrypt.GetKeyAES(sockClient);
            sockClient.Receive(AES.IV);
            
            sockClient.Send(Encoding.UTF8.GetBytes(MessageEncrypt.AESEncrypt(Environment.MachineName, AES)));
            Thread.Sleep(30);
            sockClient.Send(Encoding.UTF8.GetBytes(MessageEncrypt.AESEncrypt(Environment.UserName, AES)));
            Thread.Sleep(30);
            sockClient.Send(Encoding.UTF8.GetBytes(MessageEncrypt.AESEncrypt(Dns.GetHostByName(Environment.MachineName).AddressList[0].ToString(), AES)));
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        private void ExecuteCommand(byte[] commandBytes, int length)
        {
            commandBytes = MessageEncrypt.AESDecrypt(commandBytes, AES);
            CommandMessage message = new CommandMessage(commandBytes);

            switch (message.CommandType)
            {
                case NetworkCommands.MOUSE_MOVE:
                    mouse_event((uint)(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE | 0x2000), (uint)message.GetMouseCoordinatesX(), (uint)message.GetMouseCoordinatesY(), 0, 0);
                    break;
                case NetworkCommands.MOUSE_LEFTDOWN:
                    mouse_event((uint)(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_ABSOLUTE), (uint)message.GetMouseCoordinatesX(), (uint)message.GetMouseCoordinatesY(), 0, 0);
                    break;
                case NetworkCommands.MOUSE_RIGHTDOWN:
                    mouse_event((uint)(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_ABSOLUTE), (uint)message.GetMouseCoordinatesX(), (uint)message.GetMouseCoordinatesY(), 0, 0);
                    break;
                case NetworkCommands.MOUSE_LEFTUP:
                    mouse_event((uint)(MOUSEEVENTF_LEFTUP | MOUSEEVENTF_ABSOLUTE), (uint)message.GetMouseCoordinatesX(), (uint)message.GetMouseCoordinatesY(), 0, 0);
                    break;
                case NetworkCommands.MOUSE_RIGHTUP:
                    mouse_event((uint)(MOUSEEVENTF_RIGHTUP | MOUSEEVENTF_ABSOLUTE), (uint)message.GetMouseCoordinatesX(), (uint)message.GetMouseCoordinatesY(), 0, 0);
                    break;
                case NetworkCommands.KEYBOARD_DOWN:
                    keybd_event((byte)message.firstParam, (byte)message.secondParam, 1 | 0, (UIntPtr)0);
                    break;
                case NetworkCommands.KEYBOARD_UP:
                    keybd_event((byte)message.firstParam, (byte)message.secondParam, 1 | 2, (UIntPtr)0);
                    break;
                case NetworkCommands.MOUSE_WHEEL_ROTATED:
                    if (message.firstParam < 0)
                        mouse_event(0x0800, 0, 0, unchecked((uint)-150), 0);
                    else mouse_event(0x0800, 0, 0, unchecked((uint)150), 0);
                    break;
                case NetworkCommands.RECIVE_FILE:
                    ReceiveFile(message.firstParam);
                    break;
                case NetworkCommands.STOP_SENDING:
                    StopSendingScreen();
                    break;
                case NetworkCommands.START_SENDING:
                    StartSendingScreen();
                    break;
                case NetworkCommands.RUN_FILE:
                    RunFile(commandBytes, length);
                    break;
                case NetworkCommands.RUN_COMMAND_LINE:
                    RunCommandLine();
                    break;
                case NetworkCommands.CLOSE_CONNECTION:
                    StopSendingScreen();
                    ReceiveThread.Abort();
                    break;
            }
        }

        private void RunCommandLine()
        {
            byte[] resCommand = new byte[500];
            int count = sockClient.Receive(resCommand);
            string comandLine = CommandMessage.GetNameFromByte(resCommand, count);
            System.Diagnostics.Process.Start("cmd.exe", "/K " + comandLine);
        }

        private void RunFile(byte[] _path, int _length)
        {
            string resFilePath = GetStringFromMessage(_path, _length);
            System.Diagnostics.Process.Start(resFilePath);
        }

        private string GetStringFromMessage(byte[] _path, int _length)
        {
            byte[] b = new byte[_length - 10];
            for (int i = 12; i < _length; i++)
                b[i - 12] = _path[i];
            string str = CommandMessage.GetNameFromByte(b, _length - 10);
            return str.Substring(0, str.IndexOf('\0'));
        }

        private void ReceiveFile(int port)
        {
            try
            {
                //Коннектимся
                IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse(serverIp), port);
                Socket Connector = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Connector.Connect(EndPoint);

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

                        using (var File = new FileStream(resFilePath, FileMode.CreateNew))
                        {
                            //Записываем в файл
                            File.Write(MessageR.ToArray(), 0, MessageR.ToArray().Length);
                        }
                    }

                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Connector.Close();
                }
                Connector.Close();
            }
            catch { }
        }

    }
}
