using System;
using System.Collections.Generic;
using System.Text;

namespace TN3270Sharp
{
    public class Field
    {
        // Row (on the screen) where the field attribute character begins. integer between 1 and 24
        public int Row { get; set; }

        // Column (on the screen) where the field attribute character begins.  integer between 1 and 80
        public int Column { get; set; }

        // The text value to display on the screen
        public string Contents { get; set; }

        // Determine if the contents be edited by end-user
        public bool Write { get; set; }

        // Intense indicates this field should be displayed with high intensity.
        public bool Intensity { get; set; }

        // Hidden indicates the field content should not be displayed (e.g. a
        // password input field).
        public bool Hidden { get; set; }

        // Color is the field color. The default value is the default color.
        public Colors Color { get; set; }
        
        // Highlighting is the highlight attribute for the field. The default value
        // is the default (i.e. no) highlighting.
        public byte Highlighting { get; set; }

        // Name is the name of this field, which is used to get the user-entered
        // data. All writeable fields on a screen must have a unique name.
        public string Name { get; set; }

    }

    // Constants for valid field colors
    public enum Colors
    {
        DefaultColor = 0,
        Blue = 0xf1,
        Red = 0xf2,
        Pink = 0xf3,
        Green = 0xf4,
        Turquoise = 0xf5,
        Yellow = 0xf6,
        White = 0xf7
    }



    // Constants for field highlighting
    public static class Highlight
    {
        public const byte DefaultHighlight = 0;
        public const byte Blink            = 0xf1;
        public const byte ReverseVideo     = 0xf2;
        public const byte Underscore       = 0xf4;
    }
}
