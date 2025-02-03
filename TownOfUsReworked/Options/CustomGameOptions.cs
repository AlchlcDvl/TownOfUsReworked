namespace TownOfUsReworked.Options;

// DO NOT OVERRIDE VALUES OF ANY OF THE OPTION PROPERTIES ANY WHERE IN THE CODE OR ELSE THE OPTIONS WILL START TO FUCK OFF

[HeaderOption(MultiMenu.Main)]
public static class GameSettings
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

    [StringOption<TBMode>]
    private static TBMode TaskBar = TBMode.MeetingOnly;
    public static TBMode TaskBarMode => GameModeSettings.GameMode switch
    {
        GameMode.TaskRace or GameMode.HideAndSeek => TBMode.Normal,
        _ => TaskBar
    }; // I want this to actually change according to the game modes

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

    [NumberOption(1, 20, 0.25f, Format.Distance)]
    public static Number ReportDistance = 3.5f;

    [NumberOption(0, 3, 0.1f, Format.Time)]
    public static Number ChatCooldown = 3;

    [NumberOption(0, 2000, 50, zeroIsInf: true)]
    public static Number ChatCharacterLimit = 200;

    [NumberOption(2, 127, 1)]
    public static Number LobbySize = 15;
}

[HeaderOption(MultiMenu.Main)]
public static class GameModeSettings
{
    [StringOption<GameMode>(GameMode.None)]
    public static GameMode GameMode = GameMode.Classic;

    [ToggleOption]
    public static bool IgnoreFactionCaps = true;

    [ToggleOption]
    public static bool IgnoreAlignmentCaps = true;

    [ToggleOption]
    public static bool IgnoreLayerCaps = true;

    [StringOption<HnSMode>]
    public static HnSMode HnSMode = HnSMode.Classic;

    [NumberOption(1, 13, 1)]
    public static Number HunterCount = 1;

    [NumberOption(5f, 60f, 5f, Format.Time)]
    public static Number HuntCd = 10;

    [NumberOption(5f, 60f, 5f, Format.Time)]
    public static Number StartTime = 10;

    [ToggleOption]
    public static bool HunterVent = true;

    [NumberOption(0.1f, 1f, 0.05f, Format.Multiplier)]
    public static Number HunterVision = 0.25f;

    [NumberOption(1f, 2f, 0.05f, Format.Multiplier)]
    public static Number HuntedVision = 1.5f;

    [NumberOption(1f, 1.5f, 0.05f, Format.Multiplier)]
    public static Number HunterSpeedModifier = 1.25f;

    [ToggleOption]
    public static bool HunterFlashlight = false;

    [ToggleOption]
    public static bool HuntedFlashlight = false;

    [ToggleOption]
    public static bool HuntedChat = true;

    [ListEntry(PlayerLayerEnum.Role)]
    public static List<Enum> RoleEntry1 = [ RoleListSlot.None ];

    [ListEntry(PlayerLayerEnum.Role)]
    public static List<Enum> RoleEntry2 = [ RoleListSlot.None ];

    [ListEntry(PlayerLayerEnum.Role)]
    public static List<Enum> RoleEntry3 = [ RoleListSlot.None ];

    [ListEntry(PlayerLayerEnum.Role)]
    public static List<Enum> RoleEntry4 = [ RoleListSlot.None ];

    [ListEntry(PlayerLayerEnum.Role)]
    public static List<Enum> RoleEntry5 = [ RoleListSlot.None ];

    [ListEntry(PlayerLayerEnum.Role)]
    public static List<Enum> RoleEntry6 = [ RoleListSlot.None ];

    [ListEntry(PlayerLayerEnum.Role)]
    public static List<Enum> RoleEntry7 = [ RoleListSlot.None ];

    [ListEntry(PlayerLayerEnum.Role)]
    public static List<Enum> RoleEntry8 = [ RoleListSlot.None ];

    [ListEntry(PlayerLayerEnum.Role)]
    public static List<Enum> RoleEntry9 = [ RoleListSlot.None ];

