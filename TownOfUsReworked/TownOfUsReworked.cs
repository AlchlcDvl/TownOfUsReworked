// ReSharper disable HeuristicUnreachableCode
#pragma warning disable CS0162 // Unreachable code detected

using BepInEx.Logging;

namespace TownOfUsReworked;

// TODO: Add commenting and documentation for the codebase - IPR
// TODO: Re-add version handling
// TODO: Finish adding missing translation keys before the next release - IPR
[BepInAutoPlugin("me.alchlcdvl.reworked", "Reworked")]
[BepInDependency(ReactorPlugin.Id)]
[ReactorModFlags(ModFlags.RequireOnAllClients | ModFlags.DisableServerAuthority)]
[BepInProcess("Among Us.exe")]
#if PC
[BepInIncompatibility("MalumMenu")] // CHEATERS BEGONE! *epic thunderclap is heard behind me*
# endif
public sealed partial class TownOfUsReworked : BasePlugin
{
    // A bit of forewarning to whoever dares to look through my horrid codebase, majority of the comments and documentation have been written months,
    // maybe years after the code was initially written, so my intentions might not be clear enough

    public const string DiscordInvite = "https://discord.gg/cd27aDQDY9";
    public const string GitHubLink = "https://github.com/AlchlcDvl/TownOfUsReworked";
    public const string AssetsLink = "https://github.com/AlchlcDvl/ReworkedAssets";

    public const bool IsDev = true;
    public const bool IsStream = true;
    private const int DevBuild = 67;

    private static readonly string DataPath = Path.GetDirectoryName(Application.dataPath);
    private static readonly string PersistentDataPath = Path.GetDirectoryName(Application.persistentDataPath);
    public static readonly string Assets = Path.Combine(PersistentDataPath, "Among Us", "ReworkedAssets");
    public static readonly string Hats = Path.Combine(Assets, "Hats");
    // public static readonly string Skins = Path.Combine(Assets, "Skins");
    public static readonly string Visors = Path.Combine(Assets, "Visors");
    public static readonly string Nameplates = Path.Combine(Assets, "Nameplates");
    public static readonly string Colors = Path.Combine(Assets, "Colors");
    public static readonly string Options = Path.Combine(Assets, "Options");
    public static readonly string Images = Path.Combine(Assets, "Images");
    public static readonly string Sounds = Path.Combine(Assets, "Sounds");
    public static readonly string Bundles = Path.Combine(Assets, "Bundles");
    public static readonly string Logs = Path.Combine(Assets, "Logs");
    public static readonly string Other = Path.Combine(Assets, "Other");
    public static readonly string Hashes = Path.Combine(Assets, "Hashes");
    public static readonly string ModsFolder = Path.Combine(DataPath, "BepInEx", "plugins");

    public static readonly Assembly Core = typeof(TownOfUsReworked).Assembly;

    private static readonly string VersionSignature = Version.Contains('+') ? Version[(Version.IndexOf('+') + 1)..] : "";
    public static readonly string VersionS = Version.Contains('+') ? Version[..Version.IndexOf('+')] : Version;
    private static readonly string DevString = IsDev ? ("-dev" + DevBuild) : "";
    private const string StreamString = IsStream ? "s" : "";
    public static readonly string VersionFinal = $"v{VersionS}{DevString}{StreamString}";
    private static string VersionFull => $"{VersionFinal}+{VersionSignature}+{ModHash}";

    public static readonly Version ModVer = new(VersionS);

    public static NormalGameOptionsV09 NormalOptions => GameOptionsManager.Instance.currentNormalGameOptions;
    public static HideNSeekGameOptionsV09 HnsOptions => GameOptionsManager.Instance.currentHideNSeekGameOptions;
    // public static IGameOptions CurrentOptions => GameOptionsManager.Instance.CurrentGameOptions;

    public static bool MciActive => MciUtils.Clients.Count > 0;
    public static bool DebugMode => IsDev || ClientOptions.DebugModeOn;

    // A bunch of config stuff to ensure value persistence
    public static ConfigEntry<string> Ip;
    public static ConfigEntry<ushort> Port;

    public static ConfigEntry<bool> BlockBaseGameLogger;
    public static ConfigEntry<bool> RedirectLogger;
    public static ConfigEntry<bool> LogFromUnity;
    public static ConfigEntry<bool> Persistence;
    public static ConfigEntry<bool> SameVote;

    public static TownOfUsReworked ModInstance;

    public readonly Harmony HarmonyInstance = new(Id);

    private static string ModHash;

    public override void Load()
    {
        LogManager.Log = Log;
        DiskLog = BepInEx.Logging.Logger.Listeners.OfType<DiskLogListener>().FirstOrDefault();
        Message("Loading");

        if (CheckAbort(out var mod))
        {
            Critical($"Unsupported mod {mod} detected, aborting mod loading");
            return;
        }

        try
        {
            ModInstance = this;
            LoadComponents();
            Success($"Mod Loaded - {this}");
        }
        catch (Exception e)
        {
            Fatal($"Couldn't load the mod because:\n{e}");
            Unload();
        }
    }

