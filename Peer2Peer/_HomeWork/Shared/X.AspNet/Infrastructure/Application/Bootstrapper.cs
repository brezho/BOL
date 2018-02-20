using Host.Infrastructure.Application.HttpModules;
using Host.Infrastructure.VPP;
using X.AspNet;
using X.AspNet.Infrastructure.Application;
using X.AspNet.Infrastructure.Application.HttpModules;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;

namespace Host.Infrastructure.Application
{
    public static class Bootstrapper
    {
        internal static IWebApplication LoadedApplication;
        public static void OnBoot()
        {
            RegisterTraceListener();
            DiscoverWebApplication();

            AssemblyScanner.AutoScan();
            HostingEnvironment.RegisterVirtualPathProvider(new EmbeddedResourcesVirtualPathProvider());
            //    Logger.Debug("OnBoot", "Application pre-start on " + DateTime.UtcNow.ToShortTimeString(), "Bootstraper");
            RegisterModules();
            //      var cache = GridCache.Instance;
        }


        static void DiscoverWebApplication()
        {
            var t = TypeCatalog.Instance.GetMatchingTypes(typeof(WebApp), x => x.IsConcrete()).FirstOrDefault();
            LoadedApplication = (IWebApplication)Activator.CreateInstance(t);
        }
        static void RegisterTraceListener()
        {
            Trace.Listeners.Add(new System.Diagnostics.ConsoleTraceListener());
            // Trace.Listeners.Add(new TextWriterTraceListener(Core.Constants.Files.Trace));
            Trace.AutoFlush = true;
        }

        private static void RegisterModules()
        {
            foreach (var type in DefaultModuleTypeSet) DynamicModuleUtility.RegisterModule(type);
        }

        private static IEnumerable<Type> DefaultModuleTypeSet
        {
            get
            {
                //   yield return typeof(ErrorHandlerModule);
                yield return typeof(CustomHandlersHttpModule);
                //   yield return typeof(SetupModule);
                yield return typeof(RoutingHttpModule);
                yield return typeof(TranslationHttpModule);
                //    yield return typeof(PerfMonitorModule);
                //    yield return typeof(StartupModule);
                // TODO: Module is buggy.... take time to check why.
                //   yield return typeof(CompressionModule);
            }
        }
    }
}