    [ListEntry(PlayerLayerEnum.Role)]
    public static List<Enum> RoleEntry10 = [ RoleListSlot.None ];

    [ListEntry(PlayerLayerEnum.Role)]
    public static List<Enum> RoleEntry11 = [ RoleListSlot.None ];

    [ListEntry(PlayerLayerEnum.Role)]
    public static List<Enum> RoleEntry12 = [ RoleListSlot.None ];

    [ListEntry(PlayerLayerEnum.Role)]
    public static List<Enum> RoleEntry13 = [ RoleListSlot.None ];

    [ListEntry(PlayerLayerEnum.Role)]
    public static List<Enum> RoleEntry14 = [ RoleListSlot.None ];

    [ListEntry(PlayerLayerEnum.Role)]
    public static List<Enum> RoleEntry15 = [ RoleListSlot.None ];

    [NumberOption(0, 15, 1)]
    public static Number RevealerCount = 0;

    [NumberOption(0, 15, 1)]
    public static Number PhantomCount = 0;

    [NumberOption(0, 15, 1)]
    public static Number GhoulCount = 0;

    [NumberOption(0, 15, 1)]
    public static Number BansheeCount = 0;

    [ListEntry(PlayerLayerEnum.Role)]
    public static List<Enum> RoleBan1 = [ RoleListSlot.None ];

    [ListEntry(PlayerLayerEnum.Role)]
    public static List<Enum> RoleBan2 = [ RoleListSlot.None ];

    [ListEntry(PlayerLayerEnum.Role)]
    public static List<Enum> RoleBan3 = [ RoleListSlot.None ];

    [ListEntry(PlayerLayerEnum.Role)]
    public static List<Enum> RoleBan4 = [ RoleListSlot.None ];

    [ListEntry(PlayerLayerEnum.Role)]
    public static List<Enum> RoleBan5 = [ RoleListSlot.None ];

    [ToggleOption]
    public static bool BanCrewmate = true;

    [ToggleOption]
    public static bool BanMurderer = true;

    [ToggleOption]
    public static bool BanImpostor = true;

    [ToggleOption]
    public static bool BanAnarchist = true;
}

[HeaderOption(MultiMenu.Main)]
public static class TaskSettings
{
    [NumberOption(0, 100, 1)]
    public static Number CommonTasks = 2;

    [NumberOption(0, 100, 1)]
    public static Number LongTasks = 1;

    [NumberOption(0, 100, 1)]
    public static Number ShortTasks = 4;

    [ToggleOption]
    public static bool GhostTasksCountToWin = true;
}

[HeaderOption(MultiMenu.Main)]
public static class GameModifiers
{
    [StringOption<WhoCanVentOptions>]
    public static WhoCanVentOptions WhoCanVent = WhoCanVentOptions.Default;

    [StringOption<AnonVotes>]
    public static AnonVotes AnonymousVoting = AnonVotes.Enabled;

    [StringOption<DisableSkipButtonMeetings>]
    public static DisableSkipButtonMeetings NoSkipping = DisableSkipButtonMeetings.Never;

    [ToggleOption]
    public static bool FirstKillShield = false;

    [StringOption<WhoCanSeeFirstKillShield>]
    public static WhoCanSeeFirstKillShield WhoSeesFirstKillShield = WhoCanSeeFirstKillShield.Everyone;

    [ToggleOption]
    public static bool FactionSeeRoles = true;

    [ToggleOption]
    public static bool VisualTasks = false;

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
    public static bool VentTargeting = true;

    [ToggleOption]
    public static bool CooldownInVent = false;

    [ToggleOption]
    public static bool DeadSeeEverything = true;

    [ToggleOption]
    public static bool ParallelMedScans = false;

    [ToggleOption]
    public static bool HideVentAnims = true;

    [ToggleOption]
    public static bool JaniCanMutuallyExclusive = false;

    [ToggleOption]
    public static bool IndicateReportedBodies = false;

