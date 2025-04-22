// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace TownOfUsReworked.Modules;

public sealed class Language : Asset
{
    [JsonPropertyName("isBlank")]
    public bool IsBlank { get; set; } // I need this here to stop the translator from spamming errors for intentionally blank translations

    [JsonPropertyName("ids")]
    // ReSharper disable once InconsistentNaming
    public string[] IDs { get; set; } // For when I want multiple IDs to point to the same thing, but I'm too lazy to add their own entries to the JSON

    [JsonPropertyName("values")]
    public Dictionary<string, string> Values { get; set; }

    public string this[string lang]
    {
        get
        {
            if (IsBlank)
                return "";

            if ((!Values.TryGetValue(lang, out var result) || IsNullEmptyOrWhiteSpace(result)) && lang != "english")
                result = Values.GetValueOrDefault("english");

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