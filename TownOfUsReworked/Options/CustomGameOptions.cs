// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable RedundantDefaultMemberInitializer

namespace TownOfUsReworked.Options;

// DO NOT OVERRIDE VALUES OF THE OPTION PROPERTIES ANYWHERE IN THE CODE, OR ELSE THE OPTIONS WILL START TO FUCK OFF

[HeaderOption(MultiMenu.Main)]
public static class GameOptions
{
    [NumberOption(0.25f, 10, 0.25f, Format.Multiplier)]
    public static Number PlayerSpeed = 1.25f;

    [NumberOption(0.25f, 10, 0.25f, Format.Multiplier)]
    public static Number GhostSpeed = 3;

    [NumberOption(0.5f, 5, 0.5f, Format.Distance)]
    public static Number InteractionDistance = 2;

    [NumberOption(0, 100, 1)]
    public static Number EmergencyButtonCount = 1;

    [NumberOption(0, 300, 2.5f, Format.Time)]
    public static Number EmergencyButtonCooldown = 25;

    [NumberOption(0, 300, 5, Format.Time)]
    public static Number DiscussionTime = 30;

    [NumberOption(5, 600, 15, Format.Time)]
    public static Number VotingTime = 60;

    [NumberOption(0.5f, 10f, 0.5f, Format.Distance)]
    public static Number MaxReportDistance = 5;

    [StringOption<TBMode>]
    private static TBMode TaskBar = TBMode.MeetingOnly;
    public static TBMode TaskBarMode => GameModeSettings.GameMode switch
    {
        // I want this to actually change, according to the game modes
        Mode.TaskRace or Mode.HideAndSeek => TBMode.Normal,
        _ => TaskBar
    };

    [ToggleOption]
    public static bool ConfirmEjects = false;

    [ToggleOption]
    public static bool EjectionRevealsRoles = false;

    [ToggleOption]
    public static bool EnableInitialCds = true;

    [NumberOption(0, 30, 2.5f, Format.Time)]
    public static Number InitialCooldowns = 10;

    [ToggleOption]
    public static bool EnableMeetingCds = true;

    [NumberOption(0, 30, 2.5f, Format.Time)]
    public static Number MeetingCooldowns = 15;

    [ToggleOption]
    public static bool EnableFailCds = true;

    [NumberOption(0, 30, 2.5f, Format.Time)]
    public static Number FailCooldowns = 5;

    [NumberOption(0, 3, 0.1f, Format.Time)]
    public static Number ChatCooldown = 3;

    [NumberOption(0, 2000, 50, zeroIsInf: true)]
    public static Number ChatCharacterLimit = 200;

    [NumberOption(2, 250, 1)]
    public static Number LobbySize = 15;

    [StringOption<GhostSpawnType>]
    public static GhostSpawnType GhostSpawn = GhostSpawnType.Random;
}

[HeaderOption(MultiMenu.Main)]
public static class GameModeSettings
{
    [StringOption<Mode>(Mode.None)]
    public static Mode GameMode = Mode.Classic;

    [ToggleOption]
    public static bool IgnoreFactionCaps = true;

    [ToggleOption]
    public static bool IgnoreAlignmentCaps = true;

    [ToggleOption]
    public static bool IgnoreLayerCaps = true;

    [StringOption<HnSMode>]
    public static HnSMode HnSMode = HnSMode.Classic;

    [NumberOption(0, 15, 1)]
    public static Number RevealerCount = 0;

    [NumberOption(0, 15, 1)]
    public static Number PhantomCount = 0;

    [NumberOption(0, 15, 1)]
    public static Number GhoulCount = 0;

    [NumberOption(0, 15, 1)]
    public static Number BansheeCount = 0;

    [ToggleOption]
    public static bool BanCrewmate = true;

    [ToggleOption]
    public static bool BanMurderer = true;

    [ToggleOption]
    public static bool BanImpostor = true;

    [ToggleOption]
    public static bool BanAnarchist = true;

    [ToggleOption]
    public static bool BanCultist = true;

    [ToggleOption]
    public static bool BanZealot = true;
}

[ListHolderOption(PlayerLayerEnum.Role, false)]
public static class RoleEntryList;

[ListHolderOption(PlayerLayerEnum.Modifier, false)]
public static class ModifierEntryList;

[ListHolderOption(PlayerLayerEnum.Ability, false)]
public static class AbilityEntryList;

[ListHolderOption(PlayerLayerEnum.Disposition, false)]
public static class DispositionEntryList;

[ListHolderOption(PlayerLayerEnum.Role, true)]
public static class RoleBanList;

[ListHolderOption(PlayerLayerEnum.Modifier, true)]
public static class ModifierBanList;

[ListHolderOption(PlayerLayerEnum.Ability, true)]
public static class AbilityBanList;

[ListHolderOption(PlayerLayerEnum.Disposition, true)]
public static class DispositionBanList;

// Need singleton access because the list holders for some god forsaken reason are not working the way I wanted them to
public static class Holders
{
    public static int EntryCount;

    public static ListHolderOption RolesEntryList;
    public static ListHolderOption ModifiersEntryList;
    public static ListHolderOption AbilitiesEntryList;
    public static ListHolderOption DispositionsEntryList;

    public static ListHolderOption RolesBanList;
    public static ListHolderOption ModifiersBanList;
    public static ListHolderOption AbilitiesBanList;
    public static ListHolderOption DispositionsBanList;
}

[HeaderOption(MultiMenu.Main)]
public static class TaskOptions
{
    [NumberOption(0, 100, 1)]
    public static Number CommonTasks = 2;

    [NumberOption(0, 100, 1)]
    public static Number LongTasks = 1;

