using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rapid.NET
{
    public static class ScriptMethods
    {
        /// <summary>
        /// Executes your script's Run(...) method given the input command 
        /// line arguments. Usage information:
        ///  - If args has length 0, the runUI() delegate will be invoked.
        ///  - If the first argument in wrapped in square brackets "[...]",
        ///    then it is taken to be the name of an attribute to use to filter
        ///    potential scripts from the input assembly.
        ///  - Otherwise, the first (or next) argument is taken to be the name
        ///    of a script to execute, if found in the input assembly.
        ///  - If there is a subsequent argument, it is interpreted as an 
        ///    argument to be supplied to the Run(...) method of your script.
        ///    If the argument is a filename, the file is read as JSON and 
        ///    converted to an object. Otherwise, the argument is interpreted
        ///    as JSON text itself.
        /// 
        /// </summary>
        /// <param name="args">See summary for usage notes.</param>
        /// <param name="runUI">A method to be called to launch a UI to 
        /// manually select a script to execute, if no arguments are supplied.</param>
        /// <param name="readJsonFile">A method to search for and read a file 
        /// containing JSON text representing the input arguments to supply 
        /// to your script's Run(...) method.</param>
        /// <param name="assy">An assembly to search for your script classes.</param>
        public static void RunFromArgs(string[] args,
            Action<List<Script>> runUI, Func<string, string> readJsonFile,
            Assembly assy = null)
        {
            Assembly[] all = assy == null ? null : new Assembly[] { assy };
            RunFromArgs(all, args, runUI, readJsonFile);
        }

        public static void RunFromArgs(Assembly[] assemblies, string[] args,
            Action<List<Script>> runUI, Func<string, string> readJsonFile)
        {
            Print("args.Length = " + args.Length);
            foreach (string arg in args)
                Print(arg);

            if (assemblies == null)
                assemblies = new[] { Assembly.GetEntryAssembly() };

            //args = TryParseAttribute(assy, args, out Type attType);
            args = TryParseAttributeFilter(args, out string attName);

            List<Script> scripts = ListFromAssemblies(assemblies, attName);

            if (args.Length > 0)
                RunDirect(args, scripts, readJsonFile);
            else
                runUI(scripts);
        }

		public static void RunFromArgs(Assembly[] assemblies, string[] args, Action<List<Script>> runUI)
		{
			RunFromArgs(assemblies, args, runUI, ReadFileIfExists);
		}

		public static void RunFromArgs(string[] args, Action<List<Script>> runUI, Assembly assy = null)
		{
			RunFromArgs(args, runUI, ReadFileIfExists, assy);
		}

		private static string ReadFileIfExists(string file)
		{
			if (File.Exists(file))
				return File.ReadAllText(file);
			else
				return null;
		}

        /// <summary>
        /// Find scripts defined in any of the specified assemblies.
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="attributeFilter"></param>
        /// <returns></returns>
		public static List<Script> ListFromAssemblies(
            Assembly[] assemblies, string attributeFilter = null)
        {
            var ret = new List<Script>();
            foreach (Assembly a in assemblies)
                ret.AddRange(ListFromAssembly(a, attributeFilter));

            return ret;
        }

        /// <summary>
        /// Run the script specified in args. Look for its class definition
        /// from among the input list of scripts.
        /// </summary>
        /// <param name="args">The first argument is the name of the script. 
        /// The rest are passed into the execution of the script itself.</param>
        /// <param name="scripts"></param>
        /// <param name="readJsonFile"></param>
        /// <exception cref="Exception"></exception>
        public static void RunDirect(string[] args, List<Script> scripts,
            Func<string, string> readJsonFile)
        {
            if (args.Length == 0)
                throw new Exception("Type-of-script argument missing!");

            Script script = GetMatchingScript(args[0], scripts);

            if (script == null)
                throw new Exception("Unable to find a matching script for " + args[0]);

            object argObj = null;
            if (args.Length > 1)
            {
                string json = readJsonFile(args[1]);
                if (json != null)
                    argObj = JsonObject.Deserialize(json, script.ArgumentType);
                else
                    argObj = JsonObject.Deserialize(args[1], script.ArgumentType);
            }
            
            script.Run(argObj);
        }

        private static string[] TryParseAttributeFilter(string[] args, out string name)
        {
            name = null;
            if (args.Length == 0)
                return args;

            if (!args[0].StartsWith("[") || !args[0].EndsWith("]"))
                return args;

            name = args[0].Trim('[', ']');
            return args.Skip(1).ToArray();
        }

        private static string[] TryParseAttribute(Assembly assy,
            string[] args, out Type attType)
        {
            attType = null;
            if (args.Length == 0)
                return args;

            if (!args[0].StartsWith("[") || !args[0].EndsWith("]"))
                return args;

            string name = args[0].Trim('[', ']');
            attType = assy.GetType(name, true);
            return args.Skip(1).ToArray();
        }


        public static Script GetMatchingScript(string arg, List<Script> scripts)
        {
            string typeName = arg.ToLower();

            List<Script> exactMatches = scripts.Where(i =>
                i.FullName.ToLower() == typeName).ToList();

            if (exactMatches.Count == 1)
                return exactMatches.Single();

            List<Script> matches = scripts.Where(i =>
                i.FullName.ToLower().Contains(typeName)).ToList();

            if (matches.Count == 1)
                return matches.Single();

            return null;
        }

        /// <summary>
        /// Enumerates all valid script classes defined in the supplied assembly.
        /// </summary>
        /// <param name="assembly">An assembly to search for your script classes.</param>
        /// <param name="attType">If non-null, the type of an attribute to use 
        /// to filter your set of scripts. Only script classes with this 
        /// attribute applied will be listed.</param>
        /// <returns></returns>
        public static List<Script> ListFromAssembly(
            Assembly assembly, Type attType = null)
        {
            var result = new List<Script>();

            foreach (TypeInfo tt in assembly.ExportedTypes)
            {
                //TypeInfo ti = null;
                //try
                //{
                TypeInfo ti = tt.GetTypeInfo();
                //}
                //catch (Exception ex)
                //{
                //    Warn("Failed for: " + tt.Name);
                //}

                if (ti == null)
                    continue;

                Attribute attribute = ti.GetCustomAttribute(typeof(ScriptAttribute));
                if (attribute == null)
                    continue;

                if (ti.IsAbstract)
                    continue;

                if (!Script.TryCreateFromType(ti, out Script s))
                    continue;

                if (attType != null && !s.HasAttribute(attType))
                    continue;

                result.Add(s);

            }

            return result;
        }

        /// <summary>
        /// Find script-classes defined in the specified assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="attributeFilter"></param>
        /// <returns></returns>
        public static List<Script> ListFromAssembly(
            Assembly assembly, string attributeFilter = null)
        {
            var result = new List<Script>();

            Type attType = null;
            if (attributeFilter != null)
                attType = assembly.GetType(attributeFilter, false);

            foreach (Type tt in assembly.ExportedTypes)
            {
                TypeInfo ti = tt.GetTypeInfo();

                if (ti == null)
                    continue;

                Attribute attribute = ti.GetCustomAttribute(typeof(ScriptAttribute));
                if (attribute == null)
                    continue;

                if (ti.IsAbstract)
                    continue;

                if (!Script.TryCreateFromType(ti, out Script s))
                    continue;

                if (attType != null && !s.HasAttribute(attType))
                    continue;

                result.Add(s);

            }

            return result;
        }

        #region Simple static logging helpers...

        public static Action<string> PrintMethod = null;
        public static void Print(string s)
        {
            PrintMethod?.Invoke(s);
        }

        public static Action<string> WarnMethod = null;
        public static void Warn(string s)
        {
            WarnMethod?.Invoke(s);
        }

        #endregion
    }

}
