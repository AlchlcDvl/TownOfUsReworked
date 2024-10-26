namespace TownOfUsReworked.Options;

// DO NOT OVERRIDE VALUES OF ANY OF THE OPTION PROPERTIES ANY WHERE IN THE CODE OR ELSE THE OPTIONS WILL START TO FUCK OFF

[HeaderOption(MultiMenu.Main)]
public static class GameSettings
{
    [NumberOption(MultiMenu.Main, 0.25f, 10, 0.25f, Format.Multiplier)]
    public static Number PlayerSpeed { get; set; } = new(1.25f);

    [NumberOption(MultiMenu.Main, 0.25f, 10, 0.25f, Format.Multiplier)]
    public static Number GhostSpeed { get; set; } = new(3);

    [NumberOption(MultiMenu.Main, 0.5f, 5, 0.5f, Format.Distance)]
    public static Number InteractionDistance { get; set; } = new(2);

    [NumberOption(MultiMenu.Main, 0, 100, 1)]
    public static Number EmergencyButtonCount { get; set; } = new(1);

    [NumberOption(MultiMenu.Main, 0, 300, 2.5f, Format.Time)]
    public static Number EmergencyButtonCooldown { get; set; } = new(25);

    [NumberOption(MultiMenu.Main, 0, 300, 5, Format.Time)]
    public static Number DiscussionTime { get; set; } = new(30);

    [NumberOption(MultiMenu.Main, 5, 600, 15, Format.Time)]
    public static Number VotingTime { get; set; } = new(60);

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
    public static Number InitialCooldowns { get; set; } = new(10);

    [ToggleOption(MultiMenu.Main)]
    public static bool EnableMeetingCds { get; set; } = true;

    [NumberOption(MultiMenu.Main, 0, 30, 2.5f, Format.Time)]
    public static Number MeetingCooldowns { get; set; } = new(15);

    [ToggleOption(MultiMenu.Main)]
    public static bool EnableFailCds { get; set; } = true;

    [NumberOption(MultiMenu.Main, 0, 30, 2.5f, Format.Time)]
    public static Number FailCooldowns { get; set; } = new(5);

    [NumberOption(MultiMenu.Main, 1, 20, 0.25f, Format.Distance)]
    public static Number ReportDistance { get; set; } = new(3.5f);

    [NumberOption(MultiMenu.Main, 0, 3, 0.1f, Format.Time)]
    public static Number ChatCooldown { get; set; } = new(3);

    [NumberOption(MultiMenu.Main, 0, 2000, 50, ZeroIsInfinity = true)]
    public static Number ChatCharacterLimit { get; set; } = new(200);

    [NumberOption(MultiMenu.Main, 2, 127, 1)]
    public static Number LobbySize { get; set; } = new(15);
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
    public static Number NeutralsCount { get; set; } = new(1);

