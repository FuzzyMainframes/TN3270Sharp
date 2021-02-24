// This file is part of https://github.com/roblthegreat/TN3270Sharp
// Copyright 2020 by Robert J. Lawrence (roblthegreat), licensed under the MIT license. See
// LICENSE in the project root for license information.
//
//  Portions of this code may have originated elsewhere and will be noted in the comments as needed.
using System;
using System.Threading;

namespace TN3270Sharp.Example.App
{
    class Program
    {
        static void Main(string[] args)
        {
            //Thread t = new Thread(delegate ()
            //{
            //    // replace the IP with your system IP Address...
            //    ServerTest myServer = new ServerTest("127.0.0.1", 3270);
            //});
            //t.Start();

            //Console.WriteLine("TN3270Sharp Demo Server");
            //Console.WriteLine("Version 0.0.1");
            //Console.WriteLine("Copyright 2020 by Robert J. Lawrence (roblthegreat)");
            //Console.WriteLine("Edited 2021 by Alexandre Bencz (bencz)");
            //Console.WriteLine("https//github.com/roblthegreat/TN3270Sharp");
            Console.WriteLine("TN3270Sharp Demo Server started and listening on 127.0.0.1 port 3270");
            Console.WriteLine("Control+C to exit...\n\n");

            Test1 test1 = new Test1();
        }
    }
}
