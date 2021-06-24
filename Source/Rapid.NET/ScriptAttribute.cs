using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapid.NET
{
    public class ScriptAttribute : Attribute
    {
        public readonly string Description;

        public ScriptAttribute(string description = null)
        {
            Description = description;
        }
    }
}
