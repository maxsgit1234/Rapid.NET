using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rapid.NET
{
    /// <summary>
    /// Annotate your Script class with this attribute in order to declare 
    /// that it can be run as a script. Your script class will also need to
    /// add a "public static void Run(...);" with either 0 or 1 arguments
    /// in order to be executed as a script.
    /// </summary>
    public class ScriptAttribute : Attribute { }

    /// <summary>
    /// Annotate your script class, or any field of your Run(...) method's
    /// input argument with this attribute to display documentation in the
    /// script-launcher UI.
    /// </summary>
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
