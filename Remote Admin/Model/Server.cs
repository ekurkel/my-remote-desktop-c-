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
        public event RemoteComputersListChanged RemoteComputersListChangedEvent;

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

            Thread WaitingConnectionsThread = new Thread(WaitingConnections);
            WaitingConnectionsThread.IsBackground = true;
            WaitingConnectionsThread.Start(); //запускаем поток
        }

        private void WaitingConnections()
        {
            while (true)
            {
                try
                {
                    // Программа приостанавливается, ожидая входящее соединение
                    Socket handler = sListener.Accept();                   
                    RemoteComputers.Add(new RemoteComputer(handler));
                    RemoteComputersListChangedEvent();
                }
                catch { }
            }
        }

        private void RemoteComputerConnectionClose(RemoteComputer r)
        {
            RemoteComputerConnectionClose(RemoteComputers.IndexOf(r));
        }

        public void RemoteComputerConnectionClose(int id)
        {
            try
            {
                RemoteComputers[id].Delete();
                RemoteComputers.RemoveAt(id);            
            }
            catch { }

            RemoteComputersListChangedEvent();
        }

        public void CloseAllConnections()
        {
            for (int i = 0; i < RemoteComputers.Count; i++)
            {
                RemoteComputerConnectionClose(i);
            }
            RemoteComputers.Clear();
            RemoteComputersListChangedEvent();

        }

        public void CloseAllConnectionsAndExit()
        {
            CloseAllConnections();
            sListener.Dispose();
        }
    }
}
