namespace TownOfUsReworked.Options;

public static class Generate
{
    private static bool Generated;

    public static void GenerateAll()
    {
        if (Generated)
            return;

        Generated = true;

        // Since I can't really get a default value at compile time for client options (since they save externally), I just do this to set their "defaults" and move on from there
        ClientOptions.SetDefaults();

        // Might lead to initial performance issues trying to look through like, 200+ types
        // Could it be improved? Probably
        // Do I care enough to do that? Hell no until it becomes a problem

        // So, this is AD from a couple weeks after the above 3 comments, I thought to myself on the toilet "What if I just used a class for the headers instead of another property? Then I don't need to add a random getter setter and I don't have to make the string arrays either because I can just get the declared properties and use their names as the array" and then this genius thing was born
        AccessTools.GetTypesFromAssembly(TownOfUsReworked.Core).ForEach(y => y.GetCustomAttribute<HeaderOptionAttribute>()?.SetTypeAndOptions(y));
        AccessTools.GetTypesFromAssembly(TownOfUsReworked.Core).ForEach(y => y.GetCustomAttribute<AlignsOptionAttribute>()?.SetTypeAndOptions(y));

        // Simple enough, I'm too cautious to let something fuck me up while I set the properties
        OptionAttribute.AllOptions.ForEach(x => x.PostLoadSetup());

        OptionAttribute.SaveSettings("Default");

        LogMessage($"There exist {OptionAttribute.AllOptions.Count} total options lmao (number jumpscare)");
    }
}