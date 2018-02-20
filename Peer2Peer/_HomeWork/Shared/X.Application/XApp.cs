using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace X.Application
{
    public abstract class XApp
    {
        public static XApp Current { get; internal set; }
        public abstract bool IsLiveEnvironment { get; }
        protected internal virtual void OnInstall(params string[] args) { }
        protected internal virtual void OnInitialize(params string[] args) { }
        protected internal abstract void OnStart();
        protected internal virtual void OnStop() { }
        public string Version
        {
            get
            {
                var version = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetCallingAssembly().Location).FileVersion;
                var splitted = version.Split('.');
                var stringVer = splitted[0] + "." + splitted[1] + "." + splitted[2];
                if (!IsLiveEnvironment) { stringVer += "." + splitted[3]; }
                return stringVer;
            }
        }
    }

}
