namespace TownOfUsReworked.Modules;

public class Language : Asset
{
    [JsonPropertyName("values")]
    public LangModule[] Values { get; set; }

    [JsonPropertyName("isblank")]
    public bool IsBlank { get; set; } // I need this here to stop the translator from spamming errors for intentionally blank translations

    [JsonPropertyName("ids")]
    public string[] IDs { get; set; } // For when I want multiple IDs to point to the same thing but I'm too lazy to add their own entries to the json

    public string this[string lang]
    {
        get
        {
            if (IsBlank)
                return "";

            var result = Values.FirstOrDefault(x => x.Name == lang)?.Value;

            if (IsNullEmptyOrWhiteSpace(result) && lang != "English")
                result = Values.FirstOrDefault(x => x.Name == "English")?.Value;

            return result ?? throw new UnsupportedLanguageException($"{lang} unsupported by {ID ?? IDs[0]}");
        }
    }

    // public string GetIDs()
    // {
    //     if (ID != null)
    //         return ID;
    //     else if (IDs != null)
    //     {
    //         var result = "";
    //         IDs.ForEach(x => result += $"{x} ");
    //         return result.Trim();
    //     }

    //     return "Error";
    // }

    // public bool HasID(string id) => (ID == id || IDs?.Contains(id) == true) && !IsNullEmptyOrWhiteSpace(id) && Values.Any();

    // public override string ToString()
    // {
    //     if (ID != null)
    //         return TranslationManager.Test(ID);
    //     else
    //         return TranslationManager.Test(IDs[0]);
    // }
}

public class LangModule
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }
}