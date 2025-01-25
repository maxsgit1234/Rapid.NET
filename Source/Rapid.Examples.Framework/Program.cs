using Rapid.NET;
using Rapid.NET.Wpf;
using Rapid.NET.Wpf.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rapid.Examples.Framework
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			string assName = @"C:\Users\Maxx\Documents\GitHub\Rapid.NET\Source\Rapid.Examples.ScriptLibrary\bin\Debug\netstandard2.0\Rapid.Examples.ScriptLibrary.dll";
			Assembly ass = Assembly.LoadFrom(assName);

			Assembly[] all = new[] { ass, Assembly.GetEntryAssembly() };

			ScriptMethods.RunFromArgs(all, args, LaunchMethods.RunUI);

			//LaunchMethods.RunFromArgs(args);
		}

		//private static void RunUI(List<Script> scripts)
		//{
		//	Window w = new Window();
		//	var tree = new ScriptLaunchForm(w, scripts);
		//	w.Content = tree;
		//	w.ShowDialog();
		//}
	}

}
