using Rapid.NET;
using Rapid.NET.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherExamplesFramework
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            LaunchMethods.RunFromArgs(args);
        }
    }

    [Script]
    public class ScriptWithNoArgs
    {
        public static void Run()
        {
            Console.WriteLine("Running script from 'Other Examples' project.");
        }
    }

}
