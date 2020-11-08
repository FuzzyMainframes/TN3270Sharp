using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static TN3270Sharp.ebcdic;


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
            NetworkStream stream = client.GetStream();

            string data = null;
            Byte[] bytes = new Byte[256];

            NegotiateTelnet(stream, bytes);

            int i;
            try
            {
                DataStream.EraseWrite(stream);
                stream.Write(new byte[] {
                    (byte)ControlChars.WCCdefault });
                DataStream.SBA(stream, 1, 28);
                DataStream.SF(stream, 0xe8);
                
                stream.Write(ASCIItoEBCDIC("3270 Example Application"));
                DataStream.SBA(stream, 3, 1);
                DataStream.SF(stream, 0x60);

                stream.Write(ASCIItoEBCDIC("Welcome to the TN3270Sharp example application. Please enter your name."));
                DataStream.SBA(stream, 5, 1);
                DataStream.SF(stream, 0x60); 

                stream.Write(ASCIItoEBCDIC("First Name  . . ."));
                DataStream.SBA(stream, 5, 20);
                DataStream.SFE(stream, 0x02, 0xc0, 0xc1, 0x41, 0xf4);
                DataStream.SBA(stream, 5, 41);
                DataStream.SF(stream, 0x60);

                DataStream.SBA(stream, 6,1);
                DataStream.SF(stream, 0x60);
                stream.Write(ASCIItoEBCDIC("Last Name . . . ."));
                DataStream.SBA(stream, 6, 20);
                DataStream.SFE(stream, 0x02, 0xc0, 0xc1, 0x41, 0xf4);
                DataStream.SBA(stream, 6, 41);
                DataStream.SF(stream, 0x60);

                DataStream.SBA(stream, 7, 1);
                DataStream.SF(stream, 0x60);
                stream.Write(ASCIItoEBCDIC("Password  . . . ."));
                DataStream.SBA(stream, 7, 20);
                DataStream.SF(stream, 0x4d);
                DataStream.SBA(stream, 7, 41);
                DataStream.SF(stream, 0x60);

                DataStream.SBA(stream, 9, 1);
                DataStream.SF(stream, 0x60);
                stream.Write(ASCIItoEBCDIC("Press"));
                DataStream.SBA(stream, 9,7);
                DataStream.SF(stream, 0xe8);
                stream.Write(ASCIItoEBCDIC("enter"));
                DataStream.SBA(stream, 9,13);
                DataStream.SF(stream, 0x60);
                stream.Write(ASCIItoEBCDIC("to submit your name."));
                
                DataStream.SBA(stream, 11, 1);
                DataStream.SFE(stream, 0x02, 0xc0, 0xe8, 0x42, 0xf2);
                
                DataStream.SBA(stream, 23, 1);
                DataStream.SF(stream, 0x60);
                stream.Write(ASCIItoEBCDIC("PF3 Exit"));

                DataStream.SBA(stream, 5, 21);
                DataStream.IC(stream);

                stream.Write(new byte[] { 
                    0xff, 0xef });


                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    string hex = BitConverter.ToString(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("{1}: Received: {0}", hex, Thread.CurrentThread.ManagedThreadId);
                    AID recvdAID = (AID)bytes[0];
                    Console.WriteLine("AID: {0}  [ {1} ]", recvdAID.ToString("g"),recvdAID.ToString("d") );
                    Console.WriteLine("Cusrsor Location: {0}", BitConverter.ToString(bytes, 1, 2));

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