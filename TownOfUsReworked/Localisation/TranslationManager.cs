namespace TownOfUsReworked.Localisation;

public static class TranslationManager
{
    public static readonly List<Language> AllTranslations = [];

    private static string CurrentLanguage => DataManager.Settings.Language.CurrentLanguage switch
    {
        SupportedLangs.SChinese => "SChinese",
        _ => "English"
    };

    public static readonly string[] SupportedLangNames = [ "English", "SChinese" ];

    public static string Translate(string id, (string Key, string Value)[] toReplace, string language)
    {
        language ??= CurrentLanguage;

        try
        {
            var result = AllTranslations.Find(x => x.ID == id || x.IDs?.Contains(id) == true)?[language];
            toReplace.ForEach(x => result = result.Replace(x.Key, x.Value));
            return result ?? throw new UnsupportedLanguageException($"{language}:{id}");
        }
        catch
        {
            Error($"Unable to translate {id} to {language}");
            return id;
        }
    }

    public static string Translate(string id, params (string Key, string Value)[] toReplace) => Translate(id, toReplace, null);

    public static string Translate(string id, string language, params (string Key, string Value)[] toReplace) => Translate(id, toReplace, language);

    public static string Test(string id)
    {
        var result = $"ID: {id}";
        result += $"\nCurrent Language: {CurrentLanguage}";
        SupportedLangNames.ForEach(x => result += $"\n{x}: {Translate(id, x)}");
        return result;
    }
}