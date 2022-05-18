/*
 * This file is part of https://github.com/roblthegreat/TN3270Sharp
 *
 * Portions of this code may have been adapted or originated from another MIT 
 * licensed project and will be explicitly noted in the comments as needed.
 * 
 * MIT License
 * 
 * Copyright (c) 2020-2021 by Robert J. Lawrence (roblthegreat) and other
 * TN3270Sharp contributors.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 */
using System;
using System.IO;
using System.Net.Sockets;

namespace TN3270Sharp
{
    public class Telnet : IDisposable
    {
        protected TcpClient TcpClient { get; }
        protected Stream Stream { get; }
        protected byte[] BufferBytes { get; set; }
        protected int TotalBytesReadFromBuffer { get; set; } = 0;
        protected bool ConnectionClosed { get; private set; } = false;

        private enum TelnetState : int
        {
            Normal = 0,
            Command = 1,
            SubNegotiation = 2
        }

        public Telnet(TcpClient tcpClient, Stream stream)
        {
            TcpClient = tcpClient;
            Stream = stream;

            BufferBytes = new byte[256];
        }

        public void CloseConnection()
        {
            if (ConnectionClosed == false)
            {
                Stream.Close();
                TcpClient.Close();
                ConnectionClosed = true;
            }
        }

        public void Negotiate()
        {
            Stream.Write(new byte[] { TelnetCommands.IAC, TelnetCommands.DO, TelnetCommands.TERMINAL_TYPE });
            var x = Stream.Read(BufferBytes, 0, BufferBytes.Length);

            Stream.Write(new byte[] { TelnetCommands.IAC, TelnetCommands.SB, TelnetCommands.TERMINAL_TYPE, 0x01, TelnetCommands.IAC, TelnetCommands.SE });
            x = Stream.Read(BufferBytes, 0, BufferBytes.Length);

            Stream.Write(new byte[] { TelnetCommands.IAC, TelnetCommands.DO, TelnetCommands.EOR });
            x = Stream.Read(BufferBytes, 0, BufferBytes.Length);

            Stream.Write(new byte[] { TelnetCommands.IAC, TelnetCommands.DO, TelnetCommands.BINARY });
            x = Stream.Read(BufferBytes, 0, BufferBytes.Length);

            Stream.Write(new Byte[] { TelnetCommands.IAC, TelnetCommands.WILL, TelnetCommands.EOR, TelnetCommands.IAC, TelnetCommands.WILL, TelnetCommands.BINARY });
            x = Stream.Read(BufferBytes, 0, BufferBytes.Length);
        }

        public void UnNegotiate()
        {
            Stream.Write(new byte[] { TelnetCommands.IAC, TelnetCommands.WONT, TelnetCommands.IAC, TelnetCommands.WONT, TelnetCommands.BINARY });
            var x = Stream.Read(BufferBytes, 0, BufferBytes.Length);

            Stream.Write(new byte[] { TelnetCommands.IAC, TelnetCommands.DONT, TelnetCommands.BINARY });
            x = Stream.Read(BufferBytes, 0, BufferBytes.Length);

            Stream.Write(new byte[] { TelnetCommands.IAC, TelnetCommands.DONT, TelnetCommands.EOR });
            x = Stream.Read(BufferBytes, 0, BufferBytes.Length);

            Stream.Write(new byte[] { TelnetCommands.IAC, TelnetCommands.DONT, TelnetCommands.TERMINAL_TYPE });
            x = Stream.Read(BufferBytes, 0, BufferBytes.Length);
        }

        public void Read(Action<byte[]> action)
        {
            while (ConnectionClosed == false && (TotalBytesReadFromBuffer = Stream.Read(BufferBytes, 0, BufferBytes.Length)) != 0)
            {
                action(BufferBytes);
            }
        }

        public void SendScreen(Screen screen)
        {
            SendScreen(screen, screen.InitialCursorPosition.column, screen.InitialCursorPosition.row);
        }

        public void SendScreen(Screen screen, int row, int col)
        {
            DataStream.EraseWrite(Stream);
            Stream.Write(new byte[] { (byte)ControlChars.WCCdefault });

            foreach (Field fld in screen.Fields)
            {
                // tell the terminal where to draw field
                DataStream.SBA(Stream, fld.Row, fld.Column);
                Stream.Write(screen.BuildField(fld));

                var content = fld.Contents;
                if (fld.Name != "")
                {
                    // TODO
                }

                if (content != null && content.Length > 0)
                    Stream.Write(Ebcdic.ASCIItoEBCDIC(content));
            }
            DataStream.SBA(Stream, row, col);
            DataStream.IC(Stream);

            Stream.Write(new byte[] { TelnetCommands.IAC, 0xef });
        }

        public void Dispose()
        {
            CloseConnection();
        }
    }
}
