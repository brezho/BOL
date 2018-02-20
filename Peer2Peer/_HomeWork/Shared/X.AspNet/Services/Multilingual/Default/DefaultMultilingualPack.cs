using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.AspNet.Services.Multilingual.Default
{
    public class DefaultMultilingualPack :
        IUserLanguageProvider,
        ILanguageRepository,
        ITranslationRepository
    {
        public static readonly LanguageDTO DefaultLanguage = new LanguageDTO { Code = "EN", Description = "[[Languages.EN]]", IsDefault = true, IsEnabled = true };

        public LanguageDTO GetUserLanguage(string userName)
        {
            return DefaultLanguage;
        }

        public IEnumerable<LanguageDTO> AllLanguages()
        {
            yield return DefaultLanguage;
        }

        List<TranslationDTO> translations = new List<TranslationDTO>();
        public IEnumerable<TranslationDTO> AllTranslations()
        {
            return translations;
        }

        public void Insert(TranslationDTO translation)
        {
            System.Diagnostics.Trace.WriteLine(translation.ToString());
            translations.Add(translation);
        }
    }
}
