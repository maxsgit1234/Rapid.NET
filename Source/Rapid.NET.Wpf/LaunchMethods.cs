using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rapid.NET.Wpf
{
    public class LaunchMethods
    {
        public static void RunFromArgs(string[] args, Assembly assy = null)
        {
            ScriptMethods.RunFromArgs(args, RunUI, assy);
        }
        
        private static void RunUI(List<Script> scripts)
        {
            Window w = new Window();
            var tree = new ScriptSelectorTree(w, scripts, Console.WriteLine);
            w.Content = tree;
            w.ShowDialog();
        }

    }
}
