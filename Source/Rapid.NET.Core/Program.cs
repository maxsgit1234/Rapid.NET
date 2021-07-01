using Rapid.NET.Wpf;
using System;

namespace Rapid.NET.Core
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            LaunchMethods.PrintHello();

            LaunchMethods.RunFromArgs(args);
            Console.WriteLine("Hello World!");
            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }
    }
}
