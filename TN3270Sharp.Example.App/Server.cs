using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static TN3270Sharp.ebcdic;

namespace TN3270Sharp.Example.App
{
    class Server
    {
        TcpListener server = null;
        public Server(string ip, int port)
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
                    Console.WriteLine("Waiting for a connection...");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

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

                //stream.Write(TN3270Sharp.ebcdic.ASCIItoEBCDIC("Hello World"));

                Thread.Sleep(500);
                stream.Write(new byte[] {0xf5,0xc3, 0x11, 0x4b, 0xf0,
                    0x1d, 0xf8});
                stream.Write(new byte[] { 0xE3,0xC5,0xE2,0xE3 });
                
                stream.Write(new byte[] { 0x1d, 0xf0 });


                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    string hex = BitConverter.ToString(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("{1}: Received: {0}", Encoding.ASCII.GetString(EBCDICtoASCII(data), 0, i), Thread.CurrentThread.ManagedThreadId);

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
            stream.Write(new byte[] { 0xff, 0xfd, 0x18 }); 
            var x = stream.Read(bytes, 0, bytes.Length);

            // Term Type Sub-options
            stream.Write(new byte[] { 0xff, 0xfa, 0x18, 0x01, 0xff, 0xf0 });
            x = stream.Read(bytes, 0, bytes.Length);

            //DO EOR
            stream.Write(new byte[] { 0xff, 0xfd, 0x19 });
            x = stream.Read(bytes, 0, bytes.Length);

            // DO Binary
            stream.Write(new byte[] { 0xff, 0xfd, 0x00 });
            x = stream.Read(bytes, 0, bytes.Length);

            // WILL binary, eor
            stream.Write(new Byte[] { 0xff, 0xfb, 0x19, 0xff, 0xfb, 0x00 });
            x = stream.Read(bytes, 0, bytes.Length);
        }
    }
}