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

    public static ReworkedNumberOption LobbySize = new(2, 250, 1, defaultValue: 15);

    [StringOption<GhostSpawnType>]
    public static GhostSpawnType GhostSpawn = GhostSpawnType.Random;

    [ToggleOption]
    public static bool ShowTasks = true;
}

[HeaderOption(MultiMenu.Main)]
public static class GameModeSettings
{
    [StringOption<Mode>([Mode.None])]
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
    private static int EntryCount;
    private static int BanCount;

    public static ListHolderOption RolesEntryList;
    public static ListHolderOption ModifiersEntryList;
    public static ListHolderOption AbilitiesEntryList;
    public static ListHolderOption DispositionsEntryList;

    public static ListHolderOption RolesBanList;
    public static ListHolderOption ModifiersBanList;
    public static ListHolderOption AbilitiesBanList;
    public static ListHolderOption DispositionsBanList;

    public static void EnsureCount()
    {
        while (EntryCount < GameData.Instance.PlayerCount)
        {
            RolesEntryList.AddEntryForPlayer();
            ModifiersEntryList.AddEntryForPlayer();
            AbilitiesEntryList.AddEntryForPlayer();
            DispositionsEntryList.AddEntryForPlayer();

            EntryCount++;
        }

        while (BanCount < GameData.Instance.PlayerCount / 3)
        {
            RolesBanList.AddEntryForPlayer();
            ModifiersBanList.AddEntryForPlayer();
            AbilitiesBanList.AddEntryForPlayer();
            DispositionsBanList.AddEntryForPlayer();

            BanCount++;
        }
    }
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
    public static Faction MainBadGuys => IlluminatiUnleashed ? Faction.Illuminati : MainBadFaction;
    private static StringOption<Faction> MainBadFaction = new(include: [Faction.Compliance, Faction.Syndicate, Faction.Apocalypse, Faction.Intruder, Faction.Pandorica], defaultValue:
        Faction.Intruder) { ModifyValue = ValidateMainBadFaction };

    [ToggleOption]
    public static bool OnlyMainBadGuys = false;

    [ToggleOption]
    public static bool MainBadGuysCanSabotage = false;

    [ToggleOption]
    public static bool GhostsCanSabotage = false;

    [ToggleOption]
    public static bool IlluminatiUnleashed = false;

    [MultiSelectOption<IlluminatiType>(LeastSelected = 2)]
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
            case false when Allied.AlliedFactionBacking == AlliedFaction.Pandorica:
            {
                Allied.AlliedFactionBacking = AlliedFaction.Random;
                break;
            }
            case true when Allied.AlliedFactionBacking is AlliedFaction.Intruder or AlliedFaction.Syndicate or AlliedFaction.Apocalypse:
            {
                Allied.AlliedFactionBacking = AlliedFaction.Pandorica;
                break;
            }
        }
    }

    private static Faction ValidateMainBadFaction(Faction value, Faction field)
    {
        if (value == Faction.Compliance && !OrderOfCompliance)
            value = value < field ? Faction.Intruder : (PandoricaOpens ? Faction.Pandorica : Faction.Apocalypse);

        if (value is Faction.Intruder or Faction.Syndicate or Faction.Apocalypse && PandoricaOpens)
            value = Faction.Pandorica;

        if (value == Faction.Pandorica && !PandoricaOpens)
            value = value < field ? Faction.Apocalypse : (OrderOfCompliance ? Faction.Compliance : Faction.Intruder);

        return value;
    }
}

[HeaderOption(MultiMenu.Main)]
public static class GameModifiers
{
    [ToggleOption]
    public static bool FirstKillShield = false;

    [StringOption<WhoCanSeeFirstKillShield>]
    public static WhoCanSeeFirstKillShield WhoSeesFirstKillShield = WhoCanSeeFirstKillShield.Everyone;

    [ToggleOption]
    public static bool FactionSeeRoles = true;

    [StringOption<PlayerNames>]
    public static PlayerNames PlayerNames = PlayerNames.Obstructed;

    [ToggleOption]
    public static bool Whispers = true;

    [ToggleOption]
    public static bool WhispersAnnouncement = true;

    [ToggleOption]
    public static bool AppearanceAnimation = true;

    [ToggleOption]
    public static bool EnableAbilities = true;

    [ToggleOption]
    public static bool EnableModifiers = true;

    [ToggleOption]
    public static bool EnableDispositions = true;

