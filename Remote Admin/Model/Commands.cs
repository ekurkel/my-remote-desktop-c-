using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Remote_Admin.Model
{

    [StructLayout(LayoutKind.Sequential)]
    struct Data
    {
        public int Button;
        public int x;
        public int y;
    }

    public static class Commands
    {

        private static Data data;

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

        private const int MOUSE_MOVE = 0;
        private const int MOUSE_LEFTDOWN = 1;
        private const int MOUSE_RIGHTDOWN = 2;
        private const int MOUSE_LEFTUP = 3;
        private const int MOUSE_RIGHTUP = 4;
        private const int KEYBOARD_DOWN = 5;
        private const int KEYBOARD_UP = 6;
        private const int MOUSE_WHEEL_ROTATED1 = 7;
        private const int MOUSE_WHEEL_ROTATED2 = 8;
        private const int RECIVE_FILE = 10;

        public static void ExecuteCommand(byte[] commandBytes, string serverIP)
        {
            Data d = new Data();

            d.Button = BitConverter.ToInt32(commandBytes, 0);
            d.x = BitConverter.ToInt32(commandBytes, 4) * 65536 / Screen.PrimaryScreen.Bounds.Width;
            d.y = BitConverter.ToInt32(commandBytes, 8) * 65536 / Screen.PrimaryScreen.Bounds.Height;

            switch (d.Button)
            {
                case MOUSE_MOVE:
                    mouse_event((uint)(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE | 0x2000), (uint)d.x, (uint)d.y, 0, 0);
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
                    keybd_event(commandBytes[4], commandBytes[8], 1 | 0, (UIntPtr)0);
                    break;
                case 6:
                    keybd_event(commandBytes[4], commandBytes[8], 1 | 2, (UIntPtr)0);
                    break;
                case 7:
                    mouse_event(0x0800, 0, 0, unchecked((uint)150), 0);
                    break;
                case 8:
                    mouse_event(0x0800, 0, 0, unchecked((uint)-150), 0);
                    break;
                case 10:
                    {
                        try
                        {
                            //Коннектимся
                            IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse(serverIP), 6999);
                            Socket Connector = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                            Connector.Connect(EndPoint);
                            ReceiveFile(Connector);
                            Connector.Close();
                        }
                        catch
                        { return; }
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
                    SaveFileDialog sfd = new SaveFileDialog();

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (var File = new FileStream(sfd.FileName, FileMode.CreateNew))
                        {
                            //Записываем в файл
                            File.Write(MessageR.ToArray(), 0, MessageR.ToArray().Length);
                        }
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

        private static void SendMyMessage(Socket socket)
        {
            try
            {
                socket.Send(RawSerialize(data));
            }
            catch { }
        }

        public static void LeftMouseBtnClick(Socket s)
        {
            data.Button = MOUSE_LEFTDOWN;
            SendMyMessage(s);
        }

        public static void RightMouseBtnClick(Socket s)
        {
            data.Button = MOUSE_RIGHTDOWN;
            SendMyMessage(s);
        }

        public static void MouseMove(Socket s, int x, int y)
        {
            data.x = x;
            data.y = y;
            data.Button = MOUSE_MOVE;
            SendMyMessage(s);
        }

        public static void RightMouseBtnUp(Socket s)
        {
            data.Button = MOUSE_RIGHTUP;
            SendMyMessage(s);
        }

        public static void LeftMouseBtnUp(Socket s)
        {
            data.Button = MOUSE_LEFTUP;
            SendMyMessage(s);
        }

        public static void KeyDown(Socket s, int keyValue, int keyCode)
        {
            data.Button = 5;
            data.x = keyValue;
            data.y = keyCode;

            SendMyMessage(s);
        }

        public static void KeyUp(Socket s, int keyValue, int keyCode)
        {
            data.Button = 6;
            data.x = keyValue;
            data.y = keyCode;

            SendMyMessage(s);
        }

        public static void MouseWheel(Socket s, int value)
        {
            if (value > 0)
                data.Button = MOUSE_WHEEL_ROTATED1;
            else
                data.Button = MOUSE_WHEEL_ROTATED2;

            SendMyMessage(s);
        }

        public static void SendFile(Socket s)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                data.Button = RECIVE_FILE;
                SendMyMessage(s);

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