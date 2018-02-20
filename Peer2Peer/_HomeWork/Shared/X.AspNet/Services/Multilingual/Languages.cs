using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.AspNet.Services.Multilingual
{
    public class Languages
    {
        static Languages()
        {
            MultiLingualCache.CacheInvalidated += new CacheInvalidatedDelegate(MultiLingualCache_CacheInvalidated);
        }

        static void MultiLingualCache_CacheInvalidated()
        {
            defaultLanguage = null;
            enabled = null;
        }

        public static LanguageDTO Get(string Code)
        {
            return MultiLingualCache.AllLanguages().FirstOrDefault(x => x.Code == Code);
        }

        static LanguageDTO defaultLanguage = null;
        public static LanguageDTO ApplicationDefault
        {
            get
            {
                if (defaultLanguage == null)
                {
                    defaultLanguage = MultiLingualCache.AllLanguages().FirstOrDefault(x => x.IsDefault);
                }
                return defaultLanguage;
            }
        }

        static IEnumerable<LanguageDTO> enabled = null;
        public static IEnumerable<LanguageDTO> Enabled()
        {
            if (enabled == null)
            {
                enabled = MultiLingualCache.AllLanguages().Where(x => x.IsEnabled);
            }
            return enabled;
        }

        //internal void Validate(Language item)
        //{
        //    if(string.IsNullOrWhiteSpace(item.Code))
        //        throw new ApplicationException("[[Internal.A language must be provided a Code]]");
        //    if (IsDefault && IsEnabled)
        //        throw new ApplicationException("[[Internal.A language can not be the Default one and be Disabled at same time]]");
        //}
    }

}
