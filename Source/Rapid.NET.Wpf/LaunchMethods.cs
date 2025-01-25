using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rapid.NET.Wpf
{
	/// <summary>
	/// Methods to facilitate launching scripts (or the WPF-based script-selection UI)
	/// from a Console Application.
	/// </summary>
	public static class LaunchMethods
	{
		/// <summary>
		/// Run the script as specified by the arguments, OR run the WPF-based
		/// script-selection UI, and search for scripts in the single specified assembly.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="assembly">If null, defaults to the entry assembly.</param>
		public static void RunFromArgs(string[] args, Assembly assembly = null)
		{
			ScriptMethods.RunFromArgs(args, RunUI, assembly);
		}

		/// <summary>
		/// Run the script as specified by the arguments, OR run the WPF-based
		/// script-selection UI, and search for scripts in all the specified assemblies.
		/// </summary>
		/// <param name="all"></param>
		/// <param name="args"></param>
		public static void RunFromArgs(Assembly[] all, string[] args)
		{
			ScriptMethods.RunFromArgs(all, args, RunUI);
		}

		/// <summary>
		/// Launch the WPF-based script-selection window and populate with
		/// the specified scripts.
		/// </summary>
		/// <param name="scripts"></param>
		public static void RunUI(List<Script> scripts)
		{
			Window w = new Window();
			var tree = new ScriptLaunchForm(w, scripts);
			w.Content = tree;
			w.ShowDialog();
		}
	}
}
