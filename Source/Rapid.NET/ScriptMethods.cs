using System;
using System.Collections.Generic;
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
            Print("args.Length = " + args.Length);
            foreach (string arg in args)
                Print(arg);

            if (assy == null)
                assy = Assembly.GetEntryAssembly();

            args = TryParseAttribute(assy, args, out Type attType);

            List<Script> scripts = ListFromAssembly(assy, attType);

            if (args.Length > 0)
                RunDirect(args, scripts, readJsonFile);
            else
                runUI(scripts);
        }

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