    [NumberOption(0, 100, 1)]
    public static Number ShortTasks = 4;

    [ToggleOption]
    public static bool GhostTasksCountToWin = true;

    [ToggleOption]
    public static bool VisualTasks = false;

    [ToggleOption]
    public static bool ParallelMedScans = false;

    [ToggleOption]
    public static bool AllCanDoTasks = false;
}

[HeaderOption(MultiMenu.Main)]
public static class VotingOptions
{
    [StringOption<AnonVotes>]
    public static AnonVotes AnonymousVoting = AnonVotes.Enabled;

    [StringOption<DisableSkipButtonMeetings>]
    public static DisableSkipButtonMeetings NoSkipping = DisableSkipButtonMeetings.Never;
}

[HeaderOption(MultiMenu.Main)]
public static class BadGuysSettings
{
    [StringOption<Faction>(Faction.Crew, Faction.GameMode, Faction.Outcast, Faction.None, Faction.Illuminati), Sorted(0)]
    public static Faction MainBadGuys
    {
        get => IlluminatiUnleashed ? Faction.Illuminati : field;
        set
        {
            if (value == Faction.Compliance && !OrderOfCompliance)
                value = value < field ? Faction.Intruder : (PandoricaOpens ? Faction.Pandorica : Faction.Apocalypse);

            if (value is Faction.Intruder or Faction.Syndicate or Faction.Apocalypse && PandoricaOpens)
                value = Faction.Pandorica;

            if (value == Faction.Pandorica && !PandoricaOpens)
                value = value < field ? Faction.Apocalypse : (OrderOfCompliance ? Faction.Compliance : Faction.Intruder);

            field = value;
        }
    } = Faction.Intruder;

    [ToggleOption, Sorted(0)]
    public static bool OnlyMainBadGuys = false;

    [ToggleOption, Sorted(0)]
    public static bool MainBadGuysCanSabotage = false;

    [ToggleOption, Sorted(0)]
    public static bool GhostsCanSabotage = false;

    [ToggleOption, Sorted(0)]
    public static bool IlluminatiUnleashed = false;

    [MultiSelectOption<IlluminatiType>(LeastSelected = 2), Sorted(0)]
    public static MultiSelectValue<IlluminatiType> IlluminatiMembers = new[] { IlluminatiType.Syndicate, IlluminatiType.Intruders, IlluminatiType.Apocalypse, IlluminatiType.Neophytes,
        IlluminatiType.Killers };

    public static ReworkedToggleOption PandoricaOpens = new() { OnChanged = ValidateAlliedFaction };

    [MultiSelectOption<PandoricaType>(LeastSelected = 1)]
    public static MultiSelectValue<PandoricaType> PandoricaMembers = new[] { PandoricaType.Syndicate, PandoricaType.Intruders, PandoricaType.Apocalypse };

    [ToggleOption]
    public static bool OrderOfCompliance = false;

    [MultiSelectOption<ComplianceType>(LeastSelected = 1)]
    public static MultiSelectValue<ComplianceType> ComplianceMembers = new[] { ComplianceType.Neophytes, ComplianceType.Killers };

    public static Number BadGuyCount => MainBadGuys switch
    {
        Faction.Intruder => IntruderSettings.IntruderCount,
        Faction.Syndicate => SyndicateSettings.SyndicateCount,
        Faction.Apocalypse => ApocalypseSettings.ApocalypseCount,
        Faction.Compliance => ComplianceSettings.ComplianceCount,
        Faction.Illuminati => IlluminatiSettings.IlluminatiCount,
        Faction.Pandorica => PandoricaSettings.PandoricaCount,
        _ => 0,
    };

    private static void ValidateAlliedFaction(bool value)
    {
        switch (value)
        {
            case false when Allied.alliedFaction == AlliedFaction.Pandorica:
            {
                Allied.alliedFaction = AlliedFaction.Random;
                break;
            }
            case true when Allied.alliedFaction is AlliedFaction.Intruder or AlliedFaction.Syndicate or AlliedFaction.Apocalypse:
            {
                Allied.alliedFaction = AlliedFaction.Pandorica;
                break;
            }
        }
    }
}

[HeaderOption(MultiMenu.Main)]
public static class GameModifiers
{
    [ToggleOption, Sorted(0)]
    public static bool FirstKillShield = false;

    [StringOption<WhoCanSeeFirstKillShield>, Sorted(0)]
    public static WhoCanSeeFirstKillShield WhoSeesFirstKillShield = WhoCanSeeFirstKillShield.Everyone;

    [ToggleOption, Sorted(0)]
    public static bool FactionSeeRoles = true;

    [StringOption<PlayerNames>, Sorted(0)]
    public static PlayerNames PlayerNames = PlayerNames.Obstructed;

    [ToggleOption, Sorted(0)]
    public static bool Whispers = true;

    [ToggleOption, Sorted(0)]
    public static bool WhispersAnnouncement = true;

    [ToggleOption, Sorted(0)]
    public static bool AppearanceAnimation = true;

    [ToggleOption, Sorted(0)]
    public static bool EnableAbilities = true;

    [ToggleOption, Sorted(0)]
    public static bool EnableModifiers = true;

    [ToggleOption, Sorted(0)]
    public static bool EnableDispositions = true;

    [ToggleOption, Sorted(0)]
    public static bool DeadSeeEverything = true;

    [ToggleOption, Sorted(0)]
    public static bool JaniCanMutuallyExclusive = false;

    [MultiSelectOption<RandomSpawning>, Sorted(0)]
    public static MultiSelectValue<RandomSpawning> RandomSpawns = "";

    [ToggleOption, Sorted(0)]
    public static bool ShowKillerRoleColor = false;

