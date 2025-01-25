namespace TownOfUsReworked.Managers;

public static class TranslationManager
{
    public static readonly Dictionary<string, Language> AllTranslations = []; // Used to store all translations
    public static readonly ValueMap<StringNames, string> CustomStringNames = []; // Used to store custom string names that trace to a string id
    public static readonly Dictionary<StringNames, StringNames> VanillaToCustomMap = []; // Used to remap vanilla string names to custom ones
    public static readonly Dictionary<StringNames, List<StringNames>> CustomToCustom = []; // Used to map multiple custom ids to the same one
    public static readonly Dictionary<string, (string Key, Func<string> Value)[]> ReplacementsMap = [];
    private static readonly List<string> MissingIds = [];

    // Values for comparisons
    public static int LastID = -1;
    private static int NextID = -1;

    public static int PreviousLastID;

    public static string Translate(string id, (string Key, string Value)[] toReplace, string language)
    {
        language ??= DataManager.Settings.Language.CurrentLanguage.ToString(); // Get the current language if none is provided

        try
        {
            var result = AllTranslations[id][language]; // Get and translate the result
            toReplace.ForEach(x => result = result.Replace(x.Key, x.Value)); // Replace the placeholders with the given values
            return result;
        }
        catch
        {
            // Any error should just return the original id and let client know in the logs
            if (!MissingIds.Contains(id))
            {
                Fatal(id);
                MissingIds.Add(id);
            }

            return $"STRMISS ({id})";
        }
    }

    // Overrides for the translate method above
    public static string Translate(string id, params (string Key, string Value)[] toReplace) => Translate(id, toReplace, null);

    // public static string Translate(string id, string language, params (string Key, string Value)[] toReplace) => Translate(id, toReplace, language);

    public static bool IdExists(string id) => AllTranslations.ContainsKey(id);

    public static void DebugId(string id)
    {
        if (!IdExists(id) && !MissingIds.Contains(id))
        {
            Fatal(id);
            MissingIds.Add(id);
        }
    }

    private static StringNames GetNextName(string id, StringNames vanillaName = StringNames.None, StringNames customName = StringNames.None, (string Key, Func<string>
        Value)[] replacements = null)
    {
        if (LastID == -1)
            LastID = NextID = (int)Enum.GetValues<StringNames>().Last(); // Getting the very last enum value and casting it to int

        NextID++; // Increment to the next value
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
                CustomToCustom[customName] = [ value ];
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

        (string Key, Func<string> Value)[] replacements = null;
        var hasReplacements = CustomStringNames.TryGetValue(id, out var val) && ReplacementsMap.TryGetValue(val, out replacements);

        // Check if the id is linked to another
        if (CustomToCustom.TryFinding(x => x.Value.Contains(id), out var pair))
            id = pair.Key;

        // Try to find a language module for the given (possibly updated) ID
        if (CustomStringNames.TryGetValue(id, out var value))
        {
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
            if (!lang.Contains(value))
                result = lang;
            else
                result += $" ({value})";

            return true;
        }

        return false;
    }

    public static StringNames GetOrAddName(string id, StringNames vanillaName = StringNames.None, StringNames customName = StringNames.None, (string Key, Func<string>
        Value)[] replacements = null)
    {
        if (CustomStringNames.TryGetKey(id, out var value)) // Try to find a custom string name by the given id
            return value;

        if (VanillaToCustomMap.TryGetValue(vanillaName, out value)) // Contingency in case the value could not be found
            return value;

        return GetNextName(id, vanillaName, customName, replacements); // Add and return the new id if it could not be found
    }
}