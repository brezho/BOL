using Host.Infrastructure.VPP;
using X.AspNet.Services.Multilingual;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.AspNet.Infrastructure.Application
{
    public interface IWebApplication
    {
        IServiceContainer Services { get; }
        string PublicPrefix { get; }
        string Version();
        bool IsLiveEnvironment { get; }
        bool InsertMissingTranslations { get; }
        IUserLanguageProvider LanguageProvider { get; }
        ILanguageRepository LanguageRepository { get; }
        ITranslationRepository TranslationRepository { get; }
        VirtualPathDehydrationDelegate VirtualPathShortener { get; }

        string BasePageUrl { get;  }
    }
}