    [ToggleOption, Sorted(0)]
    public static bool PurePlayers = false;

    [Sorted(0)]
    public static ReworkedToggleOption EnableGameTimer = new() { OnChanged = TryCreatePrefab };

    [NumberOption(60, 3600, 60, Format.Time), Sorted(0)]
    public static Number GameTimer = 1200;

    [StringOption<DuringMeeting>, Sorted(0)]
    public static DuringMeeting DuringMeetings = DuringMeeting.Never;

    [NumberOption(300, 3000, 60, Format.Time, All = true), Sorted(0)]
    public static Number TimeLeft = 4;

    [NumberOption(4, 125, 1, All = true), Sorted(0)]
    public static Number PlayersLeft = 4;

    [MultiSelectOption<TimerExtension>]
    public static MultiSelectValue<TimerExtension> ExtendTimer = [];

    [NumberOption(4, 125, 1), Sorted(0)]
    public static Number TimerExtension = 4;

    private static void TryCreatePrefab(bool value)
    {
        if (value && !GameTimerHandler.Prefab)
            Coroutines.Start(CoTryCreatePrefab());
    }

    private static IEnumerator CoTryCreatePrefab()
    {
        while (!GameManagerCreator.Instance)
            yield return null;

        GameTimerHandler.Prefab = UObject.Instantiate(GameManagerCreator.Instance.HideAndSeekManagerPrefab.TimerBarPrefab, null).DontDestroy().AddComponent<GameTimerHandler>();
        GameTimerHandler.Prefab.name = "GameTimer";
        GameTimerHandler.Prefab.gameObject.SetActive(false);
    }
}

[HeaderOption(MultiMenu.Main)]
public static class VentingOptions
{
    [StringOption<WhoCanVentOptions>]
    public static WhoCanVentOptions WhoCanVent = WhoCanVentOptions.Default;

    [ToggleOption]
    public static bool FinalTwoDisableVenting = false;

    [ToggleOption]
    public static bool VentTargeting = true;

    [ToggleOption]
    public static bool CooldownInVent = false;

    [ToggleOption]
    public static bool NoVentingUncleanedVents = false;

    [ToggleOption]
    public static bool HideVentAnims = true;
}

[HeaderOption(MultiMenu.Main)]
public static class GameAnnouncementSettings
{
    [ToggleOption]
    public static bool GameAnnouncements = false;

    [ToggleOption]
    public static bool LocationReports = false;

    [StringOption<RoleFactionReports>]
    public static RoleFactionReports RoleFactionReports = RoleFactionReports.Neither;

    [StringOption<RoleFactionReports>]
    public static RoleFactionReports KillerReports = RoleFactionReports.Neither;

    [ToggleOption, Sorted(0)]
    public static bool IndicateReportedBodies = false;
}

[HeaderOption(MultiMenu.Main)]
public static class MapSettings
{
    public static MapEnum Map
    {
        get;
        set
        {
            if (value == MapEnum.LevelImpostor && !LiLoaded)
                value = field < MapEnum.LevelImpostor ? MapEnum.Random : MapEnum.Submerged;

            if (value == MapEnum.Submerged && !SubLoaded)
                value = field < MapEnum.Submerged ? MapEnum.Random : MapEnum.Fungle;

            field = value;

            try
            {
                TownOfUsReworked.NormalOptions.MapId = (byte)value;
            }
            catch {}

            try
            {
                TownOfUsReworked.HnsOptions.MapId = (byte)value;
            }
            catch {}
        }
    }

    [NumberOption(0, 100, 10, Format.Percent)]
    public static Number RandomMapSkeld = 10;

    [NumberOption(0, 100, 10, Format.Percent)]
    public static Number RandomMapMira = 10;

    [NumberOption(0, 100, 10, Format.Percent)]
    public static Number RandomMapPolus = 10;

    [NumberOption(0, 100, 10, Format.Percent)]
    public static Number RandomMapdlekS = 10;

    [NumberOption(0, 100, 10, Format.Percent)]
    public static Number RandomMapAirship = 10;

    [NumberOption(0, 100, 10, Format.Percent)]
    public static Number RandomMapFungle = 10;

    [NumberOption(0, 100, 10, Format.Percent, All = true)]
    public static Number RandomMapSubmerged = 10;

    [NumberOption(0, 100, 10, Format.Percent, All = true)]
    public static Number RandomMapLevelImpostor = 10;

    [ToggleOption]
    public static bool AutoAdjustSettings = false;

    [ToggleOption]
    public static bool SmallMapHalfVision = false;

    [NumberOption(0f, 15f, 2.5f, Format.Time)]
    public static Number SmallMapDecreasedCooldown = 0;

    [NumberOption(0f, 15f, 2.5f, Format.Time)]
    public static Number LargeMapIncreasedCooldown = 0;

    [NumberOption(0, 5, 1)]
    public static Number SmallMapIncreasedShortTasks = 0;

    [NumberOption(0, 3, 1)]
    public static Number SmallMapIncreasedLongTasks = 0;

    [NumberOption(0, 5, 1)]
    public static Number LargeMapDecreasedShortTasks = 0;

    [NumberOption(0, 3, 1)]
    public static Number LargeMapDecreasedLongTasks = 0;
}

[HeaderOption(MultiMenu.Main)]
public static class CrewSettings
{
    [NumberOption(0.25f, 5f, 0.25f, Format.Multiplier)]
    public static Number CrewVision = 1;

    [ToggleOption]
    public static bool CrewFlashlight = false;

    [StringOption<CrewVenting>]
    public static CrewVenting CrewVent = CrewVenting.Never;

