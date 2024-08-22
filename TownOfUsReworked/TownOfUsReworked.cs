namespace TownOfUsReworked;

[BepInPlugin(Id, Name, VersionString)]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency("MalumMenu", BepInDependency.DependencyFlags.SoftDependency)]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
[BepInProcess("Among Us.exe")]
public partial class TownOfUsReworked : BasePlugin
{
    public const string Id = "me.alchlcdvl.reworked";
    public const string Name = "Reworked";
    public const string VersionString = "0.6.7.0";
    public static readonly Version Version = new(VersionString);

    public const bool IsDev = true;
    public const bool IsStream = true;
    public const int DevBuild = 6;

    public static bool IsTest { get; set; }
    private static readonly string VersionS = VersionString.Remove(VersionString.Length - 2);
    private static string DevString => IsDev ? $"-dev{DevBuild}" : "";
    private static string TestString => IsTest ? "_test" : "";
    private static string StreamString => IsStream ? "s" : "";
    public static string VersionFinal => $"v{VersionS}{DevString}{StreamString}{TestString}";

    public const string Resources = "TownOfUsReworked.Resources.";

    public static readonly string DataPath = Path.GetDirectoryName(Application.dataPath);
    public static readonly string Assets = Path.Combine(DataPath, "ReworkedAssets");
    public static readonly string Hats = Path.Combine(Assets, "CustomHats");
    public static readonly string Visors = Path.Combine(Assets, "CustomVisors");
    public static readonly string Nameplates = Path.Combine(Assets, "CustomNameplates");
    public static readonly string Colors = Path.Combine(Assets, "CustomColors");
    public static readonly string Options = Path.Combine(Assets, "CustomOptions");
    public static readonly string Images = Path.Combine(Assets, "CustomImages");
    public static readonly string Sounds = Path.Combine(Assets, "CustomSounds");
    public static readonly string Misc = Path.Combine(Assets, "MiscAssets");
    public static readonly string Portal = Path.Combine(Assets, "PortalAnim");
    public static readonly string Logs = Path.Combine(Assets, "ModLogs");
    public static readonly string Other = Path.Combine(Assets, "Other");
    public static readonly string ModsFolder = Path.Combine(DataPath, "BepInEx", "plugins");

    public const string DiscordInvite = "https://discord.gg/cd27aDQDY9";
    public const string GitHubLink = "https://github.com/AlchlcDvl/TownOfUsReworked";
    public const string AssetsLink = "https://github.com/AlchlcDvl/ReworkedAssets";

    public static readonly Assembly Core = typeof(TownOfUsReworked).Assembly;

    public static NormalGameOptionsV08 NormalOptions => GameOptionsManager.Instance.currentNormalGameOptions;
    public static HideNSeekGameOptionsV08 HNSOptions => GameOptionsManager.Instance.currentHideNSeekGameOptions;

    public static bool LobbyCapped { get; set; } = true;
    public static bool Persistence { get; set; } = true;
    public static bool SameVote { get; set; } = true;
    public static bool MCIActive { get; set; }

    public readonly Harmony Harmony = new(Id);

    public override string ToString() => $"{Id} {Name} {VersionFinal} {Version}";

    public static ConfigEntry<string> Ip { get; set; }
    public static ConfigEntry<ushort> Port { get; set; }
    public static ConfigEntry<bool> LighterDarker { get; set; }
    public static ConfigEntry<bool> WhiteNameplates { get; set; }
    public static ConfigEntry<bool> NoLevels { get; set; }
    public static ConfigEntry<bool> CustomCrewColors { get; set; }
    public static ConfigEntry<bool> CustomNeutColors { get; set; }
    public static ConfigEntry<bool> CustomIntColors { get; set; }
    public static ConfigEntry<bool> CustomSynColors { get; set; }
    public static ConfigEntry<bool> CustomModColors { get; set; }
    public static ConfigEntry<bool> CustomObjColors { get; set; }
    public static ConfigEntry<bool> CustomAbColors { get; set; }
    public static ConfigEntry<bool> CustomEjects { get; set; }
    public static ConfigEntry<bool> OptimisationMode { get; set; }
    public static ConfigEntry<bool> HideOtherGhosts { get; set; }

    public static TownOfUsReworked ModInstance { get; private set; }

    public TownOfUsReworked() : base() => Logging.Log = Log;

    public override void Load()
    {
        LogMessage("Loading");

        if (InitialiseMalumMenu())
            return;

        if (CheckAbort(out var mod))
        {
            LogFatal($"Unsupported mod {mod} detected, aborting mod loading");
            return;
        }

        try
        {
            ModInstance = this;
            LoadComponents();
            LogMessage($"Mod Loaded - {this}");
        }
        catch (Exception e)
        {
            LogFatal($"Couldn't load the mod because:\n{e}");
        }
    }

    public override bool Unload()
    {
        ModInstance = null;
        Harmony.UnpatchSelf();
        LogMessage($"Mod Unloaded - {this}");
        return base.Unload();
    }
}