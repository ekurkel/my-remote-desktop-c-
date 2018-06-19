using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Remote_Admin.Model
{
    public class RemoteComputer
    {
        public string ComputerUser { get; private set; }
        public string ClientIP { get; private set; }
        public string ComputerName { get; private set; }
        public Bitmap ComputerScreen { get; private set; }
        private Socket clientSocket;
        private AesCryptoServiceProvider AES;

        public delegate void RemoteComputerScreenDelegate();
        public delegate void RemoteComputerConnectionDelegate(RemoteComputer r);
        public event RemoteComputerScreenDelegate RemoteComputerScreenHasChangedEvent;
        public static event RemoteComputerConnectionDelegate RemoteComputerConnectionCloseEvent;

        public RemoteComputer(Socket _clientSocket)
        {
            clientSocket = _clientSocket;
            AES = new AesCryptoServiceProvider();
            AES.GenerateKey();
            AES.GenerateIV();

            byte[] data = new byte[1000];

            int lengthOfMessage = clientSocket.Receive(data);
            MessageEncrypt.EncryptAndSendAESKey(clientSocket, data, AES, lengthOfMessage);

            lengthOfMessage = clientSocket.Receive(data);
            ComputerName = CommandMessage.GetNameFromByte(data, lengthOfMessage);
            ComputerName = MessageEncrypt.AESDecrypt(ComputerName, AES);

            lengthOfMessage = clientSocket.Receive(data);
            ComputerUser = CommandMessage.GetNameFromByte(data, lengthOfMessage);
            ComputerUser = MessageEncrypt.AESDecrypt(ComputerUser, AES);

            lengthOfMessage = clientSocket.Receive(data);
            ClientIP = CommandMessage.GetNameFromByte(data, lengthOfMessage);
            ClientIP = MessageEncrypt.AESDecrypt(ClientIP, AES);

            Thread ReciveScreenImageThread = new Thread(ReciveScreenImage);
            ReciveScreenImageThread.IsBackground = true;
            ReciveScreenImageThread.Start(); //запускаем поток
        }

        private void ReciveScreenImage()
        {
            byte[] bytes = new byte[10000000];

            while (true)
            {
                try
                {
                    int bytesRec = clientSocket.Receive(bytes);

                    using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
                    {
                        ms.Write(bytes, 0, bytes.Length);
                        ms.Seek(0, SeekOrigin.Begin);
                        ComputerScreen = (Bitmap)Bitmap.FromStream(ms);
                    }
                    RemoteComputerScreenHasChangedEvent();
                }
                catch
                {
                    if (clientSocket.Connected == false)
                    {
                        clientSocket.Close();
                        RemoteComputerConnectionCloseEvent(this);
                        Thread.CurrentThread.Abort();
                    }
                     
                }
            }
        }

        public void Delete()
        {
            try
            {
                SendMessage(new CommandMessage(NetworkCommands.CLOSE_CONNECTION));
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch { }
        }

        public void SendMessage(CommandMessage m)
        {
            clientSocket.Send(MessageEncrypt.AESEncrypt(m.GetBytes(), AES));
        }

        public void SendFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                SendFile(ofd.FileName);
            }
        }

        private static int countOfListener = 6999; // для сокета... 
        public void SendFile(string fileName)
        {
            countOfListener--;
            SendMessage( new CommandMessage(NetworkCommands.RECIVE_FILE, countOfListener));

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

        public void RunFile(string fileName)
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

            SendMessage( new CommandMessage(NetworkCommands.RUN_FILE));
            clientSocket.Send(Encoding.UTF8.GetBytes(resFileName));
        }

        public void RunCommandLine(string _comandLine)
        {
            SendMessage(new CommandMessage(NetworkCommands.RUN_COMMAND_LINE));
            clientSocket.Send(Encoding.UTF8.GetBytes(_comandLine));
        }
    }
}
