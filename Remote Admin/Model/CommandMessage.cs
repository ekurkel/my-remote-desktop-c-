using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Remote_Admin.Model
{
    public class CommandMessage
    {
        public NetworkCommands CommandType;
        public int firstParam;
        public int secondParam;

        public CommandMessage(NetworkCommands _command, int _x = 0,  int _y = 0)
        {
            CommandType = _command;
            firstParam = _x;
            secondParam = _y;
        }

        public CommandMessage(byte[] commandBytes)
        {
            this.CommandType = (NetworkCommands)BitConverter.ToInt32(commandBytes, 0);
            this.firstParam = BitConverter.ToInt32(commandBytes, 4);
            this.secondParam = BitConverter.ToInt32(commandBytes, 8);
        }

        public int GetMouseCoordinatesX()
        {
            return firstParam * 65536 / Screen.PrimaryScreen.Bounds.Width;
        }

        public int GetMouseCoordinatesY()
        {
            return secondParam * 65536 / Screen.PrimaryScreen.Bounds.Height;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Data
        {
            NetworkCommands command;
            int x;
            int y;
            public Data(NetworkCommands _command, int _x, int _y)
            { command = _command; x = _x; y = _y; }
        }
        public byte[] GetBytes()
        {
            Data d = new Data(CommandType, firstParam, secondParam);
            return RawSerialize(d);
        }

        public static string GetNameFromByte(byte[] _name, int _iRx)
        {
            System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
            char[] chars = new char[_iRx];
            d.GetChars(_name, 0, _iRx, chars, 0);

            return new string(chars);
        }

        private byte[] RawSerialize(object anything)
        {
            int rawsize = Marshal.SizeOf(anything);
            byte[] rawdata = new byte[rawsize];
            GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            Marshal.StructureToPtr(anything, handle.AddrOfPinnedObject(), false);
            handle.Free();
            return rawdata;
        }
    }

}
