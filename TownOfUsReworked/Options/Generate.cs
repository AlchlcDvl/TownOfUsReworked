using Cpp2IL.Core.Extensions;

namespace TownOfUsReworked.Options;

public static class Generate
{
    private static bool Generated;

    public static void GenerateAll()
    {
        if (Generated)
            return;

        Generated = true;

        // Might lead to initial performance issues trying to look through like, 200+ types
        // Could it be improved? Probably
        // Do I care enough to do that? Hell no until it becomes a problem

        // So, this is AD from a couple weeks after the above 3 comments, I thought to myself on the toilet "What if I just used a class for the headers instead of another property? Then I don't need to add a random getter setter and I don't have to make the string arrays either because I can just get the declared properties and use their names as the array" and then this genius thing was born
        AccessTools.GetTypesFromAssembly(TownOfUsReworked.Core).Where(x => x.Name is not ("Bullseye" or "Hitman" or "Sniper" or "Slayer")).ForEach(y =>
            y.GetCustomAttribute<HeaderOptionAttribute>()?.SetTypeAndOptions(y));
        AccessTools.GetTypesFromAssembly(TownOfUsReworked.Core).ForEach(y => y.GetCustomAttribute<AlignsOptionAttribute>()?.SetTypeAndOptions(y));

        // Simple enough, I'm too cautious to let something fuck me up while I set the properties
        OptionAttribute.AllOptions.ForEach(x => x.PostLoadSetup());

        OptionAttribute.SaveSettings("Default");

        // Fixing up accidental duped IDs
        var d = OptionAttribute.AllOptions.Clone();
        var opts = new List<OptionAttribute>();

        foreach (var opt in OptionAttribute.AllOptions)
        {
            if (d.Any(x => x != opt && opt.ID == x.ID))
                opts.Add(opt);
        }

        // This ensures that every ID is unique (I hope)
        foreach (var opt in opts)
        {
            var index = OptionAttribute.AllOptions.Where(x => x.ID == opt.ID).ToList().IndexOf(opt);
            opt.ID += $"{(index == 0 ? "" : index.ToString())}";
        }

        opts.Clear();
        d.Clear();

        Message($"There exist {OptionAttribute.AllOptions.Count} total options lmao (number jumpscare)");
    }
}