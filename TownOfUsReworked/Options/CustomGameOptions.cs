namespace TownOfUsReworked.Options;

// DO NOT OVERRIDE VALUES OF ANY OF THESE PROPERTIES ANY WHERE IN THE CODE OR ELSE THE OPTIONS WILL START TO FUCK OFF

[HeaderOption(MultiMenu.Main)]
public static class GameSettings
{
    [NumberOption(MultiMenu.Main, 0.25f, 10, 0.25f, Format.Multiplier)]
    public static float PlayerSpeed { get; set; } = 1.25f;

    [NumberOption(MultiMenu.Main, 0.25f, 10, 0.25f, Format.Multiplier)]
    public static float GhostSpeed { get; set; } = 3;

    [NumberOption(MultiMenu.Main, 0.5f, 5, 0.5f, Format.Distance)]
    public static float InteractionDistance { get; set; } = 2;

    [NumberOption(MultiMenu.Main, 0, 100, 1)]
    public static int EmergencyButtonCount { get; set; } = 1;

    [NumberOption(MultiMenu.Main, 0, 300, 2.5f, Format.Time)]
    public static int EmergencyButtonCooldown { get; set; } = 25;

    [NumberOption(MultiMenu.Main, 0, 300, 5, Format.Time)]
    public static int DiscussionTime { get; set; } = 30;

    [NumberOption(MultiMenu.Main, 5, 600, 15, Format.Time)]
    public static int VotingTime { get; set; } = 60;

    [StringOption(MultiMenu.Main)]
    private static TBMode TaskBar { get; set; } = TBMode.MeetingOnly;
    public static TBMode TaskBarMode => GameModeSettings.GameMode switch
    {
        GameMode.TaskRace or GameMode.HideAndSeek => TBMode.Normal,
        _ => TaskBar
    }; // I want this to actually change according to the game modes

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
}

[HeaderOption(MultiMenu.Main)]
public static class GameModeSettings
{
    [StringOption(MultiMenu.Main, [ "None" ])]
    public static GameMode GameMode { get; set; } = GameMode.Classic;

