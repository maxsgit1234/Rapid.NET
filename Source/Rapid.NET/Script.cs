using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Rapid.NET.ScriptMethods;

namespace Rapid.NET
{
    public class Script
    {
        public readonly TypeInfo HostClass;
        public readonly Type ArgumentType;
        public readonly DocumentationAttribute Documentation;
        private readonly MethodInfo _RunMethod;

        public static bool TryCreateFromType(TypeInfo ti, out Script ret)
        {
            ret = null;
            var _RunMethod = ti.GetMethod("Run", 
                BindingFlags.Static | BindingFlags.Public);

            if (_RunMethod == null)
            {
                Warn("Type '" + ti.FullName 
                    + "' does not contain a public static Run method");
                return false;
            }

            ParameterInfo[] parameters = _RunMethod.GetParameters();

            Type argType;
            if (parameters.Length == 0)
                argType = null;
            else if (parameters.Length == 1)
                argType = parameters[0].ParameterType;
            else
            {
                Warn("Run method in '" + ti.FullName
                    + "' accepts more than one argument (only 0 or 1 is allowed)");
                return false;
            }

            ret = new Script(ti, _RunMethod, argType);
            return true;
        }

        private Script(TypeInfo ti, MethodInfo runMethod, Type argType)
        {
            HostClass = ti;
            _RunMethod = runMethod;
            ArgumentType = argType;

            Documentation = ti.GetCustomAttribute<DocumentationAttribute>();
        }

        //private static ConstructorInfo GetScriptConstructor(TypeInfo ti)
        //{
        //    ConstructorInfo[] all = ti.GetConstructors();

        //    ConstructorInfo[] tagged = all
        //        .Where(i => IsTaggedScriptConstructor(i)).ToArray();

        //    if (tagged.Length == 1)
        //        return tagged.Single();
        //    if (tagged.Length > 1)
        //        throw new Exception("Only 0 or 1 constructors may be tagged with "
        //            + "the 'ScriptConstructor' attribute. " + ti.FullName + " has " + tagged.Length);

        //    ConstructorInfo[] ok = all
        //        .Where(i => i.GetParameters().Length <= 1).ToArray();

        //    if (ok.Length == 0)
        //        throw new Exception("Could not find a constructor with "
        //            + "0 or 1 parameters for type " + ti.FullName);
        //    else if (ok.Length > 1)
        //        throw new Exception("Found multiple constructors with "
        //            + "0 or 1 parameters for type " + ti.FullName 
        //            + ". Identify which one should be used by applying "
        //            + "the 'ScriptConstructor' attribute.");
        //    return ok.Single();
        //}

        //private static bool IsTaggedScriptConstructor(ConstructorInfo ci)
        //{
        //    return ci.GetCustomAttribute(typeof(ScriptConstructorAttribute)) != null;
        //}

        public bool HasAttribute(Type attType)
        {
            return HostClass.CustomAttributes.Any(i => i.AttributeType == attType);
        }

        public string FullName { get { return HostClass.FullName; } }

        public void Run(object args = null)
        {
            //if (ArgumentType == null)
            //    _Constructor.Invoke(new object[] { });
            //else
            //{
            //    _Constructor.Invoke(new object[] { args });
            //}

            if (ArgumentType == null)
                _RunMethod.Invoke(null, new object[] { });
            else
            {
                _RunMethod.Invoke(null, new object[] { args });
            }
        }

        public override string ToString()
        {
            return HostClass.Name;
        }
    }
}
