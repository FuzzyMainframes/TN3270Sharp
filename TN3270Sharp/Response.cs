// This file is part of https://github.com/roblthegreat/TN3270Sharp
// Copyright 2020 by Robert J. Lawrence (roblthegreat), licensed under the MIT license. See
// LICENSE in the project root for license information.
//
//  Portions of this code may have originated elsewhere and will be noted in the comments as needed.

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
