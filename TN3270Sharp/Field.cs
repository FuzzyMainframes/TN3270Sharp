/*
 * This file is part of https://github.com/FuzzyMainframes/TN3270Sharp
 *
 * Portions of this code may have been adapted or originated from another MIT 
 * licensed project and will be explicitly noted in the comments as needed.
 * 
 * MIT License
 * 
 * Copyright (c) 2020, 2021, 20022 by Robert J. Lawrence (roblthegreat) and other
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

    // NumericOnly indicates if the client only accepts numeric input.
    public bool NumericOnly { get; set; }

    // Color is the field color. The default value is the default color.
    public Colors Color { get; set; }

    // Highlighting is the highlight attribute for the field. The default value
    // is the default (i.e. no) highlighting.
    public Highlight Highlighting { get; set; }

    // Name is the name of this field, which is used to get the user-entered
    // data. All writeable fields on a screen must have a unique name.
    public string Name { get; set; }

    public Field() 
    {
    }

    public Field(int row, int col)
    {
        Row = row;
        Column = col;
    }
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
public enum Highlight
{
    DefaultHighlight = 0,
    Blink = 0xf1,
    ReverseVideo = 0xf2,
    Underscore = 0xf4,
}
