using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Rapid.NET
{
    //public interface IScript<T>
    //{
    //    void SetArgs(T o);
    //    void Run();
    //}

    //public class ScriptTest : IScript<ScriptTest.Config>
    //{
    //    public class Config
    //    {
    //        public int MyInt = 5;
    //    }

    //    protected Config _Cfg;

    //    public void SetArgs(Config cfg)
    //    {
    //        _Cfg = cfg;
    //    }

    //    public void Run()
    //    {
    //        Console.WriteLine("TEST");
    //    }

    //}



    //    foreach (TypeInfo tt in Assembly.GetEntryAssembly().ExportedTypes)
    //            {
    //                if (tt.Name == "ScriptTest")
    //                {
    //                    Type[] ifaces = tt.ImplementedInterfaces.ToArray();
    //                    foreach (Type t in ifaces)
    //                    {
    //                        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IScript<>))
    //                        {
    //                            Type arg = t.GetGenericArguments()[0];
    //    var x = Activator.CreateInstance(tt);
    //    var cfg = Activator.CreateInstance(arg);



    //    ConstructorInfo ci = t.GetConstructor(new Type[] { });
    //}
    //                    }

    //                }


    //            }



    [Script]
    public abstract class ScriptBase
    {
        /// <summary>
        /// Shorthand for Console.WriteLine();
        /// </summary>
        /// <param name="o"></param>
        public static void W(object o)
        {
            if (o == null)
                W("");
            else
                W(o.ToString());

        }

        private static Stopwatch _Watch = new Stopwatch();

        /// <summary>
        /// Shorthand for Console.WriteLine();
        /// </summary>
        /// <param name="s"></param>
        public static void W(string s)
        {
            lock (_Watch)
            {
                long ms = _Watch.ElapsedMilliseconds;
                _Watch.Restart();

                DateTime now = DateTime.Now;
                string ss = now.ToString("HH:mm:ss.fff")
                    + " | " + ms.ToString().PadLeft(5) + " | " + s;
                Console.WriteLine(ss);
            }
        }

    }

}
