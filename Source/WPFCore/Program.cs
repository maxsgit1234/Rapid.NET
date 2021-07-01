using Rapid.NET.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore
{


    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Assembly assy = Assembly.LoadFrom(
                Environment.CurrentDirectory + "\\Examples.exe");

            LaunchMethods.PrintHello();

            LaunchMethods.RunFromArgs(args, assy);
            Console.WriteLine("Hello World!");
            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }
    }
}