    [StringOption<RandomSpawning>]
    public static RandomSpawning RandomSpawns = RandomSpawning.Disabled;

    [ToggleOption]
    public static bool ShowKillerRoleColor = false;

    [ToggleOption]
    public static bool PurePlayers = false;

    [ToggleOption]
    public static bool NoVentingUncleanedVents = false;

    [ToggleOption]
    public static bool PandoricaOpens = false;

    [ToggleOption]
    public static bool OrderOfCompliance = false;

    [ToggleOption]
    public static bool IlluminatiUnleashed = false;
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
}

[HeaderOption(MultiMenu.Main)]
public static class MapSettings
{
    public static MapEnum Map;

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
    public static Number CrewMax = 5;

    [NumberOption(0, 14, 1)]
    public static Number CrewMin = 5;
}

[HeaderOption(MultiMenu.Main)]
public static class NeutralSettings
{
    [NumberOption(0.25f, 5f, 0.25f, Format.Multiplier)]
    public static Number NeutralVision = 1.5f;

    [ToggleOption]
    public static bool LightsAffectNeutrals = true;

    [ToggleOption]
    public static bool NeutralFlashlight = false;

    [NumberOption(0, 14, 1)]
    public static Number NeutralMax = 1;

    [NumberOption(0, 14, 1)]
    public static Number NeutralMin = 0;

    [StringOption<NoSolo>]
    public static NoSolo NoSolo = NoSolo.Never;

    [ToggleOption]
    public static bool AvoidNeutralKingmakers = false;

    [ToggleOption]
    public static bool NeutralsVent = true;
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

    [ToggleOption]
    public static bool IntrudersCanSabotage = true;

    [ToggleOption]
    public static bool GhostsCanSabotage = false;

    [NumberOption(1, 14, 1)]
    public static Number IntruderMax = 1;

    [NumberOption(1, 14, 1)]
    public static Number IntruderMin = 1;
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
    public static Number CDKillCd = 25;

    [StringOption<SyndicateVentOptions>]
    public static SyndicateVentOptions SyndicateVent = SyndicateVentOptions.Always;

    [ToggleOption]
    private static bool AltImpsPriv = false;
    public static bool AltImps => AltImpsPriv || IntruderSettings.IntruderCount == 0;

    [ToggleOption]
    public static bool GlobalDrive = false;

    [ToggleOption]
    public static bool AssignOnGameStart = false;

    [NumberOption(1, 14, 1)]
    public static Number SyndicateMax = 1;

    [NumberOption(1, 14, 1)]
    public static Number SyndicateMin = 1;
}

[AlignmentOption(RoleListSlot.CrewInvest)]
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

[AlignmentOption(RoleListSlot.CrewKill)]
public static class CrewKillingRoles
{
    [LayerOption("#7E3C64FF", LayerEnum.Bastion)]
    public static RoleOptionData Bastion;

    [LayerOption("#998040FF", LayerEnum.Veteran)]
    public static RoleOptionData Veteran;

    [LayerOption("#FFFF00FF", LayerEnum.Vigilante)]
    public static RoleOptionData Vigilante;
}

[AlignmentOption(RoleListSlot.CrewProt)]
public static class CrewProtectiveRoles
{
    [LayerOption("#660000FF", LayerEnum.Altruist)]
    public static RoleOptionData Altruist;

    [LayerOption("#006600FF", LayerEnum.Medic)]
    public static RoleOptionData Medic;

    [LayerOption("#BE1C8CFF", LayerEnum.Trapper)]
    public static RoleOptionData Trapper;

    [LayerOption("#1A3270FF", LayerEnum.Trickster)]
    public static RoleOptionData Trickster;
}

[AlignmentOption(RoleListSlot.CrewSov)]
public static class CrewSovereignRoles
{
    [LayerOption("#00CB97FF", LayerEnum.Dictator)]
    public static RoleOptionData Dictator;

