namespace TownOfUsReworked.Options2;

// DO NOT OVERRIDE VALUES OF ANY OF THESE PROPERTIES OR ELSE THE OPTIONS WILL START TO FUCK OFF
public static class CustomGameOptions2
{
    // Global Options
    [HeaderOption(MultiMenu.Main, [ "PlayerSpeed", "GhostSpeed", "InteractionDistance", "EmergencyButtonCount", "TaskBar", "VotingTime", "DiscussionTime", "EmergencyButtonCooldown", "ConfirmEjects", "EjectionRevealsRoles", "EnableInitialCds", "InitialCooldowns", "EnableMeetingCds", "MeetingCooldowns", "EnableFailCds", "FailCooldowns", "ReportDistance", "ChatCooldown", "ChatCharacterLimit", "LobbySize" ])]
    public static object GameSettings { get; set; }

    [NumberOption(MultiMenu.Main, 0.25f, 10, 0.25f, Format.Multiplier)]
    public static float PlayerSpeed { get; set; } = 1.25f;

    [NumberOption(MultiMenu.Main, 0.25f, 10, 0.25f, Format.Multiplier)]
    public static float GhostSpeed { get; set; } = 3;

    [NumberOption(MultiMenu.Main, 0.5f, 5, 0.5f, Format.Distance)]
    public static float InteractionDistance { get; set; } = 2;

    [NumberOption(MultiMenu.Main, 0, 100, 1)]
    public static int EmergencyButtonCount { get; set; } = 1;

    [NumberOption(MultiMenu.Main, 0, 300, 2.5f, Format.Time)]
    public static float EmergencyButtonCooldown { get; set; } = 25;

    [NumberOption(MultiMenu.Main, 0, 300, 5, Format.Time)]
    public static float DiscussionTime { get; set; } = 30;

    [NumberOption(MultiMenu.Main, 5, 600, 15, Format.Time)]
    public static float VotingTime { get; set; } = 60;

    [StringOption(MultiMenu.Main, typeof(AmongUs.GameOptions.TaskBarMode))]
    private static AmongUs.GameOptions.TaskBarMode TaskBar { get; set; } = AmongUs.GameOptions.TaskBarMode.MeetingOnly;
    public static AmongUs.GameOptions.TaskBarMode TaskBarMode => GameMode switch
    {
        GameMode.TaskRace or GameMode.HideAndSeek => AmongUs.GameOptions.TaskBarMode.Normal,
        _ => TaskBar
    };

