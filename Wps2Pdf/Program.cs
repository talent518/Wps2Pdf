using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Wps2Pdf
{
    class Program
    {
        static void Main(string[] args)
        {
            int i;
            Converter conv = new Converter();

            for (i = 0; i < args.Length; i++)
            {
                conv.convert(args[i]);
            }

            conv = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("Killing process");

            Process[] ps;

            ps = Process.GetProcessesByName("et");
            for(i=0;i< ps.Length;i++)
            {
                Console.WriteLine("Kill process(et.exe) for " + ps[i].Id);
                ps[i].Kill();
            }

            ps = Process.GetProcessesByName("wpp");
            for (i = 0; i < ps.Length; i++)
            {
                Console.WriteLine("Kill process(wpp) for " + ps[i].Id);
                ps[i].Kill();
            }

            ps = Process.GetProcessesByName("wps");
            for (i = 0; i < ps.Length; i++)
            {
                Console.WriteLine("Kill process(wps) for " + ps[i].Id);
                ps[i].Kill();
            }
        }
    }
}
