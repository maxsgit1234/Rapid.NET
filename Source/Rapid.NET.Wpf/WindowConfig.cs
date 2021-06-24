using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rapid.NET.Wpf
{
    public class WindowConfig
    {
        public class Area
        {
            public double Left, Top, Width, Height;
        }

        private readonly FileInfo _File;
        private Area _Area;

        public static WindowConfig CreateEmpty()
        {
            return new WindowConfig(GetStandardFile(), null);
        }

        public WindowConfig(FileInfo file, Area area)
        {
            _File = file;
            _Area = area;
        }

        public static WindowConfig FromFile(string file)
        {
            if (file == null)
                file = GetStandardFile().FullName;

            if (!File.Exists(file))
                return new WindowConfig(new FileInfo(file), null);

            Area a = JsonObject.Deserialize<Area>(File.ReadAllText(file));
            return new WindowConfig(new FileInfo(file), a);
        }

        public void SetWindowArea(Window window)
        {
            if (_Area == null)
                return;

            var screens = System.Windows.Forms.Screen.AllScreens;
            
            double zoom = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width 
                / SystemParameters.PrimaryScreenWidth;

            bool isOk = false;
            double x = _Area.Left * zoom;
            double y = _Area.Top * zoom;
            foreach (var s in screens)
            {
                if (s.Bounds.Contains((int)x, (int)y))
                {
                    isOk = true;
                    break;
                }
            }

            if (isOk)
            {
                window.Top = _Area.Top;
                window.Left = _Area.Left;
                window.Width = _Area.Width;
                window.Height = _Area.Height;
            }
            else
            {
                double w = SystemParameters.PrimaryScreenWidth;
                double h = SystemParameters.PrimaryScreenHeight;
                window.Left = w / 4;
                window.Top = h / 4;
                window.Width = 500;
                window.Height = 500;
            }
        }

        public void WriteToFile(Window window)
        {
            _Area = new Area
            {
                Left = window.Left,
                Top = window.Top,
                Width = window.Width,
                Height = window.Height,
            };

            if (!_File.Directory.Exists)
                _File.Directory.Create();
            File.WriteAllText(_File.FullName, JsonObject.Serialize(_Area));
        }

        private static FileInfo GetStandardFile()
        {
            string assName = Assembly.GetEntryAssembly().GetName().Name;
            return new FileInfo(Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.Desktop), 
                "Rapid.NET\\" + assName + "\\WindowConfig.json"));
        }
    }

}
