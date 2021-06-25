using Rapid.NET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Examples
{
    public static class TimestampedPrinter
    {
        /// <summary>
        /// Shorthand for Console.WriteLine();
        /// </summary>
        /// <param name="o"></param>
        public static void Print(object o)
        {
            if (o == null)
                Print("");
            else
                Print(o.ToString());

        }

        private static Stopwatch _Watch = new Stopwatch();

        /// <summary>
        /// Shorthand for Console.WriteLine();
        /// </summary>
        /// <param name="s"></param>
        public static void Print(string s)
        {
            lock (_Watch)
            {
                long ms = _Watch.ElapsedMilliseconds;
                _Watch.Restart();

                DateTime now = DateTime.Now;
                string ss = now.ToString("HH:mm:ss.fff")
                    + " | " + ms.ToString().PadLeft(5) + " | " + s;
                Console.WriteLine(ss);
            }
        }

    }

}
