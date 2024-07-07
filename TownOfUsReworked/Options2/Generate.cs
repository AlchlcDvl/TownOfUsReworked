namespace TownOfUsReworked.Options2;

public static class Generate2
{
    private static bool Generated;

    public static void GenerateAll()
    {
        if (Generated)
            return;

        Generated = true;

        ClientGameOptions2.SetDefaults();

        AccessTools.GetDeclaredProperties(typeof(CustomGameOptions2))?.ForEach(x => x.GetCustomAttribute<OptionAttribute>()?.SetProperty(x));
        AccessTools.GetDeclaredProperties(typeof(ClientGameOptions2))?.ForEach(x => x.GetCustomAttribute<OptionAttribute>()?.SetProperty(x));

        OptionAttribute.AllOptions.ForEach(x => x.PostLoadSetup());

        OptionAttribute.SaveSettings("Default");

        LogMessage($"There exist {OptionAttribute.AllOptions.Count + 2} total options lmao (number jumpscare)");
    }
}