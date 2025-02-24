namespace TownOfUsReworked.Options;

public static class Generate
{
    public static void GenerateAll()
    {
        // Might lead to initial performance issues trying to look through like, 200+ types
        // Could it be improved? Probably
        // Do I care enough to do that? Hell no until it becomes a problem

        // So, this is AD from a couple of weeks after the above 3 comments
        // I thought to myself on the toilet, What if I just used a class for the headers instead of another property?
        // Then I don't need to add a random getter setter. I don't have to make the string arrays too because I can just get the declared properties and use their names as the array... and then this genius thing was born
        AccessTools.GetTypesFromAssembly(TownOfUsReworked.Core).ForEach(y => y.GetCustomAttribute<BaseHeaderOptionAttribute>()?.SetTypeAndOptions(y));

        // Fixing up accidental duped IDs, this ensures that every ID is unique (I hope)
        foreach (var opts in OptionAttribute.AllOptions.SplitBy(x => x.ID))
        {
            foreach (var (index, opt) in opts.Indexed())
            {
                var toAdd = index == 0 ? "" : $"{index}";
                opt.ID += toAdd;
                opt.Name += toAdd;
            }
        }

        // Simple enough, I'm too cautious to let something fuck me up while I set the properties
        foreach (var option in OptionAttribute.AllOptions)
        {
            option.PostLoadSetup();
            option.Debug();
        }

        Enum.GetValues<RoleListSlot>().ForEach(x => TranslationManager.DebugId($"List.{x}")); // Not gonna make it dump debug statements for *every* role list entry

        OptionAttribute.SortedOptions.AddRange(OptionAttribute.GetOptions<BaseHeaderOptionAttribute>().OrderBy(x => x.Priority)); // Sorting for headers

        OptionAttribute.SaveSettings("Default");

        OptionAttribute.LoadPreset("LastUsed", null);

        Success($"There exist {OptionAttribute.AllOptions.Count(x => x is not BaseHeaderOptionAttribute)} total options lmao out of {OptionAttribute.AllOptions.Count} (number jumpscare)");
    }
}