    [LayerOption("#704FA8FF", LayerEnum.Mayor)]
    public static RoleOptionData Mayor;

    [LayerOption("#FF004EFF", LayerEnum.Monarch)]
    public static RoleOptionData Monarch;
}

[AlignmentOption(RoleListSlot.CrewSupport)]
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

    [LayerOption("#DF851FFF", LayerEnum.Shifter)]
    public static RoleOptionData Shifter;

    [LayerOption("#00EEFFFF", LayerEnum.Transporter)]
    public static RoleOptionData Transporter;
}

[AlignmentOption(RoleListSlot.CrewUtil)]
public static class CrewUtilityRoles
{
    [LayerOption("#8CFFFFFF", LayerEnum.Crewmate, true)]
    public static RoleOptionData Crewmate;

    [LayerOption("#D3D3D3FF", LayerEnum.Revealer)]
    private static RoleOptionData RevealerPriv;
    public static RoleOptionData Revealer
    {
        get
        {
            var result = RevealerPriv.Clone();

            if (IsRoleList())
                result.Count = GameModeSettings.RevealerCount;

            return result;
        }
    }
}

[AlignmentOption(RoleListSlot.NeutralApoc, true)]
public static class NeutralApocalypseRoles
{
    [LayerOption("#424242FF", LayerEnum.Pestilence, true)]
    public static RoleOptionData Pestilence;
}

[AlignmentOption(RoleListSlot.NeutralBen)]
public static class NeutralBenignRoles
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

[AlignmentOption(RoleListSlot.NeutralEvil)]
public static class NeutralEvilRoles
{
    [LayerOption("#00ACC2FF", LayerEnum.Actor)]
    public static RoleOptionData Actor;

    [LayerOption("#B51E39FF", LayerEnum.BountyHunter)]
    public static RoleOptionData BountyHunter;

    [LayerOption("#8C4005FF", LayerEnum.Cannibal)]
    public static RoleOptionData Cannibal;

    [LayerOption("#CCCCCCFF", LayerEnum.Executioner)]
    public static RoleOptionData Executioner;

    [LayerOption("#EEE5BEFF", LayerEnum.Guesser)]
    public static RoleOptionData Guesser;

    [LayerOption("#F7B3DAFF", LayerEnum.Jester)]
    public static RoleOptionData Jester;

    [LayerOption("#678D36FF", LayerEnum.Troll)]
    public static RoleOptionData Troll;
}

[AlignmentOption(RoleListSlot.NeutralHarb)]
public static class NeutralHarbingerRoles
{
    [LayerOption("#CFFE61FF", LayerEnum.Plaguebearer)]
    public static RoleOptionData Plaguebearer;
}

[AlignmentOption(RoleListSlot.NeutralKill)]
public static class NeutralKillingRoles
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

[AlignmentOption(RoleListSlot.NeutralNeo)]
public static class NeutralNeophyteRoles
{
    [LayerOption("#AC8A00FF", LayerEnum.Dracula)]
    public static RoleOptionData Dracula;

    [LayerOption("#45076AFF", LayerEnum.Jackal)]
    public static RoleOptionData Jackal;

    [LayerOption("#BF5FFFFF", LayerEnum.Necromancer)]
    public static RoleOptionData Necromancer;

    [LayerOption("#2D6AA5FF", LayerEnum.Whisperer)]
    public static RoleOptionData Whisperer;
}

[AlignmentOption(RoleListSlot.NeutralPros)]
public static class NeutralProselyteRoles
{
    [LayerOption("#11806AFF", LayerEnum.Betrayer, true)]
    public static RoleOptionData Betrayer;

    [LayerOption("#662962FF", LayerEnum.Phantom)]
    private static RoleOptionData PhantomPriv;
    public static RoleOptionData Phantom
    {
        get
        {
            var result = PhantomPriv.Clone();

            if (IsRoleList())
                result.Count = GameModeSettings.PhantomCount;

            return result;
        }
    }
}

[AlignmentOption(RoleListSlot.IntruderConceal)]
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

