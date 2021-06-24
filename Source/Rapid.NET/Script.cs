using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rapid.NET
{
    public class Script
    {
        public readonly TypeInfo HostClass;
        public readonly Type ArgumentType;
        public readonly DocumentationAttribute Documentation;
        private readonly ConstructorInfo _Constructor;
        //private readonly MethodInfo _RunMethod;

        public Script(TypeInfo ti)
        {
            HostClass = ti;
            Documentation = ti.GetCustomAttribute<DocumentationAttribute>();

            //_RunMethod = ti.GetMethod("Run", BindingFlags.Static | BindingFlags.Public);
            //if (_RunMethod == null)
            //    throw new ArgumentException("Type '" + ti.FullName
            //        + "' does not contain a public static Run method");

            //ParameterInfo[] parameters = _RunMethod.GetParameters();

            _Constructor = GetScriptConstructor(ti);

            if (_Constructor ==null)
                throw new ArgumentException("Type '" + ti.FullName
                    + "' does not contain a constructor...lol.");

            ParameterInfo[] parameters = _Constructor.GetParameters();

            if (parameters.Length == 0)
                ArgumentType = null;
            else if (parameters.Length == 1)
                ArgumentType = parameters[0].ParameterType;
            else
                throw new ArgumentException("Run method in '" + ti.FullName
                    + "' accepts more than one argument (only 0 or 1 is allowed)");
        }

        private static ConstructorInfo GetScriptConstructor(TypeInfo ti)
        {
            ConstructorInfo[] all = ti.GetConstructors();

            ConstructorInfo[] tagged = all
                .Where(i => IsTaggedScriptConstructor(i)).ToArray();

            if (tagged.Length == 1)
                return tagged.Single();
            if (tagged.Length > 1)
                throw new Exception("Only 0 or 1 constructors may be tagged with "
                    + "the 'ScriptConstructor' attribute. " + ti.FullName + " has " + tagged.Length);

            ConstructorInfo[] ok = all
                .Where(i => i.GetParameters().Length <= 1).ToArray();

            if (ok.Length == 0)
                throw new Exception("Could not find a constructor with "
                    + "0 or 1 parameters for type " + ti.FullName);
            else if (ok.Length > 1)
                throw new Exception("Found multiple constructors with "
                    + "0 or 1 parameters for type " + ti.FullName 
                    + ". Identify which one should be used by applying "
                    + "the 'ScriptConstructor' attribute.");
            return ok.Single();
        }

        private static bool IsTaggedScriptConstructor(ConstructorInfo ci)
        {
            return ci.GetCustomAttribute(typeof(ScriptConstructorAttribute)) != null;
        }

        public bool HasAttribute(Type attType)
        {
            return HostClass.CustomAttributes.Any(i => i.AttributeType == attType);
        }

        public string FullName { get { return HostClass.FullName; } }

        public void Run(object args = null)
        {
            if (ArgumentType == null)
                _Constructor.Invoke(new object[] { });
            else
            {
                _Constructor.Invoke(new object[] { args });
            }

            //if (ArgumentType == null)
            //    _RunMethod.Invoke(null, new object[] { });
            //else
            //{
            //    _RunMethod.Invoke(null, new object[] { args });
            //}
        }

        public override string ToString()
        {
            return HostClass.Name;
        }
    }
}
