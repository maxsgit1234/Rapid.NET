using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rapid.NET
{
    public class ScriptMethods
    {
        public static void RunFromArgs(string[] args, 
            Action<List<Script>> runUI, Assembly assy = null)
        {
            Console.WriteLine("args.Length = " + args.Length);
            foreach (string arg in args)
                Console.WriteLine(arg);

            if (assy == null)
                assy = Assembly.GetEntryAssembly();

            args = TryParseAttribute(assy, args, out Type attType);

            List<Script> scripts = ListFromAssembly(assy, attType);

            if (args.Length > 0)
                RunDirect(args, scripts);
            else
                runUI(scripts);
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

        public static void RunDirect(string[] args, List<Script> scripts)
        {
            if (args.Length == 0)
                throw new Exception("Type-of-script argument missing!");

            Script script = GetMatchingScript(args[0], scripts);

            if (script == null)
                throw new Exception("Unable to find a matching script for " + args[0]);

            object argObj = null;
            if (args.Length > 1)
                argObj = JsonObj.Deserialize(args[1], script.ArgumentType);
            
            script.Run(argObj);
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

        public static List<Script> ListFromAssembly(
            Assembly assembly, Type attType = null)
        {
            var result = new List<Script>();

            foreach (TypeInfo tt in assembly.ExportedTypes)
            {
                TypeInfo ti = null;
                try
                {
                    ti = tt.GetTypeInfo();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed for: " + tt.Name);
                }

                if (ti == null)
                    continue;

                Attribute attribute = ti.GetCustomAttribute(typeof(ScriptAttribute));
                if (attribute == null)
                    continue;

                if (ti.IsAbstract)
                    continue;

                Script s = new Script(ti);
                if (attType != null && !s.HasAttribute(attType))
                    continue;

                result.Add(s);

            }

            return result;
        }

    }
}