    [ToggleOption(MultiMenu.Main)]
    public static bool AddArsonist { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool AddCryomaniac { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool AddPlaguebearer { get; set; } = false;

    [StringOption(MultiMenu.Main)]
    public static HnSMode HnSMode { get; set; } = HnSMode.Classic;

    [NumberOption(MultiMenu.Main, 0, 13, 1)]
    public static Number HnSShortTasks { get; set; } = new(4);

    [NumberOption(MultiMenu.Main, 0, 13, 1)]
    public static Number HnSCommonTasks { get; set; } = new(4);

    [NumberOption(MultiMenu.Main, 0, 13, 1)]
    public static Number HnSLongTasks { get; set; } = new(4);

    [NumberOption(MultiMenu.Main, 1, 13, 1)]
    public static Number HunterCount { get; set; } = new(1);

    [NumberOption(MultiMenu.Main, 5f, 60f, 5f, Format.Time)]
    public static Number HuntCd { get; set; } = new(10);

    [NumberOption(MultiMenu.Main, 5f, 60f, 5f, Format.Time)]
    public static Number StartTime { get; set; } = new(10);

    [ToggleOption(MultiMenu.Main)]
    public static bool HunterVent { get; set; } = true;

    [NumberOption(MultiMenu.Main, 0.1f, 1f, 0.05f, Format.Multiplier)]
    public static Number HunterVision { get; set;} = new(0.25f);

    [NumberOption(MultiMenu.Main, 1f, 2f, 0.05f, Format.Multiplier)]
    public static Number HuntedVision { get; set;} = new(1.5f);

    [NumberOption(MultiMenu.Main, 1f, 1.5f, 0.05f, Format.Multiplier)]
    public static Number HunterSpeedModifier { get; set; } = new(1.25f);

    [ToggleOption(MultiMenu.Main)]
    public static bool HunterFlashlight { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool HuntedFlashlight { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool HuntedChat { get; set; } = true;

    [NumberOption(MultiMenu.Main, 0, 13, 1)]
    public static Number TRShortTasks { get; set; } = new(4);

    [NumberOption(MultiMenu.Main, 0, 13, 1)]
    public static Number TRCommonTasks { get; set; } = new(4);

    [NumberOption(MultiMenu.Main, 0, 13, 1)]
    public static Number TRLongTasks { get; set; } = new(1);
}

[HeaderOption(MultiMenu.Main)]
public static class RoleListEntries
{
    [RoleListEntry]
    public static LayerEnum Entry1 { get; set; } = LayerEnum.None;

    [RoleListEntry]
    public static LayerEnum Entry2 { get; set; } = LayerEnum.None;

    [RoleListEntry]
    public static LayerEnum Entry3 { get; set; } = LayerEnum.None;

    [RoleListEntry]
    public static LayerEnum Entry4 { get; set; } = LayerEnum.None;

    [RoleListEntry]
    public static LayerEnum Entry5 { get; set; } = LayerEnum.None;

    [RoleListEntry]
    public static LayerEnum Entry6 { get; set; } = LayerEnum.None;

    [RoleListEntry]
    public static LayerEnum Entry7 { get; set; } = LayerEnum.None;

    [RoleListEntry]
    public static LayerEnum Entry8 { get; set; } = LayerEnum.None;

    [RoleListEntry]
    public static LayerEnum Entry9 { get; set; } = LayerEnum.None;

    [RoleListEntry]
    public static LayerEnum Entry10 { get; set; } = LayerEnum.None;

    [RoleListEntry]
    public static LayerEnum Entry11 { get; set; } = LayerEnum.None;

    [RoleListEntry]
    public static LayerEnum Entry12 { get; set; } = LayerEnum.None;

    [RoleListEntry]
    public static LayerEnum Entry13 { get; set; } = LayerEnum.None;

    [RoleListEntry]
    public static LayerEnum Entry14 { get; set; } = LayerEnum.None;

    [RoleListEntry]
    public static LayerEnum Entry15 { get; set; } = LayerEnum.None;

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
public static class RoleListBans
{
    [RoleListEntry]
    public static LayerEnum Ban1 { get; set; } = LayerEnum.None;

    [RoleListEntry]
    public static LayerEnum Ban2 { get; set; } = LayerEnum.None;

    [RoleListEntry]
    public static LayerEnum Ban3 { get; set; } = LayerEnum.None;

    [RoleListEntry]
    public static LayerEnum Ban4 { get; set; } = LayerEnum.None;

    [RoleListEntry]
    public static LayerEnum Ban5 { get; set; } = LayerEnum.None;

    [ToggleOption(MultiMenu.Main)]
    public static bool BanCrewmate { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool BanMurderer { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool BanImpostor { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool BanAnarchist { get; set; } = true;
}

[HeaderOption(MultiMenu.Main)]
public static class GameModifiers
{
    [StringOption(MultiMenu.Main)]
    public static WhoCanVentOptions WhoCanVent { get; set; } = WhoCanVentOptions.Default;

    [StringOption(MultiMenu.Main)]
    public static AnonVotes AnonymousVoting { get; set; } = AnonVotes.Enabled;

    [StringOption(MultiMenu.Main)]
    public static DisableSkipButtonMeetings NoSkipping { get; set; } = DisableSkipButtonMeetings.Never;

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
    public static bool EnableDispositions { get; set; } = true;

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

    [ToggleOption(MultiMenu.Main)]
    public static bool ShowKillerRoleColor { get; set; } = false;
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
    public static Number RandomMapSkeld { get; set; } = new(10);

    [NumberOption(MultiMenu.Main, 0, 100, 10, Format.Percent)]
    public static Number RandomMapMira { get; set; } = new(10);

    [NumberOption(MultiMenu.Main, 0, 100, 10, Format.Percent)]
    public static Number RandomMapPolus { get; set; } = new(10);

    [NumberOption(MultiMenu.Main, 0, 100, 10, Format.Percent)]
    public static Number RandomMapdlekS { get; set; } = new(10);

    [NumberOption(MultiMenu.Main, 0, 100, 10, Format.Percent)]
    public static Number RandomMapAirship { get; set; } = new(10);

    [NumberOption(MultiMenu.Main, 0, 100, 10, Format.Percent)]
    public static Number RandomMapFungle { get; set; } = new(10);

    [NumberOption(MultiMenu.Main, 0, 100, 10, Format.Percent, All = true)]
    public static Number RandomMapSubmerged { get; set; } = new(10);

    [NumberOption(MultiMenu.Main, 0, 100, 10, Format.Percent, All = true)]
    public static Number RandomMapLevelImpostor { get; set; } = new(10);

    [ToggleOption(MultiMenu.Main)]
    public static bool AutoAdjustSettings { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool SmallMapHalfVision { get; set; } = false;

    [NumberOption(MultiMenu.Main, 0f, 15f, 2.5f, Format.Time)]
    public static Number SmallMapDecreasedCooldown { get; set; } = new(0);

    [NumberOption(MultiMenu.Main, 0f, 15f, 2.5f, Format.Time)]
    public static Number LargeMapIncreasedCooldown { get; set; } = new(0);

    [NumberOption(MultiMenu.Main, 0, 5, 1)]
    public static Number SmallMapIncreasedShortTasks { get; set; } = new(0);

    [NumberOption(MultiMenu.Main, 0, 3, 1)]
    public static Number SmallMapIncreasedLongTasks { get; set; } = new(0);

    [NumberOption(MultiMenu.Main, 0, 5, 1)]
    public static Number LargeMapDecreasedShortTasks { get; set; } = new(0);

    [NumberOption(MultiMenu.Main, 0, 3, 1)]
    public static Number LargeMapDecreasedLongTasks { get; set; } = new(0);
}

[HeaderOption(MultiMenu.Main)]
public static class CrewSettings
{
    [NumberOption(MultiMenu.Main, 0, 100, 1)]
    public static Number CommonTasks { get; set; } = new(2);

    [NumberOption(MultiMenu.Main, 0, 100, 1)]
    public static Number LongTasks { get; set; } = new(1);

    [NumberOption(MultiMenu.Main, 0, 100, 1)]
    public static Number ShortTasks { get; set; } = new(4);

    [ToggleOption(MultiMenu.Main)]
    public static bool GhostTasksCountToWin { get; set; } = true;

    [NumberOption(MultiMenu.Main, 0.25f, 5f, 0.25f, Format.Multiplier)]
    public static Number CrewVision { get; set; } = new(1);

    [ToggleOption(MultiMenu.Main)]
    public static bool CrewFlashlight { get; set; } = false;

    [NumberOption(MultiMenu.Main, 0, 14, 1)]
    public static Number CrewMax { get; set; } = new(5);

    [NumberOption(MultiMenu.Main, 0, 14, 1)]
    public static Number CrewMin { get; set; } = new(5);

    [StringOption(MultiMenu.Main)]
    public static CrewVenting CrewVent { get; set; } = CrewVenting.Never;
}

[HeaderOption(MultiMenu.Main)]
public static class NeutralSettings
{
    [NumberOption(MultiMenu.Main, 0.25f, 5f, 0.25f, Format.Multiplier)]
    public static Number NeutralVision { get; set; } = new(1.5f);

    [ToggleOption(MultiMenu.Main)]
    public static bool LightsAffectNeutrals { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool NeutralFlashlight { get; set; } = false;

    [NumberOption(MultiMenu.Main, 1, 14, 1)]
    public static Number NeutralMax { get; set; } = new(3);

    [NumberOption(MultiMenu.Main, 1, 14, 1)]
    public static Number NeutralMin { get; set; } = new(3);

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
    public static Number IntruderCount { get; set; } = new(1);

    [NumberOption(MultiMenu.Main, 0.25f, 5f, 0.25f, Format.Multiplier)]
    public static Number IntruderVision { get; set; } = new(2);

    [ToggleOption(MultiMenu.Main)]
    public static bool IntruderFlashlight { get; set; } = false;

    [NumberOption(MultiMenu.Main, 10f, 60f, 2.5f, Format.Time)]
    public static Number IntKillCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.Main)]
    public static bool IntrudersVent { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool IntrudersCanSabotage { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool GhostsCanSabotage { get; set; } = false;

    [NumberOption(MultiMenu.Main, 1, 14, 1)]
    public static Number IntruderMax { get; set; } = new(1);

    [NumberOption(MultiMenu.Main, 1, 14, 1)]
    public static Number IntruderMin { get; set; } = new(1);
}

[HeaderOption(MultiMenu.Main)]
public static class SyndicateSettings
{
    [NumberOption(MultiMenu.Main, 0, 4, 1)]
    public static Number SyndicateCount { get; set; } = new(1);

    [NumberOption(MultiMenu.Main, 0.25f, 5f, 0.25f, Format.Multiplier)]
    public static Number SyndicateVision { get; set; } = new(2);

    [ToggleOption(MultiMenu.Main)]
    public static bool SyndicateFlashlight { get; set; } = false;

    [NumberOption(MultiMenu.Main, 1, 10, 1)]
    public static Number ChaosDriveMeetingCount { get; set; } = new(3);

    [NumberOption(MultiMenu.Main, 10f, 60f, 2.5f, Format.Time)]
    public static Number CDKillCd { get; set; } = new(25);

    [StringOption(MultiMenu.Main)]
    public static SyndicateVentOptions SyndicateVent { get; set; } = SyndicateVentOptions.Always;

    [ToggleOption(MultiMenu.Main)]
    private static bool AltImpsPriv { get; set; } = false;
    public static bool AltImps => AltImpsPriv || IntruderSettings.IntruderCount == 0;

    [ToggleOption(MultiMenu.Main)]
    public static bool GlobalDrive { get; set; } = false;

    [NumberOption(MultiMenu.Main, 1, 14, 1)]
    public static Number SyndicateMax { get; set; } = new(1);

    [NumberOption(MultiMenu.Main, 1, 14, 1)]
    public static Number SyndicateMin { get; set; } = new(1);
}

public static class TaskSettings
{
    public static Number ShortTasks => GameModeSettings.GameMode switch
    {
        GameMode.TaskRace => GameModeSettings.TRShortTasks,
        GameMode.HideAndSeek => GameModeSettings.HnSShortTasks,
        _ => CrewSettings.ShortTasks
    };
    public static Number LongTasks => GameModeSettings.GameMode switch
    {
        GameMode.TaskRace => GameModeSettings.TRLongTasks,
        GameMode.HideAndSeek => GameModeSettings.HnSLongTasks,
        _ => CrewSettings.LongTasks
    };
    public static Number CommonTasks => GameModeSettings.GameMode switch
    {
        GameMode.TaskRace => GameModeSettings.TRCommonTasks,
        GameMode.HideAndSeek => GameModeSettings.HnSCommonTasks,
        _ => CrewSettings.CommonTasks
    };
}

[AlignsOption(LayerEnum.CrewAudit)]
public static class CrewAuditorRoles
{
    [LayersOption("#708EEFFF", LayerEnum.Mystic)]
    public static RoleOptionData Mystic { get; set; }

    [LayersOption("#C0C0C0FF", LayerEnum.VampireHunter)]
    public static RoleOptionData VampireHunter { get; set; }
}

[AlignsOption(LayerEnum.CrewInvest)]
public static class CrewInvestigativeRoles
{
    [LayersOption("#4D99E6FF", LayerEnum.Coroner)]
    public static RoleOptionData Coroner { get; set; }

    [LayersOption("#4D4DFFFF", LayerEnum.Detective)]
    public static RoleOptionData Detective { get; set; }

    [LayersOption("#A680FFFF", LayerEnum.Medium)]
    public static RoleOptionData Medium { get; set; }

    [LayersOption("#A7D1B3FF", LayerEnum.Operative)]
    public static RoleOptionData Operative { get; set; }

    [LayersOption("#71368AFF", LayerEnum.Seer)]
    public static RoleOptionData Seer { get; set; }

    [LayersOption("#FFCC80FF", LayerEnum.Sheriff)]
    public static RoleOptionData Sheriff { get; set; }

    [LayersOption("#009900FF", LayerEnum.Tracker)]
    public static RoleOptionData Tracker { get; set; }
}

[AlignsOption(LayerEnum.CrewKill)]
public static class CrewKillingRoles
{
    [LayersOption("#7E3C64FF", LayerEnum.Bastion)]
    private static RoleOptionData BastionPriv { get; set; }
    public static RoleOptionData Bastion
    {
        get
        {
            var result = BastionPriv.Clone();

            if (IsKilling())
                result.Chance = 100;

            return result;
        }
    }

    [LayersOption("#998040FF", LayerEnum.Veteran)]
    private static RoleOptionData VeteranPriv { get; set; }
    public static RoleOptionData Veteran
    {
        get
        {
            var result = VeteranPriv.Clone();

            if (IsKilling())
                result.Chance = 100;

            return result;
        }
    }

    [LayersOption("#FFFF00FF", LayerEnum.Vigilante)]
    private static RoleOptionData VigilantePriv { get; set; }
    public static RoleOptionData Vigilante
    {
        get
        {
            var result = VigilantePriv.Clone();

            if (IsKilling())
                result.Chance = 100;

            return result;
        }
    }
}

[AlignsOption(LayerEnum.CrewProt)]
public static class CrewProtectiveRoles
{
    [LayersOption("#660000FF", LayerEnum.Altruist)]
    public static RoleOptionData Altruist { get; set; }

    [LayersOption("#006600FF", LayerEnum.Medic)]
    public static RoleOptionData Medic { get; set; }

    [LayersOption("#BE1C8CFF", LayerEnum.Trapper)]
    public static RoleOptionData Trapper { get; set; }
}

[AlignsOption(LayerEnum.CrewSov)]
public static class CrewSovereignRoles
{
    [LayersOption("#00CB97FF", LayerEnum.Dictator)]
    public static RoleOptionData Dictator { get; set; }

    [LayersOption("#704FA8FF", LayerEnum.Mayor)]
    public static RoleOptionData Mayor { get; set; }

    [LayersOption("#FF004EFF", LayerEnum.Monarch)]
    public static RoleOptionData Monarch { get; set; }
}

[AlignsOption(LayerEnum.CrewSupport)]
public static class CrewSupportRoles
{
    [LayersOption("#5411F8FF", LayerEnum.Chameleon)]
    public static RoleOptionData Chameleon { get; set; }

    [LayersOption("#FFA60AFF", LayerEnum.Engineer)]
    public static RoleOptionData Engineer { get; set; }

    [LayersOption("#803333FF", LayerEnum.Escort)]
    public static RoleOptionData Escort { get; set; }

    [LayersOption("#8D0F8CFF", LayerEnum.Retributionist)]
    public static RoleOptionData Retributionist { get; set; }

    [LayersOption("#DF851FFF", LayerEnum.Shifter)]
    public static RoleOptionData Shifter { get; set; }

    [LayersOption("#00EEFFFF", LayerEnum.Transporter)]
    public static RoleOptionData Transporter { get; set; }
}

[AlignsOption(LayerEnum.CrewUtil)]
public static class CrewUtilityRoles
{
    [LayersOption("#8CFFFFFF", LayerEnum.Crewmate)]
    private static RoleOptionData CrewmatePriv { get; set; }
    public static RoleOptionData Crewmate
    {
        get
        {
            var result = CrewmatePriv.Clone();

            if (!IsCustom())
                result.Chance = 100;

            return result;
        }
    }

    [LayersOption("#D3D3D3FF", LayerEnum.Revealer)]
    private static RoleOptionData RevealerPriv { get; set; }
    public static RoleOptionData Revealer
    {
        get
        {
            var result = RevealerPriv.Clone();

            if (IsRoleList())
                result.Count = 1;

            return result;
        }
    }
}

[AlignsOption(LayerEnum.NeutralApoc, true)]
public static class NeutralApocalypseRoles
{
    [LayersOption("#424242FF", LayerEnum.Pestilence, true)]
    public static RoleOptionData Pestilence { get; set; }
}

[AlignsOption(LayerEnum.NeutralBen)]
public static class NeutralBenignRoles
{
    [LayersOption("#22FFFFFF", LayerEnum.Amnesiac)]
    public static RoleOptionData Amnesiac { get; set; }

    [LayersOption("#FFFFFFFF", LayerEnum.GuardianAngel)]
    public static RoleOptionData GuardianAngel { get; set; }

    [LayersOption("#DDDD00FF", LayerEnum.Survivor)]
    public static RoleOptionData Survivor { get; set; }

    [LayersOption("#80FF00FF", LayerEnum.Thief)]
    public static RoleOptionData Thief { get; set; }
}

[AlignsOption(LayerEnum.NeutralEvil)]
public static class NeutralEvilRoles
{
    [LayersOption("#00ACC2FF", LayerEnum.Actor)]
    public static RoleOptionData Actor { get; set; }

    [LayersOption("#B51E39FF", LayerEnum.BountyHunter)]
    public static RoleOptionData BountyHunter { get; set; }

    [LayersOption("#8C4005FF", LayerEnum.Cannibal)]
    public static RoleOptionData Cannibal { get; set; }

    [LayersOption("#CCCCCCFF", LayerEnum.Executioner)]
    public static RoleOptionData Executioner { get; set; }

    [LayersOption("#EEE5BEFF", LayerEnum.Guesser)]
    public static RoleOptionData Guesser { get; set; }

    [LayersOption("#F7B3DAFF", LayerEnum.Jester)]
    public static RoleOptionData Jester { get; set; }

    [LayersOption("#678D36FF", LayerEnum.Troll)]
    public static RoleOptionData Troll { get; set; }
}

[AlignsOption(LayerEnum.NeutralHarb)]
public static class NeutralHarbingerRoles
{
    [LayersOption("#CFFE61FF", LayerEnum.Plaguebearer)]
    public static RoleOptionData Plaguebearer { get; set; }
}

[AlignsOption(LayerEnum.NeutralKill)]
public static class NeutralKillingRoles
{
    [LayersOption("#EE7600FF", LayerEnum.Arsonist)]
    public static RoleOptionData Arsonist { get; set; }

    [LayersOption("#642DEAFF", LayerEnum.Cryomaniac)]
    public static RoleOptionData Cryomaniac { get; set; }

    [LayersOption("#00FF00FF", LayerEnum.Glitch)]
    public static RoleOptionData Glitch { get; set; }

    [LayersOption("#A12B56FF", LayerEnum.Juggernaut)]
    public static RoleOptionData Juggernaut { get; set; }

    [LayersOption("#6F7BEAFF", LayerEnum.Murderer)]
    private static RoleOptionData MurdererPriv { get; set; }
    public static RoleOptionData Murderer
    {
        get
        {
            var result = MurdererPriv.Clone();

            if (IsKilling())
                result.Chance = 5;

            return result;
        }
    }

    [LayersOption("#336EFFFF", LayerEnum.SerialKiller)]
    public static RoleOptionData SerialKiller { get; set; }

    [LayersOption("#9F703AFF", LayerEnum.Werewolf)]
    public static RoleOptionData Werewolf { get; set; }
}

[AlignsOption(LayerEnum.NeutralNeo)]
public static class NeutralNeophyteRoles
{
    [LayersOption("#AC8A00FF", LayerEnum.Dracula)]
    public static RoleOptionData Dracula { get; set; }

    [LayersOption("#45076AFF", LayerEnum.Jackal)]
    public static RoleOptionData Jackal { get; set; }

    [LayersOption("#BF5FFFFF", LayerEnum.Necromancer)]
    public static RoleOptionData Necromancer { get; set; }

    [LayersOption("#2D6AA5FF", LayerEnum.Whisperer)]
    public static RoleOptionData Whisperer { get; set; }
}

[AlignsOption(LayerEnum.NeutralPros)]
public static class NeutralProselyteRoles
{
    [LayersOption("#11806AFF", LayerEnum.Betrayer, true)]
    public static RoleOptionData Betrayer { get; set; }

    [LayersOption("#662962FF", LayerEnum.Phantom)]
    private static RoleOptionData PhantomPriv { get; set; }
    public static RoleOptionData Phantom
    {
        get
        {
            var result = PhantomPriv.Clone();

            if (IsRoleList())
                result.Count = 1;

            return result;
        }
    }
}

[AlignsOption(LayerEnum.IntruderConceal)]
public static class IntruderConcealingRoles
{
    [LayersOption("#02A752FF", LayerEnum.Blackmailer)]
    public static RoleOptionData Blackmailer { get; set; }

    [LayersOption("#378AC0FF", LayerEnum.Camouflager)]
    public static RoleOptionData Camouflager { get; set; }

    [LayersOption("#85AA5BFF", LayerEnum.Grenadier)]
    public static RoleOptionData Grenadier { get; set; }

    [LayersOption("#2647A2FF", LayerEnum.Janitor)]
    public static RoleOptionData Janitor { get; set; }
}

[AlignsOption(LayerEnum.IntruderDecep)]
public static class IntruderDeceptionRoles
{
    [LayersOption("#40B4FFFF", LayerEnum.Disguiser)]
    public static RoleOptionData Disguiser { get; set; }

    [LayersOption("#BB45B0FF", LayerEnum.Morphling)]
    public static RoleOptionData Morphling { get; set; }

    [LayersOption("#5C4F75FF", LayerEnum.Wraith)]
    public static RoleOptionData Wraith { get; set; }
}

[AlignsOption(LayerEnum.IntruderHead)]
public static class IntruderHeadRoles
{
    [LayersOption("#404C08FF", LayerEnum.Godfather)]
    public static RoleOptionData Godfather { get; set; }
}

[AlignsOption(LayerEnum.IntruderKill)]
public static class IntruderKillingRoles
{
    [LayersOption("#2BD29CFF", LayerEnum.Ambusher)]
    public static RoleOptionData Ambusher { get; set; }

    [LayersOption("#005643FF", LayerEnum.Enforcer)]
    public static RoleOptionData Enforcer { get; set; }
}

[AlignsOption(LayerEnum.IntruderSupport)]
public static class IntruderSupportRoles
{
    [LayersOption("#FFFF99FF", LayerEnum.Consigliere)]
    public static RoleOptionData Consigliere { get; set; }

    [LayersOption("#801780FF", LayerEnum.Consort)]
    public static RoleOptionData Consort { get; set; }

    [LayersOption("#AA7632FF", LayerEnum.Miner)]
    public static RoleOptionData Miner { get; set; }

    [LayersOption("#939593FF", LayerEnum.Teleporter)]
    public static RoleOptionData Teleporter { get; set; }
}

[AlignsOption(LayerEnum.IntruderUtil)]
public static class IntruderUtilityRoles
{
    [LayersOption("#F1C40FFF", LayerEnum.Ghoul)]
    private static RoleOptionData GhoulPriv { get; set; }
    public static RoleOptionData Ghoul
    {
        get
        {
            var result = GhoulPriv.Clone();

            if (IsRoleList())
                result.Count = 1;

            return result;
        }
    }

    [LayersOption("#FF1919FF", LayerEnum.Impostor)]
    private static RoleOptionData ImpostorPriv { get; set; }
    public static RoleOptionData Impostor
    {
        get
        {
            var result = ImpostorPriv.Clone();

            if (IsKilling())
                result.Chance = 5;
            else if (!IsCustom())
                result.Chance = 100;

            return result;
        }
    }
}

[AlignsOption(LayerEnum.SyndicateDisrup)]
public static class SyndicateDisruptionRoles
{
    [LayersOption("#C02525FF", LayerEnum.Concealer)]
    public static RoleOptionData Concealer { get; set; }

    [LayersOption("#FF7900FF", LayerEnum.Drunkard)]
    public static RoleOptionData Drunkard { get; set; }

    [LayersOption("#00FFFFFF", LayerEnum.Framer)]
    public static RoleOptionData Framer { get; set; }

    [LayersOption("#2DFF00FF", LayerEnum.Shapeshifter)]
    public static RoleOptionData Shapeshifter { get; set; }

    [LayersOption("#AAB43EFF", LayerEnum.Silencer)]
    public static RoleOptionData Silencer { get; set; }

    [LayersOption("#3769FEFF", LayerEnum.Timekeeper)]
    public static RoleOptionData Timekeeper { get; set; }
}

[AlignsOption(LayerEnum.SyndicateKill)]
public static class SyndicateKillingRoles
{
    [LayersOption("#C9CC3FFF", LayerEnum.Bomber)]
    public static RoleOptionData Bomber { get; set; }

    [LayersOption("#B345FFFF", LayerEnum.Collider)]
    public static RoleOptionData Collider { get; set; }

    [LayersOption("#DF7AE8FF", LayerEnum.Crusader)]
    public static RoleOptionData Crusader { get; set; }

    [LayersOption("#B5004CFF", LayerEnum.Poisoner)]
    public static RoleOptionData Poisoner { get; set; }
}

[AlignsOption(LayerEnum.SyndicatePower)]
public static class SyndicatePowerRoles
{
    [LayersOption("#FFFCCEFF", LayerEnum.Rebel)]
    public static RoleOptionData Rebel { get; set; }

    [LayersOption("#0028F5FF", LayerEnum.Spellslinger)]
    public static RoleOptionData Spellslinger { get; set; }
}

[AlignsOption(LayerEnum.SyndicateSupport)]
public static class SyndicateSupportRoles
{
    [LayersOption("#7E4D00FF", LayerEnum.Stalker)]
    public static RoleOptionData Stalker { get; set; }

    [LayersOption("#8C7140FF", LayerEnum.Warper)]
    public static RoleOptionData Warper { get; set; }
}

[AlignsOption(LayerEnum.SyndicateUtil)]
public static class SyndicateUtilityRoles
{
    [LayersOption("#008000FF", LayerEnum.Anarchist)]
    private static RoleOptionData AnarchistPriv { get; set; }
    public static RoleOptionData Anarchist
    {
        get
        {
            var result = AnarchistPriv.Clone();

            if (IsKilling())
                result.Chance = 5;
            else if (!IsCustom())
                result.Chance = 100;

            return result;
        }
    }

    [LayersOption("#E67E22FF", LayerEnum.Banshee)]
    private static RoleOptionData BansheePriv { get; set; }
    public static RoleOptionData Banshee
    {
        get
        {
            var result = BansheePriv.Clone();

            if (IsRoleList())
                result.Count = 1;

            return result;
        }
    }
}

[AlignsOption(LayerEnum.Modifier)]
public static class Modifiers
{
    [LayersOption("#612BEFFF", LayerEnum.Astral)]
    public static RoleOptionData Astral { get; set; }

    [LayersOption("#00B3B3FF", LayerEnum.Bait)]
    public static RoleOptionData Bait { get; set; }

    [LayersOption("#B34D99FF", LayerEnum.Colorblind)]
    public static RoleOptionData Colorblind { get; set; }

    [LayersOption("#456BA8FF", LayerEnum.Coward)]
    public static RoleOptionData Coward { get; set; }

    [LayersOption("#374D1EFF", LayerEnum.Diseased)]
    public static RoleOptionData Diseased { get; set; }

    [LayersOption("#758000FF", LayerEnum.Drunk)]
    public static RoleOptionData Drunk { get; set; }

    [LayersOption("#FF8080FF", LayerEnum.Dwarf)]
    public static RoleOptionData Dwarf { get; set; }

    [LayersOption("#FFB34DFF", LayerEnum.Giant)]
    public static RoleOptionData Giant { get; set; }

    [LayersOption("#2DE5BEFF", LayerEnum.Indomitable)]
    public static RoleOptionData Indomitable { get; set; }

    [LayersOption("#860B7AFF", LayerEnum.Professional)]
    public static RoleOptionData Professional { get; set; }

    [LayersOption("#1002C5FF", LayerEnum.Shy)]
    public static RoleOptionData Shy { get; set; }

    [LayersOption("#DCEE85FF", LayerEnum.VIP)]
    public static RoleOptionData VIP { get; set; }

    [LayersOption("#FFA60AFF", LayerEnum.Volatile)]
    public static RoleOptionData Volatile { get; set; }

    [LayersOption("#F6AAB7FF", LayerEnum.Yeller)]
    public static RoleOptionData Yeller { get; set; }
}

[AlignsOption(LayerEnum.Ability)]
public static class Abilities
{
    [LayersOption("#8CFFFFFF", LayerEnum.Bullseye)]
    public static RoleOptionData Bullseye { get; set; }

    [LayersOption("#E600FFFF", LayerEnum.ButtonBarry)]
    public static RoleOptionData ButtonBarry { get; set; }

    [LayersOption("#FF1919FF", LayerEnum.Hitman)]
    public static RoleOptionData Hitman { get; set; }

    [LayersOption("#26FCFBFF", LayerEnum.Insider)]
    public static RoleOptionData Insider { get; set; }

    [LayersOption("#FF804DFF", LayerEnum.Multitasker)]
    public static RoleOptionData Multitasker { get; set; }

    [LayersOption("#A84300FF", LayerEnum.Ninja)]
    public static RoleOptionData Ninja { get; set; }

    [LayersOption("#CCA3CCFF", LayerEnum.Politician)]
    public static RoleOptionData Politician { get; set; }

    [LayersOption("#FF0080FF", LayerEnum.Radar)]
    public static RoleOptionData Radar { get; set; }

    [LayersOption("#2160DDFF", LayerEnum.Ruthless)]
    public static RoleOptionData Ruthless { get; set; }

    [LayersOption("#B3B3B3FF", LayerEnum.Slayer)]
    public static RoleOptionData Slayer { get; set; }

    [LayersOption("#008000FF", LayerEnum.Sniper)]
    public static RoleOptionData Sniper { get; set; }

    [LayersOption("#D4AF37FF", LayerEnum.Snitch)]
    public static RoleOptionData Snitch { get; set; }

    [LayersOption("#66E666FF", LayerEnum.Swapper)]
    public static RoleOptionData Swapper { get; set; }

    [LayersOption("#99E699FF", LayerEnum.Tiebreaker)]
    public static RoleOptionData Tiebreaker { get; set; }

    [LayersOption("#FFFF99FF", LayerEnum.Torch)]
    public static RoleOptionData Torch { get; set; }

    [LayersOption("#E91E63FF", LayerEnum.Tunneler)]
    public static RoleOptionData Tunneler { get; set; }

    [LayersOption("#841A7FFF", LayerEnum.Underdog)]
    public static RoleOptionData Underdog { get; set; }
}

[AlignsOption(LayerEnum.Disposition)]
public static class Dispositions
{
    [LayersOption("#4545A9FF", LayerEnum.Allied)]
    public static RoleOptionData Allied { get; set; }

    [LayersOption("#4545FFFF", LayerEnum.Corrupted)]
    public static RoleOptionData Corrupted { get; set; }

    [LayersOption("#E1C849FF", LayerEnum.Defector)]
    public static RoleOptionData Defector { get; set; }

    [LayersOption("#678D36FF", LayerEnum.Fanatic)]
    public static RoleOptionData Fanatic { get; set; }

    [LayersOption("#FF351FFF", LayerEnum.Linked, Max = 7)]
    public static RoleOptionData Linked { get; set; }

    [LayersOption("#FF66CCFF", LayerEnum.Lovers, Max = 7)]
    public static RoleOptionData Lovers { get; set; }

    [LayersOption("#00EEFFFF", LayerEnum.Mafia, Min = 2)]
    public static RoleOptionData Mafia { get; set; }

    [LayersOption("#008080FF", LayerEnum.Overlord)]
    public static RoleOptionData Overlord { get; set; }

    [LayersOption("#3D2D2CFF", LayerEnum.Rivals, Max = 7)]
    public static RoleOptionData Rivals { get; set; }

    [LayersOption("#ABABFFFF", LayerEnum.Taskmaster)]
    public static RoleOptionData Taskmaster { get; set; }

    [LayersOption("#370D43FF", LayerEnum.Traitor)]
    public static RoleOptionData Traitor { get; set; }
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class CrewAuditorSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxCA { get; set; } = new(1);
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class CrewInvestigativeSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxCI { get; set; } = new(1);
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class CrewKillingSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxCK { get; set; } = new(1);
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class CrewProtectiveSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxCrP { get; set; } = new(1);
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class CrewSovereignSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxCSv { get; set; } = new(1);
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class CrewSupportSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxCS { get; set; } = new(1);
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class NeutralApocalypseSettings
{
    [ToggleOption(MultiMenu.AlignmentSubOptions)]
    public static bool DirectSpawn { get; set; } = false;

    [ToggleOption(MultiMenu.AlignmentSubOptions)]
    public static bool PlayersAlerted { get; set; } = true;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class NeutralBenignSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxNB { get; set; } = new(1);

    [ToggleOption(MultiMenu.AlignmentSubOptions)]
    public static bool VigilanteKillsBenigns { get; set; } = true;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class NeutralEvilSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxNE { get; set; } = new(1);

    [ToggleOption(MultiMenu.AlignmentSubOptions)]
    public static bool NeutralEvilsEndGame { get; set; } = false;

    [ToggleOption(MultiMenu.AlignmentSubOptions)]
    public static bool VigilanteKillsEvils { get; set; } = true;

    [ToggleOption(MultiMenu.AlignmentSubOptions)]
    public static bool NEHasImpVision { get; set; } = true;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class NeutralHarbingerSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxNH { get; set; } = new(1);

    [ToggleOption(MultiMenu.AlignmentSubOptions)]
    public static bool NHHasImpVision { get; set; } = true;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class NeutralKillingSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxNK { get; set; } = new(1);

    [ToggleOption(MultiMenu.AlignmentSubOptions)]
    public static bool NKHasImpVision { get; set; } = true;

    [ToggleOption(MultiMenu.AlignmentSubOptions)]
    public static bool KnowEachOther { get; set; } = false;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class NeutralNeophyteSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxNN { get; set; } = new(1);

    [ToggleOption(MultiMenu.AlignmentSubOptions)]
    public static bool NNHasImpVision { get; set; } = true;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class IntruderConcealingSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxIC { get; set; } = new(1);
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class IntruderDeceptionSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxID { get; set; } = new(1);
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class IntruderHeadSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxIH { get; set; } = new(1);
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class IntruderKillingSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxIK { get; set; } = new(1);
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class IntruderSupportSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxIS { get; set; } = new(1);
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class SyndicateDisruptionSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxSD { get; set; } = new(1);
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class SyndicateKillingSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxSyK { get; set; } = new(1);
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class SyndicatePowerSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxSP { get; set; } = new(1);
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class SyndicateSupportSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxSSu { get; set; } = new(1);
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class ModifiersSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxModifiers { get; set; } = new(5);

    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MinModifiers { get; set; } = new(5);
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class AbilitiesSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxAbilities { get; set; } = new(5);

    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MinAbilities { get; set; } = new(5);
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class DispositionsSettings
{
    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MaxDispositions { get; set; } = new(5);

    [NumberOption(MultiMenu.AlignmentSubOptions, 1, 14, 1)]
    public static Number MinDispositions { get; set; } = new(5);
}