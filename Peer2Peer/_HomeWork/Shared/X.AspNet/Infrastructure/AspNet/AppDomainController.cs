using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace Host.Infrastructure.AspNet
{
    public class AppDomainController
    {
        private string _virtualDir;
        private string _physicalDir;
        private string[] _prefixes;
        private AuthenticationSchemes _schemes;

        private Thread _pump;

        private AspNet _aspNetDomain;
        ManualResetEvent started;

        public AppDomainController(string[] prefixes, AuthenticationSchemes schemes)
        {
            _prefixes = prefixes;
            _schemes = schemes;
            _virtualDir = "/";
            _physicalDir = Directory.GetCurrentDirectory();
        }


        public void Start()
        {
            started = new ManualResetEvent(false);
            _pump = new Thread(new ThreadStart(Pump));
            _pump.Start();

            started.WaitOne();
        }

        public void Stop()
        {
            _aspNetDomain.Stop();
            _pump.Join();
        }

        private void Pump()
        {
            try
            {
                  _aspNetDomain = CreateWorkerAppDomainWithHost<AspNet>(_virtualDir, _physicalDir);
             //    _aspNetDomain = CreateApplicationHost<AspNet>(_virtualDir, _physicalDir);

                _aspNetDomain.Configure(_prefixes, _schemes);
                _aspNetDomain.Start();

                Console.WriteLine("Listening on:");

                foreach (var pf in _prefixes)
                {
                    Console.WriteLine(pf);
                }

                started.Set();

            }
            catch (AppDomainUnloadedException)
            {
                Console.WriteLine("Restarting due to unloaded appdomain");
                Start();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }

        //public static T CreateApplicationHost<T>(string virtualDir, string physicalDir)
        //{

        //    var asmLocation = new FileInfo(typeof(AspNet).Assembly.Location);
        //    var binFolder = asmLocation.Directory.CreateSubdirectory("bin");
        //    File.Copy(asmLocation.FullName, Path.Combine(binFolder.FullName, asmLocation.Name), true);

        //    string aspDir = HttpRuntime.AspInstallDirectory;
        //    string domainId = "ASPHOST_" + DateTime.Now.ToString().GetHashCode().ToString("x");
        //    string appName = "ASPHOST";



        //    AppDomainSetup setup = new AppDomainSetup();
        //    setup.ApplicationName = appName;

        //    //  setup.ConfigurationFile = "web.config";

        //    AppDomain loDomain = AppDomain.CreateDomain(domainId, null, setup);
        //    loDomain.SetData(".appDomain", "*");
        //    loDomain.SetData(".appPath", physicalDir);
        //    loDomain.SetData(".appVPath", virtualDir);
        //    loDomain.SetData(".domainId", domainId);
        //    loDomain.SetData(".hostingVirtualPath", virtualDir);
        //    loDomain.SetData(".hostingInstallDir", aspDir);

        //    ObjectHandle oh = loDomain.CreateInstance(typeof(T).Assembly.FullName, typeof(T).FullName);

        //    T loHost = (T)oh.Unwrap();

        //    //// *** Save virtual and physical to tell where app runs later
        //    //loHost.cVirtualPath = virtualDir;
        //    //loHost.cPhysicalDirectory = physicalDir;

        //    //// *** Save Domain so we can unload later
        //    //loHost.oAppDomain = loDomain;

        //    return loHost;
        //}
        public static T CreateWorkerAppDomainWithHost<T>(string virtualPath, string physicalPath)
               where T : MarshalByRefObject, IRegisteredObject
        {


            var asmLocation = new FileInfo(typeof(AspNet).Assembly.Location);
            var binFolder = asmLocation.Directory.CreateSubdirectory("bin");
            Directory.GetFiles(asmLocation.Directory.FullName)
                .Select(x=>new FileInfo(x))
                .ToList()
                .ForEach(x => { 
                    File.Copy(x.FullName, Path.Combine(binFolder.FullName, x.Name), true);
                
                });



            // this creates worker app domain in a way that host doesn't need to be in GAC or bin
            // using BuildManagerHost via private reflection
            string uniqueAppString = string.Concat(virtualPath, physicalPath).ToLowerInvariant();
            string appId = (uniqueAppString.GetHashCode()).ToString("x", CultureInfo.InvariantCulture);

            // create BuildManagerHost in the worker app domain
            var appManager = ApplicationManager.GetApplicationManager();

            var buildManagerHostType = typeof(HttpRuntime).Assembly.GetType("System.Web.Compilation.BuildManagerHost");
            var buildManagerHost = appManager.CreateObject(appId, buildManagerHostType, virtualPath, physicalPath, false);

            // call BuildManagerHost.RegisterAssembly to make Host type loadable in the worker app domain
            buildManagerHostType.InvokeMember("RegisterAssembly", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic, null, buildManagerHost, new object[] { typeof(T).Assembly.FullName, typeof(T).Assembly.Location });


            // create Host in the worker app domain
            return (T)appManager.CreateObject(appId, typeof(T), virtualPath, physicalPath, false);
        }
    }
}
