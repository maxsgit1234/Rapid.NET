using Rapid.NET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.Arguments
{
    /// <summary>
    /// A simple script that accept no arguments.
    /// </summary>
    [Script]
    public class ScriptWithNoArgs
    {
        public static void Run()
        {
            Console.WriteLine("Running script with no arguments.");
        }
    }

    /// <summary>
    /// A simple script that accept a single primitive argument.
    /// </summary>
    [Script]
    public class ScriptWithSimpleArg
    {
        public static void Run(int myValue)
        {
            Console.WriteLine("Running script with primitive argument: " + myValue );
        }
    }

    /// <summary>
    /// A simple script that accepts a custom, user-defined argument.
    /// </summary>
    [Script]
    public class ScriptWithCustomArgs
    {
        public class Config
        {
            [Documentation("This is a config value with documentation!")]
            public int MyInt = 3;
            public string MyString = "adsf";

            public override string ToString()
            {
                return "MyInt = " + MyInt + " | MyString = " + MyString;
            }
        }

        public static void Run(Config cfg)
        {
            Console.WriteLine("Running script with custom arguments: " + cfg);
        }
    }

    /// <summary>
    /// A simple script with a complex input argument, showcasing various
    /// data types.
    /// </summary>
    [Script]
    [Documentation("This is documentation for the script!")]
    public class ScriptWithComplexArguments
    {
        public class Config
        {
            public List<int> MyList = new List<int>() { 4, 5, 6 };
            public double[] MyArray = new double[] { 1, 2, 3 };
            public CompType Item;
            public EnumType MyEnum = EnumType.B;
            public EnumType? MyNullableEnum = EnumType.C;
            public bool MyBool = true;
            public bool? MyNullableBool = true;

            public string MyString = "asdf";
            public int MyInt = 3;
            public int? MyNullable = 4;

            public override string ToString()
            {
                return JsonObject.Serialize(this);
            }
        }

        public static void Run(Config cfg)
        {
            Console.WriteLine("Running script with complex arguments!");
            Console.WriteLine("Input argument is: " + cfg);
        }

    }

    #region Supporting types for the examples

    public enum EnumType
    {
        A = 1,
        B = 2,
        C = 3,
    }

    public class WrapType
    {
        public CompType Item;
        public string MyName = "abcd";
    }

    public class CompType
    {
        public int X, Y, Z;
    }

    #endregion

}