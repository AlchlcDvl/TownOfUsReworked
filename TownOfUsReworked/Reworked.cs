namespace TownOfUsReworked;

public static class Reworked
{
    public static void SetUpConfigs()
    {
        TownOfUsReworked.Ip = TownOfUsReworked.ModInstance.Config.Bind("Custom", "Custom Server IP", "127.0.0.1", "IP for the Custom Server");
        TownOfUsReworked.Port = TownOfUsReworked.ModInstance.Config.Bind("Custom", "Custom Server Port", (ushort)22023, "Port for the Custom Server");
        TownOfUsReworked.LighterDarker = TownOfUsReworked.ModInstance.Config.Bind("Custom", "Lighter Darker Colors", true, "Adds smaller descriptions of colors as lighter or darker for body"
            + " report purposes");
        TownOfUsReworked.WhiteNameplates = TownOfUsReworked.ModInstance.Config.Bind("Custom", "White Nameplates", false, "Enables custom nameplates");
        TownOfUsReworked.NoLevels = TownOfUsReworked.ModInstance.Config.Bind("Custom", "No Levels", false, "Enables the little level icon during meetings");
        TownOfUsReworked.CustomCrewColors = TownOfUsReworked.ModInstance.Config.Bind("Custom", "Custom Crew Colors", true, "Enables custom colors for Crew roles");
        TownOfUsReworked.CustomNeutColors = TownOfUsReworked.ModInstance.Config.Bind("Custom", "Custom Neutral Colors", true, "Enables custom colors for Neutral roles");
        TownOfUsReworked.CustomIntColors = TownOfUsReworked.ModInstance.Config.Bind("Custom", "Custom Intruder Colors", true, "Enables custom colors for Intruder roles");
        TownOfUsReworked.CustomSynColors = TownOfUsReworked.ModInstance.Config.Bind("Custom", "Custom Syndicate Colors", true, "Enables custom colors for Syndicate roles");
        TownOfUsReworked.CustomModColors = TownOfUsReworked.ModInstance.Config.Bind("Custom", "Custom Modifier Colors", true, "Enables custom colors for Modifiers");
        TownOfUsReworked.CustomObjColors = TownOfUsReworked.ModInstance.Config.Bind("Custom", "Custom Objectifier Colors", true, "Enables custom colors for Objectifiers");
        TownOfUsReworked.CustomAbColors = TownOfUsReworked.ModInstance.Config.Bind("Custom", "Custom Ability Colors", true, "Enables custom colors for Abilities");
        TownOfUsReworked.CustomEjects = TownOfUsReworked.ModInstance.Config.Bind("Custom", "Custom Ejects", true, "Enables funny ejection messages compared to the monotone \"X was ejected" +
            "\"");
        TownOfUsReworked.Regions = TownOfUsReworked.ModInstance.Config.Bind("Custom", "Regions", "{\"CurrentRegionIdx\":0,\"Regions\":[]}",
            "Create an array of Regions you want to add/update. To create this array, go to https://impostor.github.io/Impostor/ and put the Regions array from the server file in here");
        TownOfUsReworked.RegionsToRemove = TownOfUsReworked.ModInstance.Config.Bind("Custom", "Remove Regions", "", "Comma-seperated list of region names that should be removed");
    }

    public static void LoadComponents()
    {
        TownOfUsReworked.ModInstance.Harmony.PatchAll();
        SetUpConfigs();
        AllMonos.RegisterMonos();
        CustomColors.LoadColors();
        AssetLoader.LoadAssets();
        ExtraRegions.UpdateRegions();
        Info.SetAllInfo();
        AllMonos.AddComponents();
        DataManager.Player.Onboarding.ViewedHideAndSeekHowToPlay = true;
    }
}