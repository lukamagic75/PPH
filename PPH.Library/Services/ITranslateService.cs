namespace PPH.Library.Services;

public interface ITranslateService
{
    Task<string> Translate(string text, string fromLanguage, string toLanguage);
}
