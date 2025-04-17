// ReSharper disable HeuristicUnreachableCode
#pragma warning disable CS0162 // Unreachable code detected

using BepInEx.Logging;

namespace TownOfUsReworked;

// FIXME: Can't call meetings (something I'm patching is rejecting it???)
// TODO: Add commenting and documentation for the codebase
// TODO: Refactor code for handling appearances, sizes and speed
// TODO: Re-add version handling
// TODO: Finish adding missing translation keys before the next release
// TODO: Change how Ret works by using its substituted roles rather than copy pasted code
[BepInAutoPlugin("me.alchlcdvl.reworked", "Reworked")]
[BepInDependency(ReactorPlugin.Id)]
[BepInIncompatibility("MalumMenu")]
[ReactorModFlags(ModFlags.RequireOnAllClients | ModFlags.DisableServerAuthority)]
[BepInProcess("Among Us.exe")]
public sealed partial class TownOfUsReworked : BasePlugin
{
    // A bit of forewarning to whoever dares to look through my horrid codebase, majority of the comments and documentation have been written months,
    // maybe years after the code was initially written, so my intentions might not be clear enough

    public const string DiscordInvite = "https://discord.gg/cd27aDQDY9";
    public const string GitHubLink = "https://github.com/AlchlcDvl/TownOfUsReworked";
    public const string AssetsLink = "https://github.com/AlchlcDvl/ReworkedAssets";

    public const bool IsDev = true;
    public const bool IsStream = true;
    private const int DevBuild = 44;

    public const string Resources = "TownOfUsReworked.Resources.";

    public static readonly Version ModVer = new(VersionS);

    private static readonly string DataPath = Path.GetDirectoryName(Application.dataPath);
    public static readonly string Assets = Path.Combine(DataPath, "ReworkedAssets");
    public static readonly string Hats = Path.Combine(Assets, "Hats");
    public static readonly string Visors = Path.Combine(Assets, "Visors");
    public static readonly string Nameplates = Path.Combine(Assets, "Nameplates");
    public static readonly string Colors = Path.Combine(Assets, "Colors");
    public static readonly string Options = Path.Combine(DataPath, "ReworkedOptions");
    public static readonly string Images = Path.Combine(Assets, "Images");
    public static readonly string Sounds = Path.Combine(Assets, "Sounds");
    public static readonly string Bundles = Path.Combine(Assets, "Bundles");
    public static readonly string Portal = Path.Combine(Assets, "PortalAnim");
    public static readonly string Logs = Path.Combine(Assets, "Logs");
    public static readonly string Other = Path.Combine(Assets, "Other");
    public static readonly string Hashes = Path.Combine(Assets, "Hashes");
    public static readonly string ModsFolder = Path.Combine(DataPath, "BepInEx", "plugins");

    public static readonly Assembly Core = typeof(TownOfUsReworked).Assembly;

    private static string VersionSignature => Version.Contains('+') ? Version[(Version.IndexOf('+') + 1)..] : "";
    private static string VersionS => Version.Contains('+') ? Version[..Version.IndexOf('+')] : Version;
    private static string DevString => IsDev ? $"-dev{DevBuild}" : "";
    private static string StreamString => IsStream ? "s" : "";
    public static string VersionFinal => $"v{VersionS}{DevString}{StreamString}";
    private static string VersionFull => $"v{VersionFinal}+{VersionSignature}";

    public static NormalGameOptionsV08 NormalOptions => GameOptionsManager.Instance.currentNormalGameOptions;
    public static HideNSeekGameOptionsV08 HnsOptions => GameOptionsManager.Instance.currentHideNSeekGameOptions;

    public static bool MciActive => MciUtils.Clients.Count > 0;
    public static bool DebugMode => IsDev || DebugModeOn.Value;

    // A bunch of config stuff to ensure value persistence
    public static ConfigEntry<string> Ip { get; private set; }
    public static ConfigEntry<ushort> Port { get; private set; }

