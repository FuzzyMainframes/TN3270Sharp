// This file is part of https://github.com/roblthegreat/TN3270Sharp
// Copyright 2020 by Robert J. Lawrence (roblthegreat), licensed under the MIT license. See
// LICENSE in the project root for license information.
//
//  Portions of this code may have originated elsewhere and will be noted in the comments as needed.
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static TN3270Sharp.Ebcdic;

namespace TN3270Sharp.Example.App
{
    class ServerTest
    {
        TcpListener server = null;
        public ServerTest(string ip, int port)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            IPAddress localAddr = IPAddress.Parse(ip);
            server = new TcpListener(localAddr, port);
            server.Start();
            StartListener();
        }

        public void StartListener()
        {
            try
            {
                //while (true)
                //{
                    Console.WriteLine("Waiting for a connection from a TN3270 client...");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Client has connected from {0}!", client.Client.RemoteEndPoint.ToString());

                    Thread t = new Thread(new ParameterizedThreadStart(HandleDeivce));
                    t.Start(client);
                //}
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server.Stop();
            }
        }

        public void HandleDeivce(Object obj)
        {

            string PFKeys = "PF3 Exit    PF4 Color Demo    PF5 Highlight Demo    PF6 Form Demo";

            Screen FormScreen = new Screen();
            FormScreen.Fields = new List<Field>()
            {
                new Field() {Row = 1, Column = 28, Contents = "3270 Example Application", Intensity=true},
                new Field() {Row = 3, Column = 1, Contents = "Welcome to the TN3270 form demo screen. Please enter your name."},
                new Field() {Row = 5, Column = 1, Contents = "First Name  . . ."},
                new Field() {Row = 5, Column = 20, Name="fname", Write=true, Highlighting=Highlight.Underscore},
                new Field() {Row = 5, Column = 41}, // "EOF
                new Field() {Row = 6, Column = 1, Contents = "Last Name . . . ."},
                new Field() {Row = 6, Column = 20, Name="lname", Write=true, Highlighting=Highlight.Underscore},
                new Field() {Row = 6, Column = 41 }, // "EOF"
                new Field() {Row = 7, Column = 1, Contents="Password  . . . ."},
                new Field() {Row = 7, Column = 20, Name="password", Write=true, Highlighting=Highlight.Underscore, Hidden=true },
                new Field() {Row = 7, Column = 41}, // "EOF"
                new Field() {Row = 9, Column = 1, Contents="Press"},
                new Field() {Row = 9, Column = 7, Contents="ENTER", Intensity=true},
                new Field() {Row = 9, Column = 13, Contents="to submit your name."},
                new Field() {Row = 11, Column = 1, Intensity = true, Color = Colors.Red, Name="errormsg"},
                new Field() {Row = 23, Column = 1, Contents=PFKeys}
            };



            Screen TitleScreen = new Screen();
            TitleScreen.Fields = new List<Field>()
            {
                new Field() {Row = 1, Column = 28, Contents = "3270 Example Application", Intensity=true},
                new Field() {Row = 3, Column = 16, Contents = "Welcome to the TN3270Sharp example application."},
                new Field() {Row = 5, Column = 2, Contents = "This application is designed to demonstrate some of"},
                new Field() {Row = 6, Column = 2, Contents = "TN3270Sharp, an open source libarary written in C# which"},
                new Field() {Row = 7, Column = 2, Contents = "allows you to write server applications for TN3270 clients"},
                new Field() {Row = 8, Column = 2, Contents = "to run on any machine supported by Microsoft .NET Core,"},
                new Field() {Row = 9, Column = 2, Contents = "without requiring a mainframe."},

                new Field() {Row = 23, Column = 1, Contents=PFKeys}
            };

            Screen ColorScreen = new Screen();
            ColorScreen.Fields = new List<Field>()
            {
                new Field() {Row = 1, Column = 28, Contents = "3270 Example Application", Intensity=true},
                new Field() {Row = 3, Column = 31, Contents = "TN3270 Color Demo"},
                new Field() {Row = 7, Column = 10, Contents = "Default", Color=Colors.DefaultColor},
                new Field() {Row = 8, Column = 10, Contents = "Blue", Color=Colors.Blue},
                new Field() {Row = 9, Column = 10, Contents = "Green", Color=Colors.Green},
                new Field() {Row =10, Column = 10, Contents = "Pink", Color=Colors.Pink},
                new Field() {Row =11, Column = 10, Contents = "Red", Color=Colors.Red},
                new Field() {Row =12, Column = 10, Contents = "Turquoise", Color=Colors.Turquoise},
                new Field() {Row =13, Column = 10, Contents = "White", Color=Colors.White},
                new Field() {Row =14, Column = 10, Contents = "Yellow", Color=Colors.Yellow},
                new Field() {Row = 23, Column = 1, Contents=PFKeys}
            };

            Screen HighlightScreen = new Screen();
            HighlightScreen.Fields = new List<Field>()
            {
                new Field() {Row = 1, Column = 28, Contents = "3270 Example Application", Intensity=true},
                new Field() {Row = 3, Column = 29, Contents = "TN3270 Highlight Demo"},
                new Field() {Row = 7, Column = 10, Contents = "Default", Highlighting=Highlight.DefaultHighlight},
                new Field() {Row = 8, Column = 10, Contents = "Blink", Highlighting=Highlight.Blink},
                new Field() {Row = 8, Column = 16},
                new Field() {Row = 9, Column = 10, Contents = "Reverse Video",Highlighting=Highlight.ReverseVideo},
                new Field() {Row = 9, Column = 24},
                new Field() {Row =10, Column = 10, Contents = "Underscore", Highlighting=Highlight.Underscore},
                new Field() {Row = 10, Column =21},
                new Field() {Row = 23, Column = 1, Contents=PFKeys}
            };


            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();

            string data = null;
            Byte[] bytes = new Byte[256];

            NegotiateTelnet(stream, bytes);

            int i;
            try
            {
                TitleScreen.Show(stream);

                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    string hex = BitConverter.ToString(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("{1}: Received: {0}", hex, Thread.CurrentThread.ManagedThreadId);
                    AID recvdAID = (AID)bytes[0];
                    Console.WriteLine("AID: {0}  [ {1} ]", recvdAID.ToString("g"), recvdAID.ToString("d"));
                    Console.WriteLine("Cusrsor Location: {0}", BitConverter.ToString(bytes, 1, 2));


                    if (recvdAID == AID.PF3)
                    {
                        // Clear the terminal
                        Screen clearScreen = new Screen();
                        clearScreen.Fields = new List<Field>();
                        clearScreen.Show(stream);

                        client.Close();

                        break;
                    }
                    if (recvdAID == AID.PF4)
                    {
                        ColorScreen.Show(stream);
                    }
                    if (recvdAID == AID.PF5)
                    {
                        HighlightScreen.Show(stream);
                    }
                    if (recvdAID == AID.PF6)
                    {
                        FormScreen.Show(stream);
                    }



                    // FIXME -- CLEAN UP AND REFACTOR
                    Response response = new Response();

                    response.ActionID = (AID)bytes[0];

                    response.Cursor[0] = bytes[1];
                    response.Cursor[1] = bytes[2];


                    int x = 0;
                    List<byte> temp = new List<byte>();
                    List<byte[]> thelist = new List<byte[]>();

                    while (bytes[x] != 0xff)
                    {
                        if (bytes[x] != 0x11)
                        {
                            temp.Add(bytes[x]);
                        }
                        else
                        {
                            thelist.Add(temp.ToArray());
                            temp = new List<byte>();
                        }
                        x++;
                    }
                    thelist.Add(temp.ToArray());
                    // END-FIXME -- CLEAN UP AND REFACTOR

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