[AlignmentOption(RoleListSlot.IntruderDecep)]
public static class IntruderDeceptionRoles
{
    [LayerOption("#40B4FFFF", LayerEnum.Disguiser)]
    public static RoleOptionData Disguiser;

    [LayerOption("#BB45B0FF", LayerEnum.Morphling)]
    public static RoleOptionData Morphling;

    [LayerOption("#5C4F75FF", LayerEnum.Wraith)]
    public static RoleOptionData Wraith;
}

[AlignmentOption(RoleListSlot.IntruderHead)]
public static class IntruderHeadRoles
{
    [LayerOption("#404C08FF", LayerEnum.Godfather)]
    public static RoleOptionData Godfather;
}

[AlignmentOption(RoleListSlot.IntruderKill)]
public static class IntruderKillingRoles
{
    [LayerOption("#2BD29CFF", LayerEnum.Ambusher)]
    public static RoleOptionData Ambusher;

    [LayerOption("#005643FF", LayerEnum.Enforcer)]
    public static RoleOptionData Enforcer;
}

[AlignmentOption(RoleListSlot.IntruderSupport)]
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

[AlignmentOption(RoleListSlot.IntruderUtil)]
public static class IntruderUtilityRoles
{
    [LayerOption("#F1C40FFF", LayerEnum.Ghoul)]
    private static RoleOptionData GhoulPriv;
    public static RoleOptionData Ghoul
    {
        get
        {
            var result = GhoulPriv.Clone();

            if (IsRoleList())
                result.Count = GameModeSettings.GhoulCount;

            return result;
        }
    }

    [LayerOption("#FF1919FF", LayerEnum.Impostor, true)]
    public static RoleOptionData Impostor;
}

[AlignmentOption(RoleListSlot.SyndicateDisrup)]
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

[AlignmentOption(RoleListSlot.SyndicateKill)]
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

[AlignmentOption(RoleListSlot.SyndicatePower)]
public static class SyndicatePowerRoles
{
    [LayerOption("#FFFCCEFF", LayerEnum.Rebel)]
    public static RoleOptionData Rebel;

    [LayerOption("#0028F5FF", LayerEnum.Spellslinger)]
    public static RoleOptionData Spellslinger;
}

[AlignmentOption(RoleListSlot.SyndicateSupport)]
public static class SyndicateSupportRoles
{
    [LayerOption("#7E4D00FF", LayerEnum.Stalker)]
    public static RoleOptionData Stalker;

    [LayerOption("#8C7140FF", LayerEnum.Warper)]
    public static RoleOptionData Warper;
}

[AlignmentOption(RoleListSlot.SyndicateUtil)]
public static class SyndicateUtilityRoles
{
    [LayerOption("#008000FF", LayerEnum.Anarchist, true)]
    public static RoleOptionData Anarchist;

    [LayerOption("#E67E22FF", LayerEnum.Banshee)]
    private static RoleOptionData BansheePriv;
    public static RoleOptionData Banshee
    {
        get
        {
            var result = BansheePriv.Clone();

            if (IsRoleList())
                result.Count = GameModeSettings.BansheeCount;

            return result;
        }
    }
}

[AlignmentOption(colorHex: "#7F7F7FFF")]
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

    [LayerOption("#860B7AFF", LayerEnum.Professional)]
    public static RoleOptionData Professional;

    [LayerOption("#1002C5FF", LayerEnum.Shy)]
    public static RoleOptionData Shy;

    [LayerOption("#DCEE85FF", LayerEnum.VIP)]
    public static RoleOptionData VIP;

    [LayerOption("#FFA60AFF", LayerEnum.Volatile)]
    public static RoleOptionData Volatile;

    [LayerOption("#F6AAB7FF", LayerEnum.Yeller)]
    public static RoleOptionData Yeller;
}

[AlignmentOption(colorHex: "#FF9900FF")]
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

