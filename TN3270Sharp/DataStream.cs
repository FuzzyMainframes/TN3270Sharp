using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace TN3270Sharp
{
    public static class DataStream
    {
        public static void EraseWrite(NetworkStream stream)
        {
            stream.Write(new byte[] { (byte)ControlChars.EraseWrite });
        }
        public static void IC(NetworkStream stream)
        {
            stream.Write(new byte[] { (byte)ControlChars.IC });
        }
        public static void SF(NetworkStream stream, byte attributeChar)
        {
            byte[] result = new byte[] { (byte)ControlChars.SF, attributeChar };
            stream.Write(result);
        }

        public static void SFE(NetworkStream stream, byte numOfTypeValuePairs, params byte[] typeValues)
        {
            var result = new byte[(numOfTypeValuePairs*2)+2];
            result[0] = (byte)ControlChars.SFE;
            result[1] = numOfTypeValuePairs;
            typeValues.CopyTo(result, 2);
            stream.Write(result);
        }

        public static void SBA(NetworkStream stream, int row, int col)
        {
            var pos = Utils.GetPosition(row, col);
            byte[] result = new byte[] { (byte)ControlChars.SBA, pos[0], pos[1] };
            stream.Write(result);  
        }

    }
}
