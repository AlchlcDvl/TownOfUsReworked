namespace TownOfUsReworked.Modules;

public class Language : Asset
{
    [JsonPropertyName("english")]
    public string English { get; set; }

    [JsonPropertyName("schinese")]
    public string SChinese { get; set; }

    [JsonPropertyName("notes")]
    public string Notes { get; set; } // I need this here to stop the translator from spamming errors for intentionally blank translations

    [JsonPropertyName("ids")]
    public string[] IDs { get; set; } // For when I want multiple IDs to point to the same thing but I'm too lazy to add their own entries to the json

    public string this[string lang]
    {
        get
        {
            if (Notes != null)
                return "";

            var result = lang switch
            {
                "English" => English,
                "SChinese" => SChinese,
                _ => ""
            };
            var ids = GetIDs();

            if (IsNullEmptyOrWhiteSpace(result) || !TranslationManager.SupportedLangNames.Contains(lang))
            {
                if (English != null)
                {
                    Error($"Selected language is unsupported {lang} ({ids})");
                    return English;
                }
                else
                    throw new UnsupportedLanguageException($"{lang}:{ids}");
            }
            else
                return result ?? throw new UnsupportedLanguageException($"{lang}:{ids}");
        }
        set
        {
            if (lang == "English")
                English = value;
            else if (lang == "SChinese")
                SChinese = value;
            else
                throw new UnsupportedLanguageException($"{lang}:{GetIDs()}");
        }
    }

    public string GetIDs()
    {
        if (ID != null)
            return ID;
        else if (IDs != null)
        {
            var result = "";
            IDs.ForEach(x => result += $"{x} ");
            return result;
        }

        return "Error";
    }

    public bool HasID(string id) => (ID == id || IDs?.Contains(id) == true) && !IsNullEmptyOrWhiteSpace(English);

    public override string ToString()
    {
        if (ID != null)
            return TranslationManager.Test(ID);
        else
            return TranslationManager.Test(IDs[0]);
    }
}