using Host.Infrastructure.Application;
using X.AspNet.Infrastructure.Application;
using X.AspNet.Infrastructure.VPP;
using X.AspNet.Services.Multilingual;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.AspNet
{
    public abstract class WebApp : IWebApplication
    {
        public static IWebApplication Current
        {
            get
            {
                return Bootstrapper.LoadedApplication;
            }
        }

        ServiceContainer container = new ServiceContainer();
        public WebApp()
        {
            var multi = new X.AspNet.Services.Multilingual.Default.DefaultMultilingualPack();
            container.AddService(typeof(ILanguageRepository), multi);
            container.AddService(typeof(ITranslationRepository), multi);
            container.AddService(typeof(IUserLanguageProvider), multi);
        }
        public virtual bool IsLiveEnvironment { get { return false; } }
        public virtual bool InsertMissingTranslations { get { return true; } }

        public IServiceContainer Services
        {
            get { return container; }
        }

        public abstract string PublicPrefix { get; }

        public virtual IUserLanguageProvider LanguageProvider
        {
            get
            {
                return (IUserLanguageProvider)container.GetService(typeof(IUserLanguageProvider));
            }
        }

        public virtual ILanguageRepository LanguageRepository
        {
            get
            {
                return (ILanguageRepository)container.GetService(typeof(ILanguageRepository));
            }
        }

        public virtual ITranslationRepository TranslationRepository
        {
            get
            {
                return (ITranslationRepository)container.GetService(typeof(ITranslationRepository));
            }
        }

        public virtual Host.Infrastructure.VPP.VirtualPathDehydrationDelegate VirtualPathShortener
        {
            get { return x => x; }
        }

        public abstract string BasePageUrl
        {
            get;
        }
        public string Version()
        {
            var version = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetCallingAssembly().Location).FileVersion;
            var splitted = version.Split('.');

            var stringVer = splitted[0] + "." + splitted[1] + "." + splitted[2];
            if (!IsLiveEnvironment)
            {
                stringVer += "." + splitted[3];
            }
            return stringVer;
        }

    }
}
