using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Remote_Admin.Model
{
    class RemoteComputer
    {

        public string ComputerName { get; private set; }
        public Bitmap ComputerScreen { get; private set; }
        public Socket clientSocket { get; private set; }

    //  public delegate void RemoteComputerScreenChanged();
    // public event RemoteComputerScreenChanged RemoteComputerScreenHasChanged;

    public RemoteComputer(string _computerName, Socket _clientSocket)
        {
            ComputerName = _computerName;
            clientSocket = _clientSocket;

            Thread ReciveScreenImageThread = new Thread(ReciveScreenImage);
            ReciveScreenImageThread.IsBackground = true;
            ReciveScreenImageThread.Start(); //запускаем поток
        }

        public void ReciveScreenImage()
        {
            byte[] bytes = new byte[10000];

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
                    // RemoteComputerScreenHasChanged();
                }
                catch { }


            }
        }
    }
}
