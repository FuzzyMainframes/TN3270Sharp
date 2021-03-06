// This file is part of https://github.com/roblthegreat/TN3270Sharp
// Copyright 2020 by Robert J. Lawrence (roblthegreat), licensed under the MIT license. See
// LICENSE in the project root for license information.
//
//  Portions of this code may have originated elsewhere and will be noted in the comments as needed.

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