    public override bool Unload()
    {
        ModInstance = null;
        HarmonyInstance.UnpatchSelf();
        Fatal($"Mod Unloaded - {this}");
        return true;
    }

    public override string ToString() => $"{Id} {Name} {VersionFull}";

    private void SetUpConfigs()
    {
        ClientOptions.LighterDarker.Config = Config.Bind("Client", "Lighter Darker Colors", true, "Adds smaller descriptions of colors as lighter or darker for body report purposes");
        ClientOptions.WhiteNameplates.Config = Config.Bind("Client", "White Nameplates", false, "Enables custom nameplates");
        ClientOptions.NoLevels.Config = Config.Bind("Client", "No Levels", false, "Enables the little level icon during meetings");
        ClientOptions.CustomCrewColors.Config = Config.Bind("Client", "Custom Crew Colors", true, "Enables custom colors for Crew roles");
        ClientOptions.CustomNeutColors.Config = Config.Bind("Client", "Custom Outcast Colors", true, "Enables custom colors for Outcast roles");
        ClientOptions.CustomIntColors.Config = Config.Bind("Client", "Custom Intruder Colors", true, "Enables custom colors for Intruder roles");
        ClientOptions.CustomSynColors.Config = Config.Bind("Client", "Custom Syndicate Colors", true, "Enables custom colors for Syndicate roles");
        ClientOptions.CustomApocColors.Config = Config.Bind("Client", "Custom Apocalypse Colors", true, "Enables custom colors for Apocalypse roles");
        ClientOptions.CustomGmColors.Config = Config.Bind("Client", "Custom Game Mode Colors", true, "Enables custom colors for Game Mode roles");
        ClientOptions.CustomModColors.Config = Config.Bind("Client", "Custom Modifier Colors", true, "Enables custom colors for Modifiers");
        ClientOptions.CustomDispColors.Config = Config.Bind("Client", "Custom Disposition Colors", true, "Enables custom colors for Dispositions");
        ClientOptions.CustomAbColors.Config = Config.Bind("Client", "Custom Ability Colors", true, "Enables custom colors for Abilities");
        ClientOptions.CustomEjects.Config = Config.Bind("Client", "Custom Ejects", true, "Enables funny ejection messages compared to the monotone \"X was ejected\"");
        ClientOptions.HideOtherGhosts.Config = Config.Bind("Client", "Hide Other Ghosts", true, "Hides other ghosts when you are dead");
        ClientOptions.OptimisationMode.Config = Config.Bind("Client", "Optimisation Mode", false, "Disables things that would be considered resource heavy");
        ClientOptions.LockCameraSway.Config = Config.Bind("Client", "Lock Camera Sway", false, "Disables the camera bobbing around your character");
        ClientOptions.ForceUseLocal.Config = Config.Bind("Client", "Force Use Local Files", false, "Forces the loaders to pull from local json files rather than ones available online");
        ClientOptions.UseDarkTheme.Config = Config.Bind("Client", "Use Dark Theme Chat", false, "Enables dark mode for chat");
        ClientOptions.NoWelcome.Config = Config.Bind("Client", "No Welcome Message", false, "Disables the welcome message when joining a lobby for the first time in a session");
        ClientOptions.AutoPlayAgain.Config = Config.Bind("Client", "Auto Play Again", false, "Automatically calls Play Again after game ends");
        ClientOptions.DebugModeOn.Config = Config.Bind("Client", "Debug Mode", false, "Enabled Debug Mode to replicate dev mode status and provide info in the logs");

        Ip = Config.Bind("Config", "Custom Server IP", "127.0.0.1", "IP for the Custom Server");
        Port = Config.Bind("Config", "Custom Server Port", (ushort)22023, "Port for the Custom Server");

        BlockBaseGameLogger = Config.Bind("Debugging", "Block Logger", false, "Block base game Logger calls from appearing in the console");
        RedirectLogger = Config.Bind("Debugging", "Redirect Logger", false, "Redirect base game Logger calls into BepInEx logging");
        LogFromUnity = Config.Bind("Debugging", "Show Unity Logs", false, "Redirect Unity Logger calls into BepInEx logging");
        Persistence = Config.Bind("Debugging", "Persistence", false, "Enables whether or not bots will respawn after each test");
        SameVote = Config.Bind("Debugging", "Same Vote", false, "Disables whether or not each vote votes for the same player you do");
    }

    private void LoadComponents()
    {
        var text = Path.Combine(DataPath, "steam_appid.txt");

        if (!File.Exists(text))
            File.WriteAllText(text, "945360");

        using var hasher = MD5.Create();
        ModHash = BitConverter.ToString(hasher.ComputeHash(File.ReadAllBytes(Path.Combine(ModsFolder, "Reworked.dll")))).Replace("-", "").ToLowerInvariant();

        SetUpConfigs();
        HarmonyInstance.PatchAll();
        AddressablesPatch.Initialize();
        Application.add_logMessageReceived((Application.LogCallback)RedirectLoggerPatch2.UnityLog);
        IL2CPPChainloader.Instance.Finished += Initialise;
    }
}