/*
 * This file is part of https://github.com/roblthegreat/TN3270Sharp
 *
 * Portions of this code may have been adapted or originated from another MIT 
 * licensed project and will be explicitly noted in the comments as needed.
 * 
 * MIT License
 * 
 * Copyright (c) 2020, 2021, 2022 by Robert J. Lawrence (roblthegreat) and other
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

using System.Net.Sockets;

namespace TN3270Sharp;

public class Tn3270ConnectionHandler : ITn3270ConnectionHandler, IDisposable
{
    private Telnet Telnet;
    private Dictionary<AID, Action> AidActions;

    public Tn3270ConnectionHandler(TcpClient tcpClient)
    {
        Telnet = new Telnet(tcpClient, tcpClient.GetStream());
        AidActions = new Dictionary<AID, Action>();
        ResetAidActions();
    }

    //public byte[] GetBufferBytes() => BufferBytes;
    //public int GetTotalBytesReadFromBuffer() => TotalBytesReadFromBuffer;

    public void ShowScreen(Screen screen) 
        => ShowScreen(screen, true, null, null);

    public void ShowScreen(Screen screen, bool executePredefinedAidActions) 
        => ShowScreen(screen, executePredefinedAidActions, null, null);

    public void ShowScreen(Screen screen, bool executePredefinedAidActions, Action beforeScreenRenderAction) 
        => ShowScreen(screen, executePredefinedAidActions, beforeScreenRenderAction, null);

    public void ShowScreen(Screen screen, bool executePredefinedAidActions, Action<AID> screenBufferProcess) 
        => ShowScreen(screen, executePredefinedAidActions, null, screenBufferProcess);

    public void ShowScreen(Screen screen, bool executePredefinedAidActions, Action beforeScreenRenderAction, Action<AID> screenBufferProcess)
    {
        beforeScreenRenderAction?.Invoke();

        Telnet.SendScreen(screen);

        try
        {
            Telnet.Read(bufferBytes =>
            {
                var recvdAID = (AID)bufferBytes[0];

                if (executePredefinedAidActions && AidActions.ContainsKey(recvdAID) && AidActions[recvdAID] != null)
                {
                    AidActions[recvdAID]();
                }

                var response = new Response(bufferBytes);
                response.ParseFieldsScreen(screen);

                screenBufferProcess?.Invoke(recvdAID);
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            CloseConnection();
        }
    }

    public void SetAidAction(AID aidCommand, Action action) => AidActions[aidCommand] = action;

    public void NegotiateTelnet() => Telnet.Negotiate();

    public void ResetAidActions()
    {
        foreach (var aid in Enum.GetValues(typeof(AID)).Cast<AID>())
            AidActions[aid] = null;
    }

    public void CloseConnection() => Telnet.CloseConnection();

    public void Dispose() => Telnet.Dispose();
}