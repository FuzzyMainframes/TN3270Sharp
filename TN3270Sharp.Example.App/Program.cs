using System;
using System.Threading;

namespace TN3270Sharp.Example.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread t = new Thread(delegate ()
            {
                // replace the IP with your system IP Address...
                ServerTest myServer = new ServerTest("127.0.0.1", 9999);
            });
            t.Start();

            Console.WriteLine("TN3270Sharp Demo Server");
            Console.WriteLine("Version 0.0.1");
            Console.WriteLine("Copyright 2020 by Robert J. Lawrence (roblthegreat)");
            Console.WriteLine("https//github.com/roblthegreat/TN3270Sharp");
            Console.WriteLine("\n\nTN3270Sharp Demo Server started and listening on 127.0.0.1 port 9999");
            Console.WriteLine("Control+C to exit...\n\n");
        }
    }
}
