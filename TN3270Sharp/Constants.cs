using System;
using System.Collections.Generic;
using System.Text;

namespace TN3270Sharp
{
    public enum ControlChars
    {
        SF = 0x1d,
        SFE = 0x29,
        SA = 0x28,
        SBA = 0x11,
        IC = 0x13,
        PT = 0x05,
        RA = 0x3c,
        EUA = 0x12,
        WCCdefault = 0xc3,
        EraseWrite = 0xf5
    }


	// Action ID 
	public enum AID
	{
		None = 0x60,
		Enter = 0x7D,
		PF1 = 0xF1,
		PF2 = 0xF2,
		PF3 = 0xF3,
		PF4 = 0xF4,
		PF5 = 0xF5,
		PF6 = 0xF6,
		PF7 = 0xF7,
		PF8 = 0xF8,
		PF9 = 0xF9,
		PF10 = 0x7A,
		PF11 = 0x7B,
		PF12 = 0x7C,
		PF13 = 0xC1,
		PF14 = 0xC2,
		PF15 = 0xC3,
		PF16 = 0xC4,
		PF17 = 0xC5,
		PF18 = 0xC6,
		PF19 = 0xC7,
		PF20 = 0xC8,
		PF21 = 0xC9,
		PF22 = 0x4A,
		PF23 = 0x4B,
		PF24 = 0x4C,
		PA1 = 0x6C,
		PA2 = 0x6E,
		PA3 = 0x6B,
		Clear = 0x6D
	}
}