    [NumberOption(0, 14, 1)]
    public static Number CrewMin = 4;

    [NumberOption(0, 14, 1)]
    public static Number CrewMax = 5;
}

[HeaderOption(MultiMenu.Main)]
public static class OutcastSettings
{
    [NumberOption(0.25f, 5f, 0.25f, Format.Multiplier)]
    public static Number OutcastVision = 1.5f;

    [ToggleOption]
    public static bool LightsAffectOutcasts = true;

    [ToggleOption]
    public static bool OutcastFlashlight = false;

    [NumberOption(0, 14, 1)]
    public static Number OutcastMin = 0;

    [NumberOption(0, 14, 1)]
    public static Number OutcastMax = 1;

    [ToggleOption]
    public static bool AvoidOutcastKingmakers = false;

    [ToggleOption]
    public static bool OutcastsVent = true;
}

[HeaderOption(MultiMenu.Main)]
public static class IntruderSettings
{
    [NumberOption(0, 4, 1)]
    public static Number IntruderCount = 1;

    [NumberOption(0.25f, 5f, 0.25f, Format.Multiplier)]
    public static Number IntruderVision = 2;

    [ToggleOption]
    public static bool IntruderFlashlight = false;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number IntKillCd = 25;

    [ToggleOption]
    public static bool IntrudersVent = true;

    [NumberOption(0, 14, 1)]
    public static Number IntruderMin = 0;

    [NumberOption(0, 14, 1)]
    public static Number IntruderMax = 1;
}

[HeaderOption(MultiMenu.Main)]
public static class SyndicateSettings
{
    [NumberOption(0, 4, 1)]
    public static Number SyndicateCount = 1;

    [NumberOption(0.25f, 5f, 0.25f, Format.Multiplier)]
    public static Number SyndicateVision = 2;

    [ToggleOption]
    public static bool SyndicateFlashlight = false;

    [NumberOption(1, 10, 1)]
    public static Number ChaosDriveMeetingCount = 3;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number CdKillCd = 25;

    [StringOption<SyndicateVentOptions>]
    public static SyndicateVentOptions SyndicateVent = SyndicateVentOptions.Always;

    [ToggleOption]
    public static bool GlobalDrive = false;

    [ToggleOption]
    public static bool AssignOnGameStart = false;

    [NumberOption(0, 14, 1)]
    public static Number SyndicateMin = 0;

    [NumberOption(0, 14, 1)]
    public static Number SyndicateMax = 1;
}

[HeaderOption(MultiMenu.Main)]
public static class ApocalypseSettings
{
    [NumberOption(0, 4, 1)]
    public static Number ApocalypseCount = 1;

    [NumberOption(0.25f, 5f, 0.25f, Format.Multiplier)]
    public static Number ApocalypseVision = 1.5f;

    [ToggleOption]
    public static bool ApocalypseFlashlight = false;

    [NumberOption(0, 14, 1)]
    public static Number ApocalypseMin = 0;

    [NumberOption(0, 14, 1)]
    public static Number ApocalypseMax = 1;

    [ToggleOption]
    public static bool ApocalypseVent = true;

    [ToggleOption]
    public static bool DirectSpawn = false;

    [ToggleOption]
    public static bool PlayersAlerted = true;
}

[HeaderOption(MultiMenu.Main)]
public static class ComplianceSettings
{
    [NumberOption(0, 4, 1)]
    public static Number ComplianceCount = 1;

    [NumberOption(0.25f, 5f, 0.25f, Format.Multiplier)]
    public static Number ComplianceVision = 1.5f;

    [ToggleOption]
    public static bool ComplianceFlashlight = false;

    [NumberOption(0, 14, 1)]
    public static Number ComplianceMin = 0;

    [NumberOption(0, 14, 1)]
    public static Number ComplianceMax = 1;

    [ToggleOption]
    public static bool ComplianceVent = true;
}

[HeaderOption(MultiMenu.Main)]
public static class PandoricaSettings
{
    [NumberOption(0, 4, 1)]
    public static Number PandoricaCount = 1;

    [NumberOption(0.25f, 5f, 0.25f, Format.Multiplier)]
    public static Number PandoricaVision = 1.5f;

    [ToggleOption]
    public static bool PandoricaFlashlight = false;

    [NumberOption(0, 14, 1)]
    public static Number PandoricaMin = 0;

    [NumberOption(0, 14, 1)]
    public static Number PandoricaMax = 1;

    [ToggleOption]
    public static bool PandoricaVent = true;
}

[HeaderOption(MultiMenu.Main)]
public static class IlluminatiSettings
{
    [NumberOption(0, 4, 1)]
    public static Number IlluminatiCount = 1;

    [NumberOption(0.25f, 5f, 0.25f, Format.Multiplier)]
    public static Number IlluminatiVision = 1.5f;

    [ToggleOption]
    public static bool IlluminatiFlashlight = false;

    [NumberOption(0, 14, 1)]
    public static Number IlluminatiMin = 0;

    [NumberOption(0, 14, 1)]
    public static Number IlluminatiMax = 1;

    [ToggleOption]
    public static bool IlluminatiVent = true;
}

[AlignmentOption(ListSlot.CrewInvest)]
public static class CrewInvestigativeRoles
{
    [LayerOption("#4D99E6FF", LayerEnum.Coroner)]
    public static RoleOptionData Coroner;

    [LayerOption("#4D4DFFFF", LayerEnum.Detective)]
    public static RoleOptionData Detective;

    [LayerOption("#A680FFFF", LayerEnum.Medium)]
    public static RoleOptionData Medium;

    [LayerOption("#708EEFFF", LayerEnum.Mystic)]
    public static RoleOptionData Mystic;

