using SuperSocket.SocketBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Wps2Pdf
{
    class Deamon
    {

        public static void Main(string[] args)
        {
            int i;
            var appServer = new AppServer();

            //Setup the appServer
            if (!appServer.Setup(2012)) //Setup with listening port
            {
                Console.WriteLine("Failed to setup!");
                Console.ReadKey();
                return;
            }

            //Try to start the appServer
            if (!appServer.Start())
            {
                Console.WriteLine("Failed to start!");
                Console.ReadKey();
                return;
            }

            List<Converter> converters = new List<Converter>();
            List<Thread> threads = new List<Thread>();
            Converter converter;
            Thread thread;
            for (i = 0; i < Environment.ProcessorCount; i++)
            {
                converter = new Converter();
                converters.Add(converter);

                thread = new Thread(converter.run);
                thread.Start();
                threads.Add(thread);
            }

            Console.WriteLine("The server started successfully\nlisten port is " + appServer.Config.Port + "\npress key 'q' to stop it!");

            while (Console.ReadKey().KeyChar != 'q')
            {
                Console.WriteLine();
                continue;
            }
            Console.WriteLine();

            //Stop the appServer
            appServer.Stop();

            foreach (var conv in converters) {
                conv.isRun = false;
            }
            for (i=0; i<converters.Count; i++) {
                Converter.sem.Release();
            }
            foreach (var t in threads) {
                t.Join();
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("Killing process");

            
            Process[] ps;

            ps = Process.GetProcessesByName("et");
            for (i = 0; i < ps.Length; i++)
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

            Console.WriteLine();
            Console.WriteLine("The server was stopped!");
            Console.WriteLine("Please press any key and exit.");
            Console.ReadKey();
        }
    }
}
