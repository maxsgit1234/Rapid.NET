using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Reflection;

namespace Rapid.NET
{
    
    public class InvocationHistory
    {
        private readonly List<ScriptInvocation> _AllRuns;
        private readonly FileInfo _StandardFile;

        public static InvocationHistory CreateEmpty()
        {
            return new InvocationHistory(
                GetStandardFile(), new List<ScriptInvocation>());
        }

        public InvocationHistory(FileInfo file, List<ScriptInvocation> runs)
        {
            _StandardFile = file;
            _AllRuns = runs;
        }

        public void AddRun(string scriptName, object args)
        {
            string jsonArgs = JsonObj.Serialize(args);
            ScriptInvocation invocation =
                ScriptInvocation.Create(scriptName, jsonArgs);

            _AllRuns.Add(invocation);
        }

        public ScriptInvocation MostRecent(string scriptName = null)
        {
            if (scriptName == null)
                return _AllRuns.OrderByDescending(i => i.RunTime).FirstOrDefault();

            return _AllRuns.Where(i => i.ScriptName == scriptName)
                .OrderByDescending(i => i.RunTime).FirstOrDefault();
        }

        public ScriptInvocation[] PreviousInvocations(string scriptName)
        {
            return _AllRuns.Where(i => i.ScriptName == scriptName)
                .OrderByDescending(i => i.RunTime).ToArray();
        }

        public ScriptInvocation[] DistinctPreviousInvocations(string scriptName)
        {
            return DistinctInvocations(PreviousInvocations(scriptName));
        }

        public static ScriptInvocation[] DistinctInvocations(ScriptInvocation[] items)
        {
            var ret = new List<ScriptInvocation>();
            HashSet<string> already = new HashSet<string>();
            foreach (ScriptInvocation item in items)
            {
                if (already.Contains(item.Args))
                    continue;

                already.Add(item.Args);
                ret.Add(item);
            }
            return ret.ToArray();
        }

        public ScriptInvocation[] MostRecentOfEach()
        {
            var ret = new List<ScriptInvocation>();
            foreach (var kvp in _AllRuns.GroupBy(i => i.ScriptName))
                ret.Add(kvp.OrderByDescending(i => i.RunTime).First());

            return ret.OrderByDescending(i => i.RunTime).ToArray();
        }

        public static InvocationHistory FromFile(FileInfo fi = null)
        {
            if (fi == null)
                fi = GetStandardFile();
            if (!fi.Exists)
                return CreateEmpty();

            try
            {
                var items = JsonObj.Deserialize<
                    List<ScriptInvocation>>(File.ReadAllText(fi.FullName));
                return new InvocationHistory(fi, items);
            }
            catch (FormatException)
            {
                return CreateEmpty();
            }
        }

        private const int _MaxHistoryLength = 1000;

        public void WriteToFile(FileInfo fi = null)
        {
            if (fi == null)
                fi = _StandardFile;
            if (!fi.Directory.Exists)
                fi.Directory.Create();

            var toSave = _AllRuns;
            if (toSave.Count > _MaxHistoryLength)
                toSave = toSave.OrderByDescending(i => i.RunTime)
                    .Take(_MaxHistoryLength).ToList();

            File.WriteAllText(fi.FullName, JsonObj.Serialize(toSave));
        }

        private static FileInfo GetStandardFile()
        {
            string assName = Assembly.GetEntryAssembly().GetName().Name;
            return new FileInfo(Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.Desktop),
                    "Rapid.NET\\" + assName + "\\ScriptHistory.json"));
        }

    }
}
