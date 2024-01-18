namespace TownOfUsReworked;

public partial class TownOfUsReworked
{
    private void SetUpConfigs()
    {
        Ip = Config.Bind("Custom", "Custom Server IP", "127.0.0.1", "IP for the Custom Server");
        Port = Config.Bind("Custom", "Custom Server Port", (ushort)22023, "Port for the Custom Server");
        LighterDarker = Config.Bind("Custom", "Lighter Darker Colors", true, "Adds smaller descriptions of colors as lighter or darker for body report purposes");
        WhiteNameplates = Config.Bind("Custom", "White Nameplates", false, "Enables custom nameplates");
        NoLevels = Config.Bind("Custom", "No Levels", false, "Enables the little level icon during meetings");
        CustomCrewColors = Config.Bind("Custom", "Custom Crew Colors", true, "Enables custom colors for Crew roles");
        CustomNeutColors = Config.Bind("Custom", "Custom Neutral Colors", true, "Enables custom colors for Neutral roles");
        CustomIntColors = Config.Bind("Custom", "Custom Intruder Colors", true, "Enables custom colors for Intruder roles");
        CustomSynColors = Config.Bind("Custom", "Custom Syndicate Colors", true, "Enables custom colors for Syndicate roles");
        CustomModColors = Config.Bind("Custom", "Custom Modifier Colors", true, "Enables custom colors for Modifiers");
        CustomObjColors = Config.Bind("Custom", "Custom Objectifier Colors", true, "Enables custom colors for Objectifiers");
        CustomAbColors = Config.Bind("Custom", "Custom Ability Colors", true, "Enables custom colors for Abilities");
        CustomEjects = Config.Bind("Custom", "Custom Ejects", true, "Enables funny ejection messages compared to the monotone \"X was ejected\"");
        OptimisationMode = Config.Bind("Custom", "Optimisation Mode", false, "Disables things that would be considered resource heavy");
        Regions = Config.Bind("Custom", "Regions", "{\"CurrentRegionIdx\":0,\"Regions\":[]}",
            "Create an array of Regions you want to add/update. To create this array, go to https://impostor.github.io/Impostor/ and put the Regions array from the server file in here");
        RegionsToRemove = Config.Bind("Custom", "Remove Regions", "", "Comma-seperated list of region names that should be removed");
    }

    public void LoadComponents()
    {
        var text = Path.Combine(DataPath, "steam_appid.txt");

        if (!File.Exists(text))
            File.WriteAllText(text, "945360");

        Harmony.PatchAll();
        AllMonos.RegisterMonos();
        AllMonos.AddComponents();
        SetUpConfigs();
        LoadAssets();
        DataManager.Player.Onboarding.ViewedHideAndSeekHowToPlay = true;
    }
}