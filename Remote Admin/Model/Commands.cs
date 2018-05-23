using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Remote_Admin.Model
{

    [StructLayout(LayoutKind.Sequential)]
    struct Data
    {
        public int CommandType;
        public int firstParam;
        public int secondParam;
    }

    public static class Commands
    {

        private static Data data;
        private static int countOfListener = 6999; // для сокета... 


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

        public delegate void ClientDelegate();

        public static event ClientDelegate StopSendingScreenEvent;
        public static event ClientDelegate StartSendingScreenEvent;

        public static void ConnectToServer(Socket s, string comp, string name, string ip)
        {
            s.Send(Encoding.ASCII.GetBytes(comp));
            Thread.Sleep(30);
            s.Send(Encoding.UTF8.GetBytes(name));
            Thread.Sleep(30);
            s.Send(Encoding.ASCII.GetBytes(ip));
        }

        public static void ExecuteCommand(byte[] commandBytes, int length, string serverIP)
        {
            Data d = new Data();

            d.CommandType = BitConverter.ToInt32(commandBytes, 0);
            d.firstParam = BitConverter.ToInt32(commandBytes, 4) * 65536 / Screen.PrimaryScreen.Bounds.Width;
            d.secondParam = BitConverter.ToInt32(commandBytes, 8) * 65536 / Screen.PrimaryScreen.Bounds.Height;

            switch (d.CommandType)
            {
                case (int)NetworkCommands.MOUSE_MOVE:
                    mouse_event((uint)(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE | 0x2000), (uint)d.firstParam, (uint)d.secondParam, 0, 0);
                    break;
                case (int)NetworkCommands.MOUSE_LEFTDOWN:
                    mouse_event((uint)(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_ABSOLUTE), (uint)d.firstParam, (uint)d.secondParam, 0, 0);
                    break;
                case (int)NetworkCommands.MOUSE_RIGHTDOWN:
                    mouse_event((uint)(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_ABSOLUTE), (uint)d.firstParam, (uint)d.secondParam, 0, 0);
                    break;
                case (int)NetworkCommands.MOUSE_LEFTUP:
                    mouse_event((uint)(MOUSEEVENTF_LEFTUP | MOUSEEVENTF_ABSOLUTE), (uint)d.firstParam, (uint)d.secondParam, 0, 0);
                    break;
                case (int)NetworkCommands.MOUSE_RIGHTUP:
                    mouse_event((uint)(MOUSEEVENTF_RIGHTUP | MOUSEEVENTF_ABSOLUTE), (uint)d.firstParam, (uint)d.secondParam, 0, 0);
                    break;
                case (int)NetworkCommands.KEYBOARD_DOWN:
                    keybd_event(commandBytes[4], commandBytes[8], 1 | 0, (UIntPtr)0);
                    break;
                case (int)NetworkCommands.KEYBOARD_UP:
                    keybd_event(commandBytes[4], commandBytes[8], 1 | 2, (UIntPtr)0);
                    break;
                case (int)NetworkCommands.MOUSE_WHEEL_ROTATED1:
                    mouse_event(0x0800, 0, 0, unchecked((uint)150), 0);
                    break;
                case (int)NetworkCommands.MOUSE_WHEEL_ROTATED2:
                    mouse_event(0x0800, 0, 0, unchecked((uint)-150), 0);
                    break;
                case (int)NetworkCommands.RECIVE_FILE:
                    {
                        try
                        {
                            //Коннектимся
                            IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse(serverIP), BitConverter.ToInt32(commandBytes, 4));
                            Socket Connector = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                            Connector.Connect(EndPoint);
                            ReceiveFile(Connector);
                            Connector.Close();
                        }
                        catch
                        { return; }
                    }
                    break;
                case (int)NetworkCommands.STOP_SENDING:
                    {
                        StopSendingScreenEvent();
                    }
                    break;
                case (int)NetworkCommands.START_SENDING:
                    {
                        StartSendingScreenEvent();
                    }
                    break;
                case (int)NetworkCommands.RUN_FILE:
                    {
                        byte[] b = new byte[length - 10];
                        for (int i = 12; i < length; i++)
                            b[i-12] = commandBytes[i];
                        string str = Commands.GetNameFromByte(b, length - 10);
                        string resFilePath = str.Substring(0, str.IndexOf('\0'));
                        System.Diagnostics.Process.Start(resFilePath);
                    }
                    break;
            }
        }

        private static void ReceiveFile(Socket Connector)
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

        public static string GetNameFromByte(byte[] _name, int _iRx)
        {
            System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
            char[] chars = new char[_iRx];
            d.GetChars(_name, 0, _iRx, chars, 0);

            return new string(chars);
        }

        private static void SendMyMessage(Socket socket)
        {
            try
            {
                socket.Send(RawSerialize(data));
            }
            catch { }
        }

        public static void StopSendingScreen(Socket s)
        {
            data.CommandType = (int)NetworkCommands.STOP_SENDING;
            SendMyMessage(s);
        }

        public static void StartSendingScreen(Socket s)
        {
            data.CommandType = (int)NetworkCommands.START_SENDING;
            SendMyMessage(s);
        }

        public static void LeftMouseBtnClick(Socket s)
        {
            data.CommandType = (int)NetworkCommands.MOUSE_LEFTDOWN;
            SendMyMessage(s);
        }

        public static void RightMouseBtnClick(Socket s)
        {
            data.CommandType = (int)NetworkCommands.MOUSE_RIGHTDOWN;
            SendMyMessage(s);
        }

        public static void MouseMove(Socket s, int x, int y)
        {
            data.firstParam = x;
            data.secondParam = y;
            data.CommandType = (int)NetworkCommands.MOUSE_MOVE;
            SendMyMessage(s);
        }

        public static void RightMouseBtnUp(Socket s)
        {
            data.CommandType = (int)NetworkCommands.MOUSE_RIGHTUP;
            SendMyMessage(s);
        }

        public static void LeftMouseBtnUp(Socket s)
        {
            data.CommandType = (int)NetworkCommands.MOUSE_LEFTUP;
            SendMyMessage(s);
        }

        public static void KeyDown(Socket s, int keyValue, int keyCode)
        {
            data.CommandType = 5;
            data.firstParam = keyValue;
            data.secondParam = keyCode;

            SendMyMessage(s);
        }

        public static void KeyUp(Socket s, int keyValue, int keyCode)
        {
            data.CommandType = 6;
            data.firstParam = keyValue;
            data.secondParam = keyCode;

            SendMyMessage(s);
        }

        public static void MouseWheel(Socket s, int value)
        {
            if (value > 0)
                data.CommandType = (int)NetworkCommands.MOUSE_WHEEL_ROTATED1;
            else
                data.CommandType = (int)NetworkCommands.MOUSE_WHEEL_ROTATED2;

            SendMyMessage(s);
        }

        public static void SendFile(Socket s)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                SendFile(s, ofd.FileName);
            }
        }

        public static void SendFile(Socket s, string fileName)
        {
            countOfListener--;
            data.CommandType = (int)NetworkCommands.RECIVE_FILE;
            data.firstParam = countOfListener;
            SendMyMessage(s);

            //Создаем Listener на порт "countOfListener"
            TcpListener Listener = new TcpListener(countOfListener);
            //Начинаем прослушку
            Listener.Start();
            //и заведем заранее сокет
            Socket SendSocket;

            SendSocket = Listener.AcceptSocket();

            StringBuilder FileName = new StringBuilder(fileName);
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
            using (var FileStream = new FileStream(fileName, FileMode.Open))
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

        public static void RunFile(Socket s, string fileName)
        {
            StringBuilder FileName = new StringBuilder(fileName);
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

            data.CommandType = (int)NetworkCommands.RUN_FILE;
            SendMyMessage(s);
            s.Send(Encoding.UTF8.GetBytes(resFileName));
        }

    }
}