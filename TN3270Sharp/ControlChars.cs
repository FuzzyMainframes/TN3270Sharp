using System;
using System.Collections.Generic;
using System.Text;

namespace TN3270Sharp
{
    public static class ControlChars
    {
        public const byte SF = 0x1d;
        public const byte SFE = 0x29;
        public const byte SA = 0x28;
        public const byte SBA = 0x11;
        public const byte IC = 0x13;
        public const byte PT = 0x05;
        public const byte RA = 0x3c;
        public const byte EUA = 0x12;
        public const byte WCCdefault = 0xc3;
        public const byte EraseWrite = 0xf5;

    }
}
