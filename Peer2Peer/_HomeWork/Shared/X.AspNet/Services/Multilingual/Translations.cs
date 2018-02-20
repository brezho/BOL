using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace X.AspNet.Services.Multilingual
{
    public class Translations
    {
        public static class ConfigKeys
        {
            public const string InsertMissingLabels = "Translation.InsertMissingLabels";
        }

        static bool ExceptionRaisedOnce = false;

        public const string ResourceKeyRegEx = @"[\[]{2,3}[\w]+\.[\w\s\*\/\\\?\.\(\)\-#:;&'\+\{\}]*[\]]{2,3}";

        public static object locker = new object();

        /// <summary>
        /// Translates the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static string Translate(string stream)
        {
            var provider = WebApp.Current.LanguageProvider;
            if (!provider.IsNull())
            {
                return Translate(stream, provider.GetUserLanguage(Operator.Current).Code);
            }
            if (!ExceptionRaisedOnce)
            {
                System.Diagnostics.Trace.TraceError("To use Translate method without arguments you should make your Application implement IUserLanguageProvider");
                ExceptionRaisedOnce = true;
            }
            if (Languages.ApplicationDefault.IsNull())
            {
                return Translate(stream, null);
            }
            return Translate(stream, Languages.ApplicationDefault.Code);
        }

        /// <summary>
        /// Translates the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="languageId">The language id.</param>
        /// <returns></returns>
        /// 
        static Regex r = new Regex(ResourceKeyRegEx, RegexOptions.Compiled);

        public static string Translate(string stream, string LanguageCode)
        {
            if (stream == null) return null;
            if (!LanguageCode.IsFilled()) return stream;
            return r.Replace(stream, m => GetTranslation(m.Value, LanguageCode));
            //  return Regex.Replace(stream, ResourceKeyRegEx, m => GetTranslation(m.Value, LanguageCode), RegexOptions.Compiled);
        }

        /// <summary>
        /// Gets the translation for the given resource key (label).
        /// The format of the <paramref name="resourceKey"/> must match with
        /// one the following patterns: [[label]] or [[[label]]].
        /// The first pattern represents the long translation and the second
        /// pattern represents the short translation of the label.
        /// </summary>
        /// <param name="resourceKey">The resource key.</param>
        /// <returns>
        /// The translation corresponding to the <paramref name="resourceKey"/> 
        /// parameter.
        /// </returns>
        public static string GetTranslation(string resourceKey)
        {
            EnsureMultiLingualCache();

            var provider = WebApp.Current.LanguageProvider;
            if (!provider.IsNull())
            {
                return GetTranslation(resourceKey, provider.GetUserLanguage(Operator.Current).Code);
            }
            System.Diagnostics.Trace.TraceError("To use Translate method without arguments you should make your Application implement IUserLanguageProvider");
            return GetTranslation(resourceKey, Languages.ApplicationDefault.Code);
        }

        /// <summary>
        /// Gets the translation for the given resource key (label) and languageId.
        /// The format of the <paramref name="resourceKey"/> must match with
        /// one the following patterns: [[label]] or [[[label]]].
        /// The first pattern represents the long translation and the second
        /// pattern represents the short translation of the label.
        /// If the translation is not found in the given language id, the default
        /// language id is used.
        /// </summary>
        /// <param name="resourceKey">The resource key.</param>
        /// <param name="languageId">The language id.</param>
        /// <returns>
        /// The translation corresponding to the <paramref name="resourceKey"/> 
        /// parameter.
        /// </returns>
        public static string GetTranslation(string resourceKey, string languageCode)
        {
            if (resourceKey == null || resourceKey.Length == 0) return null;

            string label = null;

            try
            {
                if (resourceKey.StartsWith("[[")) resourceKey = resourceKey.TrimStart("[[").TrimEnd("]]");

                if (languageCode.IsNotNull())
                {
                    var lbl = MultiLingualCache.AllTranslations()
                        .Where(x => String.Equals(x.Key, resourceKey, StringComparison.InvariantCultureIgnoreCase) && x.LanguageCode == languageCode)
                        .FirstOrDefault();

                    if (lbl != null) label = lbl.Value;

                    if (label == null && languageCode != Languages.ApplicationDefault.Code)
                    {
                        lbl = MultiLingualCache.AllTranslations()
                            .Where(x => String.Equals(x.Key, resourceKey, StringComparison.InvariantCultureIgnoreCase) && x.LanguageCode == languageCode)
                            .FirstOrDefault();

                        if (lbl != null) label = lbl.Value;
                    }
                }

                if (label == null)
                {
                    var index = resourceKey.LastIndexOf('.');
                    label = resourceKey.Substring(index + 1, resourceKey.Length - index - 1);

                    if (label != null && WebApp.Current.InsertMissingTranslations)
                    {
                        // Insert the label into the Framework database
                        WebApp.Current.TranslationRepository.Insert(
                            new TranslationDTO
                            {
                                Key = resourceKey,
                                Value = label,
                                LanguageCode = Languages.ApplicationDefault.Code
                            });

                        MultiLingualCache.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error: Multilingual.Transalation.GetTranslation",
                    "ResourceKey: " + resourceKey + Environment.NewLine + Environment.NewLine + ex.Format());
            }

            return label;
        }

        /// <summary>
        /// Extracts the label code from bracketed format.
        /// </summary>
        /// <param name="bracketed">The bracketed.</param>
        /// <returns></returns>
        public string ExtractLabelCodeFromBracketed(string bracketed)
        {
            if (bracketed == null || bracketed.Length == 0 || bracketed.IndexOf("[[") < 0) return bracketed;

            bool isShort = bracketed.IndexOf("[[[") >= 0;
            return isShort ? bracketed.Substring(3, bracketed.Length - 6) : bracketed.Substring(2, bracketed.Length - 4);
        }

        private static void EnsureMultiLingualCache()
        {
            lock (locker)
            {
                if (!MultiLingualCache.AllTranslations().IsFilled() || !MultiLingualCache.AllLanguages().IsFilled())
                {
                    MultiLingualCache.Reload();
                }
            }
        }
    }
}
