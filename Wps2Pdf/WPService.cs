using System;
using System.Configuration.Install;
using System.IO;
using System.Reflection;
using System.ServiceProcess;

namespace Wps2Pdf
{
    partial class Wps2Pdf : ServiceBase
    {
        public Wps2Pdf()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (Deamon.Start(args))
            {
                base.OnStart(args);

                //Deamon.sem.WaitOne();
            }
            else
            {
                base.ExitCode = 1;
            }
        }

        protected override void OnStop()
        {
            //Deamon.sem.Release();

            if (Deamon.Stop())
            {
                base.OnStop();
            }
            else
            {
                base.ExitCode = 1;
            }
        }

        protected override void OnShutdown()
        {
            //Deamon.sem.Release();

            if (Deamon.Stop())
            {
                base.OnShutdown();
            }
            else
            {
                base.ExitCode = 1;
            }
        }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        public static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                string _exePath = Assembly.GetExecutingAssembly().Location;

                if (args.Length == 0)
                {
                    Usage(_exePath);
                }
                else
                {
                    switch (args[0])
                    {
                        case "-i":
                            if (GetService() == null)
                            {
                                ManagedInstallerClass.InstallHelper(new string[] { _exePath });
                            }
                            else
                            {
                                Console.WriteLine("服务已存在！");
                            }
                            break;
                        case "-u":
                            if (GetService() == null)
                            {
                                Console.WriteLine("服务不存在！");
                            }
                            else
                            {
                                ManagedInstallerClass.InstallHelper(new string[] { "/u", _exePath });
                            }
                            break;
                        case "-r":
                            Deamon.Main(args);
                            break;
                        default:
                            Usage(_exePath);
                            break;
                    }
                }
            }
            else
            {
                ServiceBase.Run(new Wps2Pdf());
            }
        }

        private static void Usage(string _exePath)
        {
            Console.WriteLine("Usage: {0} {{-i|-u|-r}}", Path.GetFileName(_exePath));
            Console.WriteLine("  -i       Install Service");
            Console.WriteLine("  -u       Uninstall Service");
            Console.WriteLine("  -r       Console Run");
        }

        public static ServiceController GetService()
        {
            foreach (ServiceController s in ServiceController.GetServices())
            {
                if (s.ServiceName == "Wps2Pdf")
                {
                    return s;
                }
            }

            return null;
        }
    }
}
