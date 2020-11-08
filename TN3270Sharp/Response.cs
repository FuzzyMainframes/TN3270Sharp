using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace TN3270Sharp
{
    public class Response
    {
        public AID ActionID { get; set; }
        public byte[] Cursor { get; set; } = new byte[2];
        public int Row { get; set; }
        public int Column { get; set; }

        public Dictionary<byte[], string> Map { get; set; } = new Dictionary<byte[], string>();


    }
}
