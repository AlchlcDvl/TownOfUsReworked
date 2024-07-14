namespace TownOfUsReworked.Options2;

public static class Generate2
{
    private static bool Generated;

    public static void GenerateAll()
    {
        if (Generated)
            return;

        Generated = true;

        // Since I can't really get a default value at compile time for client options (since they save externally), I just do this to set their "defaults" and move on from there
        ClientGameOptions2.SetDefaults();

        // Might lead to initial performance issues trying to look through like, 200+ types
        // Could it be improved? Probably
        // Do I care enough to do that? Hell no until it becomes a problem
        AccessTools.GetTypesFromAssembly(TownOfUsReworked.Core).ForEach(y => AccessTools.GetDeclaredProperties(y).ForEach(x => x.GetCustomAttribute<OptionAttribute>()?.SetProperty(x)));

        // Simple enough, I'm too cautious to let something fuck me up while I set the properties
        OptionAttribute.AllOptions.ForEach(x => x.PostLoadSetup());

        OptionAttribute.SaveSettings("Default");

        LogMessage($"There exist {OptionAttribute.AllOptions.Count} total options lmao (number jumpscare)");
    }
}