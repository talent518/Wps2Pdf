using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Wps2Pdf
{
    class Deamon
    {
        public static Semaphore sem = new Semaphore(0, 2);

        private static List<Converter> converters = new List<Converter>();
        private static List<Thread> threads = new List<Thread>();
        private static Converter converter = null;
        private static Thread thread = null;

        private static AppServer appServer = null;
        private static ServerConfig config = new ServerConfig {
            Ip = "Any",
            Port = 2012,
            TextEncoding = "UTF-8"
        };

        private static int i;

        public static ILog Logger = null;

        public static bool Start(string[] args)
        {
            if (appServer != null) {
                appServer.Logger.Error("Called Deamon.Start method!");

                return false;
            }

            appServer = new AppServer();

            //Setup the appServer
            if (!appServer.Setup(config)) //Setup with listening port
            {
                appServer.Logger.Error("Failed to setup!");
                return false;
            }

            //Try to start the appServer
            if (!appServer.Start())
            {
                appServer.Logger.Error("Failed to start!");
                return false;
            }

            Logger = appServer.Logger;

            appServer.Logger.Info("Converter.init");
            Converter.init();

            for (i = 0; i < Environment.ProcessorCount; i++)
            {
                converter = new Converter();
                converters.Add(converter);

                thread = new Thread(converter.run);
                thread.Start();
                threads.Add(thread);
            }

            appServer.Logger.InfoFormat("The server started successfully and listen port is {0}.", appServer.Config.Port);

            return true;
        }

        public static bool Stop()
        {
            if (appServer == null) {
                appServer.Logger.Error("Not called Deamon.Stop method!");

                return false;
            }

            Logger = null;

            //Stop the appServer
            appServer.Stop();

            foreach (var conv in converters)
            {
                conv.isRun = false;
            }
            for (i = 0; i < converters.Count; i++)
            {
                Converter.sem.Release();
            }
            foreach (var t in threads)
            {
                t.Join();
            }

            appServer.Logger.Info("Converter.destory");
            Converter.destory();

            converters.Clear();
            threads.Clear();
            Converter.queue.Clear();
            Converter.sem.Close();

            GC.Collect();
            GC.WaitForPendingFinalizers();

           /* Process[] ps;

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
            }*/

            appServer.Logger.Info("The server was stopped!");

            appServer = null;

            return true;
        }

        public static void Main(string[] args)
        {
            if (Start(args))
            {
                sem.WaitOne();

                Stop();
            }
        }
    }
}
