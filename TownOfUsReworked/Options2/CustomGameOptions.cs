namespace TownOfUsReworked.Options2;

// DO NOT OVERRIDE VALUES OF ANY OF THESE PROPERTIES ANY WHERE IN THE CODE OR ELSE THE OPTIONS WILL START TO FUCK OFF
public static class CustomGameOptions2
{
    // Global Options
    [HeaderOption(MultiMenu2.Main, [ "PlayerSpeed", "GhostSpeed", "InteractionDistance", "EmergencyButtonCount", "TaskBar", "VotingTime", "DiscussionTime", "LobbySize", "ReportDistance",
        "EmergencyButtonCooldown", "ConfirmEjects", "EjectionRevealsRoles", "EnableInitialCds", "InitialCooldowns", "EnableMeetingCds", "MeetingCooldowns", "EnableFailCds", "ChatCooldown",
        "FailCooldowns", "ChatCharacterLimit" ])]
    public static bool GameSettings { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 0.25f, 10, 0.25f, Format.Multiplier)]
    public static float PlayerSpeed { get; set; } = 1.25f;

    [NumberOption(MultiMenu2.Main, 0.25f, 10, 0.25f, Format.Multiplier)]
    public static float GhostSpeed { get; set; } = 3;

    [NumberOption(MultiMenu2.Main, 0.5f, 5, 0.5f, Format.Distance)]
    public static float InteractionDistance { get; set; } = 2;

    [NumberOption(MultiMenu2.Main, 0, 100, 1)]
    public static int EmergencyButtonCount { get; set; } = 1;

    [NumberOption(MultiMenu2.Main, 0, 300, 2.5f, Format.Time)]
    public static float EmergencyButtonCooldown { get; set; } = 25;

    [NumberOption(MultiMenu2.Main, 0, 300, 5, Format.Time)]
    public static float DiscussionTime { get; set; } = 30;

    [NumberOption(MultiMenu2.Main, 5, 600, 15, Format.Time)]
    public static float VotingTime { get; set; } = 60;

    [StringOption(MultiMenu2.Main)]
    private static TBMode TaskBar { get; set; } = TBMode.MeetingOnly;
    public static TBMode TaskBarMode => GameMode switch
    {
        GameMode.TaskRace or GameMode.HideAndSeek => TBMode.Normal,
        _ => TaskBar
    };

    [ToggleOption(MultiMenu2.Main)]
    public static bool ConfirmEjects { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EjectionRevealsRoles { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableInitialCds { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 0, 30, 2.5f, Format.Time)]
    public static float InitialCooldowns { get; set; } = 10;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableMeetingCds { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 0, 30, 2.5f, Format.Time)]
    public static float MeetingCooldowns { get; set; } = 15;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableFailCds { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 0, 30, 2.5f, Format.Time)]
    public static float FailCooldowns { get; set; } = 5;

    [NumberOption(MultiMenu2.Main, 1, 20, 0.25f, Format.Distance)]
    public static float ReportDistance { get; set; } = 3.5f;

    [NumberOption(MultiMenu2.Main, 0, 3, 0.1f, Format.Time)]
    public static float ChatCooldown { get; set; } = 3;

    [NumberOption(MultiMenu2.Main, 50, 2000, 50)]
    public static int ChatCharacterLimit { get; set; } = 200;

    [NumberOption(MultiMenu2.Main, 2, 127, 1)]
    public static int LobbySize { get; set; } = 15;

    // Game Modes
    [HeaderOption(MultiMenu2.Main, [ "GameMode", "IgnoreAlignmentCaps", "IgnoreFactionCaps", "IgnoreLayerCaps", "NeutralsCount", "AddArsonist", "AddCryomaniac", "TRCommonTasks",
        "AddPlaguebearer", "HnSMode", "HnSShortTasks", "HnSCommonTasks", "HnSLongTasks", "HunterCount", "HuntCd", "StartTime", "HunterVent", "HunterVision", "HuntedChat", "HuntedVision",
        "HunterSpeedModifier", "HunterFlashlight", "HuntedFlashlight", "TRShortTasks" ])]
    public static bool GameModeSettings { get; set; } = true;

    [StringOption(MultiMenu2.Main, [ "None" ])]
    public static GameMode GameMode { get; set; } = GameMode.Classic;

    [ToggleOption(MultiMenu2.Main)]
    public static bool IgnoreFactionCaps { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool IgnoreAlignmentCaps { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool IgnoreLayerCaps { get; set; } = false;

    [NumberOption(MultiMenu2.Main, 0, 13, 1)]
    public static int NeutralsCount { get; set; } = 1;

    [ToggleOption(MultiMenu2.Main)]
    public static bool AddArsonist { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool AddCryomaniac { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool AddPlaguebearer { get; set; } = false;

    [StringOption(MultiMenu2.Main)]
    public static HnSMode HnSMode { get; set; } = HnSMode.Classic;

    [NumberOption(MultiMenu2.Main, 0, 13, 1)]
    public static int HnSShortTasks { get; set; } = 4;

    [NumberOption(MultiMenu2.Main, 0, 13, 1)]
    public static int HnSCommonTasks { get; set; } = 4;

    [NumberOption(MultiMenu2.Main, 0, 13, 1)]
    public static int HnSLongTasks { get; set; } = 4;

    [NumberOption(MultiMenu2.Main, 1, 13, 1)]
    public static int HunterCount { get; set; } = 1;

    [NumberOption(MultiMenu2.Main, 5f, 60f, 5f, Format.Time)]
    public static float HuntCd { get; set; } = 10;

    [NumberOption(MultiMenu2.Main, 5f, 60f, 5f, Format.Time)]
    public static float StartTime { get; set; } = 10;

    [ToggleOption(MultiMenu2.Main)]
    public static bool HunterVent { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 0.1f, 1f, 0.05f, Format.Multiplier)]
    public static float HunterVision { get; set;} = 0.25f;

    [NumberOption(MultiMenu2.Main, 1f, 2f, 0.05f, Format.Multiplier)]
    public static float HuntedVision { get; set;} = 1.5f;

    [NumberOption(MultiMenu2.Main, 1f, 1.5f, 0.05f, Format.Multiplier)]
    public static float HunterSpeedModifier { get; set; } = 1.25f;

    [ToggleOption(MultiMenu2.Main)]
    public static bool HunterFlashlight { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool HuntedFlashlight { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool HuntedChat { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 0, 13, 1)]
    public static int TRShortTasks { get; set; } = 4;

    [NumberOption(MultiMenu2.Main, 0, 13, 1)]
    public static int TRCommonTasks { get; set; } = 4;

    // Game Modifiers
    [HeaderOption(MultiMenu2.Main, [ "WhoCanVent", "AnonymousVoting", "NoSkipping", "FirstKillShield", "WhoSeesFirstKillShield", "FactionSeeRoles", "VisualTasks", "HideVentAnims",
        "PlayerNames", "Whispers", "WhispersAnnouncement", "AppearanceAnimation", "EnableAbilities", "EnableModifiers", "EnableObjectifiers", "VentTargeting", "RandomSpawns",
        "CooldownInVent", "DeadSeeEverything", "ParallelMedScans", "JaniCanMutuallyExclusive", "IndicateReportedBodies" ])]
    public static bool GameModifiers { get; set; } = true;

    [StringOption(MultiMenu2.Main)]
    public static WhoCanVentOptions WhoCanVent { get; set; } = WhoCanVentOptions.Default;

    [StringOption(MultiMenu2.Main)]
    public static AnonVotes AnonymousVoting { get; set; } = AnonVotes.Enabled;

    [StringOption(MultiMenu2.Main)]
    public static DisableSkipButtonMeetings NoSkipping { get; set; } = DisableSkipButtonMeetings.No;

    [ToggleOption(MultiMenu2.Main)]
    public static bool FirstKillShield { get; set; } = false;

    [StringOption(MultiMenu2.Main)]
    public static WhoCanSeeFirstKillShield WhoSeesFirstKillShield { get; set; } = WhoCanSeeFirstKillShield.Everyone;

    [ToggleOption(MultiMenu2.Main)]
    public static bool FactionSeeRoles { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool VisualTasks { get; set; } = false;

    [StringOption(MultiMenu2.Main)]
    public static PlayerNames PlayerNames { get; set; } = PlayerNames.Obstructed;

    [ToggleOption(MultiMenu2.Main)]
    public static bool Whispers { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool WhispersAnnouncement { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool AppearanceAnimation { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableAbilities { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableModifiers { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableObjectifiers { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool VentTargeting { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool CooldownInVent { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool DeadSeeEverything { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool ParallelMedScans { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool HideVentAnims { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool JaniCanMutuallyExclusive { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool IndicateReportedBodies { get; set; } = false;

    [StringOption(MultiMenu2.Main)]
    public static RandomSpawning RandomSpawns { get; set; } = RandomSpawning.Disabled;

    // Game Announcements
    [HeaderOption(MultiMenu2.Main, [ "GameAnnouncements", "LocationReports", "RoleFactionReports", "KillerReports" ])]
    public static bool GameAnnouncementSettings { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool GameAnnouncements { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool LocationReports { get; set; } = false;

    [StringOption(MultiMenu2.Main)]
    public static RoleFactionReports RoleFactionReports { get; set; } = RoleFactionReports.Neither;

    [StringOption(MultiMenu2.Main)]
    public static RoleFactionReports KillerReports { get; set; } = RoleFactionReports.Neither;

    // Map Settings
    public static MapEnum Map { get; set; }

    [HeaderOption(MultiMenu2.Main, [ "RandomMapSkeld", "RandomMapMira", "RandomMapPolus", "RandomMapdlekS", "RandomMapAirship", "RandomMapFungle", "RandomMapSubmerged",
        "RandomMapLevelImpostor", "AutoAdjustSettings", "SmallMapHalfVision", "SmallMapDecreasedCooldown", "LargeMapIncreasedCooldown", "SmallMapIncreasedShortTasks",
        "SmallMapIncreasedLongTasks", "LargeMapDecreasedShortTasks", "LargeMapDecreasedLongTasks"])]
    public static bool MapSettings { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 0, 100, 10, Format.Percent)]
    public static int RandomMapSkeld { get; set; } = 10;

    [NumberOption(MultiMenu2.Main, 0, 100, 10, Format.Percent)]
    public static int RandomMapMira { get; set; } = 10;

    [NumberOption(MultiMenu2.Main, 0, 100, 10, Format.Percent)]
    public static int RandomMapPolus { get; set; } = 10;

    [NumberOption(MultiMenu2.Main, 0, 100, 10, Format.Percent)]
    public static int RandomMapdlekS { get; set; } = 10;

    [NumberOption(MultiMenu2.Main, 0, 100, 10, Format.Percent)]
    public static int RandomMapAirship { get; set; } = 10;

    [NumberOption(MultiMenu2.Main, 0, 100, 10, Format.Percent)]
    public static int RandomMapFungle { get; set; } = 10;

    [NumberOption(MultiMenu2.Main, 0, 100, 10, Format.Percent, All = true)]
    public static int RandomMapSubmerged { get; set; } = 10;

    [NumberOption(MultiMenu2.Main, 0, 100, 10, Format.Percent, All = true)]
    public static int RandomMapLevelImpostor { get; set; } = 10;

    [ToggleOption(MultiMenu2.Main)]
    public static bool AutoAdjustSettings { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool SmallMapHalfVision { get; set; } = false;

    [NumberOption(MultiMenu2.Main, 0f, 15f, 2.5f, Format.Time)]
    public static float SmallMapDecreasedCooldown { get; set; } = 0f;

    [NumberOption(MultiMenu2.Main, 0f, 15f, 2.5f, Format.Time)]
    public static float LargeMapIncreasedCooldown { get; set; } = 0f;

    [NumberOption(MultiMenu2.Main, 0, 5, 1)]
    public static int SmallMapIncreasedShortTasks { get; set; } = 0;

    [NumberOption(MultiMenu2.Main, 0, 3, 1)]
    public static int SmallMapIncreasedLongTasks { get; set; } = 0;

    [NumberOption(MultiMenu2.Main, 0, 5, 1)]
    public static int LargeMapDecreasedShortTasks { get; set; } = 0;

    [NumberOption(MultiMenu2.Main, 0, 3, 1)]
    public static int LargeMapDecreasedLongTasks { get; set; } = 0;

    [HeaderOption(MultiMenu2.Main, [ "CamouflagedComms", "CamouflagedMeetings", "NightVision", "EvilsIgnoreNV", "OxySlow", "ReactorShake" ])]
    public static bool BetterSabotages { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool CamouflagedComms { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool CamouflagedMeetings { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool NightVision { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EvilsIgnoreNV { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool OxySlow { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 0, 100, 5, Format.Percent)]
    public static int ReactorShake { get; set; } = 30;

    [HeaderOption(MultiMenu2.Main, [ "EnableBetterSkeld", "SkeldVentImprovements", "SkeldReactorTimer", "SkeldO2Timer" ])]
    public static bool BetterSkeld { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableBetterSkeld { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool SkeldVentImprovements { get; set; } = false;

    [NumberOption(MultiMenu2.Main, 30f, 90f, 5f, Format.Time)]
    public static float SkeldReactorTimer { get; set; } = 60f;

    [NumberOption(MultiMenu2.Main, 30f, 90f, 5f, Format.Time)]
    public static float SkeldO2Timer { get; set; } = 60f;

    [HeaderOption(MultiMenu2.Main, [ "EnableBetterMiraHQ", "MiraHQVentImprovements", "MiraReactorTimer", "MiraO2Timer" ])]
    public static bool BetterMiraHQ { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableBetterMiraHQ { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool MiraHQVentImprovements { get; set; } = false;

    [NumberOption(MultiMenu2.Main, 30f, 90f, 5f, Format.Time)]
    public static float MiraReactorTimer { get; set; } = 60f;

    [NumberOption(MultiMenu2.Main, 30f, 90f, 5f, Format.Time)]
    public static float MiraO2Timer { get; set; } = 60f;

    [HeaderOption(MultiMenu2.Main, [ "EnableBetterPolus", "PolusVentImprovements", "VitalsLab", "ColdTempDeathValley", "WifiChartCourseSwap", "SeismicTimer" ])]
    public static bool BetterPolus { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableBetterPolus { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool PolusVentImprovements { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool VitalsLab { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool ColdTempDeathValley { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool WifiChartCourseSwap { get; set; } = false;

    [NumberOption(MultiMenu2.Main, 30f, 90f, 5f, Format.Time)]
    public static float SeismicTimer { get; set; } = 60f;

    [HeaderOption(MultiMenu2.Main, [ "EnableBetterAirship", "SpawnType", "MoveVitals", "MoveFuel", "MoveDivert", "MoveAdmin", "MoveElectrical", "MinDoorSwipeTime", "CrashTimer" ])]
    public static bool BetterAirship { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableBetterAirship { get; set; } = true;

    [StringOption(MultiMenu2.Main)]
    public static AirshipSpawnType SpawnType { get; set; } = AirshipSpawnType.Normal;

    [ToggleOption(MultiMenu2.Main)]
    public static bool MoveVitals { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool MoveFuel { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool MoveDivert { get; set; } = false;

    [StringOption(MultiMenu2.Main)]
    public static MoveAdmin MoveAdmin { get; set; } = MoveAdmin.DontMove;

    [StringOption(MultiMenu2.Main)]
    public static MoveElectrical MoveElectrical { get; set; } = MoveElectrical.DontMove;

    [NumberOption(MultiMenu2.Main, 0f, 10f, 0.1f)]
    public static float MinDoorSwipeTime { get; set; } = 0.4f;

    [NumberOption(MultiMenu2.Main, 30f, 100f, 5f, Format.Time)]
    public static float CrashTimer { get; set; } = 90f;

    [HeaderOption(MultiMenu2.Main, [ "EnableBetterFungle", "FungleReactorTimer", "FungleMixupTimer" ])]
    public static bool BetterFungle { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableBetterFungle { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 30f, 90f, 5f, Format.Time)]
    public static float FungleReactorTimer { get; set; } = 60f;

    [NumberOption(MultiMenu2.Main, 4f, 20f, 1f, Format.Time)]
    public static float FungleMixupTimer { get; set; } = 8f;

    // Crew Settings
    [HeaderOption(MultiMenu2.Main, [ "CommonTasks", "LongTasks", "ShortTasks", "GhostTasksCountToWin", "CrewVision", "CrewFlashlight", "CrewMax", "CrewMin", "CrewVent" ])]
    public static bool CrewSettings { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 0, 100, 1)]
    public static int CommonTasks { get; set; } = 2;

    [NumberOption(MultiMenu2.Main, 0, 100, 1)]
    public static int LongTasks { get; set; } = 1;

    [NumberOption(MultiMenu2.Main, 0, 100, 1)]
    public static int ShortTasks { get; set; } = 4;

    [ToggleOption(MultiMenu2.Main)]
    public static bool GhostTasksCountToWin { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 0.25f, 5f, 0.25f, Format.Multiplier)]
    public static float CrewVision { get; set; } = 1f;

    [ToggleOption(MultiMenu2.Main)]
    public static bool CrewFlashlight { get; set; } = false;

    [NumberOption(MultiMenu2.Main, 0, 14, 1)]
    public static int CrewMax { get; set; } = 5;

    [NumberOption(MultiMenu2.Main, 0, 14, 1)]
    public static int CrewMin { get; set; } = 5;

    [ToggleOption(MultiMenu2.Main)]
    public static bool CrewVent { get; set; } = false;

    [HeaderOption(MultiMenu2.Layer, [ "Altruist", "Bastion", "Chameleon", "Coroner", "Crewmate", "Detective", "Dictator", "Engineer", "Escort", "Mayor", "Medic", "Medium", "Monarch",
        "Mystic", "Operative", "Retributionist", "Revealer", "Seer", "Sheriff", "Shifter", "Tracker", "Transporter", "Trapper", "VampireHunter", "Veteran", "Vigilante", "Actor", "Amnesiac", "Arsonist", "BountyHunter", "Cannibal", "Cryomaniac", "Dracula", "Executioner", "Glitch", "GuardianAngel", "Guesser", "Jackal", "Jester", "Juggernaut", "Murderer", "Necromancer", "Pestilence", "Phantom", "Plaguebearer", "SerialKiller", "Survivor", "Thief", "Troll", "Werewolf", "Whisperer", "Ambusher", "Blackmailer", "Camouflager", "Consigliere", "Consort", "Disguiser", "Enforcer", "Ghoul", "Godfather", "Grenadier", "Impostor", "Janitor", "Miner", "Morphling", "Teleporter", "Wraith", "Anarchist", "Banshee", "Bomber", "Collider", "Concealer", "Crusader", "Drunkard", "Framer", "Poisoner", "Rebel", "Shapeshifter", "Silencer", "Spellslinger", "Stalker", "Timekeeper", "Warper", "Hunter", "Hunted", "Runner", "NoneRole", "Astral", "Bait", "Colorblind", "Coward", "Diseased", "Drunk", "Dwarf", "Giant", "Indomitable", "Professional", "Shy", "VIP", "Volatile", "Yeller", "NoneModifier", "Allied", "Corrupted", "Defector", "Fanatic", "Linked", "Lovers", "Mafia", "Overlord", "Rivals", "Taskmaster", "Traitor", "ButtonBarry", "CrewAssassin", "Insider", "IntruderAssassin", "Multitasker", "NeutralAssassin", "Ninja", "Politician", "Radar", "Ruthless", "Snitch", "Swapper", "SyndicateAssassin", "Tiebreaker", "Torch", "Tunneler", "Underdog" ], HeaderType.Layer)] // Don't ask, it's currently for testing
    public static bool Roles { get; set; } = true;

    // Crew Options
    [LayersOption(MultiMenu2.Layer, "#704FA8FF", LayerEnum.Mayor)]
    public static RoleOptionData Mayor { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#FFFF00FF", LayerEnum.Vigilante)]
    public static RoleOptionData Vigilante { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#FFA60AFF", LayerEnum.Engineer)]
    public static RoleOptionData Engineer { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#006600FF", LayerEnum.Medic)]
    public static RoleOptionData Medic { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#FFCC80FF", LayerEnum.Sheriff)]
    public static RoleOptionData Sheriff { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#660000FF", LayerEnum.Altruist)]
    public static RoleOptionData Altruist { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#998040FF", LayerEnum.Veteran)]
    public static RoleOptionData Veteran { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#009900FF", LayerEnum.Tracker)]
    public static RoleOptionData Tracker { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#00EEFFFF", LayerEnum.Transporter)]
    public static RoleOptionData Transporter { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#A680FFFF", LayerEnum.Medium)]
    public static RoleOptionData Medium { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#4D99E6FF", LayerEnum.Coroner)]
    public static RoleOptionData Coroner { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#A7D1B3FF", LayerEnum.Operative)]
    public static RoleOptionData Operative { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#4D4DFFFF", LayerEnum.Detective)]
    public static RoleOptionData Detective { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#DF851FFF", LayerEnum.Shifter)]
    public static RoleOptionData Shifter { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#C0C0C0FF", LayerEnum.VampireHunter)]
    public static RoleOptionData VampireHunter { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#803333FF", LayerEnum.Escort)]
    public static RoleOptionData Escort { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#7E3C64FF", LayerEnum.Bastion)]
    public static RoleOptionData Bastion { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#D3D3D3FF", LayerEnum.Revealer)]
    public static RoleOptionData Revealer { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#708EEFFF", LayerEnum.Mystic)]
    public static RoleOptionData Mystic { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#8D0F8CFF", LayerEnum.Retributionist)]
    public static RoleOptionData Retributionist { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#5411F8FF", LayerEnum.Chameleon)]
    public static RoleOptionData Chameleon { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#71368AFF", LayerEnum.Seer)]
    public static RoleOptionData Seer { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#FF004EFF", LayerEnum.Monarch)]
    public static RoleOptionData Monarch { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#00CB97FF", LayerEnum.Dictator)]
    public static RoleOptionData Dictator { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#BE1C8CFF", LayerEnum.Trapper)]
    public static RoleOptionData Trapper { get; set; } = new(0, 0, false, false);

    // Neutral Options
    [LayersOption(MultiMenu2.Layer, "#F7B3DAFF", LayerEnum.Jester)]
    public static RoleOptionData Jester { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#CCCCCCFF", LayerEnum.Executioner)]
    public static RoleOptionData Executioner { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#00FF00FF", LayerEnum.Glitch)]
    public static RoleOptionData Glitch { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#EE7600FF", LayerEnum.Arsonist)]
    public static RoleOptionData Arsonist { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#22FFFFFF", LayerEnum.Amnesiac)]
    public static RoleOptionData Amnesiac { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#DDDD00FF", LayerEnum.Survivor)]
    public static RoleOptionData Survivor { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#FFFFFFFF", LayerEnum.GuardianAngel)]
    public static RoleOptionData GuardianAngel { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#CFFE61FF", LayerEnum.Plaguebearer)]
    public static RoleOptionData Plaguebearer { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#9F703AFF", LayerEnum.Werewolf)]
    public static RoleOptionData Werewolf { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#8C4005FF", LayerEnum.Cannibal)]
    public static RoleOptionData Cannibal { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#A12B56FF", LayerEnum.Juggernaut)]
    public static RoleOptionData Juggernaut { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#AC8A00FF", LayerEnum.Dracula)]
    public static RoleOptionData Dracula { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#6F7BEAFF", LayerEnum.Murderer)]
    public static RoleOptionData Murderer { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#336EFFFF", LayerEnum.SerialKiller)]
    public static RoleOptionData SerialKiller { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#642DEAFF", LayerEnum.Cryomaniac)]
    public static RoleOptionData Cryomaniac { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#80FF00FF", LayerEnum.Thief)]
    public static RoleOptionData Thief { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#678D36FF", LayerEnum.Troll)]
    public static RoleOptionData Troll { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#45076AFF", LayerEnum.Jackal)]
    public static RoleOptionData Jackal { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#662962FF", LayerEnum.Phantom)]
    public static RoleOptionData Phantom { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#BF5FFFFF", LayerEnum.Necromancer)]
    public static RoleOptionData Necromancer { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#2D6AA5FF", LayerEnum.Whisperer)]
    public static RoleOptionData Whisperer { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#EEE5BEFF", LayerEnum.Guesser)]
    public static RoleOptionData Guesser { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#00ACC2FF", LayerEnum.Actor)]
    public static RoleOptionData Actor { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#B51E39FF", LayerEnum.BountyHunter)]
    public static RoleOptionData BountyHunter { get; set; } = new(0, 0, false, false);

    // Intruder Options
    [LayersOption(MultiMenu2.Layer, "#FFFF99FF", LayerEnum.Consigliere)]
    public static RoleOptionData Consigliere { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#85AA5BFF", LayerEnum.Grenadier)]
    public static RoleOptionData Grenadier { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#BB45B0FF", LayerEnum.Morphling)]
    public static RoleOptionData Morphling { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#5C4F75FF", LayerEnum.Wraith)]
    public static RoleOptionData Wraith { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#378AC0FF", LayerEnum.Camouflager)]
    public static RoleOptionData Camouflager { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#2647A2FF", LayerEnum.Janitor)]
    public static RoleOptionData Janitor { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#AA7632FF", LayerEnum.Miner)]
    public static RoleOptionData Miner { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#02A752FF", LayerEnum.Blackmailer)]
    public static RoleOptionData Blackmailer { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#40B4FFFF", LayerEnum.Disguiser)]
    public static RoleOptionData Disguiser { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#801780FF", LayerEnum.Consort)]
    public static RoleOptionData Consort { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#939593FF", LayerEnum.Teleporter)]
    public static RoleOptionData Teleporter { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#404C08FF", LayerEnum.Godfather)]
    public static RoleOptionData Godfather { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#2BD29CFF", LayerEnum.Ambusher)]
    public static RoleOptionData Ambusher { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#F1C40FFF", LayerEnum.Ghoul)]
    public static RoleOptionData Ghoul { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#005643FF", LayerEnum.Enforcer)]
    public static RoleOptionData Enforcer { get; set; } = new(0, 0, false, false);

    // Syndicate Options
    [LayersOption(MultiMenu2.Layer, "#8C7140FF", LayerEnum.Warper)]
    public static RoleOptionData Warper { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#00FFFFFF", LayerEnum.Framer)]
    public static RoleOptionData Framer { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#FFFCCEFF", LayerEnum.Rebel)]
    public static RoleOptionData Rebel { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#C02525FF", LayerEnum.Concealer)]
    public static RoleOptionData Concealer { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#2DFF00FF", LayerEnum.Shapeshifter)]
    public static RoleOptionData Shapeshifter { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#C9CC3FFF", LayerEnum.Bomber)]
    public static RoleOptionData Bomber { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#B5004CFF", LayerEnum.Poisoner)]
    public static RoleOptionData Poisoner { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#DF7AE8FF", LayerEnum.Crusader)]
    public static RoleOptionData Crusader { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#E67E22FF", LayerEnum.Banshee)]
    public static RoleOptionData Banshee { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#B345FFFF", LayerEnum.Collider)]
    public static RoleOptionData Collider { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#7E4D00FF", LayerEnum.Stalker)]
    public static RoleOptionData Stalker { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#0028F5FF", LayerEnum.Spellslinger)]
    public static RoleOptionData Spellslinger { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#FF7900FF", LayerEnum.Drunkard)]
    public static RoleOptionData Drunkard { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#3769FEFF", LayerEnum.Timekeeper)]
    public static RoleOptionData Timekeeper { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#AAB43EFF", LayerEnum.Silencer)]
    public static RoleOptionData Silencer { get; set; } = new(0, 0, false, false);

    // Modifier Options
    [LayersOption(MultiMenu2.Layer, "#00B3B3FF", LayerEnum.Bait)]
    public static RoleOptionData Bait { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#456BA8FF", LayerEnum.Coward)]
    public static RoleOptionData Coward { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#374D1EFF", LayerEnum.Diseased)]
    public static RoleOptionData Diseased { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#758000FF", LayerEnum.Drunk)]
    public static RoleOptionData Drunk { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#FF8080FF", LayerEnum.Dwarf)]
    public static RoleOptionData Dwarf { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#FFB34DFF", LayerEnum.Giant)]
    public static RoleOptionData Giant { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#FFA60AFF", LayerEnum.Volatile)]
    public static RoleOptionData Volatile { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#DCEE85FF", LayerEnum.VIP)]
    public static RoleOptionData VIP { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#1002C5FF", LayerEnum.Shy)]
    public static RoleOptionData Shy { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#860B7AFF", LayerEnum.Professional)]
    public static RoleOptionData Professional { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#2DE5BEFF", LayerEnum.Indomitable)]
    public static RoleOptionData Indomitable { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#612BEFFF", LayerEnum.Astral)]
    public static RoleOptionData Astral { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#F6AAB7FF", LayerEnum.Yeller)]
    public static RoleOptionData Yeller { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#B34D99FF", LayerEnum.Colorblind)]
    public static RoleOptionData Colorblind { get; set; } = new(0, 0, false, false);

    // Ability Options
    [LayersOption(MultiMenu2.Layer, "#FFFF99FF", LayerEnum.Torch)]
    public static RoleOptionData Torch { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#E91E63FF", LayerEnum.Tunneler)]
    public static RoleOptionData Tunneler { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#E600FFFF", LayerEnum.ButtonBarry)]
    public static RoleOptionData ButtonBarry { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#99E699FF", LayerEnum.Tiebreaker)]
    public static RoleOptionData Tiebreaker { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#D4AF37FF", LayerEnum.Snitch)]
    public static RoleOptionData Snitch { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#841A7FFF", LayerEnum.Underdog)]
    public static RoleOptionData Underdog { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#26FCFBFF", LayerEnum.Insider)]
    public static RoleOptionData Insider { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#FF0080FF", LayerEnum.Radar)]
    public static RoleOptionData Radar { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#FF804DFF", LayerEnum.Multitasker)]
    public static RoleOptionData Multitasker { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#2160DDFF", LayerEnum.Ruthless)]
    public static RoleOptionData Ruthless { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#A84300FF", LayerEnum.Ninja)]
    public static RoleOptionData Ninja { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#CCA3CCFF", LayerEnum.Politician)]
    public static RoleOptionData Politician { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#66E666FF", LayerEnum.Swapper)]
    public static RoleOptionData Swapper { get; set; } = new(0, 0, false, false);

    // Objectifier Options
    [LayersOption(MultiMenu2.Layer, "#FF66CCFF", LayerEnum.Lovers)]
    public static RoleOptionData Lovers { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#370D43FF", LayerEnum.Traitor)]
    public static RoleOptionData Traitor { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#3D2D2CFF", LayerEnum.Rivals)]
    public static RoleOptionData Rivals { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#678D36FF", LayerEnum.Fanatic)]
    public static RoleOptionData Fanatic { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#ABABFFFF", LayerEnum.Taskmaster)]
    public static RoleOptionData Taskmaster { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#008080FF", LayerEnum.Overlord)]
    public static RoleOptionData Overlord { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#4545FFFF", LayerEnum.Corrupted)]
    public static RoleOptionData Corrupted { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#4545A9FF", LayerEnum.Allied)]
    public static RoleOptionData Allied { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#00EEFFFF", LayerEnum.Mafia)]
    public static RoleOptionData Mafia { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#E1C849FF", LayerEnum.Defector)]
    public static RoleOptionData Defector { get; set; } = new(0, 0, false, false);

    [LayersOption(MultiMenu2.Layer, "#FF351FFF", LayerEnum.Linked)]
    public static RoleOptionData Linked { get; set; } = new(0, 0, false, false);
}