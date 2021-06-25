using Rapid.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples
{
    /// <summary>
    /// The simplest possible example implementation of a Script. This script
    /// does not accept any arguments, and will simply run any code placed in
    /// the 'Run' method when invoked.
    /// </summary>
    [Script]
    public class HelloWorldExample
    {
        public static void Run()
        {
            Console.WriteLine("Hello world from within a script!");
        }
    }
}
