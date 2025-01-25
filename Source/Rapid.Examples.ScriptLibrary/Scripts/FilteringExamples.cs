using Rapid.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.Filtering
{
    /// <summary>
    /// An example of a user-defined attribute that can be used to filter
    /// which scripts appear in the UI. To filter for only scripts which 
    /// have this attribute, try this: 
    /// 'Examples.exe [Examples.Filtering.MyFilteringAttribute]'
    /// </summary>
    public class MyFilteringAttribute : Attribute { }

    [Script, MyFiltering]
    public class FilteredScriptA { public static void Run() { } }

    [Script, MyFiltering]
    public class FilteredScriptB { public static void Run() { } }

    [Script, MyFiltering]
    public class FilteredScriptC { public static void Run() { } }

}
