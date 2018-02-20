using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace X.Application
{
    public static class Bootstrapper<T> where T : X.Application.XApp, new()
    {
        public static void Run(params string[] args)
        {
            TypeCatalog.Instance.Add(new FileInfo(Assembly.GetEntryAssembly().Location).Directory);

            XApp.Current = new T();

            if (args.Length > 0)
            {
                if (args.Any(x => x.EndsWith("install", StringComparison.CurrentCultureIgnoreCase)))
                {
                    if (!IsElevated)
                    {
                        if (!args.Any(x => x.Equals("--force", StringComparison.CurrentCultureIgnoreCase)))
                        {
                            // Let's attempt to run in elevated mode..(add --force to command line for disambiguation)
                            var info = new ProcessStartInfo(Assembly.GetEntryAssembly().Location, string.Join(" ", args) + " --force")
                            {
                                Verb = "runas", // indicates to elevate privileges
                            };

                            var process = new Process
                            {
                                EnableRaisingEvents = true, // enable WaitForExit()
                                StartInfo = info
                            };

                            process.Start();

                            // sleep calling process thread until evoked process exit
                            process.WaitForExit();

                            //Process.GetCurrentProcess().Kill(); 
                        }
                        else
                        {
                            // Attempt to run install but we are not elevated 
                            // when the --force argument is set...
                            Console.WriteLine("Could not gain elevated privileges... something's wrong");
                            Console.WriteLine("setup aborted");
                            System.Threading.Thread.Sleep(2000);
                            return;
                        }
                    }
                    else
                    {
                        // do stuff under elevated privileges
                        Console.WriteLine("Elevated privileges gained={0}", IsElevated);
                        typeof(IInstallRoutine).Hype()
                            .GetMatchingTypes(x => x.IsConcrete())
                            .OrderBy(x => x.FullName)
                            .GetOne<IInstallRoutine>()
                            .ForEach(x => x.OnInstall());

                        XApp.Current.OnInstall(args);

                        System.Threading.Thread.Sleep(2000);
                        return;
                    }
                }
            }

            typeof(IInitializeRoutine).Hype()
                .GetMatchingTypes(x => x.IsConcrete())
                .OrderBy(x => x.FullName)
                .GetOne<IInitializeRoutine>()
                .ForEach(x => x.OnInitialize());

            XApp.Current.OnInitialize(args);


            typeof(IOnStartRoutine).Hype()
                .GetMatchingTypes(x => x.IsConcrete())
                .OrderBy(x => x.FullName)
                .GetOne<IOnStartRoutine>()
                .ForEach(x => x.OnStart());

            XApp.Current.OnStart();

            XApp.Current.OnStop();

            typeof(X.Application.IOnStopRoutine).Hype()
                .GetMatchingTypes(x => x.IsConcrete())
                .OrderBy(x => x.FullName)
                .GetOne<IOnStopRoutine>()
                .ForEach(x => x.OnStop());
        }

        static bool IsElevated
        {
            get
            {
                return new WindowsPrincipal
                    (WindowsIdentity.GetCurrent()).IsInRole
                    (WindowsBuiltInRole.Administrator);
            }
        }

    }
}
