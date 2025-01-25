using Rapid.NET;
using Rapid.NET.Wpf;
using System;
using System.IO;
using System.Reflection;

namespace Rapid.Examples.Framework
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			// If you only want to use scripts defined in the current project,
			// uncomment the line below:
			//LaunchMethods.RunFromArgs(args);

			// If you want to specify which assemblies should be included when looking for scripts:
			string assName = Path.Combine(Environment.CurrentDirectory, "Rapid.Examples.ScriptLibrary.dll");
			Assembly ass = Assembly.LoadFrom(assName);
			Assembly[] all = new[] { ass, Assembly.GetEntryAssembly() };
			LaunchMethods.RunFromArgs(all, args);
		}

	}

	[Script]
	public class FrameworkScriptExample
	{

		public static void Run()
		{
			Console.WriteLine("Hello from a .NET Framework-defined script class!");
		}

	}

}
