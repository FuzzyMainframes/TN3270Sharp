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

namespace TN3270Sharp
{
    public static class TelnetCommands
    {
        public const byte BINARY = 0;
        public const byte TERMINAL_TYPE = 0x18;
        public const byte EOR = 0x19;
        public const byte TN3270_REGIME = 0x1d;
        public const byte SE = 0xf0;
        public const byte NOP = 0xf1;
        public const byte DM = 0xf2;
        public const byte BRK = 0xf3;
        public const byte IP = 0xf4;
        public const byte AO = 0xf5;
        public const byte AYT = 0xf6;
        public const byte EC = 0xf7;
        public const byte EL = 0xf8;
        public const byte GA = 0xf9;
        public const byte SB = 0xfa;
        public const byte WILL = 0xfb;
        public const byte WONT = 0xfc;
        public const byte DO = 0xfd;
        public const byte DONT = 0xfe;
        public const byte IAC = 0xff;
    }
}