    [LayerOption("#A7D1B3FF", LayerEnum.Operative)]
    public static RoleOptionData Operative;

    [LayerOption("#71368AFF", LayerEnum.Seer)]
    public static RoleOptionData Seer;

    [LayerOption("#FFCC80FF", LayerEnum.Sheriff)]
    public static RoleOptionData Sheriff;

    [LayerOption("#009900FF", LayerEnum.Tracker)]
    public static RoleOptionData Tracker;
}

[AlignmentOption(ListSlot.CrewKill)]
public static class CrewKillingRoles
{
    [LayerOption("#7E3C64FF", LayerEnum.Bastion)]
    public static RoleOptionData Bastion;

    [LayerOption("#998040FF", LayerEnum.Veteran)]
    public static RoleOptionData Veteran;

    [LayerOption("#FFFF00FF", LayerEnum.Vigilante)]
    public static RoleOptionData Vigilante;
}

[AlignmentOption(ListSlot.CrewProt)]
public static class CrewProtectiveRoles
{
    [LayerOption("#660000FF", LayerEnum.Altruist)]
    public static RoleOptionData Altruist;

    [LayerOption("#006600FF", LayerEnum.Medic)]
    public static RoleOptionData Medic;

    [LayerOption("#BE1C8CFF", LayerEnum.Trapper)]
    public static RoleOptionData Trapper;
}

[AlignmentOption(ListSlot.CrewSov)]
public static class CrewSovereignRoles
{
    [LayerOption("#1A3270FF", LayerEnum.Democrat)]
    public static RoleOptionData Democrat;

    [LayerOption("#00CB97FF", LayerEnum.Dictator)]
    public static RoleOptionData Dictator;

    [LayerOption("#704FA8FF", LayerEnum.Mayor, true)]
    public static RoleOptionData Mayor;

    [LayerOption("#FF004EFF", LayerEnum.Monarch)]
    public static RoleOptionData Monarch;
}

[AlignmentOption(ListSlot.CrewSupport)]
public static class CrewSupportRoles
{
    [LayerOption("#5411F8FF", LayerEnum.Chameleon)]
    public static RoleOptionData Chameleon;

    [LayerOption("#FFA60AFF", LayerEnum.Engineer)]
    public static RoleOptionData Engineer;

    [LayerOption("#803333FF", LayerEnum.Escort)]
    public static RoleOptionData Escort;

    [LayerOption("#8D0F8CFF", LayerEnum.Retributionist)]
    public static RoleOptionData Retributionist;

    [LayerOption("#00EEFFFF", LayerEnum.Transporter)]
    public static RoleOptionData Transporter;
}

[AlignmentOption(ListSlot.CrewUtil)]
public static class CrewUtilityRoles
{
    [LayerOption("#8CFFFFFF", LayerEnum.Crewmate, true)]
    public static RoleOptionData Crewmate;

    [LayerOption("#D3D3D3FF", LayerEnum.Revealer)]
    public static RoleOptionData Revealer;
}

[AlignmentOption(ListSlot.OutcastBen)]
public static class OutcastBenignRoles
{
    [LayerOption("#22FFFFFF", LayerEnum.Amnesiac)]
    public static RoleOptionData Amnesiac;

    [LayerOption("#FFFFFFFF", LayerEnum.GuardianAngel)]
    public static RoleOptionData GuardianAngel;

    [LayerOption("#DDDD00FF", LayerEnum.Survivor)]
    public static RoleOptionData Survivor;

    [LayerOption("#80FF00FF", LayerEnum.Thief)]
    public static RoleOptionData Thief;
}

[AlignmentOption(ListSlot.OutcastEvil)]
public static class OutcastEvilRoles
{
    [LayerOption("#00ACC2FF", LayerEnum.Actor)]
    public static RoleOptionData Actor;

    [LayerOption("#B51E39FF", LayerEnum.BountyHunter)]
    public static RoleOptionData BountyHunter;

    [LayerOption("#CCCCCCFF", LayerEnum.Executioner)]
    public static RoleOptionData Executioner;

    [LayerOption("#EEE5BEFF", LayerEnum.Guesser)]
    public static RoleOptionData Guesser;

    [LayerOption("#F7B3DAFF", LayerEnum.Jester)]
    public static RoleOptionData Jester;

    [LayerOption("#DF851FFF", LayerEnum.Shifter)]
    public static RoleOptionData Shifter;

    [LayerOption("#678D36FF", LayerEnum.Troll)]
    public static RoleOptionData Troll;
}

[AlignmentOption(ListSlot.OutcastKill)]
public static class OutcastKillingRoles
{
    [LayerOption("#EE7600FF", LayerEnum.Arsonist)]
    public static RoleOptionData Arsonist;

    [LayerOption("#642DEAFF", LayerEnum.Cryomaniac)]
    public static RoleOptionData Cryomaniac;

    [LayerOption("#00FF00FF", LayerEnum.Glitch)]
    public static RoleOptionData Glitch;

    [LayerOption("#A12B56FF", LayerEnum.Juggernaut)]
    public static RoleOptionData Juggernaut;

    [LayerOption("#6F7BEAFF", LayerEnum.Murderer)]
    public static RoleOptionData Murderer;

    [LayerOption("#336EFFFF", LayerEnum.SerialKiller)]
    public static RoleOptionData SerialKiller;

    [LayerOption("#9F703AFF", LayerEnum.Werewolf)]
    public static RoleOptionData Werewolf;
}

[AlignmentOption(ListSlot.OutcastNeo)]
public static class OutcastNeophyteRoles
{
    [LayerOption("#AC8A00FF", LayerEnum.Dracula)]
    public static RoleOptionData Dracula;