    [ToggleOption(MultiMenu.Main)]
    public static bool IgnoreFactionCaps { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool IgnoreAlignmentCaps { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool IgnoreLayerCaps { get; set; } = false;

    [NumberOption(MultiMenu.Main, 0, 13, 1)]
    public static int NeutralsCount { get; set; } = 1;

    [ToggleOption(MultiMenu.Main)]
    public static bool AddArsonist { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool AddCryomaniac { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool AddPlaguebearer { get; set; } = false;

    [StringOption(MultiMenu.Main)]
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

    [NumberOption(MultiMenu.Main, 0, 13, 1)]
    public static int TRShortTasks { get; set; } = 4;

    [NumberOption(MultiMenu.Main, 0, 13, 1)]
    public static int TRCommonTasks { get; set; } = 4;

    [NumberOption(MultiMenu.Main, 0, 13, 1)]
    public static int TRLongTasks { get; set; } = 1;

    [ToggleOption(MultiMenu.Main)]
    public static bool BanCrewmate { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool BanMurderer { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool BanImpostor { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool BanAnarchist { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool EnableRevealer { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool EnablePhantom { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool EnableGhoul { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool EnableBanshee { get; set; } = false;
}

[HeaderOption(MultiMenu.Main)]
public static class GameModifiers
{
    [StringOption(MultiMenu.Main)]
    public static WhoCanVentOptions WhoCanVent { get; set; } = WhoCanVentOptions.Default;

    [StringOption(MultiMenu.Main)]
    public static AnonVotes AnonymousVoting { get; set; } = AnonVotes.Enabled;

    [StringOption(MultiMenu.Main)]
    public static DisableSkipButtonMeetings NoSkipping { get; set; } = DisableSkipButtonMeetings.No;

    [ToggleOption(MultiMenu.Main)]
    public static bool FirstKillShield { get; set; } = false;

    [StringOption(MultiMenu.Main)]
    public static WhoCanSeeFirstKillShield WhoSeesFirstKillShield { get; set; } = WhoCanSeeFirstKillShield.Everyone;

    [ToggleOption(MultiMenu.Main)]
    public static bool FactionSeeRoles { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool VisualTasks { get; set; } = false;

    [StringOption(MultiMenu.Main)]
    public static PlayerNames PlayerNames { get; set; } = PlayerNames.Obstructed;

    [ToggleOption(MultiMenu.Main)]
    public static bool Whispers { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool WhispersAnnouncement { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool AppearanceAnimation { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool EnableAbilities { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool EnableModifiers { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool EnableObjectifiers { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool VentTargeting { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool CooldownInVent { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool DeadSeeEverything { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool ParallelMedScans { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool HideVentAnims { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool JaniCanMutuallyExclusive { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool IndicateReportedBodies { get; set; } = false;

    [StringOption(MultiMenu.Main)]
    public static RandomSpawning RandomSpawns { get; set; } = RandomSpawning.Disabled;
}

[HeaderOption(MultiMenu.Main)]
public static class GameAnnouncementSettings
{
    [ToggleOption(MultiMenu.Main)]
    public static bool GameAnnouncements { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool LocationReports { get; set; } = false;

    [StringOption(MultiMenu.Main)]
    public static RoleFactionReports RoleFactionReports { get; set; } = RoleFactionReports.Neither;

    [StringOption(MultiMenu.Main)]
    public static RoleFactionReports KillerReports { get; set; } = RoleFactionReports.Neither;
}

[HeaderOption(MultiMenu.Main)]
public static class MapSettings
{
    public static MapEnum Map { get; set; }

    [NumberOption(MultiMenu.Main, 0, 100, 10, Format.Percent)]
    public static int RandomMapSkeld { get; set; } = 10;

    [NumberOption(MultiMenu.Main, 0, 100, 10, Format.Percent)]
    public static int RandomMapMira { get; set; } = 10;

    [NumberOption(MultiMenu.Main, 0, 100, 10, Format.Percent)]
    public static int RandomMapPolus { get; set; } = 10;

    [NumberOption(MultiMenu.Main, 0, 100, 10, Format.Percent)]
    public static int RandomMapdlekS { get; set; } = 10;

    [NumberOption(MultiMenu.Main, 0, 100, 10, Format.Percent)]
    public static int RandomMapAirship { get; set; } = 10;

    [NumberOption(MultiMenu.Main, 0, 100, 10, Format.Percent)]
    public static int RandomMapFungle { get; set; } = 10;

    [NumberOption(MultiMenu.Main, 0, 100, 10, Format.Percent, All = true)]
    private static int RandomMapSubmergedPriv { get; set; } = 10;
    public static int RandomMapSubmerged => SubLoaded ? RandomMapSubmergedPriv : 0;

    [NumberOption(MultiMenu.Main, 0, 100, 10, Format.Percent, All = true)]
    private static int RandomMapLevelImpostorPriv { get; set; } = 10;
    public static int RandomMapLevelImpostor => LILoaded ? RandomMapLevelImpostorPriv : 0;

    [ToggleOption(MultiMenu.Main)]
    public static bool AutoAdjustSettings { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool SmallMapHalfVision { get; set; } = false;

    [NumberOption(MultiMenu.Main, 0f, 15f, 2.5f, Format.Time)]
    public static float SmallMapDecreasedCooldown { get; set; } = 0f;

    [NumberOption(MultiMenu.Main, 0f, 15f, 2.5f, Format.Time)]
    public static float LargeMapIncreasedCooldown { get; set; } = 0f;

    [NumberOption(MultiMenu.Main, 0, 5, 1)]
    public static int SmallMapIncreasedShortTasks { get; set; } = 0;

    [NumberOption(MultiMenu.Main, 0, 3, 1)]
    public static int SmallMapIncreasedLongTasks { get; set; } = 0;

    [NumberOption(MultiMenu.Main, 0, 5, 1)]
    public static int LargeMapDecreasedShortTasks { get; set; } = 0;

    [NumberOption(MultiMenu.Main, 0, 3, 1)]
    public static int LargeMapDecreasedLongTasks { get; set; } = 0;
}

[HeaderOption(MultiMenu.Main)]
public static class CrewSettings
{
    [NumberOption(MultiMenu.Main, 0, 100, 1)]
    public static int CommonTasks { get; set; } = 2;

    [NumberOption(MultiMenu.Main, 0, 100, 1)]
    public static int LongTasks { get; set; } = 1;

    [NumberOption(MultiMenu.Main, 0, 100, 1)]
    public static int ShortTasks { get; set; } = 4;

    [ToggleOption(MultiMenu.Main)]
    public static bool GhostTasksCountToWin { get; set; } = true;

    [NumberOption(MultiMenu.Main, 0.25f, 5f, 0.25f, Format.Multiplier)]
    public static float CrewVision { get; set; } = 1f;

    [ToggleOption(MultiMenu.Main)]
    public static bool CrewFlashlight { get; set; } = false;

    [NumberOption(MultiMenu.Main, 0, 14, 1)]
    public static int CrewMax { get; set; } = 5;

    [NumberOption(MultiMenu.Main, 0, 14, 1)]
    public static int CrewMin { get; set; } = 5;

    [StringOption(MultiMenu.Main)]
    public static CrewVenting CrewVent { get; set; } = CrewVenting.Never;
}

[HeaderOption(MultiMenu.Main)]
public static class NeutralSettings
{
    [NumberOption(MultiMenu.Main, 0.25f, 5f, 0.25f, Format.Multiplier)]
    public static float NeutralVision { get; set; } = 1.5f;

    [ToggleOption(MultiMenu.Main)]
    public static bool LightsAffectNeutrals { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool NeutralFlashlight { get; set; } = false;

    [NumberOption(MultiMenu.Main, 1, 14, 1)]
    public static int NeutralMax { get; set; } = 5;

    [NumberOption(MultiMenu.Main, 1, 14, 1)]
    public static int NeutralMin { get; set; } = 5;

    [StringOption(MultiMenu.Main)]
    public static NoSolo NoSolo { get; set; } = NoSolo.Never;

    [ToggleOption(MultiMenu.Main)]
    public static bool AvoidNeutralKingmakers { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool NeutralsVent { get; set; } = true;
}

[HeaderOption(MultiMenu.Main)]
public static class IntruderSettings
{
    [NumberOption(MultiMenu.Main, 0, 4, 1)]
    public static int IntruderCount { get; set; } = 1;

    [NumberOption(MultiMenu.Main, 0.25f, 5f, 0.25f, Format.Multiplier)]
    public static float IntruderVision { get; set; } = 2f;

    [ToggleOption(MultiMenu.Main)]
    public static bool IntruderFlashlight { get; set; } = false;

    [NumberOption(MultiMenu.Main, 10f, 60f, 2.5f, Format.Time)]
    public static float IntKillCd { get; set; } = 25f;

    [ToggleOption(MultiMenu.Main)]
    public static bool IntrudersVent { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool IntrudersCanSabotage { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool GhostsCanSabotage { get; set; } = false;

    [NumberOption(MultiMenu.Main, 1, 14, 1)]
    public static int IntruderMax { get; set; } = 5;

    [NumberOption(MultiMenu.Main, 1, 14, 1)]
    public static int IntruderMin { get; set; } = 5;
}

[HeaderOption(MultiMenu.Main)]
public static class SyndicateSettings
{
    [NumberOption(MultiMenu.Main, 0, 4, 1)]
    public static int SyndicateCount { get; set; } = 1;

    [NumberOption(MultiMenu.Main, 0.25f, 5f, 0.25f, Format.Multiplier)]
    public static float SyndicateVision { get; set; } = 2f;

    [ToggleOption(MultiMenu.Main)]
    public static bool SyndicateFlashlight { get; set; } = false;

    [NumberOption(MultiMenu.Main, 1, 10, 1)]
    public static int ChaosDriveMeetingCount { get; set; } = 3;

    [NumberOption(MultiMenu.Main, 10f, 60f, 2.5f, Format.Time)]
    public static float CDKillCd { get; set; } = 25f;

    [StringOption(MultiMenu.Main)]
    public static SyndicateVentOptions SyndicateVent { get; set; } = SyndicateVentOptions.Always;

    [ToggleOption(MultiMenu.Main)]
    private static bool AltImpsPriv { get; set; } = false;
    public static bool AltImps => AltImpsPriv || IntruderSettings.IntruderCount == 0;

    [ToggleOption(MultiMenu.Main)]
    public static bool GlobalDrive { get; set; } = false;

    [NumberOption(MultiMenu.Main, 1, 14, 1)]
    public static int SyndicateMax { get; set; } = 5;

    [NumberOption(MultiMenu.Main, 1, 14, 1)]
    public static int SyndicateMin { get; set; } = 5;
}

public static class TaskSettings
{
    public static int ShortTasks => GameModeSettings.GameMode switch
    {
        GameMode.TaskRace => GameModeSettings.TRShortTasks,
        GameMode.HideAndSeek => GameModeSettings.HnSShortTasks,
        _ => CrewSettings.ShortTasks
    };
    public static int LongTasks => GameModeSettings.GameMode switch
    {
        GameMode.TaskRace => GameModeSettings.TRLongTasks,
        GameMode.HideAndSeek => GameModeSettings.HnSLongTasks,
        _ => CrewSettings.LongTasks
    };
    public static int CommonTasks => GameModeSettings.GameMode switch
    {
        GameMode.TaskRace => GameModeSettings.TRCommonTasks,
        GameMode.HideAndSeek => GameModeSettings.HnSCommonTasks,
        _ => CrewSettings.CommonTasks
    };
}

[AlignsOption(MultiMenu.Layer, Alignment.CrewAudit)]
public static class CrewAuditorRoles
{
    [LayersOption(MultiMenu.Layer, "#708EEFFF", LayerEnum.Mystic)]
    public static RoleOptionData Mystic { get; set; }

    [LayersOption(MultiMenu.Layer, "#C0C0C0FF", LayerEnum.VampireHunter)]
    public static RoleOptionData VampireHunter { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.CrewInvest)]
public static class CrewInvestigativeRoles
{
    [LayersOption(MultiMenu.Layer, "#4D99E6FF", LayerEnum.Coroner)]
    public static RoleOptionData Coroner { get; set; }

    [LayersOption(MultiMenu.Layer, "#4D4DFFFF", LayerEnum.Detective)]
    public static RoleOptionData Detective { get; set; }

    [LayersOption(MultiMenu.Layer, "#A680FFFF", LayerEnum.Medium)]
    public static RoleOptionData Medium { get; set; }

    [LayersOption(MultiMenu.Layer, "#A7D1B3FF", LayerEnum.Operative)]
    public static RoleOptionData Operative { get; set; }

    [LayersOption(MultiMenu.Layer, "#71368AFF", LayerEnum.Seer)]
    public static RoleOptionData Seer { get; set; }

    [LayersOption(MultiMenu.Layer, "#FFCC80FF", LayerEnum.Sheriff)]
    public static RoleOptionData Sheriff { get; set; }

    [LayersOption(MultiMenu.Layer, "#009900FF", LayerEnum.Tracker)]
    public static RoleOptionData Tracker { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.CrewKill)]
public static class CrewKillingRoles
{
    [LayersOption(MultiMenu.Layer, "#7E3C64FF", LayerEnum.Bastion)]
    private static RoleOptionData BastionPriv { get; set; }
    public static RoleOptionData Bastion
    {
        get
        {
            var result = BastionPriv.Clone();

            if (IsKilling)
                result.Chance = 100;

            return result;
        }
    }

    [LayersOption(MultiMenu.Layer, "#998040FF", LayerEnum.Veteran)]
    private static RoleOptionData VeteranPriv { get; set; }
    public static RoleOptionData Veteran
    {
        get
        {
            var result = VeteranPriv.Clone();

            if (IsKilling)
                result.Chance = 100;

            return result;
        }
    }

    [LayersOption(MultiMenu.Layer, "#FFFF00FF", LayerEnum.Vigilante)]
    private static RoleOptionData VigilantePriv { get; set; }
    public static RoleOptionData Vigilante
    {
        get
        {
            var result = VigilantePriv.Clone();

            if (IsKilling)
                result.Chance = 100;

            return result;
        }
    }
}

[AlignsOption(MultiMenu.Layer, Alignment.CrewProt)]
public static class CrewProtectiveRoles
{
    [LayersOption(MultiMenu.Layer, "#660000FF", LayerEnum.Altruist)]
    public static RoleOptionData Altruist { get; set; }

    [LayersOption(MultiMenu.Layer, "#006600FF", LayerEnum.Medic)]
    public static RoleOptionData Medic { get; set; }

    [LayersOption(MultiMenu.Layer, "#BE1C8CFF", LayerEnum.Trapper)]
    public static RoleOptionData Trapper { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.CrewSov)]
public static class CrewSovereignRoles
{
    [LayersOption(MultiMenu.Layer, "#00CB97FF", LayerEnum.Dictator)]
    public static RoleOptionData Dictator { get; set; }

    [LayersOption(MultiMenu.Layer, "#704FA8FF", LayerEnum.Mayor)]
    public static RoleOptionData Mayor { get; set; }

    [LayersOption(MultiMenu.Layer, "#FF004EFF", LayerEnum.Monarch)]
    public static RoleOptionData Monarch { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.CrewSupport)]
public static class CrewSupportRoles
{
    [LayersOption(MultiMenu.Layer, "#5411F8FF", LayerEnum.Chameleon)]
    public static RoleOptionData Chameleon { get; set; }

    [LayersOption(MultiMenu.Layer, "#FFA60AFF", LayerEnum.Engineer)]
    public static RoleOptionData Engineer { get; set; }

    [LayersOption(MultiMenu.Layer, "#803333FF", LayerEnum.Escort)]
    public static RoleOptionData Escort { get; set; }

    [LayersOption(MultiMenu.Layer, "#8D0F8CFF", LayerEnum.Retributionist)]
    public static RoleOptionData Retributionist { get; set; }

    [LayersOption(MultiMenu.Layer, "#DF851FFF", LayerEnum.Shifter)]
    public static RoleOptionData Shifter { get; set; }

    [LayersOption(MultiMenu.Layer, "#00EEFFFF", LayerEnum.Transporter)]
    public static RoleOptionData Transporter { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.CrewUtil)]
public static class CrewUtilityRoles
{
    [LayersOption(MultiMenu.Layer, "#8CFFFFFF", LayerEnum.Crewmate)]
    private static RoleOptionData CrewmatePriv { get; set; }
    public static RoleOptionData Crewmate
    {
        get
        {
            var result = CrewmatePriv.Clone();

            if (!IsCustom)
                result.Chance = 100;

            return result;
        }
    }

    [LayersOption(MultiMenu.Layer, "#D3D3D3FF", LayerEnum.Revealer)]
    private static RoleOptionData RevealerPriv { get; set; }
    public static RoleOptionData Revealer
    {
        get
        {
            var result = RevealerPriv.Clone();

            if (IsRoleList)
                result.Count = 1;

            return result;
        }
    }
}

[AlignsOption(MultiMenu.Layer, Alignment.NeutralBen)]
public static class NeutralBenignRoles
{
    [LayersOption(MultiMenu.Layer, "#22FFFFFF", LayerEnum.Amnesiac)]
    public static RoleOptionData Amnesiac { get; set; }

    [LayersOption(MultiMenu.Layer, "#FFFFFFFF", LayerEnum.GuardianAngel)]
    public static RoleOptionData GuardianAngel { get; set; }

    [LayersOption(MultiMenu.Layer, "#DDDD00FF", LayerEnum.Survivor)]
    public static RoleOptionData Survivor { get; set; }

    [LayersOption(MultiMenu.Layer, "#80FF00FF", LayerEnum.Thief)]
    public static RoleOptionData Thief { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.NeutralEvil)]
public static class NeutralEvilRoles
{
    [LayersOption(MultiMenu.Layer, "#00ACC2FF", LayerEnum.Actor)]
    public static RoleOptionData Actor { get; set; }

    [LayersOption(MultiMenu.Layer, "#B51E39FF", LayerEnum.BountyHunter)]
    public static RoleOptionData BountyHunter { get; set; }

    [LayersOption(MultiMenu.Layer, "#8C4005FF", LayerEnum.Cannibal)]
    public static RoleOptionData Cannibal { get; set; }

    [LayersOption(MultiMenu.Layer, "#CCCCCCFF", LayerEnum.Executioner)]
    public static RoleOptionData Executioner { get; set; }

    [LayersOption(MultiMenu.Layer, "#EEE5BEFF", LayerEnum.Guesser)]
    public static RoleOptionData Guesser { get; set; }

    [LayersOption(MultiMenu.Layer, "#F7B3DAFF", LayerEnum.Jester)]
    public static RoleOptionData Jester { get; set; }

    [LayersOption(MultiMenu.Layer, "#678D36FF", LayerEnum.Troll)]
    public static RoleOptionData Troll { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.NeutralHarb)]
public static class NeutralHarbingerRoles
{
    [LayersOption(MultiMenu.Layer, "#CFFE61FF", LayerEnum.Plaguebearer)]
    public static RoleOptionData Plaguebearer { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.NeutralKill)]
public static class NeutralKillingRoles
{
    [LayersOption(MultiMenu.Layer, "#EE7600FF", LayerEnum.Arsonist)]
    public static RoleOptionData Arsonist { get; set; }

    [LayersOption(MultiMenu.Layer, "#642DEAFF", LayerEnum.Cryomaniac)]
    public static RoleOptionData Cryomaniac { get; set; }

    [LayersOption(MultiMenu.Layer, "#00FF00FF", LayerEnum.Glitch)]
    public static RoleOptionData Glitch { get; set; }

    [LayersOption(MultiMenu.Layer, "#A12B56FF", LayerEnum.Juggernaut)]
    public static RoleOptionData Juggernaut { get; set; }

    [LayersOption(MultiMenu.Layer, "#6F7BEAFF", LayerEnum.Murderer)]
    private static RoleOptionData MurdererPriv { get; set; }
    public static RoleOptionData Murderer
    {
        get
        {
            var result = MurdererPriv.Clone();

            if (IsKilling)
                result.Chance = 5;

            return result;
        }
    }

    [LayersOption(MultiMenu.Layer, "#336EFFFF", LayerEnum.SerialKiller)]
    public static RoleOptionData SerialKiller { get; set; }

    [LayersOption(MultiMenu.Layer, "#9F703AFF", LayerEnum.Werewolf)]
    public static RoleOptionData Werewolf { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.NeutralNeo)]
public static class NeutralNeophyteRoles
{
    [LayersOption(MultiMenu.Layer, "#AC8A00FF", LayerEnum.Dracula)]
    public static RoleOptionData Dracula { get; set; }

    [LayersOption(MultiMenu.Layer, "#45076AFF", LayerEnum.Jackal)]
    public static RoleOptionData Jackal { get; set; }

    [LayersOption(MultiMenu.Layer, "#BF5FFFFF", LayerEnum.Necromancer)]
    public static RoleOptionData Necromancer { get; set; }

    [LayersOption(MultiMenu.Layer, "#2D6AA5FF", LayerEnum.Whisperer)]
    public static RoleOptionData Whisperer { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.NeutralPros)]
public static class NeutralProselyteRoles
{
    [LayersOption(MultiMenu.Layer, "#662962FF", LayerEnum.Phantom)]
    private static RoleOptionData PhantomPriv { get; set; }
    public static RoleOptionData Phantom
    {
        get
        {
            var result = PhantomPriv.Clone();

            if (IsRoleList)
                result.Count = 1;

            return result;
        }
    }
}

[AlignsOption(MultiMenu.Layer, Alignment.IntruderConceal)]
public static class IntruderConcealingRoles
{
    [LayersOption(MultiMenu.Layer, "#02A752FF", LayerEnum.Blackmailer)]
    public static RoleOptionData Blackmailer { get; set; }

    [LayersOption(MultiMenu.Layer, "#378AC0FF", LayerEnum.Camouflager)]
    public static RoleOptionData Camouflager { get; set; }

    [LayersOption(MultiMenu.Layer, "#85AA5BFF", LayerEnum.Grenadier)]
    public static RoleOptionData Grenadier { get; set; }

    [LayersOption(MultiMenu.Layer, "#2647A2FF", LayerEnum.Janitor)]
    public static RoleOptionData Janitor { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.IntruderDecep)]
public static class IntruderDeceptionRoles
{
    [LayersOption(MultiMenu.Layer, "#40B4FFFF", LayerEnum.Disguiser)]
    public static RoleOptionData Disguiser { get; set; }

    [LayersOption(MultiMenu.Layer, "#BB45B0FF", LayerEnum.Morphling)]
    public static RoleOptionData Morphling { get; set; }

    [LayersOption(MultiMenu.Layer, "#5C4F75FF", LayerEnum.Wraith)]
    public static RoleOptionData Wraith { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.IntruderHead)]
public static class IntruderHeadRoles
{
    [LayersOption(MultiMenu.Layer, "#404C08FF", LayerEnum.Godfather)]
    public static RoleOptionData Godfather { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.IntruderKill)]
public static class IntruderKillingRoles
{
    [LayersOption(MultiMenu.Layer, "#2BD29CFF", LayerEnum.Ambusher)]
    public static RoleOptionData Ambusher { get; set; }

    [LayersOption(MultiMenu.Layer, "#005643FF", LayerEnum.Enforcer)]
    public static RoleOptionData Enforcer { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.IntruderSupport)]
public static class IntruderSupportRoles
{
    [LayersOption(MultiMenu.Layer, "#FFFF99FF", LayerEnum.Consigliere)]
    public static RoleOptionData Consigliere { get; set; }

    [LayersOption(MultiMenu.Layer, "#801780FF", LayerEnum.Consort)]
    public static RoleOptionData Consort { get; set; }

    [LayersOption(MultiMenu.Layer, "#AA7632FF", LayerEnum.Miner)]
    public static RoleOptionData Miner { get; set; }

    [LayersOption(MultiMenu.Layer, "#939593FF", LayerEnum.Teleporter)]
    public static RoleOptionData Teleporter { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.IntruderUtil)]
public static class IntruderUtilityRoles
{
    [LayersOption(MultiMenu.Layer, "#F1C40FFF", LayerEnum.Ghoul)]
    private static RoleOptionData GhoulPriv { get; set; }
    public static RoleOptionData Ghoul
    {
        get
        {
            var result = GhoulPriv.Clone();

            if (IsRoleList)
                result.Count = 1;

            return result;
        }
    }

    [LayersOption(MultiMenu.Layer, "#FF1919FF", LayerEnum.Impostor)]
    private static RoleOptionData ImpostorPriv { get; set; }
    public static RoleOptionData Impostor
    {
        get
        {
            var result = ImpostorPriv.Clone();

            if (IsKilling)
                result.Chance = 5;
            else if (!IsCustom)
                result.Chance = 100;

            return result;
        }
    }
}

[AlignsOption(MultiMenu.Layer, Alignment.SyndicateDisrup)]
public static class SyndicateDisruptionRoles
{
    [LayersOption(MultiMenu.Layer, "#C02525FF", LayerEnum.Concealer)]
    public static RoleOptionData Concealer { get; set; }

    [LayersOption(MultiMenu.Layer, "#FF7900FF", LayerEnum.Drunkard)]
    public static RoleOptionData Drunkard { get; set; }

    [LayersOption(MultiMenu.Layer, "#00FFFFFF", LayerEnum.Framer)]
    public static RoleOptionData Framer { get; set; }

    [LayersOption(MultiMenu.Layer, "#2DFF00FF", LayerEnum.Shapeshifter)]
    public static RoleOptionData Shapeshifter { get; set; }

    [LayersOption(MultiMenu.Layer, "#AAB43EFF", LayerEnum.Silencer)]
    public static RoleOptionData Silencer { get; set; }

    [LayersOption(MultiMenu.Layer, "#3769FEFF", LayerEnum.Timekeeper)]
    public static RoleOptionData Timekeeper { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.SyndicateKill)]
public static class SyndicateKillingRoles
{
    [LayersOption(MultiMenu.Layer, "#C9CC3FFF", LayerEnum.Bomber)]
    public static RoleOptionData Bomber { get; set; }

    [LayersOption(MultiMenu.Layer, "#B345FFFF", LayerEnum.Collider)]
    public static RoleOptionData Collider { get; set; }

    [LayersOption(MultiMenu.Layer, "#DF7AE8FF", LayerEnum.Crusader)]
    public static RoleOptionData Crusader { get; set; }

    [LayersOption(MultiMenu.Layer, "#B5004CFF", LayerEnum.Poisoner)]
    public static RoleOptionData Poisoner { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.SyndicatePower)]
public static class SyndicatePowerRoles
{
    [LayersOption(MultiMenu.Layer, "#FFFCCEFF", LayerEnum.Rebel)]
    public static RoleOptionData Rebel { get; set; }

    [LayersOption(MultiMenu.Layer, "#0028F5FF", LayerEnum.Spellslinger)]
    public static RoleOptionData Spellslinger { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.SyndicateSupport)]
public static class SyndicateSupportRoles
{
    [LayersOption(MultiMenu.Layer, "#7E4D00FF", LayerEnum.Stalker)]
    public static RoleOptionData Stalker { get; set; }

    [LayersOption(MultiMenu.Layer, "#8C7140FF", LayerEnum.Warper)]
    public static RoleOptionData Warper { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.SyndicateUtil)]
public static class SyndicateUtilityRoles
{
    [LayersOption(MultiMenu.Layer, "#008000FF", LayerEnum.Anarchist)]
    private static RoleOptionData AnarchistPriv { get; set; }
    public static RoleOptionData Anarchist
    {
        get
        {
            var result = AnarchistPriv.Clone();

            if (IsKilling)
                result.Chance = 5;
            else if (!IsCustom)
                result.Chance = 100;

            return result;
        }
    }

    [LayersOption(MultiMenu.Layer, "#E67E22FF", LayerEnum.Banshee)]
    private static RoleOptionData BansheePriv { get; set; }
    public static RoleOptionData Banshee
    {
        get
        {
            var result = BansheePriv.Clone();

            if (IsRoleList)
                result.Count = 1;

            return result;
        }
    }
}

[AlignsOption(MultiMenu.Layer, Alignment.Modifier)]
public static class Modifiers
{
    [LayersOption(MultiMenu.Layer, "#612BEFFF", LayerEnum.Astral)]
    public static RoleOptionData Astral { get; set; }

    [LayersOption(MultiMenu.Layer, "#00B3B3FF", LayerEnum.Bait)]
    public static RoleOptionData Bait { get; set; }

    [LayersOption(MultiMenu.Layer, "#B34D99FF", LayerEnum.Colorblind)]
    public static RoleOptionData Colorblind { get; set; }

    [LayersOption(MultiMenu.Layer, "#456BA8FF", LayerEnum.Coward)]
    public static RoleOptionData Coward { get; set; }

    [LayersOption(MultiMenu.Layer, "#374D1EFF", LayerEnum.Diseased)]
    public static RoleOptionData Diseased { get; set; }

    [LayersOption(MultiMenu.Layer, "#758000FF", LayerEnum.Drunk)]
    public static RoleOptionData Drunk { get; set; }

    [LayersOption(MultiMenu.Layer, "#FF8080FF", LayerEnum.Dwarf)]
    public static RoleOptionData Dwarf { get; set; }

    [LayersOption(MultiMenu.Layer, "#FFB34DFF", LayerEnum.Giant)]
    public static RoleOptionData Giant { get; set; }

    [LayersOption(MultiMenu.Layer, "#2DE5BEFF", LayerEnum.Indomitable)]
    public static RoleOptionData Indomitable { get; set; }

    [LayersOption(MultiMenu.Layer, "#860B7AFF", LayerEnum.Professional)]
    public static RoleOptionData Professional { get; set; }

    [LayersOption(MultiMenu.Layer, "#1002C5FF", LayerEnum.Shy)]
    public static RoleOptionData Shy { get; set; }

    [LayersOption(MultiMenu.Layer, "#DCEE85FF", LayerEnum.VIP)]
    public static RoleOptionData VIP { get; set; }

    [LayersOption(MultiMenu.Layer, "#FFA60AFF", LayerEnum.Volatile)]
    public static RoleOptionData Volatile { get; set; }

    [LayersOption(MultiMenu.Layer, "#F6AAB7FF", LayerEnum.Yeller)]
    public static RoleOptionData Yeller { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.Ability)]
public static class Abilities
{
    [LayersOption(MultiMenu.Layer, "#8CFFFFFF", LayerEnum.Bullseye)]
    public static RoleOptionData Bullseye { get; set; }

    [LayersOption(MultiMenu.Layer, "#E600FFFF", LayerEnum.ButtonBarry)]
    public static RoleOptionData ButtonBarry { get; set; }

    [LayersOption(MultiMenu.Layer, "#FF1919FF", LayerEnum.Hitman)]
    public static RoleOptionData Hitman { get; set; }

    [LayersOption(MultiMenu.Layer, "#26FCFBFF", LayerEnum.Insider)]
    public static RoleOptionData Insider { get; set; }

    [LayersOption(MultiMenu.Layer, "#FF804DFF", LayerEnum.Multitasker)]
    public static RoleOptionData Multitasker { get; set; }

    [LayersOption(MultiMenu.Layer, "#A84300FF", LayerEnum.Ninja)]
    public static RoleOptionData Ninja { get; set; }

    [LayersOption(MultiMenu.Layer, "#CCA3CCFF", LayerEnum.Politician)]
    public static RoleOptionData Politician { get; set; }

    [LayersOption(MultiMenu.Layer, "#FF0080FF", LayerEnum.Radar)]
    public static RoleOptionData Radar { get; set; }

    [LayersOption(MultiMenu.Layer, "#2160DDFF", LayerEnum.Ruthless)]
    public static RoleOptionData Ruthless { get; set; }

    [LayersOption(MultiMenu.Layer, "#B3B3B3FF", LayerEnum.Slayer)]
    public static RoleOptionData Slayer { get; set; }

    [LayersOption(MultiMenu.Layer, "#008000FF", LayerEnum.Sniper)]
    public static RoleOptionData Sniper { get; set; }

    [LayersOption(MultiMenu.Layer, "#D4AF37FF", LayerEnum.Snitch)]
    public static RoleOptionData Snitch { get; set; }

    [LayersOption(MultiMenu.Layer, "#66E666FF", LayerEnum.Swapper)]
    public static RoleOptionData Swapper { get; set; }

    [LayersOption(MultiMenu.Layer, "#99E699FF", LayerEnum.Tiebreaker)]
    public static RoleOptionData Tiebreaker { get; set; }

    [LayersOption(MultiMenu.Layer, "#FFFF99FF", LayerEnum.Torch)]
    public static RoleOptionData Torch { get; set; }

    [LayersOption(MultiMenu.Layer, "#E91E63FF", LayerEnum.Tunneler)]
    public static RoleOptionData Tunneler { get; set; }

    [LayersOption(MultiMenu.Layer, "#841A7FFF", LayerEnum.Underdog)]
    public static RoleOptionData Underdog { get; set; }
}

[AlignsOption(MultiMenu.Layer, Alignment.Objectifier)]
public static class Objectifiers
{
    [LayersOption(MultiMenu.Layer, "#4545A9FF", LayerEnum.Allied)]
    public static RoleOptionData Allied { get; set; }

    [LayersOption(MultiMenu.Layer, "#4545FFFF", LayerEnum.Corrupted)]
    public static RoleOptionData Corrupted { get; set; }

    [LayersOption(MultiMenu.Layer, "#E1C849FF", LayerEnum.Defector)]
    public static RoleOptionData Defector { get; set; }

    [LayersOption(MultiMenu.Layer, "#678D36FF", LayerEnum.Fanatic)]
    public static RoleOptionData Fanatic { get; set; }

    [LayersOption(MultiMenu.Layer, "#FF351FFF", LayerEnum.Linked, Min = 1, Max = 7)]
    public static RoleOptionData Linked { get; set; }

    [LayersOption(MultiMenu.Layer, "#FF66CCFF", LayerEnum.Lovers, Min = 1, Max = 7)]
    public static RoleOptionData Lovers { get; set; }

    [LayersOption(MultiMenu.Layer, "#00EEFFFF", LayerEnum.Mafia, Min = 2)]
    public static RoleOptionData Mafia { get; set; }

    [LayersOption(MultiMenu.Layer, "#008080FF", LayerEnum.Overlord)]
    public static RoleOptionData Overlord { get; set; }

    [LayersOption(MultiMenu.Layer, "#3D2D2CFF", LayerEnum.Rivals, Min = 1, Max = 7)]
    public static RoleOptionData Rivals { get; set; }

    [LayersOption(MultiMenu.Layer, "#ABABFFFF", LayerEnum.Taskmaster)]
    public static RoleOptionData Taskmaster { get; set; }

    [LayersOption(MultiMenu.Layer, "#370D43FF", LayerEnum.Traitor)]
    public static RoleOptionData Traitor { get; set; }
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class CrewAuditorSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxCA { get; set; } = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class CrewInvestigativeSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxCI { get; set; } = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class CrewKillingSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxCK { get; set; } = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class CrewProtectiveSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxCrP { get; set; } = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class CrewSovereignSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxCsV { get; set; } = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class CrewSupportSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxCS { get; set; } = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class NeutralBenignSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxNB { get; set; } = 1;

    [ToggleOption(MultiMenu.AlignmentSubOptions)]
    public static bool VigilanteKillsBenigns { get; set; } = true;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class NeutralEvilSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxNE { get; set; } = 1;

    [ToggleOption(MultiMenu.AlignmentSubOptions)]
    public static bool NeutralEvilsEndGame { get; set; } = false;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class NeutralHarbingerSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxNE { get; set; } = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class NeutralKillingSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxNK { get; set; } = 1;

    [ToggleOption(MultiMenu.AlignmentSubOptions)]
    public static bool HasImpVision { get; set; } = true;

    [ToggleOption(MultiMenu.AlignmentSubOptions)]
    public static bool KnowEachOther { get; set; } = false;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class NeutralNeophyteSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxNN { get; set; } = 1;

    [ToggleOption(MultiMenu.AlignmentSubOptions)]
    public static bool HasImpVision { get; set; } = true;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class IntruderConcealingSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxIC { get; set; } = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class IntruderDeceptionSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxID { get; set; } = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class IntruderHeadSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxIH { get; set; } = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class IntruderKillingSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxIK { get; set; } = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class IntruderSupportSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxIS { get; set; } = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class SyndicateDisruptionSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxSD { get; set; } = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class SyndicateKillingSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxSyK { get; set; } = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class SyndicatePowerSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxSP { get; set; } = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class SyndicateSupportSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxSSu { get; set; } = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class ModifiersSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxModifiers { get; set; } = 5;

    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MinModifiers { get; set; } = 5;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class AbilitiesSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxAbilities { get; set; } = 5;

    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MinAbilities { get; set; } = 5;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class ObjectifiersSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MaxObjectifiers { get; set; } = 5;

    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static int MinObjectifiers { get; set; } = 5;
}