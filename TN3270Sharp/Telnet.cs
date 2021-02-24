using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace TN3270Sharp
{
    public class Telnet
    {
        protected TcpClient TcpClient { get; }
        protected NetworkStream NetworkStream { get; }
        protected byte[] BufferBytes { get; set; }
        protected int TotalBytesReadFromBuffer { get; set; } = 0;

        private enum TelnetState : int
        {
            Normal = 0,
            Command = 1,
            SubNeg = 2
        }

        public Telnet(TcpClient tcpClient, NetworkStream networkStream)
        {
            TcpClient = tcpClient;
            NetworkStream = networkStream;

            BufferBytes = new byte[256];
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
    }
}
