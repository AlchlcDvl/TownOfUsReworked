using System.Threading.Tasks;

namespace TownOfUsReworked.Managers;

public static class TranslationManager
{
    public static readonly Dictionary<string, Language> AllTranslations = []; // Used to store all translations
    private static readonly ValueMap<StringNames, string> CustomStringNames = []; // Used to store custom string names that trace to a string id
    public static readonly Dictionary<StringNames, StringNames> VanillaToCustomMap = []; // Used to remap vanilla string names to custom ones
    private static readonly Dictionary<StringNames, List<StringNames>> CustomToCustom = []; // Used to map multiple custom ids to the same one
    private static readonly Dictionary<string, (string Key, Func<string> Value)[]> ReplacementsMap = [];
    private static readonly List<string> MissingIds = [];
    private static int NextID;

    private static string Translate(string id, (string Key, string Value)[] toReplace, string language)
    {
        id = id.ToLower();
        language ??= DataManager.Settings.Language.CurrentLanguage.ToString().ToLower(); // Get the current language if none is provided

        try
        {
            var result = AllTranslations[id][language]; // Get and translate the result
            toReplace.Do(x => result = result.Replace(x.Key, x.Value)); // Replace the placeholders with the given values
            return result;
        }
        catch
        {
            // Any error should just return the original id and let the client know in the logs
            DebugId(id);
            return $"STRMISS ({id})";
        }
    }

    // Overrides for the translation method above
    public static string Translate(string id, params (string Key, string Value)[] toReplace) => Translate(id, toReplace, null);

    // public static string Translate(string id, string language, params (string Key, string Value)[] toReplace) => Translate(id, toReplace, language);

    public static bool IdExists(string id) => AllTranslations.ContainsKey(id.ToLower());

    public static void DebugId(string id)
    {
        id = id.ToLower();

        if (IdExists(id) || MissingIds.Contains(id))
            return;

        Fatal(id);
        MissingIds.Add(id);
    }

    private static StringNames AddNextName(string id, StringNames vanillaName = StringNames.None, StringNames customName = StringNames.None, (string Key, Func<string> Value)[] replacements = null)
    {
        id = id.ToLower();
        NextID--; // Decrement to the next value
        var value = (StringNames)NextID; // Cast the int to the enum
        CustomStringNames[value] = id; // Add the id to the dictionary

        // If the custom value overrides translations of a vanilla one, add it to the dictionary for later remapping
        if (vanillaName != StringNames.None)
            VanillaToCustomMap[vanillaName] = value;

        // If the custom value basically has the same id as the provided one, then add it to the dictionary for easy mapping
        if (customName != StringNames.None)
        {
            if (CustomToCustom.TryGetValue(customName, out var list))
                list.Add(value);
            else
                CustomToCustom[customName] = [value];
        }

        if (replacements != null)
            ReplacementsMap[id] = replacements;

        return value;
    }

    public static bool Translate(StringNames id, out string result)
    {
        result = "STRMISS";

        // Check if the given ID has a custom mapping and update the ID if found
        if (VanillaToCustomMap.TryGetValue(id, out var customId))
            id = customId;

        // Finding replacements that change in value at certain times or are to be invoked later due to delayed stuff
        (string Key, Func<string> Value)[] replacements = null;
        var hasReplacements = CustomStringNames.TryGetValue(id, out var val) && ReplacementsMap.TryGetValue(val, out replacements);

        // Check if the id is linked to another
        if (CustomToCustom.TryFinding(x => x.Value.Contains(id), out var pair))
            id = pair.Key;

        // Try to find a language module for the given (possibly updated) ID
        if (!CustomStringNames.TryGetValue(id, out var value))
            return false;

        // Don't ask me why I did this, I don't know either
        (string Key, Func<string> Value)[] otherReplacements = null;
        var hasOtherReplacements = val != value && ReplacementsMap.TryGetValue(value, out otherReplacements);
        var lang = hasOtherReplacements
            ? (hasReplacements
                ? Translate(value, [ .. replacements.Select(x => (x.Key, x.Value())), .. otherReplacements.Select(x => (x.Key, x.Value())) ])
                : Translate(value, [ .. otherReplacements.Select(x => (x.Key, x.Value())) ]))
            : (hasReplacements
                ? Translate(value, [ .. replacements.Select(x => (x.Key, x.Value())) ])
                : Translate(value));

        // If the translation is successful, update the result with the translated string
        if (value != null && !lang.Contains(value))
            result = lang;
        else
            result += $" ({value})";

        return true;
    }

    public static StringNames GetOrAddName(string id, StringNames vanillaName = StringNames.None, StringNames customName = StringNames.None, (string Key, Func<string> Value)[] replacements = null)
    {
        id = id.ToLower();

        if (CustomStringNames.TryGetKey(id, out var value)) // Try to find a custom string name by the given id
            return value;

        return VanillaToCustomMap.TryGetValue(vanillaName, out value) ? // Contingency in case the value could not be found
            value : AddNextName(id, vanillaName, customName, replacements); // Add and return the new id if it could not be found
    }
}