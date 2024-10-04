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
        CustomDispColors = Config.Bind("Custom", "Custom Disposition Colors", true, "Enables custom colors for Dispositions");
        CustomAbColors = Config.Bind("Custom", "Custom Ability Colors", true, "Enables custom colors for Abilities");
        CustomEjects = Config.Bind("Custom", "Custom Ejects", true, "Enables funny ejection messages compared to the monotone \"X was ejected\"");
        HideOtherGhosts = Config.Bind("Custom", "Hide Other Ghosts", true, "Hides other ghosts when you are dead");
        OptimisationMode = Config.Bind("Custom", "Optimisation Mode", false, "Disables things that would be considered resource heavy");

        RedirectLogger = Config.Bind("Debugging", "Redirect Logger", false, "Redirect base game Logger calls into BepInEx logging");
        AutoPlayAgain = Config.Bind("Debugging", "Auto Play Again", false, "Automatically calls Play Again after game ends");
        DisableTimeout = Config.Bind("Debugging", "Disable Timeout", false, "Disable the network disconnection timeout");
        LobbyCapped = Config.Bind("Debugging", "Lobby Capped", false, "Caps the bot count to the lobby size");
        Persistence = Config.Bind("Debugging", "Persistence", false, "Enables whether or not bots will respawn after each test");
        SameVote = Config.Bind("Debugging", "Same Vote", false, "Disables whether or not each vote votes for the same player you do");
    }

    public void LoadComponents()
    {
        ReactorCredits.Register("TownOfUsReworked", VersionFinal, IsDev || IsStream, location => location == ReactorCredits.Location.MainMenu || LobbyBehaviour.Instance);
        Harmony.PatchAll();
        var text = Path.Combine(DataPath, "steam_appid.txt");

        if (!File.Exists(text))
            File.WriteAllText(text, "945360");

        NormalGameOptionsV08.MinPlayers = Enumerable.Repeat(1, 127).ToArray();
        AllMonos.RegisterMonos();
        SetUpConfigs();
        LoadAssets();
    }
}