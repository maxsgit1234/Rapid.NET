using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeployNuget
{
    class Program
    {
        static void Main(string[] args)
        {
            string version = "1.0.7";
            string rapidDir = Path.GetFullPath(@"..\..\..\..\..");
            string slnFile = rapidDir + "\\Source\\Rapid.NET.sln";
            string stdFile = rapidDir + "\\Source\\Rapid.NET\\Rapid.NET.csproj";
            string wpfFile = rapidDir + "\\Source\\Rapid.NET.Wpf\\Rapid.NET.Wpf.nuspec";
            string netFile = rapidDir + "\\Source\\Rapid.NET.Core\\Rapid.NET.Core.csproj";
            string bldDir = rapidDir + "\\Resources\\DeployNuget\\Build";
            string outDir = rapidDir + "\\Resources\\DeployNuget\\NuPkg";
            string nuDir = rapidDir + "\\Resources\\DeployNuget\\DeployNuget";
            string wpfDir = rapidDir + "\\Source\\Rapid.NET.Wpf";
            string stdText = File.ReadAllText(stdFile);
            string wpfText = File.ReadAllText(wpfFile);
            string netText = File.ReadAllText(netFile);

            // Update version numbers in each of the 3 files.
            stdText = ReplaceTag(stdText, "Version", version);
            wpfText = ReplaceTag(wpfText, "version", version);
            netText = ReplaceTag(netText, "Version", version);

            File.WriteAllText(stdFile, stdText);
            File.WriteAllText(wpfFile, wpfText);
            File.WriteAllText(netFile, netText);

            // Build solution
            InstallMethods.BuildSln(
                slnFile, true, true, bldDir, null, Console.WriteLine);

            // Create .nupkg file for the Rapid.NET.Wpf project
            string[] ret = ProcessHelpers.RunCommands(nuDir, true, true,
                new string[]{
                    "nuget.exe pack " + rapidDir + "\\Source\\Rapid.NET.Wpf\\Rapid.NET.Wpf.csproj -OutputDirectory " + wpfDir,
                }
            );

            foreach (string line in ret)
                Console.WriteLine(line);

            //ProcessHelpers.RunProcess(nuDir + "\\nuget.exe",
            //    "pack " + rapidDir + "\\Source\\Rapid.NET.Wpf\\Rapid.NET.Wpf.csproj >> C:\\Rapid.NET\\debug.txt",
            //    true, true, true);

            // Copy Nuget files to an output directory...
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            File.Copy(
                bldDir + "\\Release\\Rapid.NET." + version + ".nupkg",
                outDir + "\\Rapid.NET." + version + ".nupkg", true);
            File.Copy(
                bldDir + "\\Release\\Rapid.NET.Core." + version + ".nupkg",
                outDir + "\\Rapid.NET.Core." + version + ".nupkg", true);

            File.Copy(
                wpfDir + "\\Rapid.NET.Wpf." + version + ".nupkg",
                outDir + "\\Rapid.NET.Wpf." + version + ".nupkg", true);

            Console.WriteLine("DONE");
        }

        private static string ReplaceTag(string text, string tag, string value)
        {
            int k0 = text.IndexOf("<" + tag + ">");
            int k1 = text.IndexOf("</" + tag + ">", k0);

            int ix0 = k0 + tag.Length + 2;
            int ix1 = k1;

            string ret = text.Substring(0, ix0)
                + value + text.Substring(ix1, text.Length - ix1);
            return ret;
        }

    }
}
