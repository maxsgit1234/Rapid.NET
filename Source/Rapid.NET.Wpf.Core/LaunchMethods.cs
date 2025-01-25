using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rapid.NET.Wpf
{
	public static class LaunchMethods
	{
		public static void RunFromArgs(string[] args, Assembly assembly = null)
		{
			ScriptMethods.RunFromArgs(args, RunUI, assembly);
		}

		public static void RunFromArgs(Assembly[] all, string[] args)
		{
			ScriptMethods.RunFromArgs(all, args, RunUI);
		}

		public static void RunUI(List<Script> scripts)
		{
			Window w = new Window();
			var tree = new ScriptLaunchForm(w, scripts);
			w.Content = tree;
			w.ShowDialog();
		}
	}
}