    [LayerOption("#45076AFF", LayerEnum.Jackal)]
    public static RoleOptionData Jackal;

    [LayerOption("#BF5FFFFF", LayerEnum.Necromancer)]
    public static RoleOptionData Necromancer;

    [LayerOption("#2D6AA5FF", LayerEnum.Whisperer)]
    public static RoleOptionData Whisperer;

    [LayerOption("#7EFBC2FF", LayerEnum.Zealot)]
    public static RoleOptionData Zealot;
}

[AlignmentOption(ListSlot.OutcastPros)]
public static class OutcastProselyteRoles
{
    [LayerOption("#11806AFF", LayerEnum.Betrayer, true)]
    public static RoleOptionData Betrayer;

    [LayerOption("#662962FF", LayerEnum.Phantom)]
    public static RoleOptionData Phantom;
}

[AlignmentOption(ListSlot.IntruderConceal)]
public static class IntruderConcealingRoles
{
    [LayerOption("#02A752FF", LayerEnum.Blackmailer)]
    public static RoleOptionData Blackmailer;

    [LayerOption("#378AC0FF", LayerEnum.Camouflager)]
    public static RoleOptionData Camouflager;

    [LayerOption("#85AA5BFF", LayerEnum.Grenadier)]
    public static RoleOptionData Grenadier;

    [LayerOption("#2647A2FF", LayerEnum.Janitor)]
    public static RoleOptionData Janitor;
}

[AlignmentOption(ListSlot.IntruderDecep)]
public static class IntruderDeceptionRoles
{
    [LayerOption("#40B4FFFF", LayerEnum.Disguiser)]
    public static RoleOptionData Disguiser;

    [LayerOption("#BB45B0FF", LayerEnum.Morphling)]
    public static RoleOptionData Morphling;

    [LayerOption("#5C4F75FF", LayerEnum.Wraith)]
    public static RoleOptionData Wraith;
}

[AlignmentOption(ListSlot.IntruderHead)]
public static class IntruderHeadRoles
{
    [LayerOption("#404C08FF", LayerEnum.Godfather)]
    public static RoleOptionData Godfather;
}

[AlignmentOption(ListSlot.IntruderKill)]
public static class IntruderKillingRoles
{
    [LayerOption("#2BD29CFF", LayerEnum.Ambusher)]
    public static RoleOptionData Ambusher;

    [LayerOption("#005643FF", LayerEnum.Enforcer)]
    public static RoleOptionData Enforcer;
}

[AlignmentOption(ListSlot.IntruderSupport)]
public static class IntruderSupportRoles
{
    [LayerOption("#FFFF99FF", LayerEnum.Consigliere)]
    public static RoleOptionData Consigliere;

    [LayerOption("#801780FF", LayerEnum.Consort)]
    public static RoleOptionData Consort;

    [LayerOption("#AA7632FF", LayerEnum.Miner)]
    public static RoleOptionData Miner;

    [LayerOption("#939593FF", LayerEnum.Teleporter)]
    public static RoleOptionData Teleporter;
}

[AlignmentOption(ListSlot.IntruderUtil)]
public static class IntruderUtilityRoles
{
    [LayerOption("#F1C40FFF", LayerEnum.Ghoul)]
    public static RoleOptionData Ghoul;

    [LayerOption("#FF1919FF", LayerEnum.Impostor, true)]
    public static RoleOptionData Impostor;
}

[AlignmentOption(ListSlot.SyndicateDisrup)]
public static class SyndicateDisruptionRoles
{
    [LayerOption("#C02525FF", LayerEnum.Concealer)]
    public static RoleOptionData Concealer;

    [LayerOption("#FF7900FF", LayerEnum.Drunkard)]
    public static RoleOptionData Drunkard;

    [LayerOption("#00FFFFFF", LayerEnum.Framer)]
    public static RoleOptionData Framer;

    [LayerOption("#2DFF00FF", LayerEnum.Shapeshifter)]
    public static RoleOptionData Shapeshifter;

    [LayerOption("#AAB43EFF", LayerEnum.Silencer)]
    public static RoleOptionData Silencer;

    [LayerOption("#3769FEFF", LayerEnum.Timekeeper)]
    public static RoleOptionData Timekeeper;
}

[AlignmentOption(ListSlot.SyndicateKill)]
public static class SyndicateKillingRoles
{
    [LayerOption("#C9CC3FFF", LayerEnum.Bomber)]
    public static RoleOptionData Bomber;

    [LayerOption("#B345FFFF", LayerEnum.Collider)]
    public static RoleOptionData Collider;

    [LayerOption("#DF7AE8FF", LayerEnum.Crusader)]
    public static RoleOptionData Crusader;

    [LayerOption("#B5004CFF", LayerEnum.Poisoner)]
    public static RoleOptionData Poisoner;
}

[AlignmentOption(ListSlot.SyndicatePower)]
public static class SyndicatePowerRoles
{
    [LayerOption("#FFFCCEFF", LayerEnum.Rebel)]
    public static RoleOptionData Rebel;

    [LayerOption("#0028F5FF", LayerEnum.Spellslinger)]
    public static RoleOptionData Spellslinger;
}

[AlignmentOption(ListSlot.SyndicateSupport)]
public static class SyndicateSupportRoles
{
    [LayerOption("#7E4D00FF", LayerEnum.Stalker)]
    public static RoleOptionData Stalker;

    [LayerOption("#8C7140FF", LayerEnum.Warper)]
    public static RoleOptionData Warper;
}

[AlignmentOption(ListSlot.SyndicateUtil)]
public static class SyndicateUtilityRoles
{
    [LayerOption("#008000FF", LayerEnum.Anarchist, true)]
    public static RoleOptionData Anarchist;

