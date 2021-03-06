// This file is part of https://github.com/roblthegreat/TN3270Sharp
// Copyright 2020 by Robert J. Lawrence (roblthegreat), licensed under the MIT license. See
// LICENSE in the project root for license information.
//
//  Portions of this code may have originated elsewhere and will be noted in the comments as needed.

using System.Linq;
using System.Collections.Generic;

namespace TN3270Sharp
{
    public class Screen
    {
        public string Name { get; set; }
        public List<Field> Fields { get; set; }


        // Adapted from https://github.com/racingmars/go3270/blob/master/screen.go
        // Copyright 2020 by Matthew R. Wilson, licensed under the MIT license.
        // GetPosition translates row and col to buffer address control characters.
        // Borrowed from racingmars/go3270
        //
        // C#-ification and further changes are Copyright 20200 by Robert J. Lawrence (roblthegreat)
        // licened under the MIT license.
        public byte[] BuildField(Field fld)
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

        public string GetFieldData(string fieldName)
        {
            var field = Fields.Where(x => x.Name == fieldName).FirstOrDefault();
            if (field == null)
                return null;

            return field.Contents;
        }

        public void SetFieldValue(int row, int col, string data)
        {
            var field = Fields.Where(x => x.Row == row && x.Column == col).FirstOrDefault();
            if (field == null)
                return;

            field.Contents = data;
        }

        public void SetFieldValue(string fieldName, string fieldData)
        {
            var field = Fields.Where(x => x.Name == fieldName).FirstOrDefault();
            if (field == null)
                return;

            field.Contents = fieldData;
        }

        public void ClearFieldValue(string fieldName)
        {
            var field = Fields.Where(x => x.Name == fieldName).FirstOrDefault();
            if (field == null)
                return;

            field.Contents = "";
        }
    }
}
