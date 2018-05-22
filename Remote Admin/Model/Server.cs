using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Remote_Admin.Model
{
    public class Server
    {
        public List<RemoteComputer> RemoteComputers { get; private set; }
        private Socket sListener;
        // Объявляем делегат
        public delegate void RemoteComputersListChanged();
        public event RemoteComputersListChanged RemoteComputersListHasChanged;

        public Server()
        {
            RemoteComputers = new List<RemoteComputer>();

            RemoteComputer.RemoteComputerConnectionCloseEvent += RemoteComputerConnectionClose;
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
            byte[] data = new byte[100];

            while (true)
            {
                try
                {
                    // Программа приостанавливается, ожидая входящее соединение
                    Socket handler = sListener.Accept();
                    int iRx = handler.Receive(data);
                    string comp = GetNameFromByte(data, iRx);
                    iRx = handler.Receive(data);
                    string name = GetNameFromByte(data, iRx);
                    iRx = handler.Receive(data);
                    string ip = GetNameFromByte(data, iRx);
                    RemoteComputers.Add(new RemoteComputer(comp, name, ip, handler));

                    RemoteComputersListHasChanged();
                }
                catch { }
            }
        }

        private string GetNameFromByte(byte[] _name, int _iRx)
        {
            System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
            char[] chars = new char[_iRx];
            d.GetChars(_name, 0, _iRx, chars, 0);

            return new string(chars);
        }

        private void RemoteComputerConnectionClose(RemoteComputer r)
        {
            CloseConnections(RemoteComputers.IndexOf(r));
        }

        public void CloseConnections(int id)
        {
            try
            {
                RemoteComputers[id].clientSocket.Send(new byte [] {100});
                RemoteComputers[id].clientSocket.Shutdown(SocketShutdown.Both);
                RemoteComputers[id].clientSocket.Close();
                RemoteComputers.RemoveAt(id);

                
            }
            catch { }

            RemoteComputersListHasChanged();
        }

        public void CloseAllConnections()
        {
            for (int i = 0; i < RemoteComputers.Count; i++)
            {
                CloseConnections(i);
            }
            RemoteComputers.Clear();
            RemoteComputersListHasChanged();

        }

        public void CloseCloseAllConnectionsAndExit()
        {
            CloseAllConnections();
            sListener.Dispose();
        }
    }
}
