using System;
using System.Collections.Generic;
using System.Text;

namespace TN3270Sharp.Example.App
{
    public class Test1
    {
        public Test1() 
        {
            Func<bool> closeServerFunction = () =>
            {
                while (Console.ReadLine() != "-q") ;
                return true;
            };

            Tn3270Server tn3270Server = new Tn3270Server(3270);
            tn3270Server.StartListener(closeServerFunction,
                (dataByteArray, data) =>
                {
                    
                });
        }
    }
}
