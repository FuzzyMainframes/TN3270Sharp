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

namespace TN3270Sharp;

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

[Flags]
public enum AttribChar
{
    Protected = 0b00100000,
    Unprotected = 0b00000000,
    Numeric = 0b00010000,
    Alpha = 0b00000000,
    Normal = 0b00000000,
    Intensity = 0b00001000,
    Hidden = 0b00001100,
    MDT = 0b000000001
}
