using Rapid.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples
{
    public class ConstructExample : ScriptBase
    {

        public struct Config
        {
            public int MyInt;
        }

        public ConstructExample()
        {
            W("Zero arguments!");
        }

        [ScriptConstructor]
        public ConstructExample(Config cfg)
        {
            W("CFG is: " + cfg);
        }

        public ConstructExample(int a, int b)
        {
            W("Multiple arguments in constructor!");
        }
    }
}
