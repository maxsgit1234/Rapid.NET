
using Rapid.NET;
using Rapid.NET.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Examples
{
    //class Program
    //{
    //    [STAThread]
    //    static void Main(string[] args)
    //    {
    //        LaunchMethods.RunFromArgs(args);

    //        Console.WriteLine("Press ENTER to exit.");
    //        Console.ReadLine();
    //    }
    //}

    

    
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ScriptMethods.PrintMethod = TimestampedPrinter.Print;
            ScriptMethods.WarnMethod = s => TimestampedPrinter.Print("WARNING: " + s);

            LaunchMethods.RunFromArgs(args);

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }

    }


}
