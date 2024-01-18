namespace TownOfUsReworked.Localisation;

public static class TranslationManager
{
    public static readonly List<Language> AllTranslations = new();

    public static string CurrentLanguage => DataManager.Settings.Language.CurrentLanguage switch
    {
        SupportedLangs.SChinese => "SChinese",
        _ => "English"
    };

    public static readonly string[] SupportedLangNames = { "English", "SChinese" };

    public static string Translate(string id, string language = null)
    {
        language ??= CurrentLanguage;

        try
        {
            return AllTranslations.Find(x => x.ID == id)[language];
        }
        catch
        {
            LogError($"Unable to translate {id} to {language}");
            return id;
        }
    }

    public static string Test(string id)
    {
        var result = $"ID: {id}";
        result += $"\nCurrent Language: {CurrentLanguage}";
        SupportedLangNames.ForEach(x => result += $"\n{x}: {Translate(id, x)}");
        return result;
    }
}