    [ToggleOption(MultiMenu.Main)]
    public static bool ConfirmEjects { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool EjectionRevealsRoles { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool EnableInitialCds { get; set; } = true;

    [NumberOption(MultiMenu.Main, 0, 30, 2.5f, Format.Time)]
    public static float InitialCooldowns { get; set; } = 10;

    [ToggleOption(MultiMenu.Main)]
    public static bool EnableMeetingCds { get; set; } = true;

    [NumberOption(MultiMenu.Main, 0, 30, 2.5f, Format.Time)]
    public static float MeetingCooldowns { get; set; } = 15;

    [ToggleOption(MultiMenu.Main)]
    public static bool EnableFailCds { get; set; } = true;

    [NumberOption(MultiMenu.Main, 0, 30, 2.5f, Format.Time)]
    public static float FailCooldowns { get; set; } = 5;

    [NumberOption(MultiMenu.Main, 1, 20, 0.25f, Format.Distance)]
    public static float ReportDistance { get; set; } = 3.5f;

    [NumberOption(MultiMenu.Main, 0, 3, 0.1f, Format.Time)]
    public static float ChatCooldown { get; set; } = 3;

    [NumberOption(MultiMenu.Main, 50, 2000, 50)]
    public static int ChatCharacterLimit { get; set; } = 200;

    [NumberOption(MultiMenu.Main, 2, 127, 1)]
    public static int LobbySize { get; set; } = 15;

    // Game Modes
    [HeaderOption(MultiMenu.Main, [ "GameMode" ])]
    public static object GameModeSettings { get; set; }

    [StringOption(MultiMenu.Main, typeof(GameMode), [ "None" ])]
    public static GameMode GameMode { get; set; } = GameMode.Classic;

    // Classic/Custom Settings
    [HeaderOption(MultiMenu.Main, [ "IgnoreAlignmentCaps", "IgnoreFactionCaps", "IgnoreLayerCaps" ])]
    public static object ClassCustSettings { get; set; }

    [ToggleOption(MultiMenu.Main)]
    public static bool IgnoreFactionCaps { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool IgnoreAlignmentCaps { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool IgnoreLayerCaps { get; set; } = false;

    // All Any/Role List Settings
    [HeaderOption(MultiMenu.Main, [ "EnableUniques" ])]
    public static object AARLSettings { get; set; }

    [ToggleOption(MultiMenu.Main)]
    public static bool EnableUniques { get; set; } = false;

    // Killing Only Settings
    [HeaderOption(MultiMenu.Main, [ "NeutralsCount", "AddArsonist", "AddCryomaniac", "AddPlaguebearer" ])]
    public static object KOSettings { get; set; }

    [NumberOption(MultiMenu.Main, 0, 13, 1)]
    public static int NeutralsCount { get; set; } = 1;

    [ToggleOption(MultiMenu.Main)]
    public static bool AddArsonist { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool AddCryomaniac { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool AddPlaguebearer { get; set; } = false;

    // Hide And Seek Settings
    [HeaderOption(MultiMenu.Main, [ "HnSMode", "HnSShortTasks", "HnSCommonTasks", "HnSLongTasks", "HunterCount", "HuntCd", "StartTime", "HunterVent", "HunterVision", "HuntedVision", "HunterSpeedModifier", "HunterFlashlight", "HuntedFlashlight", "HuntedChat" ])]
    public static object HnSSettings { get; set; }

    [StringOption(MultiMenu.Main, typeof(HnSMode))]
    public static HnSMode HnSMode { get; set; } = HnSMode.Classic;

    [NumberOption(MultiMenu.Main, 0, 13, 1)]
    public static int HnSShortTasks { get; set; } = 4;

    [NumberOption(MultiMenu.Main, 0, 13, 1)]
    public static int HnSCommonTasks { get; set; } = 4;

    [NumberOption(MultiMenu.Main, 0, 13, 1)]
    public static int HnSLongTasks { get; set; } = 4;

    [NumberOption(MultiMenu.Main, 1, 13, 1)]
    public static int HunterCount { get; set; } = 1;

    [NumberOption(MultiMenu.Main, 5f, 60f, 5f, Format.Time)]
    public static float HuntCd { get; set; } = 10;

    [NumberOption(MultiMenu.Main, 5f, 60f, 5f, Format.Time)]
    public static float StartTime { get; set; } = 10;

    [ToggleOption(MultiMenu.Main)]
    public static bool HunterVent { get; set; } = true;

    [NumberOption(MultiMenu.Main, 0.1f, 1f, 0.05f, Format.Multiplier)]
    public static float HunterVision { get; set;} = 0.25f;

    [NumberOption(MultiMenu.Main, 1f, 2f, 0.05f, Format.Multiplier)]
    public static float HuntedVision { get; set;} = 1.5f;

    [NumberOption(MultiMenu.Main, 1f, 1.5f, 0.05f, Format.Multiplier)]
    public static float HunterSpeedModifier { get; set; } = 1.25f;

    [ToggleOption(MultiMenu.Main)]
    public static bool HunterFlashlight { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool HuntedFlashlight { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool HuntedChat { get; set; } = true;

    // Task Race Settings
    [HeaderOption(MultiMenu.Main, [ "TRShortTasks", "TRCommonTasks" ])]
    public static object TRSettings { get; set; }

    [NumberOption(MultiMenu.Main, 0, 13, 1)]
    public static int TRShortTasks { get; set; } = 4;

    [NumberOption(MultiMenu.Main, 0, 13, 1)]
    public static int TRCommonTasks { get; set; } = 4;

    // Game Modifiers
    [HeaderOption(MultiMenu.Main, [ "WhoCanVent", "AnonymousVoting", "NoSkipping" ])]
    public static object GameModifiers { get; set; }

    [StringOption(MultiMenu.Main, typeof(WhoCanVentOptions))]
    public static WhoCanVentOptions WhoCanVent { get; set; } = WhoCanVentOptions.Default;

    [StringOption(MultiMenu.Main, typeof(AnonVotes))]
    public static AnonVotes AnonymousVoting { get; set; } = AnonVotes.Enabled;

    [StringOption(MultiMenu.Main, typeof(DisableSkipButtonMeetings))]
    public static DisableSkipButtonMeetings NoSkipping { get; set; } = DisableSkipButtonMeetings.No;

    // Map Settings
    public static MapEnum Map { get; set; }

    [HeaderOption(MultiMenu.Crew, [], HeaderType.Layer)]
    public static object tEST { get; set; }

    [LayersOption(MultiMenu.Crew, "#837456FF", LayerEnum.Altruist)]
    public static RoleOptionData Test { get; set; } = new(0, 0, false, false);
}