    [LayerOption("#E67E22FF", LayerEnum.Banshee)]
    public static RoleOptionData Banshee;
}

[AlignmentOption(ListSlot.ApocDeity, true)]
public static class ApocalypseDeityRoles
{
    [LayerOption("#A7C596FF", LayerEnum.Gluttony, true)]
    public static RoleOptionData Gluttony;

    [LayerOption("#424242FF", LayerEnum.Pestilence, true)]
    public static RoleOptionData Pestilence;

    [LayerOption("#E1E4E4FF", LayerEnum.Void, true)]
    public static RoleOptionData Void;
}

[AlignmentOption(ListSlot.ApocHarb)]
public static class ApocalypseHarbingerRoles
{
    [LayerOption("#8C4005FF", LayerEnum.Cannibal)]
    public static RoleOptionData Cannibal;

    [LayerOption("#99007FFF", LayerEnum.Cultist, true)]
    public static RoleOptionData Cultist;

    [LayerOption("#CFFE61FF", LayerEnum.Plaguebearer)]
    public static RoleOptionData Plaguebearer;
}

[AlignmentOption(ListSlot.GameMode, true)]
public static class GameModeRoles
{
    [LayerOption("#ECC23EFF", LayerEnum.Runner, true)]
    public static RoleOptionData Runner;

    [LayerOption("#FF004EFF", LayerEnum.Hunter, true)]
    public static RoleOptionData Hunter;

    [LayerOption("#1F51FFFF", LayerEnum.Hunted, true)]
    public static RoleOptionData Hunted;
}

[AlignmentOption(ListSlot.Modifiers)]
public static class Modifiers
{
    [LayerOption("#612BEFFF", LayerEnum.Astral)]
    public static RoleOptionData Astral;

    [LayerOption("#00B3B3FF", LayerEnum.Bait)]
    public static RoleOptionData Bait;

    [LayerOption("#B34D99FF", LayerEnum.Colorblind)]
    public static RoleOptionData Colorblind;

    [LayerOption("#456BA8FF", LayerEnum.Coward)]
    public static RoleOptionData Coward;

    [LayerOption("#374D1EFF", LayerEnum.Diseased)]
    public static RoleOptionData Diseased;

    [LayerOption("#758000FF", LayerEnum.Drunk)]
    public static RoleOptionData Drunk;

    [LayerOption("#FF8080FF", LayerEnum.Dwarf)]
    public static RoleOptionData Dwarf;

    [LayerOption("#FFB34DFF", LayerEnum.Giant)]
    public static RoleOptionData Giant;

    [LayerOption("#2DE5BEFF", LayerEnum.Indomitable)]
    public static RoleOptionData Indomitable;

    [LayerOption("#1002C5FF", LayerEnum.Shy)]
    public static RoleOptionData Shy;

    [LayerOption("#DCEE85FF", LayerEnum.Vip)]
    public static RoleOptionData Vip;

    [LayerOption("#FFA60AFF", LayerEnum.Volatile)]
    public static RoleOptionData Volatile;

    [LayerOption("#F6AAB7FF", LayerEnum.Yeller)]
    public static RoleOptionData Yeller;
}

[AlignmentOption(ListSlot.Abilities)]
public static class Abilities
{
    [LayerOption("#073763FF", LayerEnum.Assassin, true)]
    public static RoleOptionData Assassin;

    [LayerOption("#8CFFFFFF", LayerEnum.Bullseye)]
    public static RoleOptionData Bullseye;

    [LayerOption("#E600FFFF", LayerEnum.ButtonBarry)]
    public static RoleOptionData ButtonBarry;

    [LayerOption("#FF1919FF", LayerEnum.Hitman)]
    public static RoleOptionData Hitman;

    [LayerOption("#26FCFBFF", LayerEnum.Insider)]
    public static RoleOptionData Insider;

    [LayerOption("#FF804DFF", LayerEnum.Multitasker)]
    public static RoleOptionData Multitasker;

    [LayerOption("#A84300FF", LayerEnum.Ninja)]
    public static RoleOptionData Ninja;

    [LayerOption("#CCA3CCFF", LayerEnum.Politician)]
    public static RoleOptionData Politician;

    [LayerOption("#FF0080FF", LayerEnum.Radar)]
    public static RoleOptionData Radar;

    [LayerOption("#99007FFF", LayerEnum.Ritualist)]
    public static RoleOptionData Ritualist;

    [LayerOption("#2160DDFF", LayerEnum.Ruthless)]
    public static RoleOptionData Ruthless;

    [LayerOption("#B3B3B3FF", LayerEnum.Slayer)]
    public static RoleOptionData Slayer;

    [LayerOption("#008000FF", LayerEnum.Sniper)]
    public static RoleOptionData Sniper;

    [LayerOption("#D4AF37FF", LayerEnum.Snitch)]
    public static RoleOptionData Snitch;

    [LayerOption("#66E666FF", LayerEnum.Swapper)]
    public static RoleOptionData Swapper;

    [LayerOption("#99E699FF", LayerEnum.Tiebreaker)]
    public static RoleOptionData Tiebreaker;

    [LayerOption("#FFFF99FF", LayerEnum.Torch)]
    public static RoleOptionData Torch;

    [LayerOption("#E91E63FF", LayerEnum.Tunneler)]
    public static RoleOptionData Tunneler;

    [LayerOption("#841A7FFF", LayerEnum.Underdog)]
    public static RoleOptionData Underdog;
}

[AlignmentOption(ListSlot.Dispositions)]
public static class Dispositions
{
    [LayerOption("#4545A9FF", LayerEnum.Allied, All = true)]
    public static RoleOptionData Allied;

