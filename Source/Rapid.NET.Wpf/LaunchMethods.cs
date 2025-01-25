using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rapid.NET.Wpf
{
    public class LaunchMethods
    {
        public static void PrintHello()
        {
            Console.WriteLine("Hello from a WPF framework project method!");
        }

        public static void RunFromArgs(Assembly[] assemblies, string[] args)
        {
            ScriptMethods.RunFromArgs(assemblies, args, RunUI, ReadFileIfExists);
        }

        public static void RunFromArgs(string[] args, Assembly assy = null)
        {
            ScriptMethods.RunFromArgs(args, RunUI, ReadFileIfExists, assy);
        }

        private static string ReadFileIfExists(string file)
        {
            if (File.Exists(file))
                return File.ReadAllText(file);
            else
                return null;
        }

        private static void RunUI(List<Script> scripts)
        {
            Window w = new Window();
            var tree = new ScriptLaunchForm(w, scripts);
            w.Content = tree;
            w.ShowDialog();
        }

    }
}