[AlignmentOption(colorHex: "#DD585BFF")]
public static class Dispositions
{
    [LayerOption("#4545A9FF", LayerEnum.Allied)]
    public static RoleOptionData Allied;

    [LayerOption("#4545FFFF", LayerEnum.Corrupted)]
    public static RoleOptionData Corrupted;

    [LayerOption("#E1C849FF", LayerEnum.Defector)]
    public static RoleOptionData Defector;

    [LayerOption("#678D36FF", LayerEnum.Fanatic)]
    public static RoleOptionData Fanatic;

    [LayerOption("#FF351FFF", LayerEnum.Linked, max: 7)]
    private static RoleOptionData LinkedPriv;
    public static RoleOptionData Linked
    {
        get
        {
            var result = LinkedPriv.Clone();
            result.Count *= 2;
            return result;
        }
    }

    [LayerOption("#FF66CCFF", LayerEnum.Lovers, max: 7)]
    private static RoleOptionData LoversPriv;
    public static RoleOptionData Lovers
    {
        get
        {
            var result = LoversPriv.Clone();
            result.Count *= 2;
            return result;
        }
    }

    [LayerOption("#00EEFFFF", LayerEnum.Mafia, min: 2)]
    public static RoleOptionData Mafia;

    [LayerOption("#008080FF", LayerEnum.Overlord)]
    public static RoleOptionData Overlord;

    [LayerOption("#3D2D2CFF", LayerEnum.Rivals, max: 7)]
    private static RoleOptionData RivalsPriv;
    public static RoleOptionData Rivals
    {
        get
        {
            var result = RivalsPriv.Clone();
            result.Count *= 2;
            return result;
        }
    }

    [LayerOption("#ABABFFFF", LayerEnum.Taskmaster)]
    public static RoleOptionData Taskmaster;

    [LayerOption("#370D43FF", LayerEnum.Traitor)]
    public static RoleOptionData Traitor;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class CrewInvestigativeSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxCI = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class CrewKillingSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxCK = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class CrewProtectiveSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxCrP = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class CrewSovereignSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxCSv = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class CrewSupportSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxCS = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class NeutralApocalypseSettings
{
    [ToggleOption]
    public static bool DirectSpawn = false;

    [ToggleOption]
    public static bool PlayersAlerted = true;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class NeutralBenignSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxNB = 1;

    [ToggleOption]
    public static bool VigilanteKillsBenigns = true;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class NeutralEvilSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxNE = 1;

    [ToggleOption]
    public static bool NeutralEvilsEndGame = false;

    [ToggleOption]
    public static bool VigilanteKillsEvils = true;

    [ToggleOption]
    public static bool NEHasImpVision = true;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class NeutralHarbingerSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxNH = 1;

    [ToggleOption]
    public static bool NHHasImpVision = true;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class NeutralKillingSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxNK = 1;

    [ToggleOption]
    public static bool NKHasImpVision = true;

    [ToggleOption]
    public static bool KnowEachOther = false;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class NeutralNeophyteSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxNN = 1;

    [ToggleOption]
    public static bool NNHasImpVision = true;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class IntruderConcealingSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxIC = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class IntruderDeceptionSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxID = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class IntruderHeadSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxIH = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class IntruderKillingSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxIK = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class IntruderSupportSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxIS = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class SyndicateDisruptionSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxSD = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class SyndicateKillingSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxSyK = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class SyndicatePowerSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxSP = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class SyndicateSupportSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxSSu = 1;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class ModifiersSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxModifiers = 5;

    [NumberOption(1, 14, 1)]
    public static Number MinModifiers = 5;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class AbilitiesSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxAbilities = 5;

    [NumberOption(1, 14, 1)]
    public static Number MinAbilities = 5;
}

[HeaderOption(MultiMenu.AlignmentSubOptions)]
public static class DispositionsSettings
{
    [NumberOption(1, 14, 1)]
    public static Number MaxDispositions = 5;

    [NumberOption(1, 14, 1)]
    public static Number MinDispositions = 5;
}