    [LayerOption("#4545FFFF", LayerEnum.Corrupted)]
    public static RoleOptionData Corrupted;

    [LayerOption("#E1C849FF", LayerEnum.Defector)]
    public static RoleOptionData Defector;

    [LayerOption("#678D36FF", LayerEnum.Fanatic)]
    public static RoleOptionData Fanatic;

    [LayerOption("#FF351FFF", LayerEnum.Linked, min: 2, max: 14, change: 2)]
    public static RoleOptionData Linked;

    [LayerOption("#FF66CCFF", LayerEnum.Lovers, min: 2, max: 14, change: 2)]
    public static RoleOptionData Lovers;

    [LayerOption("#00EEFFFF", LayerEnum.Mafia, min: 2)]
    public static RoleOptionData Mafia;

    [LayerOption("#008080FF", LayerEnum.Overlord)]
    public static RoleOptionData Overlord;

    [LayerOption("#3D2D2CFF", LayerEnum.Rivals, min: 2, max: 14, change: 2)]
    public static RoleOptionData Rivals;

    [LayerOption("#ABABFFFF", LayerEnum.Taskmaster)]
    public static RoleOptionData Taskmaster;

    [LayerOption("#370D43FF", LayerEnum.Traitor)]
    public static RoleOptionData Traitor;
}

[AlignmentHeaderOption(ListSlot.CrewInvest)]
public static class CrewInvestigativeSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MaxCi
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 1;
}

[AlignmentHeaderOption(ListSlot.CrewKill)]
public static class CrewKillingSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MaxCk
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 1;
}

[AlignmentHeaderOption(ListSlot.CrewProt)]
public static class CrewProtectiveSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MaxCrP
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 1;
}

[AlignmentHeaderOption(ListSlot.CrewSov)]
public static class CrewSovereignSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MaxCSv
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 1;
}

[AlignmentHeaderOption(ListSlot.CrewSupport)]
public static class CrewSupportSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MaxCs
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 1;
}

[AlignmentHeaderOption(ListSlot.OutcastBen)]
public static class OutcastBenignSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MaxNb
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 1;

    [ToggleOption]
    public static bool VigilanteKillsBenigns = true;
}

[AlignmentHeaderOption(ListSlot.OutcastEvil)]
public static class OutcastEvilSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MaxNe
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 1;

    [ToggleOption]
    public static bool OutcastEvilsEndGame = false;

    [ToggleOption]
    public static bool VigilanteKillsEvils = true;

    [ToggleOption]
    public static bool NeHaveImpVision = true;
}

[AlignmentHeaderOption(ListSlot.OutcastKill)]
public static class OutcastKillingSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MaxNk
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 1;

    [ToggleOption]
    public static bool NkHaveImpVision = true;

    [ToggleOption]
    public static bool KnowEachOther = false;

    [ToggleOption]
    public static bool WinSolo = false;
}

[AlignmentHeaderOption(ListSlot.OutcastNeo)]
public static class OutcastNeophyteSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MaxNn
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 1;

    [ToggleOption]
    public static bool NnHaveImpVision = true;
}

[AlignmentHeaderOption(ListSlot.IntruderConceal)]
public static class IntruderConcealingSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MaxIc
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 1;
}

[AlignmentHeaderOption(ListSlot.IntruderDecep)]
public static class IntruderDeceptionSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MaxID
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 1;
}

[AlignmentHeaderOption(ListSlot.IntruderHead)]
public static class IntruderHeadSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MaxIh
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 1;
}

[AlignmentHeaderOption(ListSlot.IntruderKill)]
public static class IntruderKillingSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MaxIK
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 1;

    [ToggleOption]
    public static bool KillCdLinked = false;
}

[AlignmentHeaderOption(ListSlot.IntruderSupport)]
public static class IntruderSupportSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MaxIs
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 1;
}

[AlignmentHeaderOption(ListSlot.SyndicateDisrup)]
public static class SyndicateDisruptionSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MaxSD
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 1;
}

[AlignmentHeaderOption(ListSlot.SyndicateKill)]
public static class SyndicateKillingSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MaxSyK
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 1;
}

[AlignmentHeaderOption(ListSlot.SyndicatePower)]
public static class SyndicatePowerSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MaxSp
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 1;
}

[AlignmentHeaderOption(ListSlot.SyndicateSupport)]
public static class SyndicateSupportSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MaxSSu
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 1;
}

[AlignmentHeaderOption(ListSlot.ApocHarb)]
public static class ApocalypseHarbingerSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MaxAh
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 1;
}

[AlignmentHeaderOption(ListSlot.Modifiers)]
public static class ModifiersSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MinModifiers
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 4;

    [NumberOption(0, 250, 1)]
    public static Number MaxModifiers
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 5;
}

[AlignmentHeaderOption(ListSlot.Abilities)]
public static class AbilitiesSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MinAbilities
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 4;

    [NumberOption(0, 250, 1)]
    public static Number MaxAbilities
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 5;
}

[AlignmentHeaderOption(ListSlot.Dispositions)]
public static class DispositionsSettings
{
    [NumberOption(0, 250, 1)]
    public static Number MinDispositions
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 4;

    [NumberOption(0, 250, 1)]
    public static Number MaxDispositions
    {
        get;
        set => HandleMaxMinLimits.Set(value, ref field);
    } = 5;
}

public static class HandleMaxMinLimits
{
    public static void Set(Number value, ref Number backing)
    {
        if (value > GameOptions.LobbySize)
            value = backing == 0 ? GameOptions.LobbySize : 0;

        backing = value;
    }
}