// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace TownOfUsReworked.Modules;

public sealed class Language : Asset
{
    [JsonPropertyName("values")]
    public LangModule[] Values { get; set; }

    [JsonPropertyName("isblank")]
    public bool IsBlank { get; set; } // I need this here to stop the translator from spamming errors for intentionally blank translations

    [JsonPropertyName("ids")]
    // ReSharper disable once InconsistentNaming
    public string[] IDs { get; set; } // For when I want multiple IDs to point to the same thing, but I'm too lazy to add their own entries to the JSON

    [JsonIgnore]
    public Dictionary<string, string> Modules { get; } = [];

    public string this[string lang]
    {
        get
        {
            if (IsBlank)
                return "";

            if ((!Modules.TryGetValue(lang, out var result) || IsNullEmptyOrWhiteSpace(result)) && lang != "English")
                result = Modules.GetValueOrDefault("English");

            return IsNullEmptyOrWhiteSpace(result) ? throw new($"{lang} unsupported by {ID ?? IDs[0]}") : result;
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

    // public override string ToString() => TranslationManager.Test(ID ?? IDs[0]);
}

public struct LangModule
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }
}