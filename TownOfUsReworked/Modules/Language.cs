namespace TownOfUsReworked.Modules;

public class Language
{
    public string ID { get; set; }
    public string English { get; set; }
    public string SChinese { get; set; }

    public string this[string lang]
    {
        get
        {
            var result = lang switch
            {
                "English" => English,
                "SChinese" => SChinese,
                _ => ""
            };

            if (IsNullEmptyOrWhiteSpace(result) || !SupportedLangs.Contains(lang))
                throw new NotImplementedException(lang);
            else
                return result;
        }
    }

    public override string ToString() => Test(ID);

    public static string CurrentLanguage
    {
        get => (int)DataManager.Settings.Language.CurrentLanguage switch
        {
            13 => "SChinese",
            _ => "English"
        };
    }

    private static readonly List<string> SupportedLangs = new() { "English", "SChinese" };

    public static string Translate(string id, string language = null)
    {
        var lang = language ?? CurrentLanguage;

        try
        {
            return AssetLoader.AllTranslations.Find(x => x.ID == id)[lang];
        }
        catch
        {
            LogError($"Unable to translate {id} to {lang}");
            return id;
        }
    }

    public static string Test(string id)
    {
        var result = $"ID: {id}";
        result += $"\nCurrent Language: {CurrentLanguage}";
        SupportedLangs.ForEach(x => result += $"\n{x}: {Translate(id, x)}");
        return result;
    }
}