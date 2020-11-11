// This file is part of https://github.com/roblthegreat/TN3270Sharp
// Copyright 2020 by Robert J. Lawrence (roblthegreat), licensed under the MIT license. See
// LICENSE in the project root for license information.
//
//  Portions of this code may have originated elsewhere and will be noted in the comments as needed.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace TN3270Sharp
{
    public class Screen
    {
        public string Name { get; set; }
        public List<Field> Fields { get; set; }

        // Initially adapted from https://github.com/racingmars/go3270/blob/master/screen.go
        // Copyright 2020 by Matthew R. Wilson, licensed under the MIT license.
        // GetPosition translates row and col to buffer address control characters.
        public void Show(NetworkStream stream, int row, int col)
        {
            DataStream.EraseWrite(stream);
            stream.Write(new byte[] {
                    (byte)ControlChars.WCCdefault });

            foreach (Field fld in Fields)
            {
                

                // tell the terminal where to draw field
                DataStream.SBA(stream, fld.Row, fld.Column);
                stream.Write(BuildField(fld));


                var content = fld.Contents;
                if (fld.Name != "")
                {
                    // TODO
                }
                
                if (content != null && content.Length > 0 )
                    stream.Write(ebcdic.ASCIItoEBCDIC(content));
            }
            DataStream.SBA(stream, row, col);
            DataStream.IC(stream);

            stream.Write(new byte[] { TelnetCommands.IAC, 0xef });

        }

        public void Show(NetworkStream stream)
        {
            this.Show(stream, 1, 1);
        }

        // Adapted from https://github.com/racingmars/go3270/blob/master/screen.go
        // Copyright 2020 by Matthew R. Wilson, licensed under the MIT license.
        // GetPosition translates row and col to buffer address control characters.
        // Borrowed from racingmars/go3270
        //
        // C#-ification and further changes are Copyright 20200 by Robert J. Lawrence (roblthegreat)
        // licened under the MIT license.
        byte[] BuildField(Field fld)
        {
            List<byte> buffer = new List<byte>();
            if (fld.Color == Colors.DefaultColor && fld.Highlighting == Highlight.DefaultHighlight)
            {
                // We can use a simple SF
                buffer.Add((byte)ControlChars.SF);
                buffer.Add((byte)(
                    (fld.Write == true ? AttribChar.Unprotected : AttribChar.Protected) |
                    (fld.Intensity == true ? AttribChar.Intensity : AttribChar.Normal) |
                    (fld.Hidden == true ? AttribChar.Hidden : AttribChar.Normal)
                    ));
                return buffer.ToArray();
            }

            // otherwise we need to use SFE (SF Extended)
            buffer.Add((byte)ControlChars.SFE);
            int paramCount = 1;

            if (fld.Color != Colors.DefaultColor)
                paramCount++;

            if (fld.Highlighting != Highlight.DefaultHighlight)
                paramCount++;

            buffer.Add((byte)paramCount);

            // Basic field attribute
            buffer.Add(0xc0);
            buffer.Add((byte)(
                (fld.Write == true ? AttribChar.Unprotected : AttribChar.Protected) |
                (fld.Intensity == true ? AttribChar.Intensity : AttribChar.Normal) |
                (fld.Hidden == true ? AttribChar.Hidden : AttribChar.Normal)
                ));

            // Highlighting Attribute
            if (fld.Highlighting != Highlight.DefaultHighlight)
            {
                buffer.Add(0x41);
                buffer.Add(fld.Highlighting);
            }

            // Color attribute
            if (fld.Color != Colors.DefaultColor)
            {
                buffer.Add(0x42);
                buffer.Add((byte)fld.Color);
            }
            return buffer.ToArray();
        }
    }
}
