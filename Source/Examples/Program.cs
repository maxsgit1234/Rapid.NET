
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

    public class MyFilteringAttribute : Attribute { }

    
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            LaunchMethods.RunFromArgs(args);
        }
    }

    [Script]
    [MyFiltering]
    public class MySimpleScript
    {
        public MySimpleScript()
        {
            Console.WriteLine("This is a super simple script!");
            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }
    }

    [Script]
    public class MyScriptWithArgs
    {
        public class Config
        {
            public int MyInt = 3;
            public string MyString = "adsf";

            public override string ToString()
            {
                return "MyInt = " + MyInt + " | MyString = " + MyString;
            }
        }

        public MyScriptWithArgs(Config cfg)
        {
            Console.WriteLine("Script was run with argument: " + cfg);
            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }
    }


}
