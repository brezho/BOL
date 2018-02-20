using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.AspNet.Services.Multilingual
{
    delegate void CacheInvalidatedDelegate();
    static class MultiLingualCache
    {
        public static event CacheInvalidatedDelegate CacheInvalidated;
        private static readonly List<LanguageDTO> _languagesCache = new List<LanguageDTO>();
        private static readonly List<TranslationDTO> _translationsCache = new List<TranslationDTO>();

        static MultiLingualCache()
        {
            Reload();
        }

        internal static void Reload()
        {
            _languagesCache.Clear();
            _translationsCache.Clear();
            if (WebApp.Current.LanguageRepository.IsNotNull()) _languagesCache.AddRange(WebApp.Current.LanguageRepository.AllLanguages());
            if (WebApp.Current.TranslationRepository.IsNotNull()) _translationsCache.AddRange(WebApp.Current.TranslationRepository.AllTranslations());
            if (CacheInvalidated != null) CacheInvalidated();
        }

        public static IEnumerable<LanguageDTO> AllLanguages()
        {
            return _languagesCache;
        }

        public static IEnumerable<TranslationDTO> AllTranslations()
        {
            return _translationsCache;
        }
    }
}
