namespace TownOfUsReworked;

[BepInPlugin(Id, Name, VersionString)]
[BepInDependency(ReactorPlugin.Id)]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
[BepInProcess("Among Us.exe")]
public class TownOfUsReworked : BasePlugin
{
    public const string Id = "me.alchlcdvl.reworked";
    public const string Name = "TownOfUsReworked";
    public const string VersionString = "0.5.4.0";
    public static readonly Version Version = new(VersionString);

    public const bool IsDev = false;
    public static bool IsTest { get; set; }
    private static string VersionS => VersionString.Remove(VersionString.Length - 2);
    private static string DevString => IsDev ? "-dev" : "";
    private static string TestString => IsTest ? "_test" : "";
    public static string VersionFinal => $"v{VersionS}{DevString}{TestString}";

    public const string Resources = "TownOfUsReworked.Resources.";
    public const string Buttons = $"{Resources}Buttons.";
    public const string Misc = $"{Resources}Misc.";
    public const string Portal = $"{Resources}Portal.";
    public const string Presets = $"{Resources}Presets.";
    public const string Sounds = $"{Resources}Sounds.";

    public static string DataPath => $"{Path.GetDirectoryName(Application.dataPath)}\\";
    public static string Hats => $"{DataPath}CustomHats\\";
    public static string Visors => $"{DataPath}CustomVisors\\";
    public static string Nameplates => $"{DataPath}CustomNameplates\\";
    public static string ModsFolder => $"{DataPath}\\BepInEx\\plugins\\";

    public const string DiscordInvite = "https://discord.gg/cd27aDQDY9";
    public const string GitHubLink = "https://github.com/AlchlcDvl/TownOfUsReworked";
    public const string AssetsLink = "https://github.com/AlchlcDvl/ReworkedAssets";

    public static Assembly Core => typeof(TownOfUsReworked).Assembly;

    public static NormalGameOptionsV07 NormalOptions => GameOptionsManager.Instance.currentNormalGameOptions;
    public static HideNSeekGameOptionsV07 HNSOptions => GameOptionsManager.Instance.currentHideNSeekGameOptions;

    public static bool LobbyCapped { get; set; } = true;
    public static bool Persistence { get; set; } = true;
    public static bool SameVote { get; set; } = true;
    public static bool MCIActive { get; set; }

    public readonly Harmony Harmony = new(Id);

    public override string ToString() => $"{Id} {Name} {VersionFinal} {Version}";

    public static ConfigEntry<string> Ip { get; set; }
    public static ConfigEntry<ushort> Port { get; set; }
    public static ConfigEntry<string> Regions { get; set; }
    public static ConfigEntry<string> RegionsToRemove { get; set; }
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

    public static TownOfUsReworked ModInstance { get; private set; }

    public override void Load()
    {
        Logging.Init();
        LogMessage("Loading");
        CheckAbort(out var abort, out var mod);

        if (abort)
        {
            LogFatal($"Unsupported mod {mod} detected, aborting mod loading");
            return;
        }

        try
        {
            Harmony.PatchAll();

            ModInstance = this;

            DataManager.Player.Onboarding.ViewedHideAndSeekHowToPlay = true;

            NormalGameOptionsV07.MaxImpostors = Enumerable.Repeat(127, 127).ToArray();
            NormalGameOptionsV07.MinPlayers = Enumerable.Repeat(1, 127).ToArray();

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
            Regions = Config.Bind("Custom", "Regions", "{\"CurrentRegionIdx\":0,\"Regions\":[]}",
                "Create an array of Regions you want to add/update. To create this array, go to https://impostor.github.io/Impostor/ and put the Regions array from the server file in here");
            RegionsToRemove = Config.Bind("Custom", "Remove Regions", "", "Comma-seperated list of region names that should be removed");

            AllMonos.RegisterMonos();
            CustomColors.LoadColors();
            AssetLoader.LoadAssets();
            ExtraRegions.UpdateRegions();
            Info.SetAllInfo();
            AllMonos.AddComponents();

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
        return base.Unload();
    }
}