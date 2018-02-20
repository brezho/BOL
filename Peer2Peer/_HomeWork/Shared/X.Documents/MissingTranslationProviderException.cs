using System;

namespace Core.Documents
{
    public class MissingTranslationProviderException : ApplicationException
    {
        public MissingTranslationProviderException()
            : base("The input template expects that a TranslationProvider is provided")
        {
        }
    }
}