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

namespace TN3270Sharp.Example.App
{
    public class Test1
    {
        private enum ProgramScreen : int
        {
            FormScreen = 0,
            FormScreenInside = 1,
            TitleScreen = 2,
            ColorScreen = 3,
            HighlightScreen = 4 
        }

        public Test1() 
        {
        }

        public void CreateServer()
        {
            Func<bool> closeServerFunction = () =>
            {
                //while (Console.ReadLine() != "-q") ;
                return false;
            };

            Tn3270Server tn3270Server = new Tn3270Server(3270);
            tn3270Server.StartListener(
                closeServerFunction,
                () => {
                    ConsoleColor.Green.WriteLine("New connection...");
                },
                () => {
                    ConsoleColor.Yellow.WriteLine("Closed connection...");
                },
                (tn3270ConnectionHandler) =>
                {
                    Dictionary<ProgramScreen, Screen> screens = DefineScreens();

                    // Screens Actions
                    Action<AID> formScreenAction = null;
                    Action<AID> formScreenInsideAction = null;

                    formScreenAction = (aidReceived) =>
                    {
                        if (aidReceived == AID.Enter)
                        {
                            var fName = screens[ProgramScreen.FormScreen].GetFieldData("fname");
                            var lName = screens[ProgramScreen.FormScreen].GetFieldData("lname");
                            var password = screens[ProgramScreen.FormScreen].GetFieldData("password");

                            // Check for errors...
                            string errorMessage = null;
                            if (String.IsNullOrEmpty(fName))
                                errorMessage = "First Name field is required.";  
                            else if (String.IsNullOrEmpty(lName))
                                errorMessage = "Last Name field is required.";
                            else if (String.IsNullOrEmpty(password))
                                errorMessage = "Password field is required.";

                            if(!String.IsNullOrEmpty(password) && password.Trim().ToUpper() != "ADMIN")
                                errorMessage = "Invalid password.";

                            if (!String.IsNullOrEmpty(errorMessage))
                            {
                                screens[ProgramScreen.FormScreen].SetFieldValue("errormsg", errorMessage);
                                tn3270ConnectionHandler.ShowScreen(screens[ProgramScreen.FormScreen], true, formScreenAction);
                            }
                            else
                            {
                                screens[ProgramScreen.FormScreenInside].SetFieldValue("fname", fName);
                                screens[ProgramScreen.FormScreenInside].SetFieldValue("lname", lName);
                                tn3270ConnectionHandler.ShowScreen(screens[ProgramScreen.FormScreenInside], true, formScreenInsideAction);
                            }
                        }
                    };

                    formScreenInsideAction = (aidReceived) =>
                    {
                        if (aidReceived == AID.Enter)
                        {
                            tn3270ConnectionHandler.ShowScreen(screens[ProgramScreen.FormScreen], true, 
                                () => {
                                    screens[ProgramScreen.FormScreen].ClearFieldValue("fname");
                                    screens[ProgramScreen.FormScreen].ClearFieldValue("lname");
                                    screens[ProgramScreen.FormScreen].ClearFieldValue("password");
                                    screens[ProgramScreen.FormScreen].ClearFieldValue("errormsg");
                                },
                                formScreenAction);
                        };
                    };

                    tn3270ConnectionHandler.SetAidAction(AID.PF3, () => { tn3270ConnectionHandler.CloseConnection(); });
                    tn3270ConnectionHandler.SetAidAction(AID.PF4, () => { tn3270ConnectionHandler.ShowScreen(screens[ProgramScreen.ColorScreen], true, formScreenAction); });
                    tn3270ConnectionHandler.SetAidAction(AID.PF5, () => { tn3270ConnectionHandler.ShowScreen(screens[ProgramScreen.HighlightScreen], true, formScreenAction); });
                    tn3270ConnectionHandler.SetAidAction(AID.PF6, () => { tn3270ConnectionHandler.ShowScreen(screens[ProgramScreen.FormScreen], true, formScreenAction); });

                    tn3270ConnectionHandler.ShowScreen(screens[ProgramScreen.FormScreen], true, formScreenAction);

                    //tn3270ConnectionHandler.ShowScreen(screens[ProgramScreen.FormScreen], true, (aidReceived) =>
                    //{
                    //    //screens[ProgramScreen.FormScreen].Fields
                    //    //                .Where(x => x.Write == true && !String.IsNullOrEmpty(x.Contents))
                    //    //                .ForEach(x =>
                    //    //                {
                    //    //                    Console.WriteLine($"Field {x.Name}: {x.Contents}");
                    //    //                });
                    //
                    //
                    //    if (aidReceived == AID.Enter)
                    //    {
                    //        var fName = screens[ProgramScreen.FormScreen].GetFieldData("fname");
                    //        var lName = screens[ProgramScreen.FormScreen].GetFieldData("lname");
                    //
                    //        screens[ProgramScreen.FormScreenInside].SetFieldValue("fname", fName);
                    //        screens[ProgramScreen.FormScreenInside].SetFieldValue("lname", lName);
                    //        tn3270ConnectionHandler.ShowScreen(screens[ProgramScreen.FormScreenInside]);
                    //    }
                    //});
                });
        }