    public static ConfigEntry<bool> LighterDarker { get; private set; }
    public static ConfigEntry<bool> WhiteNameplates { get; private set; }
    public static ConfigEntry<bool> NoLevels { get; private set; }
    public static ConfigEntry<bool> CustomCrewColors { get; private set; }
    public static ConfigEntry<bool> CustomNeutColors { get; private set; }
    public static ConfigEntry<bool> CustomApocColors { get; private set; }
    public static ConfigEntry<bool> CustomIntColors { get; private set; }
    public static ConfigEntry<bool> CustomSynColors { get; private set; }
    public static ConfigEntry<bool> CustomGmColors { get; private set; }
    public static ConfigEntry<bool> CustomModColors { get; private set; }
    public static ConfigEntry<bool> CustomDispColors { get; private set; }
    public static ConfigEntry<bool> CustomAbColors { get; private set; }
    public static ConfigEntry<bool> CustomEjects { get; private set; }
    public static ConfigEntry<bool> OptimisationMode { get; private set; }
    public static ConfigEntry<bool> HideOtherGhosts { get; private set; }
    public static ConfigEntry<bool> LockCameraSway { get; private set; }
    public static ConfigEntry<bool> ForceUseLocal { get; private set; }
    public static ConfigEntry<bool> UseDarkTheme { get; private set; }
    public static ConfigEntry<bool> NoWelcome { get; private set; }
    public static ConfigEntry<bool> AutoPlayAgain { get; private set; }
    public static ConfigEntry<bool> DebugModeOn { get; private set; }

    public static ConfigEntry<bool> BlockBaseGameLogger { get; private set; }
    public static ConfigEntry<bool> RedirectLogger { get; private set; }
    public static ConfigEntry<bool> LogFromUnity { get; private set; }
    public static ConfigEntry<bool> DisableTimeout { get; private set; }
    public static ConfigEntry<bool> Persistence { get; private set; }
    public static ConfigEntry<bool> SameVote { get; private set; }

    public static TownOfUsReworked ModInstance { get; private set; }

    public readonly Harmony Harmony = new(Id);

    public override void Load()
    {
        Logging.Log = Log;
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
        Harmony.UnpatchSelf();
        Fatal($"Mod Unloaded - {this}");
        return true;
    }

    public override string ToString() => $"{Id} {Name} {VersionFull}";

    private void SetUpConfigs()
    {
        LighterDarker = Config.Bind("Client", "Lighter Darker Colors", true, "Adds smaller descriptions of colors as lighter or darker for body report purposes");
        WhiteNameplates = Config.Bind("Client", "White Nameplates", false, "Enables custom nameplates");
        NoLevels = Config.Bind("Client", "No Levels", false, "Enables the little level icon during meetings");
        CustomCrewColors = Config.Bind("Client", "Custom Crew Colors", true, "Enables custom colors for Crew roles");
        CustomNeutColors = Config.Bind("Client", "Custom Neutral Colors", true, "Enables custom colors for Neutral roles");
        CustomIntColors = Config.Bind("Client", "Custom Intruder Colors", true, "Enables custom colors for Intruder roles");
        CustomSynColors = Config.Bind("Client", "Custom Syndicate Colors", true, "Enables custom colors for Syndicate roles");
        CustomApocColors = Config.Bind("Client", "Custom Apocalypse Colors", true, "Enables custom colors for Apocalypse roles");
        CustomGmColors = Config.Bind("Client", "Custom Game Mode Colors", true, "Enables custom colors for Game Mode roles");
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
        AutoPlayAgain = Config.Bind("Client", "Auto Play Again", false, "Automatically calls Play Again after game ends");
        DebugModeOn = Config.Bind("Client", "Debug Mode", false, "Enabled Debug Mode to replicate dev mode status and provide info in the logs");

        Ip = Config.Bind("Config", "Custom Server IP", "127.0.0.1", "IP for the Custom Server");
        Port = Config.Bind("Config", "Custom Server Port", (ushort)22023, "Port for the Custom Server");

        BlockBaseGameLogger = Config.Bind("Debugging", "Block Logger", false, "Block base game Logger calls from appearing in the console");
        RedirectLogger = Config.Bind("Debugging", "Redirect Logger", false, "Redirect base game Logger calls into BepInEx logging");
        LogFromUnity = Config.Bind("Debugging", "Show Unity Logs", false, "Redirect Unity Logger calls into BepInEx logging");
        DisableTimeout = Config.Bind("Debugging", "Disable Timeout", false, "Disable the network disconnection timeout");
        Persistence = Config.Bind("Debugging", "Persistence", false, "Enables whether or not bots will respawn after each test");
        SameVote = Config.Bind("Debugging", "Same Vote", false, "Disables whether or not each vote votes for the same player you do");
    }

    private void LoadComponents()
    {
        var text = Path.Combine(DataPath, "steam_appid.txt");

        if (!File.Exists(text))
            File.WriteAllText(text, "945360");

        SetUpConfigs();
        Harmony.PatchAll();
        AddressablesPatch.Initialize();
        Application.add_logMessageReceived((Action<string, string, LogType>)RedirectLoggerPatch2.UnityLog);
        IL2CPPChainloader.Instance.Finished += Initialise;
    }
}