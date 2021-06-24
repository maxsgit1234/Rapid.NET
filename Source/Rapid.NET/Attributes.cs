using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rapid.NET
{
    public class ScriptAttribute : Attribute { }

    public class ScriptConstructorAttribute : Attribute { }

    public class DocumentationAttribute : Attribute
    {
        public readonly string Description;

        public DocumentationAttribute(string description)
        {
            while (Regex.IsMatch(description, @"^(\r|\n|\t| )"))
                description = description.Substring(1);
            string[] lines = Regex.Split(description, @"\r\n|\r|\n");
            if (lines.Length > 1)
            {
                description = lines
                    .Select(s => s.TrimStart())
                    .Aggregate((a, b) => a + "\r\n" + b);
            }
            Description = description;
        }
    }
}