        private Dictionary<ProgramScreen, Screen> DefineScreens()
        {
            string PFKeys = "PF3 Exit    PF4 Color Demo    PF5 Highlight Demo    PF6 Form Demo";

            Screen FormScreen = new Screen();
            FormScreen.Fields = new List<Field>()
            {
                new Field() {Row = 1, Column = 28, Contents = "3270 Example Application", Intensity=true},
                new Field() {Row = 3, Column = 1, Contents = "Welcome to the TN3270 form demo screen. Please enter your name."},
                new Field() {Row = 5, Column = 1, Contents = "First Name  . . ."},
                new Field() {Row = 5, Column = 20, Name="fname", Write=true, Highlighting=Highlight.Underscore},
                new Field() {Row = 5, Column = 41}, // "EOF
                new Field() {Row = 6, Column = 1, Contents = "Last Name . . . ."},
                new Field() {Row = 6, Column = 20, Name="lname", Write=true, Highlighting=Highlight.Underscore},
                new Field() {Row = 6, Column = 41 }, // "EOF"
                new Field() {Row = 7, Column = 1, Contents="Password  . . . ."},
                new Field() {Row = 7, Column = 20, Name="password", Write=true, Highlighting=Highlight.Underscore, Hidden=true },
                new Field() {Row = 7, Column = 41}, // "EOF"
                new Field() {Row = 9, Column = 1, Contents="Press"},
                new Field() {Row = 9, Column = 7, Contents="ENTER", Intensity=true},
                new Field() {Row = 9, Column = 13, Contents="to submit your name."},
                new Field() {Row = 11, Column = 1, Intensity = true, Color = Colors.Red, Name="errormsg"},
                new Field() {Row = 23, Column = 1, Contents=PFKeys}
            };
            FormScreen.InitialCursorPosition = (5, 21);

            Screen FormScreenInside = new Screen();
            FormScreenInside.Fields = new List<Field>()
            {
                new Field() {Row = 1, Column = 28, Contents = "3270 Example Application", Intensity=true},
                new Field() {Row = 3, Column = 1, Contents = "Thank you for submitting your name. Here's what I know:"},
                new Field() {Row = 5, Column = 1, Contents = "Your first name is"},
                new Field() {Row = 5, Column = 20, Name="fname"},
                new Field() {Row = 6, Column = 1, Contents = "And your last name is"},
                new Field() {Row = 6, Column = 23, Name="lname"},
                new Field() {Row = 11, Column = 1, Intensity = true, Color = Colors.Red, Name="errormsg"},
                new Field() {Row = 23, Column = 1, Contents=PFKeys}
            };

            Screen TitleScreen = new Screen();
            TitleScreen.Fields = new List<Field>()
            {
                new Field() {Row = 1, Column = 28, Contents = "3270 Example Application", Intensity=true},
                new Field() {Row = 3, Column = 16, Contents = "Welcome to the TN3270Sharp example application."},
                new Field() {Row = 5, Column = 2, Contents = "This application is designed to demonstrate some of"},
                new Field() {Row = 6, Column = 2, Contents = "TN3270Sharp, an open source libarary written in C# which"},
                new Field() {Row = 7, Column = 2, Contents = "allows you to write server applications for TN3270 clients"},
                new Field() {Row = 8, Column = 2, Contents = "to run on any machine supported by Microsoft .NET Core,"},
                new Field() {Row = 9, Column = 2, Contents = "without requiring a mainframe."},

                new Field() {Row = 23, Column = 1, Contents=PFKeys}
            };

            Screen ColorScreen = new Screen();
            ColorScreen.Fields = new List<Field>()
            {
                new Field() {Row = 1, Column = 28, Contents = "3270 Example Application", Intensity=true},
                new Field() {Row = 3, Column = 31, Contents = "TN3270 Color Demo"},
                new Field() {Row = 7, Column = 10, Contents = "Default", Color=Colors.DefaultColor},
                new Field() {Row = 8, Column = 10, Contents = "Blue", Color=Colors.Blue},
                new Field() {Row = 9, Column = 10, Contents = "Green", Color=Colors.Green},
                new Field() {Row =10, Column = 10, Contents = "Pink", Color=Colors.Pink},
                new Field() {Row =11, Column = 10, Contents = "Red", Color=Colors.Red},
                new Field() {Row =12, Column = 10, Contents = "Turquoise", Color=Colors.Turquoise},
                new Field() {Row =13, Column = 10, Contents = "White", Color=Colors.White},
                new Field() {Row =14, Column = 10, Contents = "Yellow", Color=Colors.Yellow},
                new Field() {Row = 23, Column = 1, Contents=PFKeys}
            };

            Screen HighlightScreen = new Screen();
            HighlightScreen.Fields = new List<Field>()
            {
                new Field() {Row = 1, Column = 28, Contents = "3270 Example Application", Intensity=true},
                new Field() {Row = 3, Column = 29, Contents = "TN3270 Highlight Demo"},
                new Field() {Row = 7, Column = 10, Contents = "Default", Highlighting=Highlight.DefaultHighlight},
                new Field() {Row = 8, Column = 10, Contents = "Blink", Highlighting=Highlight.Blink},
                new Field() {Row = 8, Column = 16},
                new Field() {Row = 9, Column = 10, Contents = "Reverse Video",Highlighting=Highlight.ReverseVideo},
                new Field() {Row = 9, Column = 24},
                new Field() {Row =10, Column = 10, Contents = "Underscore", Highlighting=Highlight.Underscore},
                new Field() {Row = 10, Column =21},
                new Field() {Row = 23, Column = 1, Contents=PFKeys}
            };

            Dictionary<ProgramScreen, Screen> screens = new Dictionary<ProgramScreen, Screen>();
            screens[ProgramScreen.FormScreen] = FormScreen;
            screens[ProgramScreen.FormScreenInside] = FormScreenInside;
            screens[ProgramScreen.TitleScreen] = TitleScreen;
            screens[ProgramScreen.ColorScreen] = ColorScreen;
            screens[ProgramScreen.HighlightScreen] = HighlightScreen;

            return screens;
        }
    }
}
