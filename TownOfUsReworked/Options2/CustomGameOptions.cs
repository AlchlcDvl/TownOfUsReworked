namespace TownOfUsReworked.Options2;

// DO NOT OVERRIDE VALUES OF ANY OF THESE PROPERTIES OR ELSE THE OPTIONS WILL START TO FUCK OFF
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
    [HeaderOption(MultiMenu2.Main, [ "GameMode" ])]
    public static bool GameModeSettings { get; set; } = true;

    [StringOption(MultiMenu2.Main, [ "None" ])]
    public static GameMode GameMode { get; set; } = GameMode.Classic;

    // Classic/Custom Settings
    [HeaderOption(MultiMenu2.Main, [ "IgnoreAlignmentCaps", "IgnoreFactionCaps", "IgnoreLayerCaps" ])]
    public static bool ClassCustSettings { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool IgnoreFactionCaps { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool IgnoreAlignmentCaps { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool IgnoreLayerCaps { get; set; } = false;

    // Role List Settings
    [HeaderOption(MultiMenu2.Main, [ "" ])]
    public static bool RLSettings { get; set; } = true;

    // Killing Only Settings
    [HeaderOption(MultiMenu2.Main, [ "NeutralsCount", "AddArsonist", "AddCryomaniac", "AddPlaguebearer" ])]
    public static bool KOSettings { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 0, 13, 1)]
    public static int NeutralsCount { get; set; } = 1;

    [ToggleOption(MultiMenu2.Main)]
    public static bool AddArsonist { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool AddCryomaniac { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool AddPlaguebearer { get; set; } = false;

    // Hide And Seek Settings
    [HeaderOption(MultiMenu2.Main, [ "HnSMode", "HnSShortTasks", "HnSCommonTasks", "HnSLongTasks", "HunterCount", "HuntCd", "StartTime", "HunterVent", "HunterVision", "HuntedVision", "HunterSpeedModifier", "HunterFlashlight", "HuntedFlashlight", "HuntedChat" ])]
    public static bool HnSSettings { get; set; } = true;

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

    // Task Race Settings
    [HeaderOption(MultiMenu2.Main, [ "TRShortTasks", "TRCommonTasks" ])]
    public static bool TRSettings { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 0, 13, 1)]
    public static int TRShortTasks { get; set; } = 4;

    [NumberOption(MultiMenu2.Main, 0, 13, 1)]
    public static int TRCommonTasks { get; set; } = 4;

    // Game Modifiers
    [HeaderOption(MultiMenu2.Main, [ "WhoCanVent", "AnonymousVoting", "NoSkipping" ])]
    public static bool GameModifiers { get; set; } = true;

    [StringOption(MultiMenu2.Main)]
    public static WhoCanVentOptions WhoCanVent { get; set; } = WhoCanVentOptions.Default;

    [StringOption(MultiMenu2.Main)]
    public static AnonVotes AnonymousVoting { get; set; } = AnonVotes.Enabled;

    [StringOption(MultiMenu2.Main)]
    public static DisableSkipButtonMeetings NoSkipping { get; set; } = DisableSkipButtonMeetings.No;

    // Map Settings
    public static MapEnum Map { get; set; }

    [HeaderOption(MultiMenu2.Layer, [ "Test" ], HeaderType.Layer)]
    public static bool tEST { get; set; } = true;

    [LayersOption(MultiMenu2.Layer, "#837456FF", LayerEnum.Altruist, [ "AnonymousVoting2" ])]
    public static RoleOptionData Test { get; set; } = new(0, 0, false, false);

    [StringOption(MultiMenu2.LayerSubOptions)]
    public static AnonVotes AnonymousVoting2 { get; set; } = AnonVotes.Enabled;
}