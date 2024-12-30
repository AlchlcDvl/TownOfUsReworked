namespace TownOfUsReworked;

public partial class TownOfUsReworked
{
    private void SetUpConfigs()
    {
        LighterDarker = Config.Bind("Client", "Lighter Darker Colors", true, "Adds smaller descriptions of colors as lighter or darker for body report purposes");
        WhiteNameplates = Config.Bind("Client", "White Nameplates", false, "Enables custom nameplates");
        NoLevels = Config.Bind("Client", "No Levels", false, "Enables the little level icon during meetings");
        CustomCrewColors = Config.Bind("Client", "Custom Crew Colors", true, "Enables custom colors for Crew roles");
        CustomNeutColors = Config.Bind("Client", "Custom Neutral Colors", true, "Enables custom colors for Neutral roles");
        CustomIntColors = Config.Bind("Client", "Custom Intruder Colors", true, "Enables custom colors for Intruder roles");
        CustomSynColors = Config.Bind("Client", "Custom Syndicate Colors", true, "Enables custom colors for Syndicate roles");
        CustomModColors = Config.Bind("Client", "Custom Modifier Colors", true, "Enables custom colors for Modifiers");
        CustomDispColors = Config.Bind("Client", "Custom Disposition Colors", true, "Enables custom colors for Dispositions");
        CustomAbColors = Config.Bind("Client", "Custom Ability Colors", true, "Enables custom colors for Abilities");
        CustomEjects = Config.Bind("Client", "Custom Ejects", true, "Enables funny ejection messages compared to the monotone \"X was ejected\"");
        HideOtherGhosts = Config.Bind("Client", "Hide Other Ghosts", true, "Hides other ghosts when you are dead");
        OptimisationMode = Config.Bind("Client", "Optimisation Mode", false, "Disables things that would be considered resource heavy");
        LockCameraSway = Config.Bind("Client", "Lock Camera Sway", false, "Disables the camera bobbing around your character");
        ForceUseLocal = Config.Bind("Client", "Force Use Local Files", false, "Forces the loaders to pull from local json files rather than ones available online");
        UseDarkTheme = Config.Bind("Client", "Use Dark Theme Chat", false, "Enables dark mode for chat");
        NoWelcome = Config.Bind("Client", "No Welcome Message", false, "Disables the welcome message when joining a lobby for the first time in a session");

        Ip = Config.Bind("Config", "Custom Server IP", "127.0.0.1", "IP for the Custom Server");
        Port = Config.Bind("Config", "Custom Server Port", (ushort)22023, "Port for the Custom Server");

        RedirectLogger = Config.Bind("Debugging", "Redirect Logger", false, "Redirect base game Logger calls into BepInEx logging");
        AutoPlayAgain = Config.Bind("Debugging", "Auto Play Again", false, "Automatically calls Play Again after game ends");
        DisableTimeout = Config.Bind("Debugging", "Disable Timeout", false, "Disable the network disconnection timeout");
        Persistence = Config.Bind("Debugging", "Persistence", false, "Enables whether or not bots will respawn after each test");
        SameVote = Config.Bind("Debugging", "Same Vote", false, "Disables whether or not each vote votes for the same player you do");
    }

    public void LoadComponents()
    {
        Harmony.PatchAll();
        var text = Path.Combine(DataPath, "steam_appid.txt");

        if (!File.Exists(text))
            File.WriteAllText(text, "945360");

        NormalGameOptionsV08.MinPlayers = Enumerable.Repeat(1, 127).ToArray();
        AllMonos.RegisterMonos();
        SetUpConfigs();
        ReworkedStart = TranslationManager.GetNextName("Translation.ReworkedStart", isStartup: true);
        LoadAssets();
        IL2CPPChainloader.Instance.Finished += Initialise;
    }
}