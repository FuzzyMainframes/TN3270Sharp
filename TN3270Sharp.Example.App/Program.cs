/*
 * This file is part of https://github.com/FuzzyMainframes/TN3270Sharp/TN3270Sharp
 *
 * Portions of this code may have been adapted or originated from another MIT 
 * licensed project and will be explicitly noted in the comments as needed.
 * 
 * MIT License
 * 
 * Copyright (c) 2020, 2021, 2022, by Robert J. Lawrence (roblthegreat) and other
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

namespace TN3270Sharp.Example.App
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("TN3270Sharp Demo Server");
            //Console.WriteLine("Version 0.0.1");
            //Console.WriteLine("Copyright 2020 by Robert J. Lawrence (roblthegreat)");
            //Console.WriteLine("Edited 2021 by Alexandre Bencz (bencz)");
            //Console.WriteLine("https//github.com/roblthegreat/TN3270Sharp");
            Console.WriteLine("TN3270Sharp Demo Server started and listening on 0.0.0.0 port 3270");
            Console.WriteLine("Control+C to exit...\n\n");

            var exampleApp = new Example3270App();
            exampleApp.CreateServer();
        }
    }
}
