namespace TownOfUsReworked.Options2;

// DO NOT OVERRIDE VALUES OF ANY OF THESE PROPERTIES ANY WHERE IN THE CODE OR ELSE THE OPTIONS WILL START TO FUCK OFF
public static class CustomGameOptions2
{
    // Global Options
    [HeaderOption(MultiMenu2.Main, [ "PlayerSpeed", "GhostSpeed", "InteractionDistance", "EmergencyButtonCount", "TaskBar", "VotingTime", "DiscussionTime", "EmergencyButtonCooldown", "ConfirmEjects", "EjectionRevealsRoles", "EnableInitialCds", "InitialCooldowns", "EnableMeetingCds", "MeetingCooldowns", "EnableFailCds", "FailCooldowns", "ReportDistance", "ChatCooldown", "ChatCharacterLimit", "LobbySize" ])]
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
    [HeaderOption(MultiMenu2.Main, [ "GameMode", "IgnoreAlignmentCaps", "IgnoreFactionCaps", "IgnoreLayerCaps", "NeutralsCount", "AddArsonist", "AddCryomaniac", "AddPlaguebearer", "HnSMode",
        "HnSShortTasks", "HnSCommonTasks", "HnSLongTasks", "HunterCount", "HuntCd", "StartTime", "HunterVent", "HunterVision", "HuntedVision", "HunterSpeedModifier", "HunterFlashlight",
        "HuntedFlashlight", "HuntedChat", "TRShortTasks", "TRCommonTasks" ])]
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
    [HeaderOption(MultiMenu2.Main, [ "WhoCanVent", "AnonymousVoting", "NoSkipping", "FirstKillShield", "WhoSeesFirstKillShield", "FactionSeeRoles", "VisualTasks",
        "PlayerNames", "Whispers", "WhispersAnnouncement", "AppearanceAnimation", "EnableAbilities", "EnableModifiers", "EnableObjectifiers", "VentTargeting",
        "CooldownInVent", "DeadSeeEverything", "ParallelMedScans", "HideVentAnims", "JaniCanMutuallyExclusive", "IndicateReportedBodies", "RandomSpawns" ])]
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

    [HeaderOption(MultiMenu2.Main, [ "EnableBetterAirship", "SpawnType", "MoveVitals", "MoveFuel", "MoveDivert", "MoveAdmin", "MoveElectrical", "MinDoorSwipeTime",
        "CrashTimer" ])]
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

    // [HeaderOption(MultiMenu2.Layer, [ "Test" ], HeaderType.Layer)]
    // public static bool tEST { get; set; } = true;

    // [LayersOption(MultiMenu2.Layer, "#837456FF", LayerEnum.Altruist, [ "AnonymousVoting2" ])]
    // public static RoleOptionData Test { get; set; } = new(0, 0, false, false);

    // [StringOption(MultiMenu2.LayerSubOptions)]
    // public static AnonVotes AnonymousVoting2 { get; set; } = AnonVotes.Enabled;
}