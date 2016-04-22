using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Wps2Pdf
{
    class Deamon
    {
        public static Semaphore sem = new Semaphore(0, 2);

        public static void Main(string[] args)
        {
            int i;
            var appServer = new AppServer();

            //Setup the appServer
            if (!appServer.Setup(2012)) //Setup with listening port
            {
                appServer.Logger.Error("Failed to setup!");
                return;
            }

            //Try to start the appServer
            if (!appServer.Start())
            {
                appServer.Logger.Error("Failed to start!");
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

            appServer.Logger.InfoFormat("The server started successfully and listen port is {0}.", appServer.Config.Port);

            sem.WaitOne();

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

            converters.Clear();
            threads.Clear();
            Converter.queue.Clear();
            Converter.sem.Close();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            Process[] ps;

            ps = Process.GetProcessesByName("et");
            for (i = 0; i < ps.Length; i++)
            {
                appServer.Logger.WarnFormat("Kill process(et.exe) for {0}", ps[i].Id);
                ps[i].Kill();
            }

            ps = Process.GetProcessesByName("wpp");
            for (i = 0; i < ps.Length; i++)
            {
                appServer.Logger.WarnFormat("Kill process(wpp.exe) for {0}", ps[i].Id);
                ps[i].Kill();
            }

            ps = Process.GetProcessesByName("wps");
            for (i = 0; i < ps.Length; i++)
            {
                appServer.Logger.WarnFormat("Kill process(wps.exe) for {0}", ps[i].Id);
                ps[i].Kill();
            }

            appServer.Logger.Info("The server was stopped!");
        }
    }
}
