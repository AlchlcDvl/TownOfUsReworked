namespace TownOfUsReworked.Modules;

public class Language : Asset
{
    [JsonPropertyName("english")]
    public string English { get; set; }

    [JsonPropertyName("schinese")]
    public string SChinese { get; set; }

    [JsonPropertyName("ids")]
    public List<string> IDs { get; set; } // For when I want multiple IDs to point to the same thing but I'm too lazy to add their own entries to the json

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

            if (IsNullEmptyOrWhiteSpace(result) || !TranslationManager.SupportedLangNames.Contains(lang))
            {
                if (English != null)
                {
                    LogError($"Selected language is unsupported {lang}");
                    return English;
                }
                else
                    throw new UnsupportedLanguageException(lang);
            }
            else
                return result;
        }
        set
        {
            if (lang == "English")
                English = value;
            else if (lang == "SChinese")
                SChinese = value;
            else
                throw new UnsupportedLanguageException(lang);
        }
    }

    public override string ToString()
    {
        if (ID != null)
            return TranslationManager.Test(ID);
        else
        {
            var result = "";
            IDs.ForEach(id => result += $"{TranslationManager.Test(id)}\n");
            return result;
        }
    }
}