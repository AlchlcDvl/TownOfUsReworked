namespace TownOfUsReworked.Modules;

public class Language
{
    [JsonPropertyName("id")]
    public string ID { get; set; }

    [JsonPropertyName("english")]
    public string English { get; set; }

    [JsonPropertyName("schinese")]
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

            if (IsNullEmptyOrWhiteSpace(result) || !TranslationManager.SupportedLangNames.Contains(lang))
                throw new NotImplementedException(lang);
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
                throw new NotImplementedException(lang);
        }
    }

    public override string ToString() => TranslationManager.Test(ID);
}

public class LanguageJSON
{
    [JsonPropertyName("languages")]
    public List<Language> Languages { get; set; }
}