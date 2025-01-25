using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rapid.NET.Wpf.Core
{
	public static class LaunchMethods
	{
		public static void RunUI(List<Script> scripts)
		{
			Window w = new Window();
			var tree = new ScriptLaunchForm(w, scripts);
			w.Content = tree;
			w.ShowDialog();
		}
	}
}
