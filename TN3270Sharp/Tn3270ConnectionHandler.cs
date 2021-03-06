using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

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

                //while (ConnectionClosed == false && (TotalBytesReadFromBuffer = NetworkStream.Read(BufferBytes, 0, BufferBytes.Length)) != 0)
                //{
                //    if(BufferBytes[0] == TelnetOptions.IAC)
                //    {
                //        Console.WriteLine("IAC");
                //    }
                //
                //    AID recvdAID = (AID)BufferBytes[0];
                //
                //    if (executePredefinedAidActions == true && AidActions.ContainsKey(recvdAID) && AidActions[recvdAID] != null)
                //    {
                //        AidActions[recvdAID]();
                //        if (ConnectionClosed == true)
                //            break;
                //    }
                //
                //    Response response = new Response(BufferBytes);
                //    response.ParseFieldsScreen(screen);
                //
                //    if(screenBufferProcess != null)
                //        screenBufferProcess(recvdAID);
                //}
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