using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static TN3270Sharp.ebcdic;
using TN3270Sharp;

namespace TN3270Sharp.Example.App
{
    class ServerTest
    {
        TcpListener server = null;
        public ServerTest(string ip, int port)
        {
            IPAddress localAddr = IPAddress.Parse(ip);
            server = new TcpListener(localAddr, port);
            server.Start();
            StartListener();
        }

        public void StartListener()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for a connection from a TN3270 client...");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Client has connected from {0}!", client.Client.RemoteEndPoint.ToString());

                    Thread t = new Thread(new ParameterizedThreadStart(HandleDeivce));
                    t.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server.Stop();
            }
        }

        public void HandleDeivce(Object obj)
        {
            TcpClient client = (TcpClient)obj;
            var stream = client.GetStream();
            //string imei = String.Empty;

            string data = null;
            Byte[] bytes = new Byte[256];

            NegotiateTelnet(stream, bytes);

            int i;
            try
            {
                stream.Write(new byte[] {
                    ControlChars.EraseWrite,
                    ControlChars.WCCdefault,
                    ControlChars.SBA, 0x40, 0x5b,
                    ControlChars.SF, 0xe8});

                stream.Write(ASCIItoEBCDIC("3270 Example Application"));

                stream.Write(new byte[] {ControlChars.SBA, 0xc2, 0x60,
                    ControlChars.SF, 0x60});
                stream.Write(ASCIItoEBCDIC("Welcome to the TN3270Sharp example application. Please enter your name."));

                stream.Write(new byte[] {ControlChars.SBA, 0xc5, 0x40,
                    ControlChars.SF, 0x60});
                stream.Write(ASCIItoEBCDIC("First Name  . . ."));

                stream.Write(new byte[] {ControlChars.SBA, 0xc5, 0xd3, 
                    ControlChars.SFE, 0x02, 0xc0, 0xc1, 0x41, 0xf4, 
                    ControlChars.SBA, 0xc5, 0xe8, 
                    ControlChars.SF, 0x60, 
                    ControlChars.SBA, 0xc6, 0x50, 
                    ControlChars.SF, 0x60});
                stream.Write(ASCIItoEBCDIC("Last Name . . . ."));

                stream.Write(new byte[] {
                    ControlChars.SBA, 0xc6, 0xe3, 
                    ControlChars.SFE, 0x02, 0xc0, 0xc1, 0x41, 0xf4, 
                    ControlChars.SBA, 0xc6, 0xf8, 
                    ControlChars.SF, 0x60, 
                    ControlChars.SBA, 0xc7, 0x60, 
                    ControlChars.SF, 0x60});
                stream.Write(ASCIItoEBCDIC("Password . . . ."));

                stream.Write(new byte[] {
                    ControlChars.SBA, 0xc7, 0xf3, 
                    ControlChars.SF, 0x4d, 
                    ControlChars.SBA, 0xc8, 0xc8, 
                    ControlChars.SF, 0x60, 
                    ControlChars.SBA, 0x4a, 0x40, 
                    ControlChars.SF, 0x60, 
                    0xd7, 0x99, 0x85, 0xa2, 0xa2, 
                    ControlChars.SBA, 0x4a, 0xc6, 
                    ControlChars.SF, 0xe8, 
                    0x85, 0x95, 0xa3, 0x85, 0x99, 
                    ControlChars.SBA, 0x4a, 0x4c, 
                    ControlChars.SF, 0x60});
                stream.Write(ASCIItoEBCDIC("Press enter to submit your name."));

                stream.Write(new byte[] {
                    ControlChars.SBA, 0x4c, 0x60, 
                    ControlChars.SFE, 0x02, 0xc0, 0xe8, 0x42, 0xf2, 
                    ControlChars.SBA, 0x5b, 0x60, 
                    ControlChars.SF, 0x60});
                stream.Write(ASCIItoEBCDIC("PF3 Exit"));

                stream.Write(new byte[] {
                    ControlChars.SBA, 0xc5, 0xd4, 
                    ControlChars.IC, 
                    0xff, 0xef });


                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    string hex = BitConverter.ToString(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("{1}: Received: {0}", Encoding.ASCII.GetString(EBCDICtoASCII(data)), Thread.CurrentThread.ManagedThreadId);

                    string str = "Hey Device!";
                    Byte[] reply = System.Text.Encoding.ASCII.GetBytes(str);
                    //stream.Write(reply, 0, reply.Length);
                    Console.WriteLine("{1}: Sent: {0}", str, Thread.CurrentThread.ManagedThreadId);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
                client.Close();
            }
        }








        // NegotiateTelnet will naively (e.g. not checking client responses) negotiate
        // the options necessary for tn3270 on a new telnet connection.
        private static void NegotiateTelnet(NetworkStream stream, byte[] bytes)
        {
            // Telnet Negotiation

            // Term Type
            stream.Write(new byte[] { TelnetCommands.IAC, TelnetCommands.DO, TelnetCommands.TERMINAL_TYPE }); 
            var x = stream.Read(bytes, 0, bytes.Length);

            // Term Type Sub-options
            stream.Write(new byte[] { TelnetCommands.IAC, TelnetCommands.SB, TelnetCommands.TERMINAL_TYPE, 0x01, 
                TelnetCommands.IAC, TelnetCommands.SE });
            x = stream.Read(bytes, 0, bytes.Length);

            //DO EOR
            stream.Write(new byte[] { TelnetCommands.IAC, TelnetCommands.DO, TelnetCommands.EOR });
            x = stream.Read(bytes, 0, bytes.Length);

            // DO Binary
            stream.Write(new byte[] { TelnetCommands.IAC, TelnetCommands.DO, TelnetCommands.BINARY });
            x = stream.Read(bytes, 0, bytes.Length);

            // WILL binary, eor
            stream.Write(new Byte[] { TelnetCommands.IAC, TelnetCommands.WILL, TelnetCommands.EOR, 
                TelnetCommands.IAC, TelnetCommands.WILL, TelnetCommands.BINARY });
            x = stream.Read(bytes, 0, bytes.Length);
        }
    }
}