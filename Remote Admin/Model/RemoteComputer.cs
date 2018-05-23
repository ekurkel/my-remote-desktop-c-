using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Remote_Admin.Model
{
    public class RemoteComputer
    {
        public string ComputerUser { get; private set; }
        public string ClientIP { get; private set; }
        public string ComputerName { get; private set; }
        public Bitmap ComputerScreen { get; private set; }
        public Socket clientSocket { get; private set; }


        public delegate void RemoteComputerScreenDelegate();
        public delegate void RemoteComputerConnectionDelegate(RemoteComputer r);
        public event RemoteComputerScreenDelegate RemoteComputerScreenHasChangedEvent;
        public static event RemoteComputerConnectionDelegate RemoteComputerConnectionCloseEvent;

        public RemoteComputer(string _computerName, string _computerUser, string _ip, Socket _clientSocket)
        {
            ComputerName = _computerName;
            ComputerUser = _computerUser;
            ClientIP = _ip;
            clientSocket = _clientSocket;

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
    }
}
