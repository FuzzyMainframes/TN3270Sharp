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
        public int Row { get; set; }
        public int Column { get; set; }
        public Dictionary<byte[], string> Map { get; set; } = new Dictionary<byte[], string>();
        public byte[] BufferBytes { get; }

        public Response(byte[] bufferBytes, Screen screen) 
        {
            BufferBytes = bufferBytes;

            ReadAction();
            ReadFields(screen);
        }

        private void ReadAction()
        {
            ActionID = (AID)BufferBytes[0];
        }

        private Tuple<int, int> ReadPosition(int row, int col)
        {
            var hi = Utils.IODecodes[row] << 6;
            var lo = Utils.IODecodes[col];

            var address = hi | lo;

            int nRow = (address % 80);
            int nCol = ((address - nRow) / 80);

            return new Tuple<int, int>(nCol, nRow);
        }

        private void ReadFields(Screen screen)
        {
            var inField = false;
            var fieldBytes = new List<byte>();
            Tuple<int, int> fieldPosition = null;

            for (int i = 0; i < BufferBytes.Length; i++)
            {
                var b = BufferBytes[i];
                if (b == 0xff)
                {
                    if (i + 1 >= BufferBytes.Length)
                        return;

                    if (BufferBytes[i + 1] == 0xef)
                    {
                        var data = Ebcdic.EBCDICtoASCII(fieldBytes.ToArray());

                        //Console.WriteLine("Field {0}/{1}: {2}", fieldPosition.Item1, fieldPosition.Item2, data);
                        screen.SetFieldValue(fieldPosition.Item1 + 1, fieldPosition.Item2, data);

                        fieldBytes.Clear();

                        return;
                    }
                }
                if(b == 0x11) 
                {
                    // Increment the buffer index
                    i += 1;

                    if (inField == true)
                    {
                        var data = Ebcdic.EBCDICtoASCII(fieldBytes.ToArray());

                        //Console.WriteLine("Field {0}/{1}: {2}", fieldPosition.Item1, fieldPosition.Item2, data);
                        screen.SetFieldValue(fieldPosition.Item1 + 1, fieldPosition.Item2, data);
                    }

                    fieldBytes.Clear();
                    inField = true;

                    fieldPosition = ReadPosition(BufferBytes[i], BufferBytes[++i]);

                    continue;
                }
                if (!inField)
                    continue;

                fieldBytes.Add(b);
            }
        }
    }
}
