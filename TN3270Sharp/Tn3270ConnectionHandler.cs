using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace TN3270Sharp
{
    public class Tn3270ConnectionHandler : ITn3270ConnectionHandler, IDisposable
    {
        private TcpClient TcpClient { get; }
        private NetworkStream NetworkStream { get; }

        private byte[] BufferBytes { get; set; }
        private int TotalBytesReadFromBuffer { get; set; } = 0;
        private Dictionary<AID, Action> AidActions = new Dictionary<AID, Action>();
        private bool ConnectionClosed { get; set; } = false;

        public Tn3270ConnectionHandler(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
            NetworkStream = tcpClient.GetStream();

            BufferBytes = new byte[256];
        }

        public byte[] GetBufferBytes() => BufferBytes;
        public int GetTotalBytesReadFromBuffer() => TotalBytesReadFromBuffer;

        public void ShowScreen(Screen screen)
        {
            ShowScreen(screen, true, null);
        }

        public void ShowScreen(Screen screen, bool executePredefinedAidActions, Action<AID> screenBufferProcess)
        {
            screen.Show(NetworkStream);

            try
            {
                while (ConnectionClosed == false && (TotalBytesReadFromBuffer = NetworkStream.Read(BufferBytes, 0, BufferBytes.Length)) != 0)
                {
                    AID recvdAID = (AID)BufferBytes[0];

                    if (executePredefinedAidActions == true && AidActions.ContainsKey(recvdAID))
                        AidActions[recvdAID]();
                    screenBufferProcess(recvdAID);


                    //
                    Response response = new Response();

                    response.ActionID = (AID)BufferBytes[0];

                    response.Cursor[0] = BufferBytes[1];
                    response.Cursor[1] = BufferBytes[2];


                    int x = 0;
                    List<byte> temp = new List<byte>();
                    List<byte[]> thelist = new List<byte[]>();

                    while (BufferBytes[x] != 0xff)
                    {
                        if (BufferBytes[x] != 0x11)
                        {
                            temp.Add(BufferBytes[x]);
                        }
                        else
                        {
                            thelist.Add(temp.ToArray());
                            temp = new List<byte>();
                        }
                        x++;
                    }
                    thelist.Add(temp.ToArray());
                }
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
            // Telnet Negotiation

            // Term Type
            NetworkStream.Write(new byte[] { TelnetCommands.IAC, TelnetCommands.DO, TelnetCommands.TERMINAL_TYPE });
            var x = NetworkStream.Read(BufferBytes, 0, BufferBytes.Length);

            // Term Type Sub-options
            NetworkStream.Write(new byte[] { TelnetCommands.IAC, TelnetCommands.SB, TelnetCommands.TERMINAL_TYPE, 0x01, TelnetCommands.IAC, TelnetCommands.SE });
            x = NetworkStream.Read(BufferBytes, 0, BufferBytes.Length);

            //DO EOR
            NetworkStream.Write(new byte[] { TelnetCommands.IAC, TelnetCommands.DO, TelnetCommands.EOR });
            x = NetworkStream.Read(BufferBytes, 0, BufferBytes.Length);

            // DO Binary
            NetworkStream.Write(new byte[] { TelnetCommands.IAC, TelnetCommands.DO, TelnetCommands.BINARY });
            x = NetworkStream.Read(BufferBytes, 0, BufferBytes.Length);

            // WILL binary, eor
            NetworkStream.Write(new Byte[] { TelnetCommands.IAC, TelnetCommands.WILL, TelnetCommands.EOR, TelnetCommands.IAC, TelnetCommands.WILL, TelnetCommands.BINARY });
            x = NetworkStream.Read(BufferBytes, 0, BufferBytes.Length);
        }

        //public void ResetAidActions()
        //{
        //    AidActions[AID.None] = null;
        //    AidActions[AID.Enter] = null;
        //    AidActions[AID.PF1] = null;
        //    AidActions[AID.PF2] = null;
        //    AidActions[AID.PF3] = null;
        //    AidActions[AID.PF4] = null;
        //    AidActions[AID.PF5] = null;
        //    AidActions[AID.PF6] = null;
        //    AidActions[AID.PF7] = null;
        //    AidActions[AID.PF8] = null;
        //    AidActions[AID.PF9] = null;
        //    AidActions[AID.PF10] = null;
        //    AidActions[AID.PF11] = null;
        //    AidActions[AID.PF12] = null;
        //    AidActions[AID.PF13] = null;
        //    AidActions[AID.PF14] = null;
        //    AidActions[AID.PF15] = null;
        //    AidActions[AID.PF16] = null;
        //    AidActions[AID.PF17] = null;
        //    AidActions[AID.PF18] = null;
        //    AidActions[AID.PF19] = null;
        //    AidActions[AID.PF20] = null;
        //    AidActions[AID.PF21] = null;
        //    AidActions[AID.PF22] = null;
        //    AidActions[AID.PF23] = null;
        //    AidActions[AID.PF24] = null;
        //    AidActions[AID.PA1] = null;
        //    AidActions[AID.PA2] = null;
        //    AidActions[AID.PA3] = null;
        //    AidActions[AID.Clear] = null;
        //}

        public void CloseConnection()
        {
            if (ConnectionClosed == false)
            {
                NetworkStream.Close();
                TcpClient.Close();
                ConnectionClosed = true;
            }
        }

        public void Dispose()
        {
            CloseConnection();
        }
    }
}