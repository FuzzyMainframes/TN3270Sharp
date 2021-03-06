// This file is part of https://github.com/roblthegreat/TN3270Sharp
// Copyright 2020 by Robert J. Lawrence (roblthegreat), licensed under the MIT license. See
// LICENSE in the project root for license information.
//
//  Portions of this code may have originated elsewhere and will be noted in the comments as needed.

using System.Text;

namespace TN3270Sharp
{
    public static class Ebcdic
    {
        private static Encoding AsciiEncoding { get; } = Encoding.ASCII;
        private static Encoding EbcdicEncoding { get; set; }

        public static void SetEbcdicEncoding(string encoding)
        {
            EbcdicEncoding = Encoding.GetEncoding(encoding);
        }

        public static byte[] ASCIItoEBCDIC(string asciiString)
        {         
            return Encoding.Convert(AsciiEncoding, EbcdicEncoding, AsciiEncoding.GetBytes(asciiString));
        }

        public static string EBCDICtoASCII(byte[] ebcdicString)
        {  
            return Encoding.ASCII.GetString(Encoding.Convert(EbcdicEncoding, AsciiEncoding, ebcdicString));
        }

        public static byte[] EBCDICtoASCII(string ebcdicString)
        {
            return Encoding.Convert(EbcdicEncoding, AsciiEncoding, EbcdicEncoding.GetBytes(ebcdicString));
        }
    }
}
