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
            : this("0.0.0.0", port)
        {
        }

        public Tn3270Server(string ipAddress, int port)
            : this(ipAddress, port, "IBM037")
        {
        }

        public Tn3270Server(string ipAddress, int port, string defaultEbcdicEncoding)
        {
            IpAddress = ipAddress;
            Port = port;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Ebcdic.SetEbcdicEncoding(defaultEbcdicEncoding);
        }

        public void StartListener(Action whenHasNewConnection, Action whenConnectionIsClosed, Action<ITn3270ConnectionHandler> handleConnectionAction, CancellationToken cancellationToken)
        {
            var server = new TcpListener(IPAddress.Parse(IpAddress), Port);
            server.Start();
            
            while (!cancellationToken.IsCancellationRequested)
            {
                var client = server.AcceptTcpClient();
                new Thread(() =>
                {
                    whenHasNewConnection();

                    using (var tn3270ConnectionHandler = new Tn3270ConnectionHandler(client))
                    {
                        tn3270ConnectionHandler.NegotiateTelnet();
                        handleConnectionAction(tn3270ConnectionHandler);
                    }

                    whenConnectionIsClosed();
                }).Start(cancellationToken);
            }

            server.Stop();
        }
    }
}
