using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PH = DeployNuget.ProcessHelpers;
using F = System.IO.File;

namespace DeployNuget
{
    public static class InstallMethods
    {
        private const string _MsBuild = "\"C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Community\\MSBuild\\Current\\Bin\\MSBuild\"";
        
        private static string BldCmd(string sln, bool dbg,
            string outputdir = null, string projectName = null)
        {
            string cfg = dbg ? "Debug" : "Release";

            string targ = projectName == null ?
                "-t:rebuild" : "-t:" + projectName.Replace(".", "_") + ":rebuild";
            string args = targ + @" -clp:Summary;ErrorsOnly -p:Configuration=" + cfg + " -p:Platform=\"Any CPU\"";

            if (outputdir != null)
                args += " /p:OutputPath=" + outputdir;

            return _MsBuild + " \"" + sln + "\" " + args;
        }


        public static void BuildSln(string filename,
            string outputdir = null, Action<string> W = null)
        {
            if (W == null)
                W = _ => { };

            bool dbg = false;
            bool rel = false;
            bool outputOk = outputdir != null;

            string here = Environment.CurrentDirectory;

            if (outputOk || Path.GetFileName(here).ToLower() != "debug")
                dbg = true;
            if (outputOk || Path.GetFileName(here).ToLower() != "release")
                rel = true;

            BuildSln(filename, dbg, rel, outputdir, null, W);
        }

        public static void BuildSln(string filename,
            bool buildBdg, bool buildRel, string outputdir = null,
            string projectName = null,
            Action<string> W = null)
        {
            if (W == null)
                W = _ => { };

            string sln = Path.GetFullPath(filename);

            string outDbg = outputdir == null ? null : outputdir + "\\Debug";
            string outRel = outputdir == null ? null : outputdir + "\\Release";

            List<string> lines = new List<string>();
            if (buildBdg)
                lines.Add(BldCmd(sln, true, outDbg, projectName));
            if (buildRel)
                lines.Add(BldCmd(sln, false, outRel, projectName));


            W("About to execute build commands: " + lines.Count);
            foreach (string line in lines)
                W(line);

            //W("bat file is: " + batFile);
            //F.WriteAllLines(batFile, lines.ToArray());
            W("Running build process...");
            string[] ret = PH.RunCommands(Environment.CurrentDirectory, lines.ToArray());
            W("got result: " + ret.Length);
            foreach (string line in ret)
                W(line);

            //PH.RunProcess("cmd.exe", "/c " + batFile);
            W("Build process is done.");
        }

        public static void CopyDir(string dir, string dest, params string[] searchExcept)
        {
            if (!Directory.Exists(dir))
                return;

            if (!Directory.Exists(dest))
                Directory.CreateDirectory(dest);

            foreach (string dirPath in Directory.GetDirectories(
                dir, "*", SearchOption.AllDirectories))
            {
                if (searchExcept != null && searchExcept.Any(i => dirPath.Contains(i)))
                    continue;
                Directory.CreateDirectory(dirPath.Replace(dir, dest));
            }

            // Copy all the files & Replaces any files with the same name
            int ncopy = 0;
            int nskip = 0;
            foreach (string newPath in Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories))
            {
                while (true)
                {
                    try
                    {
                        string to = newPath.Replace(dir, dest);
                        if (ShouldCopy(newPath, to))
                        {
                            F.Copy(newPath, to, true);
                            ncopy++;
                        }
                        else
                            nskip++;

                        break;
                    }
                    catch (Exception e)
                    {
                        // TODO: Add name of file...or error message, or whatever.
                        Console.WriteLine("Failed to copy file....");
                        Thread.Sleep(1000);
                    }
                }
            }

            Console.WriteLine("COPIED " + ncopy + " FILES; SKIPPED " + nskip + " FILES.");
        }

        public static bool ShouldCopy(string from, string to)
        {
            if (!F.Exists(to))
                return true;

            FileInfo fiTo = new FileInfo(to);
            FileInfo fiFrom = new FileInfo(from);

            return fiTo.LastWriteTimeUtc < fiFrom.LastWriteTimeUtc;
        }

        public static void FC(string from, string dest, params string[] files)
        {
            foreach (string file in files)
                F.Copy(PC(from, file), PC(dest, file), true);
        }

        public static string PC(params string[] s)
        {
            return Path.Combine(s);
        }

    }
}
