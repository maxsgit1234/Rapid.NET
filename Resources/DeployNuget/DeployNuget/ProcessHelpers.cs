using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using F = System.IO.File;

namespace DeployNuget
{
    public static class ProcessHelpers
    {
        public static string[] RunCommands(string dir, params string[] lines)
        {
            return RunCommands(dir, true, false, lines);
        }

        public static string[] RunCommands(
            string dir, bool throwOnCrash, bool createWindow, params string[] lines)
        {
            string t = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss-fff");

            string log = "autolog_" + t + ".txt";
            string ldir = "C:\\Rapid.NET\\Logs\\";
            if (!Directory.Exists(ldir))
                Directory.CreateDirectory(ldir);

            string lf = ldir + log;
            if (F.Exists(lf))
                F.Delete(lf);

            var ret = new List<string>();
            ret.Add("cd " + dir);
            ret.AddRange(lines.Select(i => i + " >> " + lf));
            lines = ret.ToArray();


            string file = "temp_" + t + ".bat";
            F.WriteAllLines(file, lines);
            RunProcess("cmd.exe", "/c " + file, throwOnCrash, true, createWindow);
            F.Delete(file);

            if (!F.Exists(lf))
                return null;

            string[] output = F.ReadAllLines(lf);
            F.Delete(lf);
            return output;
        }


        public static void RunProcess(string cmd, string args,
            bool throwOnCrash = true, bool waitForExit = true,
            bool createWindow = false)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo(cmd, args);
            processInfo.CreateNoWindow = false;
            processInfo.UseShellExecute = createWindow;
            Process process = Process.Start(processInfo);


            if (waitForExit)
            {
                process.WaitForExit();

                if (throwOnCrash && process.ExitCode != 0)
                    throw new Exception("crashed.");
                Console.WriteLine("Exited with: " + process.ExitCode);
            }
            else
            {
                Thread.Sleep(2000);
                Console.WriteLine("Waiting for 2 seconds to let the process startup...");
            }
        }

        public static void CrashDumpAndExit(string message, Exception e,
            byte[] context = null, bool exit = false, Action<string> w = null)
        {
            string s = "FATAL EXCEPTION: " + message + " | " + e;
            Console.WriteLine(s);

            string f1 = "CRASH_"
                + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string f2 = Environment.GetFolderPath(
                Environment.SpecialFolder.Desktop) + "\\" + f1;

            string[] lines = new string[] { s };

            F.WriteAllLines(f1 + ".txt", lines);
            F.WriteAllLines(f2 + ".txt", lines);

            if (context != null)
            {
                F.WriteAllBytes(f1 + ".dat", context);
                F.WriteAllBytes(f2 + ".dat", context);
            }

            w?.Invoke(s);

            if (exit)
                Environment.Exit(99);
        }

        public static Process[] KillAllProcessesInDirs(params string[] dirs)
        {
            Process[] ps = Process.GetProcesses();
            var ret = new List<Process>();
            foreach (Process p in ps)
            {
                try
                {
                    if (dirs.Any(i => p.MainModule.FileName.Contains(i)))
                    {
                        ret.Add(p);
                        p.Kill();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed for " + p.ProcessName + ": " + ex.Message);
                }
            }

            return ret.ToArray();
        }

    }
}

