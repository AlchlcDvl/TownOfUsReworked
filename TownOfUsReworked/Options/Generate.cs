namespace TownOfUsReworked.Options;

/// <summary>
/// The static class to generate options based on their relevant attributes during game initialisation.
/// </summary>
public static class Generate
{
    /// <summary>
    /// Called to generate the necessary setting for the game option attributes. This takes place only once and during game initialisation.
    /// </summary>
    public static void GenerateAll()
    {
        // Might lead to initial performance issues trying to look through like, 200+ types
        // Could it be improved? Probably
        // Do I care enough to do that? Hell no until it becomes a problem

        // So, this is AD from a couple of weeks after the above 3 comments
        // I thought to myself on the toilet, What if I just used a class for the headers instead of another property?
        // Then I don't need to add a random getter setter. I don't have to make the string arrays too because I can just get the declared properties and use their names as the array...
            // and then this genius thing was born
        var assassin = typeof(Assassin);
        AccessTools.GetTypesFromAssembly(TownOfUsReworked.Core)
            // For some reason the attribute is being inherited by the assassin classes, even though I've stated that it shouldn't in the attribute usage
            // Future AD here: this is probably from the fact that Assassin itself is an abstract class
            .Where(x => !assassin.IsAssignableFrom(x) || x == assassin)
            .Do(y => y.GetCustomAttribute<BaseHeaderOptionAttribute>()?.SetTypeAndOptions(y));

        foreach (var opts in Option.AllOptions.SplitBy(x => x.ID))
        {
            foreach (var (index, opt) in opts.Indexed())
            {
                // Fixing up accidental duped IDs, this ensures that every ID is unique (I hope)
                var toAdd = index == 0 ? "" : $"{index}";
                opt.ID += toAdd;
                opt.Name += toAdd;

                // Simple enough, I'm too cautious to let something fuck me up while I set the properties
                opt.PostLoadSetup();

                // Debugging and dropping ids in case I've forgotten to add translations for some settings and their values
                if (TownOfUsReworked.IsDev)
                    opt.Debug();
            }
        }

        if (TownOfUsReworked.IsDev)
        {
            // Sanitizing the languages json in case there are translation ids of settings that don't exist anymore
            foreach (var id in TranslationManager.AllTranslations.Keys.Where(x => x.StartsWith("customoption") && !x.EndsWithAny("entry", "ban")))
            {
                if (!Option.AllOptions.Any(x => x.IsId(id)))
                    Fatal(id);
            }

            // Not gonna make it dump debug statements for *every* role list entry
            Enum.GetValues<ListSlot>().Except([ListSlot.NeutralPros, ListSlot.ApocHarb, ListSlot.ApocDeity, ListSlot.GameMode, ListSlot.Abilities, ListSlot.Dispositions, ListSlot.Modifiers])
                .Do(x => TranslationManager.DebugId($"List.{x}"));
        }

        Option.SortedOptions.AddRange(Option.GetOptions<BaseHeaderOption>().OrderBy(x => x.Order)); // Sorting for headers

        Option.SaveSettings("Default"); // Save the current values as the default preset

        Option.LoadPreset("LastUsed", null); // Load the last used settings preset to ensure values persist

        Success($"There exist {Option.AllOptions.Count - Option.SortedOptions.Count} guaranteed options with 1040 more possible ones with {Option.SortedOptions.Count} headers (number jumpscare lmao)"); // Debug
    }
}