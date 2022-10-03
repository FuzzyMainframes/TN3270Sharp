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


using System.IO;

namespace TN3270Sharp
{
    public static class DataStream
    {
        public static void EraseWrite(Stream stream)
        {
            stream.Write(new[] { (byte)ControlChars.EraseWrite });
        }

        public static void IC(Stream stream)
        {
            stream.Write(new[] { (byte)ControlChars.IC });
        }

        public static void SF(Stream stream, byte attributeChar)
        {
            byte[] result = { (byte)ControlChars.SF, attributeChar };
            stream.Write(result);
        }

        public static void SFE(Stream stream, byte numOfTypeValuePairs, params byte[] typeValues)
        {
            var result = new byte[(numOfTypeValuePairs * 2) + 2];
            result[0] = (byte)ControlChars.SFE;
            result[1] = numOfTypeValuePairs;
            typeValues.CopyTo(result, 2);
            stream.Write(result);
        }

        public static void SBA(Stream stream, int row, int col)
        {
            var pos = Utils.GetPosition(row, col);
            byte[] result = { (byte)ControlChars.SBA, pos[0], pos[1] };
            stream.Write(result);
        }
    }
}
