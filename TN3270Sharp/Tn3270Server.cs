using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TN3270Sharp
{
    public class Tn3270Server
    {
        private string IpAddress { get; set; }
        private int Port { get; set; }

        public Tn3270Server(int port)
            : this("127.0.0.1", port)
        {
        }

        public Tn3270Server(string ipAddress, int port)
            : this(ipAddress, port, "IBM037")
        {

        }

        public Tn3270Server(string ipAddress, int port, string defaultEbcdicEncoding)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Ebcdic.SetEbcdicEncoding(defaultEbcdicEncoding);
        }

        public void StartListener(Func<bool> breakCondition, Action<Func<byte[]>, Func<string>> handleConnectionAction)
        {
            var server = new TcpListener(IPAddress.Parse(IpAddress), Port);
            server.Start();
            
            while (!breakCondition())
            {
                TcpClient client = server.AcceptTcpClient();
                new Thread(() =>
                {
                    string data = null;
                    byte[] bytes = new byte[256];
                    
                    Func<byte[]> getBufferBytes = () =>
                    {
                        return bytes;
                    };

                    Func<string> getData = () =>
                    {
                        return data;
                    };

                    Func<NetworkStream> getNetworkStream = () =>
                    {
                        return client.GetStream();
                    };

                    NegotiateTelnet(client.GetStream(), bytes);

                }).Start();
            }

            server.Stop();
        }

        private static void NegotiateTelnet(NetworkStream stream, byte[] bytes)
        {
            // Telnet Negotiation

            // Term Type
            stream.Write(new byte[] { TelnetCommands.IAC, TelnetCommands.DO, TelnetCommands.TERMINAL_TYPE });
            var x = stream.Read(bytes, 0, bytes.Length);

            // Term Type Sub-options
            stream.Write(new byte[] { TelnetCommands.IAC, TelnetCommands.SB, TelnetCommands.TERMINAL_TYPE, 0x01, TelnetCommands.IAC, TelnetCommands.SE });
            x = stream.Read(bytes, 0, bytes.Length);

            //DO EOR
            stream.Write(new byte[] { TelnetCommands.IAC, TelnetCommands.DO, TelnetCommands.EOR });
            x = stream.Read(bytes, 0, bytes.Length);

            // DO Binary
            stream.Write(new byte[] { TelnetCommands.IAC, TelnetCommands.DO, TelnetCommands.BINARY });
            x = stream.Read(bytes, 0, bytes.Length);

            // WILL binary, eor
            stream.Write(new Byte[] { TelnetCommands.IAC, TelnetCommands.WILL, TelnetCommands.EOR, TelnetCommands.IAC, TelnetCommands.WILL, TelnetCommands.BINARY });
            x = stream.Read(bytes, 0, bytes.Length);
        }
    }
}
