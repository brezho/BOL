namespace X.Documents
{
    public interface ITranslationProvider
    {
        string TranslateStream(string stream);
        string GetTranslation(string key);
        string GetTranslation(string resourceKey, string languageId);
    }
}
