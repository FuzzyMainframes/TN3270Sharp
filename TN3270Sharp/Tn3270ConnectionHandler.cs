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

/* Thanks go to Alexandre Bencz (bencz) for the major re-write of the connection handling*/

using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace TN3270Sharp
{
    public class Tn3270ConnectionHandler : ITn3270ConnectionHandler, IDisposable
    {
        private Telnet Telnet { get; set; }
        private Dictionary<AID, Action> AidActions = new Dictionary<AID, Action>();

        public Tn3270ConnectionHandler(TcpClient tcpClient)
        {
            Telnet = new Telnet(tcpClient, tcpClient.GetStream());
            ResetAidActions();
        }

        //public byte[] GetBufferBytes() => BufferBytes;
        //public int GetTotalBytesReadFromBuffer() => TotalBytesReadFromBuffer;

        public void ShowScreen(Screen screen)
        {
            ShowScreen(screen, true, null, null);
        }

        public void ShowScreen(Screen screen, bool executePredefinedAidActions)
        {
            ShowScreen(screen, executePredefinedAidActions, null, null);
        }

        public void ShowScreen(Screen screen, bool executePredefinedAidActions, Action beforeScreenRenderAction)
        {
            ShowScreen(screen, executePredefinedAidActions, beforeScreenRenderAction, null);
        }

        public void ShowScreen(Screen screen, bool executePredefinedAidActions, Action<AID> screenBufferProcess)
        {
            ShowScreen(screen, executePredefinedAidActions, null, screenBufferProcess);
        }

        public void ShowScreen(Screen screen, bool executePredefinedAidActions, Action beforeScreenRenderAction, Action<AID> screenBufferProcess)
        {
            if (beforeScreenRenderAction != null)
                beforeScreenRenderAction();

            Telnet.SendScreen(screen);

            try
            {
                Telnet.Read((bufferBytes) =>
                {
                    AID recvdAID = (AID)bufferBytes[0];

                    if (executePredefinedAidActions == true && AidActions.ContainsKey(recvdAID) && AidActions[recvdAID] != null)
                    {
                        AidActions[recvdAID]();
                    }

                    Response response = new Response(bufferBytes);
                    response.ParseFieldsScreen(screen);

                    if (screenBufferProcess != null)
                        screenBufferProcess(recvdAID);
                });
            }
            catch (Exception ex)
            {
                CloseConnection();
            }
        }

        public void SetAidAction(AID aidCommand, Action action)
        {
            AidActions[aidCommand] = action;
        }

        public void NegotiateTelnet()
        {
            Telnet.Negotiate();
        }

        public void ResetAidActions()
        {
            AidActions[AID.None] = null;
            AidActions[AID.Enter] = null;
            AidActions[AID.PF1] = null;
            AidActions[AID.PF2] = null;
            AidActions[AID.PF3] = null;
            AidActions[AID.PF4] = null;
            AidActions[AID.PF5] = null;
            AidActions[AID.PF6] = null;
            AidActions[AID.PF7] = null;
            AidActions[AID.PF8] = null;
            AidActions[AID.PF9] = null;
            AidActions[AID.PF10] = null;
            AidActions[AID.PF11] = null;
            AidActions[AID.PF12] = null;
            AidActions[AID.PF13] = null;
            AidActions[AID.PF14] = null;
            AidActions[AID.PF15] = null;
            AidActions[AID.PF16] = null;
            AidActions[AID.PF17] = null;
            AidActions[AID.PF18] = null;
            AidActions[AID.PF19] = null;
            AidActions[AID.PF20] = null;
            AidActions[AID.PF21] = null;
            AidActions[AID.PF22] = null;
            AidActions[AID.PF23] = null;
            AidActions[AID.PF24] = null;
            AidActions[AID.PA1] = null;
            AidActions[AID.PA2] = null;
            AidActions[AID.PA3] = null;
            AidActions[AID.Clear] = null;
        }

        public void CloseConnection()
        {
            Telnet.CloseConnection();
        }

        public void Dispose()
        {
            Telnet.Dispose();
        }
    }
}