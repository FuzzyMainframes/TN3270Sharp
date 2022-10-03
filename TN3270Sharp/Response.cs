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

using System;
using System.Collections.Generic;

namespace TN3270Sharp
{
    public class Response
    {
        public AID ActionID { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public Dictionary<byte[], string> Map { get; set; } = new Dictionary<byte[], string>();
        public byte[] BufferBytes { get; }

        public Response(byte[] bufferBytes) 
        {
            BufferBytes = bufferBytes;

            ReadAction();
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

            var nRow = (address % 80);
            var nCol = ((address - nRow) / 80);

            return new Tuple<int, int>(nCol, nRow);
        }

        public void ParseFieldsScreen(Screen screen)
        {
            var inField = false;
            var fieldBytes = new List<byte>();
            Tuple<int, int> fieldPosition = null;

            for (var i = 0; i < BufferBytes.Length; i++)
            {
                var b = BufferBytes[i];
                if (b == 0xff)
                {
                    if (i + 1 >= BufferBytes.Length)
                        return;

                    if (BufferBytes[i + 1] == 0xef && fieldPosition != null)
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
                    if (inField && fieldPosition != null)
                    {
                        var data = Ebcdic.EBCDICtoASCII(fieldBytes.ToArray());

                        //Console.WriteLine("Field {0}/{1}: {2}", fieldPosition.Item1, fieldPosition.Item2, data);
                        screen.SetFieldValue(fieldPosition.Item1 + 1, fieldPosition.Item2, data);
                    }

                    fieldBytes.Clear();
                    inField = true;

                    fieldPosition = ReadPosition(BufferBytes[++i], BufferBytes[++i]);

                    continue;
                }
                if (!inField)
                    continue;

                fieldBytes.Add(b);
            }
        }
    }
}
