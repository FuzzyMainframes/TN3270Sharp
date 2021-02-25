using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace TN3270Sharp
{
    public class Telnet : IDisposable
    {
        protected TcpClient TcpClient { get; }
        protected NetworkStream NetworkStream { get; }
        protected byte[] BufferBytes { get; set; }
        protected int TotalBytesReadFromBuffer { get; set; } = 0;
        protected bool ConnectionClosed { get; private set; } = false;

        private enum TelnetState : int
        {
            Normal = 0,
            Command = 1,
            SubNegotiation = 2
        }

        public Telnet(TcpClient tcpClient, NetworkStream networkStream)
        {
            TcpClient = tcpClient;
            NetworkStream = networkStream;

            BufferBytes = new byte[256];
        }

        public void CloseConnection()
        {
            if (ConnectionClosed == false)
            {
                NetworkStream.Close();
                TcpClient.Close();
                ConnectionClosed = true;
            }
        }

        public void Negotiate()
        {
            NetworkStream.Write(new byte[] { TelnetOptions.IAC, TelnetOptions.DO, TelnetOptions.TERMINAL_TYPE });
            var x = NetworkStream.Read(BufferBytes, 0, BufferBytes.Length);

            NetworkStream.Write(new byte[] { TelnetOptions.IAC, TelnetOptions.SB, TelnetOptions.TERMINAL_TYPE, 0x01, TelnetOptions.IAC, TelnetOptions.SE });
            x = NetworkStream.Read(BufferBytes, 0, BufferBytes.Length);

            NetworkStream.Write(new byte[] { TelnetOptions.IAC, TelnetOptions.DO, TelnetOptions.EOR });
            x = NetworkStream.Read(BufferBytes, 0, BufferBytes.Length);

            NetworkStream.Write(new byte[] { TelnetOptions.IAC, TelnetOptions.DO, TelnetOptions.BINARY });
            x = NetworkStream.Read(BufferBytes, 0, BufferBytes.Length);

            NetworkStream.Write(new Byte[] { TelnetOptions.IAC, TelnetOptions.WILL, TelnetOptions.EOR, TelnetOptions.IAC, TelnetOptions.WILL, TelnetOptions.BINARY });
            x = NetworkStream.Read(BufferBytes, 0, BufferBytes.Length);
        }

        public void UnNegotiate()
        {
            NetworkStream.Write(new byte[] { TelnetOptions.IAC, TelnetOptions.WONT, TelnetOptions.IAC, TelnetOptions.WONT, TelnetOptions.BINARY });
            var x = NetworkStream.Read(BufferBytes, 0, BufferBytes.Length);

            NetworkStream.Write(new byte[] { TelnetOptions.IAC, TelnetOptions.DONT, TelnetOptions.BINARY });
            x = NetworkStream.Read(BufferBytes, 0, BufferBytes.Length);

            NetworkStream.Write(new byte[] { TelnetOptions.IAC, TelnetOptions.DONT, TelnetOptions.EOR });
            x = NetworkStream.Read(BufferBytes, 0, BufferBytes.Length);

            NetworkStream.Write(new byte[] { TelnetOptions.IAC, TelnetOptions.DONT, TelnetOptions.TERMINAL_TYPE });
            x = NetworkStream.Read(BufferBytes, 0, BufferBytes.Length);
        }

        public void Read(Action<byte[]> action)
        {
            while (ConnectionClosed == false && (TotalBytesReadFromBuffer = NetworkStream.Read(BufferBytes, 0, BufferBytes.Length)) != 0)
            {
                action(BufferBytes);
            }
        }

        public void SendScreen(Screen screen)
        {
            SendScreen(screen, 1, 1);
        }

        public void SendScreen(Screen screen, int row, int col)
        {
            DataStream.EraseWrite(NetworkStream);
            NetworkStream.Write(new byte[] { (byte)ControlChars.WCCdefault });

            foreach (Field fld in screen.Fields)
            {
                // tell the terminal where to draw field
                DataStream.SBA(NetworkStream, fld.Row, fld.Column);
                NetworkStream.Write(screen.BuildField(fld));

                var content = fld.Contents;
                if (fld.Name != "")
                {
                    // TODO
                }

                if (content != null && content.Length > 0)
                    NetworkStream.Write(Ebcdic.ASCIItoEBCDIC(content));
            }
            DataStream.SBA(NetworkStream, row, col);
            DataStream.IC(NetworkStream);

            NetworkStream.Write(new byte[] { TelnetOptions.IAC, 0xef });
        }

        public void Dispose()
        {
            CloseConnection();
        }
    }
}
