using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Rapid.NET
{
    public class ScriptInvocation
    {
        public DateTime RunTime;
        public string ScriptName;
        public string Args;

        [JsonConstructor]
        public ScriptInvocation(
            DateTime runTime, string scriptName, string args)
        {
            RunTime = runTime;
            ScriptName = scriptName;
            Args = args;
        }

        public static ScriptInvocation Create(string name, string args)
        {
            return new ScriptInvocation(DateTime.UtcNow, name, args);
        }

        public override string ToString()
        {
            return RunTime.ToString("MM/dd HH:mm:ss") + " " + ScriptName + ": " + Args;
        }

        //public string ShortString()
        //{
        //    return LocalTime() + " " + ReverseComName(ScriptName);
        //}

        public string HistoryString()
        {
            return LocalTime() + " " + Args.Replace("\r\n", "").Replace(" ", "");
        }

        public string LocalTime()
        {
            return RunTime.ToLocalTime().ToString("MM/dd HH:mm:ss");
        }

        public string ReverseComNames(out string first)
        {
            return ReverseComNames(ScriptName, out first);
        }

        public static string ReverseComNames(string name, out string first)
        {
            string[] words = name.Split('.');

            first = words[words.Length - 1];
            string ret = " (";
            for (int i = 0; i < words.Length - 1; i++)
            {
                ret += words[i];
                if (i < words.Length - 2)
                    ret += ".";
            }

            return ret + ")";

            //for (int i = words.Length - 1; i >= 0; i--)
            //{
            //    ret += words[i];
            //    if (i > 0)
            //        ret += ".";
            //}

            return ret;
        }

    }
}
