namespace TownOfUsReworked.Managers;

public static class TranslationManager
{
    public static readonly Dictionary<string, Language> AllTranslations = []; // Used to store all translations
    public static readonly Dictionary<StringNames, string> CustomStringNames = []; // Used to store custom string names
    public static readonly Dictionary<string, StringNames> CustomStringNamesAgain = []; // Used to store custom string names, but in reverse
    public static readonly Dictionary<StringNames, StringNames> VanillaToCustomMap = []; // Used to remap vanilla string names to custom ones

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
            return $"STRMISS ({id})";
        }
    }

    // Overrides for the translate method above
    public static string Translate(string id, params (string Key, string Value)[] toReplace) => Translate(id, toReplace, null);

    // public static string Translate(string id, string language, params (string Key, string Value)[] toReplace) => Translate(id, toReplace, language);

    public static bool IdExists(string id) => AllTranslations.ContainsKey(id);

    public static StringNames GetNextName(string id, StringNames vanillaName = StringNames.None, bool isStartup = false)
    {
        if (LastID == -1)
            LastID = NextID = (int)Enum.GetValues<StringNames>().Last(); // Getting the very last enum value and casting it to int

        NextID++; // Increment to the next value
        var value = (StringNames)NextID; // Cast the int to the enum
        CustomStringNames[value] = id; // Add the id to the dictionary
        CustomStringNamesAgain[id] = value; // Add the id to the reverse dictionary

        // If the custom value overrides translations of a vanilla one, add it to the dictionary for later remapping
        if (vanillaName != StringNames.None)
            VanillaToCustomMap[vanillaName] = value;

        // Debug log the id if I forgot to add a translation for it
        if (!isStartup && !IdExists(id))
            Fatal(id);

        return value;
    }

    public static bool Translate(StringNames id, out string result)
    {
        result = "STRMISS";

        // Check if the given ID has a custom mapping and update the ID if found
        if (VanillaToCustomMap.TryGetValue(id, out var customId))
            id = customId;

        // Try to find a language module for the given (possibly updated) ID
        if (CustomStringNames.TryGetValue(id, out var value))
        {
            var lang = Translate(value);

            // If the translation is successful, update the result with the translated string
            if (!lang.Contains(value))
                result = lang;
            else
                result += $" ({value})";

            return true;
        }

        return false;
    }

    // public static StringNames GetOrAddName(string id, StringNames vanillaName = StringNames.None, bool isStartup = false)
    // {
    //     if (CustomStringNamesAgain.TryGetValue(id, out var value)) // Try to find a custom string name by the given id
    //         return value;

    //     if (VanillaToCustomMap.TryGetValue(vanillaName, out var customName)) // Contingency in case the value could not be found
    //         return customName;

    //     return GetNextName(id, vanillaName, isStartup); // Add the new id if it could not be found
    // }
}