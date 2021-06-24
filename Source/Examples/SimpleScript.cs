using Rapid.NET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples
{
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

        //public int W;
    }

    public class MySimpleAttribute : Attribute
    { }

    [MySimple]
    public class SimpleScript : ScriptBase
    {
        public class Config
        {
            //public List<int> MyList = new List<int>() { 4, 5, 6 };
            //public double[] MyArray = new double[] { 1, 2, 3 };
            //public WrapType Item;
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
            new SimpleScript(cfg);
        }

        public SimpleScript(Config cfg)
        {
            W("This is a simple script!");
            W("CFG is: " + cfg);
        }



    }

}