    [ToggleOption]
    public static bool DeadSeeEverything = true;

    [ToggleOption]
    public static bool JaniCanMutuallyExclusive = false;

    [MultiSelectOption<RandomSpawning>]
    public static MultiSelectValue<RandomSpawning> RandomSpawns = [];

    [ToggleOption]
    public static bool ShowKillerRoleColor = false;

    [ToggleOption]
    public static bool PurePlayers = false;

    public static ReworkedToggleOption EnableGameTimer = new() { OnChanged = TryCreatePrefab };

    [NumberOption(60, 3600, 60, Format.Time)]
    public static Number GameTimer = 1200;

    [StringOption<DuringMeeting>]
    public static DuringMeeting DuringMeetings = DuringMeeting.Never;

    [NumberOption(300, 3000, 60, Format.Time, All = true)]
    public static Number TimeLeft = 4;

    [NumberOption(4, 125, 1, All = true)]
    public static Number PlayersLeft = 4;

    [MultiSelectOption<TimerExtension>]
    public static MultiSelectValue<TimerExtension> ExtendTimer = [];

    [NumberOption(4, 125, 1)]
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

    [ToggleOption]
    public static bool IndicateReportedBodies = false;
}

[HeaderOption(MultiMenu.Main)]
public static class MapSettings
{
    public static Map Map
    {
        get;
        set
        {
            if (value == Map.LevelImpostor && !LiLoaded)
                value = field < Map.LevelImpostor ? Map.Random : Map.Submerged;

            if (value == Map.Submerged && !SubLoaded)
                value = field < Map.Submerged ? Map.Random : Map.Fungle;

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
    [LayerOption("#4D99E6FF", Layer.Coroner)]
    public static RoleOptionData Coroner;

    [LayerOption("#4D4DFFFF", Layer.Detective)]
    public static RoleOptionData Detective;

    [LayerOption("#A680FFFF", Layer.Medium)]
    public static RoleOptionData Medium;

    [LayerOption("#708EEFFF", Layer.Mystic)]
    public static RoleOptionData Mystic;

    [LayerOption("#A7D1B3FF", Layer.Operative)]
    public static RoleOptionData Operative;

    [LayerOption("#71368AFF", Layer.Seer)]
    public static RoleOptionData Seer;

    [LayerOption("#FFCC80FF", Layer.Sheriff)]
    public static RoleOptionData Sheriff;

    [LayerOption("#009900FF", Layer.Tracker)]
    public static RoleOptionData Tracker;
}

[AlignmentOption(ListSlot.CrewKill)]
public static class CrewKillingRoles
{
    [LayerOption("#7E3C64FF", Layer.Bastion)]
    public static RoleOptionData Bastion;

    [LayerOption("#998040FF", Layer.Veteran)]
    public static RoleOptionData Veteran;

    [LayerOption("#FFFF00FF", Layer.Vigilante)]
    public static RoleOptionData Vigilante;
}

[AlignmentOption(ListSlot.CrewProt)]
public static class CrewProtectiveRoles
{
    [LayerOption("#660000FF", Layer.Altruist)]
    public static RoleOptionData Altruist;

    [LayerOption("#006600FF", Layer.Medic)]
    public static RoleOptionData Medic;

    [LayerOption("#BE1C8CFF", Layer.Trapper)]
    public static RoleOptionData Trapper;
}

[AlignmentOption(ListSlot.CrewSov)]
public static class CrewSovereignRoles
{
    [LayerOption("#1A3270FF", Layer.Democrat)]
    public static RoleOptionData Democrat;

    [LayerOption("#00CB97FF", Layer.Dictator)]
    public static RoleOptionData Dictator;

    [LayerOption("#704FA8FF", Layer.Mayor, true)]
    public static RoleOptionData Mayor;

    [LayerOption("#FF004EFF", Layer.Monarch)]
    public static RoleOptionData Monarch;
}

[AlignmentOption(ListSlot.CrewSupport)]
public static class CrewSupportRoles
{
    [LayerOption("#5411F8FF", Layer.Chameleon)]
    public static RoleOptionData Chameleon;

    [LayerOption("#FFA60AFF", Layer.Engineer)]
    public static RoleOptionData Engineer;

    [LayerOption("#803333FF", Layer.Escort)]
    public static RoleOptionData Escort;

    [LayerOption("#8D0F8CFF", Layer.Retributionist)]
    public static RoleOptionData Retributionist;

    [LayerOption("#00EEFFFF", Layer.Transporter)]
    public static RoleOptionData Transporter;
}

[AlignmentOption(ListSlot.CrewUtil)]
public static class CrewUtilityRoles
{
    [LayerOption("#8CFFFFFF", Layer.Crewmate, true)]
    public static RoleOptionData Crewmate;

    [LayerOption("#D3D3D3FF", Layer.Revealer)]
    public static RoleOptionData Revealer;
}

[AlignmentOption(ListSlot.OutcastBen)]
public static class OutcastBenignRoles
{
    [LayerOption("#22FFFFFF", Layer.Amnesiac)]
    public static RoleOptionData Amnesiac;

    [LayerOption("#FFFFFFFF", Layer.GuardianAngel)]
    public static RoleOptionData GuardianAngel;

    [LayerOption("#DDDD00FF", Layer.Survivor)]
    public static RoleOptionData Survivor;

    [LayerOption("#80FF00FF", Layer.Thief)]
    public static RoleOptionData Thief;
}

[AlignmentOption(ListSlot.OutcastEvil)]
public static class OutcastEvilRoles
{
    [LayerOption("#00ACC2FF", Layer.Actor)]
    public static RoleOptionData Actor;

    [LayerOption("#B51E39FF", Layer.BountyHunter)]
    public static RoleOptionData BountyHunter;

    [LayerOption("#CCCCCCFF", Layer.Executioner)]
    public static RoleOptionData Executioner;

    [LayerOption("#EEE5BEFF", Layer.Guesser)]
    public static RoleOptionData Guesser;

    [LayerOption("#F7B3DAFF", Layer.Jester)]
    public static RoleOptionData Jester;

    [LayerOption("#DF851FFF", Layer.Shifter)]
    public static RoleOptionData Shifter;

    [LayerOption("#678D36FF", Layer.Troll)]
    public static RoleOptionData Troll;
}

[AlignmentOption(ListSlot.OutcastKill)]
public static class OutcastKillingRoles
{
    [LayerOption("#EE7600FF", Layer.Arsonist)]
    public static RoleOptionData Arsonist;

    [LayerOption("#642DEAFF", Layer.Cryomaniac)]
    public static RoleOptionData Cryomaniac;

    [LayerOption("#00FF00FF", Layer.Glitch)]
    public static RoleOptionData Glitch;

    [LayerOption("#A12B56FF", Layer.Juggernaut)]
    public static RoleOptionData Juggernaut;

    [LayerOption("#6F7BEAFF", Layer.Murderer)]
    public static RoleOptionData Murderer;

    [LayerOption("#336EFFFF", Layer.SerialKiller)]
    public static RoleOptionData SerialKiller;

    [LayerOption("#9F703AFF", Layer.Werewolf)]
    public static RoleOptionData Werewolf;
}

[AlignmentOption(ListSlot.OutcastNeo)]
public static class OutcastNeophyteRoles
{
    [LayerOption("#AC8A00FF", Layer.Dracula)]
    public static RoleOptionData Dracula;

    [LayerOption("#45076AFF", Layer.Jackal)]
    public static RoleOptionData Jackal;

    [LayerOption("#BF5FFFFF", Layer.Necromancer)]
    public static RoleOptionData Necromancer;

    [LayerOption("#2D6AA5FF", Layer.Whisperer)]
    public static RoleOptionData Whisperer;

    [LayerOption("#7EFBC2FF", Layer.Zealot)]
    public static RoleOptionData Zealot;
}

[AlignmentOption(ListSlot.OutcastPros)]
public static class OutcastProselyteRoles
{
    [LayerOption("#11806AFF", Layer.Betrayer, true)]
    public static RoleOptionData Betrayer;

    [LayerOption("#662962FF", Layer.Phantom)]
    public static RoleOptionData Phantom;
}

[AlignmentOption(ListSlot.IntruderConceal)]
public static class IntruderConcealingRoles
{
    [LayerOption("#02A752FF", Layer.Blackmailer)]
    public static RoleOptionData Blackmailer;

    [LayerOption("#378AC0FF", Layer.Camouflager)]
    public static RoleOptionData Camouflager;

    [LayerOption("#85AA5BFF", Layer.Grenadier)]
    public static RoleOptionData Grenadier;

    [LayerOption("#2647A2FF", Layer.Janitor)]
    public static RoleOptionData Janitor;
}

[AlignmentOption(ListSlot.IntruderDecep)]
public static class IntruderDeceptionRoles
{
    [LayerOption("#40B4FFFF", Layer.Disguiser)]
    public static RoleOptionData Disguiser;

    [LayerOption("#BB45B0FF", Layer.Morphling)]
    public static RoleOptionData Morphling;

    [LayerOption("#5C4F75FF", Layer.Wraith)]
    public static RoleOptionData Wraith;
}

[AlignmentOption(ListSlot.IntruderHead)]
public static class IntruderHeadRoles
{
    [LayerOption("#404C08FF", Layer.Godfather)]
    public static RoleOptionData Godfather;
}

[AlignmentOption(ListSlot.IntruderKill)]
public static class IntruderKillingRoles
{
    [LayerOption("#2BD29CFF", Layer.Ambusher)]
    public static RoleOptionData Ambusher;

    [LayerOption("#005643FF", Layer.Enforcer)]
    public static RoleOptionData Enforcer;
}

[AlignmentOption(ListSlot.IntruderSupport)]
public static class IntruderSupportRoles
{
    [LayerOption("#FFFF99FF", Layer.Consigliere)]
    public static RoleOptionData Consigliere;

    [LayerOption("#801780FF", Layer.Consort)]
    public static RoleOptionData Consort;

    [LayerOption("#AA7632FF", Layer.Miner)]
    public static RoleOptionData Miner;

    [LayerOption("#939593FF", Layer.Teleporter)]
    public static RoleOptionData Teleporter;
}

[AlignmentOption(ListSlot.IntruderUtil)]
public static class IntruderUtilityRoles
{
    [LayerOption("#F1C40FFF", Layer.Ghoul)]
    public static RoleOptionData Ghoul;

    [LayerOption("#FF1919FF", Layer.Impostor, true)]
    public static RoleOptionData Impostor;
}

[AlignmentOption(ListSlot.SyndicateDisrup)]
public static class SyndicateDisruptionRoles
{
    [LayerOption("#C02525FF", Layer.Concealer)]
    public static RoleOptionData Concealer;

    [LayerOption("#FF7900FF", Layer.Drunkard)]
    public static RoleOptionData Drunkard;

    [LayerOption("#00FFFFFF", Layer.Framer)]
    public static RoleOptionData Framer;

    [LayerOption("#2DFF00FF", Layer.Shapeshifter)]
    public static RoleOptionData Shapeshifter;

    [LayerOption("#AAB43EFF", Layer.Silencer)]
    public static RoleOptionData Silencer;

    [LayerOption("#3769FEFF", Layer.Timekeeper)]
    public static RoleOptionData Timekeeper;
}

[AlignmentOption(ListSlot.SyndicateKill)]
public static class SyndicateKillingRoles
{
    [LayerOption("#C9CC3FFF", Layer.Bomber)]
    public static RoleOptionData Bomber;

    [LayerOption("#B345FFFF", Layer.Collider)]
    public static RoleOptionData Collider;

    [LayerOption("#DF7AE8FF", Layer.Crusader)]
    public static RoleOptionData Crusader;

    [LayerOption("#B5004CFF", Layer.Poisoner)]
    public static RoleOptionData Poisoner;
}

[AlignmentOption(ListSlot.SyndicateHead)]
public static class SyndicateHeadRoles
{
    [LayerOption("#FFFCCEFF", Layer.Rebel)]
    public static RoleOptionData Rebel;

    [LayerOption("#0028F5FF", Layer.Spellslinger)]
    public static RoleOptionData Spellslinger;
}

[AlignmentOption(ListSlot.SyndicateSupport)]
public static class SyndicateSupportRoles
{
    [LayerOption("#7E4D00FF", Layer.Stalker)]
    public static RoleOptionData Stalker;

    [LayerOption("#8C7140FF", Layer.Warper)]
    public static RoleOptionData Warper;
}

[AlignmentOption(ListSlot.SyndicateUtil)]
public static class SyndicateUtilityRoles
{
    [LayerOption("#008000FF", Layer.Anarchist, true)]
    public static RoleOptionData Anarchist;

    [LayerOption("#E67E22FF", Layer.Banshee)]
    public static RoleOptionData Banshee;
}

[AlignmentOption(ListSlot.ApocDeity, true)]
public static class ApocalypseDeityRoles
{
    [LayerOption("#A7C596FF", Layer.Gluttony, true)]
    public static RoleOptionData Gluttony;

    [LayerOption("#424242FF", Layer.Pestilence, true)]
    public static RoleOptionData Pestilence;

    [LayerOption("#E1E4E4FF", Layer.Void, true)]
    public static RoleOptionData Void;
}

[AlignmentOption(ListSlot.ApocHarb)]
public static class ApocalypseHarbingerRoles
{
    [LayerOption("#8C4005FF", Layer.Cannibal)]
    public static RoleOptionData Cannibal;

    [LayerOption("#99007FFF", Layer.Cultist, true)]
    public static RoleOptionData Cultist;

    [LayerOption("#CFFE61FF", Layer.Plaguebearer)]
    public static RoleOptionData Plaguebearer;
}

[AlignmentOption(ListSlot.GameMode, true)]
public static class GameModeRoles
{
    [LayerOption("#ECC23EFF", Layer.Runner, true)]
    public static RoleOptionData Runner;

    [LayerOption("#FF004EFF", Layer.Hunter, true)]
    public static RoleOptionData Hunter;

    [LayerOption("#1F51FFFF", Layer.Hunted, true)]
    public static RoleOptionData Hunted;
}

[AlignmentOption(ListSlot.Modifiers)]
public static class Modifiers
{
    [LayerOption("#612BEFFF", Layer.Astral)]
    public static RoleOptionData Astral;

    [LayerOption("#00B3B3FF", Layer.Bait)]
    public static RoleOptionData Bait;

    [LayerOption("#B34D99FF", Layer.Colorblind)]
    public static RoleOptionData Colorblind;

    [LayerOption("#456BA8FF", Layer.Coward)]
    public static RoleOptionData Coward;

    [LayerOption("#374D1EFF", Layer.Diseased)]
    public static RoleOptionData Diseased;

    [LayerOption("#758000FF", Layer.Drunk)]
    public static RoleOptionData Drunk;

    [LayerOption("#FF8080FF", Layer.Dwarf)]
    public static RoleOptionData Dwarf;

    [LayerOption("#FFB34DFF", Layer.Giant)]
    public static RoleOptionData Giant;

    [LayerOption("#2DE5BEFF", Layer.Indomitable)]
    public static RoleOptionData Indomitable;

    [LayerOption("#1002C5FF", Layer.Shy)]
    public static RoleOptionData Shy;

    [LayerOption("#DCEE85FF", Layer.Vip)]
    public static RoleOptionData Vip;

    [LayerOption("#FFA60AFF", Layer.Volatile)]
    public static RoleOptionData Volatile;

    [LayerOption("#F6AAB7FF", Layer.Yeller)]
    public static RoleOptionData Yeller;
}

[AlignmentOption(ListSlot.Abilities)]
public static class Abilities
{
    [LayerOption("#073763FF", Layer.Assassin, true)]
    public static RoleOptionData Assassin;

    [LayerOption("#8CFFFFFF", Layer.Bullseye)]
    public static RoleOptionData Bullseye;

    [LayerOption("#E600FFFF", Layer.ButtonBarry)]
    public static RoleOptionData ButtonBarry;

    [LayerOption("#FF1919FF", Layer.Hitman)]
    public static RoleOptionData Hitman;

    [LayerOption("#26FCFBFF", Layer.Insider)]
    public static RoleOptionData Insider;

    [LayerOption("#FF804DFF", Layer.Multitasker)]
    public static RoleOptionData Multitasker;

    [LayerOption("#A84300FF", Layer.Ninja)]
    public static RoleOptionData Ninja;

    [LayerOption("#CCA3CCFF", Layer.Politician)]
    public static RoleOptionData Politician;

    [LayerOption("#FF0080FF", Layer.Radar)]
    public static RoleOptionData Radar;

    [LayerOption("#99007FFF", Layer.Ritualist)]
    public static RoleOptionData Ritualist;

    [LayerOption("#2160DDFF", Layer.Ruthless)]
    public static RoleOptionData Ruthless;

    [LayerOption("#B3B3B3FF", Layer.Slayer)]
    public static RoleOptionData Slayer;

    [LayerOption("#008000FF", Layer.Sniper)]
    public static RoleOptionData Sniper;

    [LayerOption("#D4AF37FF", Layer.Snitch)]
    public static RoleOptionData Snitch;

    [LayerOption("#66E666FF", Layer.Swapper)]
    public static RoleOptionData Swapper;

    [LayerOption("#99E699FF", Layer.Tiebreaker)]
    public static RoleOptionData Tiebreaker;

    [LayerOption("#FFFF99FF", Layer.Torch)]
    public static RoleOptionData Torch;

    [LayerOption("#E91E63FF", Layer.Tunneler)]
    public static RoleOptionData Tunneler;

    [LayerOption("#841A7FFF", Layer.Underdog)]
    public static RoleOptionData Underdog;
}

[AlignmentOption(ListSlot.Dispositions)]
public static class Dispositions
{
    [LayerOption("#4545A9FF", Layer.Allied, All = true)]
    public static RoleOptionData Allied;

    [LayerOption("#4545FFFF", Layer.Corrupted)]
    public static RoleOptionData Corrupted;

    [LayerOption("#E1C849FF", Layer.Defector)]
    public static RoleOptionData Defector;

    [LayerOption("#678D36FF", Layer.Fanatic)]
    public static RoleOptionData Fanatic;

    [LayerOption("#FF351FFF", Layer.Linked, min: 2, max: 14, change: 2)]
    public static RoleOptionData Linked;

    [LayerOption("#FF66CCFF", Layer.Lovers, min: 2, max: 14, change: 2)]
    public static RoleOptionData Lovers;

    [LayerOption("#00EEFFFF", Layer.Mafia, min: 2)]
    public static RoleOptionData Mafia;

    [LayerOption("#008080FF", Layer.Overlord)]
    public static RoleOptionData Overlord;

    [LayerOption("#3D2D2CFF", Layer.Rivals, min: 2, max: 14, change: 2)]
    public static RoleOptionData Rivals;

    [LayerOption("#ABABFFFF", Layer.Taskmaster)]
    public static RoleOptionData Taskmaster;

    [LayerOption("#370D43FF", Layer.Traitor)]
    public static RoleOptionData Traitor;
}

[AlignmentHeaderOption(ListSlot.CrewInvest)]
public static class CrewInvestigativeSettings
{
    public static ReworkedNumberOption MaxCi = new(0, 250, 1, defaultValue: 1) { ModifyValue = HandleMaxMinLimits.Set };
}

[AlignmentHeaderOption(ListSlot.CrewKill)]
public static class CrewKillingSettings
{
    public static ReworkedNumberOption MaxCk = new(0, 250, 1, defaultValue: 1) { ModifyValue = HandleMaxMinLimits.Set };
}

[AlignmentHeaderOption(ListSlot.CrewProt)]
public static class CrewProtectiveSettings
{
    public static ReworkedNumberOption MaxCrP = new(0, 250, 1, defaultValue: 1) { ModifyValue = HandleMaxMinLimits.Set };
}

[AlignmentHeaderOption(ListSlot.CrewSov)]
public static class CrewSovereignSettings
{
    public static ReworkedNumberOption MaxCSv = new(0, 250, 1, defaultValue: 1) { ModifyValue = HandleMaxMinLimits.Set };
}

[AlignmentHeaderOption(ListSlot.CrewSupport)]
public static class CrewSupportSettings
{
    public static ReworkedNumberOption MaxCs = new(0, 250, 1, defaultValue: 1) { ModifyValue = HandleMaxMinLimits.Set };
}

[AlignmentHeaderOption(ListSlot.OutcastBen)]
public static class OutcastBenignSettings
{
    public static ReworkedNumberOption MaxNb = new(0, 250, 1, defaultValue: 1) { ModifyValue = HandleMaxMinLimits.Set };

    [ToggleOption]
    public static bool VigilanteKillsBenigns = true;
}

[AlignmentHeaderOption(ListSlot.OutcastEvil)]
public static class OutcastEvilSettings
{
    public static ReworkedNumberOption MaxNe = new(0, 250, 1, defaultValue: 1) { ModifyValue = HandleMaxMinLimits.Set };

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
    public static ReworkedNumberOption MaxNk = new(0, 250, 1, defaultValue: 1) { ModifyValue = HandleMaxMinLimits.Set };

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
    public static ReworkedNumberOption MaxNn = new(0, 250, 1, defaultValue: 1) { ModifyValue = HandleMaxMinLimits.Set };

    [ToggleOption]
    public static bool NnHaveImpVision = true;
}

[AlignmentHeaderOption(ListSlot.IntruderConceal)]
public static class IntruderConcealingSettings
{
    public static ReworkedNumberOption MaxIc = new(0, 250, 1, defaultValue: 1) { ModifyValue = HandleMaxMinLimits.Set };
}

[AlignmentHeaderOption(ListSlot.IntruderDecep)]
public static class IntruderDeceptionSettings
{
    public static ReworkedNumberOption MaxID = new(0, 250, 1, defaultValue: 1) { ModifyValue = HandleMaxMinLimits.Set };
}

[AlignmentHeaderOption(ListSlot.IntruderHead)]
public static class IntruderHeadSettings
{
    public static ReworkedNumberOption MaxIh = new(0, 250, 1, defaultValue: 1) { ModifyValue = HandleMaxMinLimits.Set };
}

[AlignmentHeaderOption(ListSlot.IntruderKill)]
public static class IntruderKillingSettings
{
    public static ReworkedNumberOption MaxIK = new(0, 250, 1, defaultValue: 1) { ModifyValue = HandleMaxMinLimits.Set };

    [ToggleOption]
    public static bool KillCdLinked = false;
}

[AlignmentHeaderOption(ListSlot.IntruderSupport)]
public static class IntruderSupportSettings
{
    public static ReworkedNumberOption MaxIs = new(0, 250, 1, defaultValue: 1) { ModifyValue = HandleMaxMinLimits.Set };
}

[AlignmentHeaderOption(ListSlot.SyndicateDisrup)]
public static class SyndicateDisruptionSettings
{
    public static ReworkedNumberOption MaxSD = new(0, 250, 1, defaultValue: 1) { ModifyValue = HandleMaxMinLimits.Set };
}

[AlignmentHeaderOption(ListSlot.SyndicateKill)]
public static class SyndicateKillingSettings
{
    public static ReworkedNumberOption MaxSyK = new(0, 250, 1, defaultValue: 1) { ModifyValue = HandleMaxMinLimits.Set };
}

[AlignmentHeaderOption(ListSlot.SyndicateHead)]
public static class SyndicateHeadSettings
{
    public static ReworkedNumberOption MaxSh = new(0, 250, 1, defaultValue: 1) { ModifyValue = HandleMaxMinLimits.Set };
}

[AlignmentHeaderOption(ListSlot.SyndicateSupport)]
public static class SyndicateSupportSettings
{
    public static ReworkedNumberOption MaxSSu = new(0, 250, 1, defaultValue: 1) { ModifyValue = HandleMaxMinLimits.Set };
}

[AlignmentHeaderOption(ListSlot.ApocHarb)]
public static class ApocalypseHarbingerSettings
{
    public static ReworkedNumberOption MaxAh = new(0, 250, 1, defaultValue: 1) { ModifyValue = HandleMaxMinLimits.Set };
}

[AlignmentHeaderOption(ListSlot.Modifiers)]
public static class ModifiersSettings
{
    public static ReworkedNumberOption MinModifiers = new(0, 250, 1, defaultValue: 4) { ModifyValue = HandleMaxMinLimits.Set };
    public static ReworkedNumberOption MaxModifiers = new(0, 250, 1, defaultValue: 5) { ModifyValue = HandleMaxMinLimits.Set };
}

[AlignmentHeaderOption(ListSlot.Abilities)]
public static class AbilitiesSettings
{
    public static ReworkedNumberOption MinAbilities = new(0, 250, 1, defaultValue: 4) { ModifyValue = HandleMaxMinLimits.Set };
    public static ReworkedNumberOption MaxAbilities = new(0, 250, 1, defaultValue: 5) { ModifyValue = HandleMaxMinLimits.Set };
}

[AlignmentHeaderOption(ListSlot.Dispositions)]
public static class DispositionsSettings
{
    public static ReworkedNumberOption MinDispositions = new(0, 250, 1, defaultValue: 4) { ModifyValue = HandleMaxMinLimits.Set };
    public static ReworkedNumberOption MaxDispositions = new(0, 250, 1, defaultValue: 5) { ModifyValue = HandleMaxMinLimits.Set };
}

public static class HandleMaxMinLimits
{
    public static Number Set(Number value, Number backing)
    {
        if (value > GameOptions.LobbySize)
            value = backing == 0 ? GameOptions.LobbySize : 0;

        return value;
    }
}