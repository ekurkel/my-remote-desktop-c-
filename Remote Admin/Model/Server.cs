using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Remote_Admin.Model
{
    class Server
    {
        public List<RemoteComputer> RemoteComputers { get; private set; }
        private Socket sListener;
        // Объявляем делегат
        public delegate void RemoteComputersListChanged();
        public event RemoteComputersListChanged RemoteComputersListHasChanged;

        public Server()
        {
            RemoteComputers = new List<RemoteComputer>();
            StartServer();
        }

        public void StartServer()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 1991);

            sListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sListener.Bind(ipEndPoint);
            sListener.Listen(30);

            Thread RunThread = new Thread(Run);
            RunThread.IsBackground = true;
            RunThread.Start(); //запускаем поток
        }

        private void Run()
        {
            byte[] name = new byte[30];
            while (true)
            {
                // Программа приостанавливается, ожидая входящее соединение
                Socket handler = sListener.Accept();
                int iRx = handler.Receive(name);
                string recv = GetNameFromByte(name, iRx);
                RemoteComputers.Add(new RemoteComputer(recv, handler));

                RemoteComputersListHasChanged();
            }
        }

        private string GetNameFromByte(byte[] _name, int _iRx)
        {
            System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
            char[] chars = new char[_iRx];
            d.GetChars(_name, 0, _iRx, chars, 0);

            return new string(chars);
        }
    }
}
