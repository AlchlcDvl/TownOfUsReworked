namespace TownOfUsReworked.Data;

public enum ActionsRPC
{
    Mine,
    CallMeeting,
    Drop,
    BaitReport,
    Convert,
    BypassKill,
    FadeBody,
    ForceKill,
    SetUninteractable,
    Burn,
    PlaceHit,
    ButtonAction,
    LayerAction,
    Cancel,
    PublicReveal,
    Infect,

    None
}

public enum GlitchActionsRPC
{
    Mimic,
    Hack
}

public enum PoliticianActionsRPC
{
    Add,
    Remove
}

public enum ThiefActionsRPC
{
    Steal,
    Guess
}

public enum AbilityTypes
{
    Alive,
    Dead,
    Vent,
    Targetless,
    Console,
    None
}

public enum MeetingTypes
{
    Toggle,
    Click
}

public enum InfoType
{
    Role,
    Objectifier,
    Ability,
    Modifier,
    Lore,
    Faction,
    SubFaction,
    Alignment,
    GameMode,
    Symbol,
    Other,

    None
}

public enum Faction
{
    None,
    Crew,
    Intruder,
    Neutral,
    Syndicate,
    GameMode
}

public enum Alignment
{
    CrewSupport,
    CrewInvest,
    CrewProt,
    CrewKill,
    CrewUtil,
    CrewSov,
    CrewAudit,
    CrewConceal,
    CrewDecep,
    CrewPower,
    CrewDisrup,
    CrewHead,

    IntruderSupport,
    IntruderConceal,
    IntruderDecep,
    IntruderKill,
    IntruderUtil,
    IntruderInvest,
    IntruderProt,
    IntruderSov,
    IntruderAudit,
    IntruderPower,
    IntruderDisrup,
    IntruderHead,

    NeutralKill,
    NeutralNeo,
    NeutralEvil,
    NeutralBen,
    NeutralPros,
    NeutralApoc,
    NeutralHarb,
    NeutralInvest,
    NeutralAudit,
    NeutralSov,
    NeutralProt,
    NeutralSupport,
    NeutralUtil,
    NeutralConceal,
    NeutralDecep,
    NeutralDisrup,
    NeutralPower,
    NeutralHead,

    SyndicateKill,
    SyndicateSupport,
    SyndicateDisrup,
    SyndicatePower,
    SyndicateUtil,
    SyndicateInvest,
    SyndicateProt,
    SyndicateSov,
    SyndicateAudit,
    SyndicateConceal,
    SyndicateDecep,
    SyndicateHead,

    GameModeTaskRace,
    GameModeHideAndSeek,

    None
}

public enum SubFaction
{
    Undead,
    Cabal,
    Reanimated,
    Sect,

    None
}

public enum CustomOptionType
{
    Header,
    Toggle,
    Number,
    String,
    Button,
    Layers,
    Entry
}

public enum MultiMenu
{
    Main,
    Crew,
    Neutral,
    Intruder,
    Syndicate,
    Modifier,
    Objectifier,
    Ability,
    RoleList,
    Client,
    Presets,
    RoleListEntry,
    Layer
}

public enum WhoCanVentOptions
{
    Everyone,
    Default,
    NoOne
}

public enum DisableSkipButtonMeetings
{
    No,
    Emergency,
    Always
}

public enum RoleFactionReports
{
    Neither,
    Role,
    Faction
}

public enum AirshipSpawnType
{
    Normal,
    Fixed,
    RandomSynchronized,
    Meeting
}

public enum MoveAdmin
{
    DontMove,
    Cockpit,
    MainHall
}

public enum MoveElectrical
{
    DontMove,
    Vault,
    Electrical
}

public enum GameMode
{
    Classic,
    AllAny,
    KillingOnly,
    RoleList,
    HideAndSeek,
    TaskRace,
    Custom,
    Vanilla,
    None
}

public enum HnSMode
{
    Classic,
    Infection
}

public enum MapEnum
{
    Skeld,
    MiraHQ,
    Polus,
    dlekS,
    Airship,
    Fungle,
    Submerged,
    LevelImpostor,
    Random
}

public enum TaskBar
{
    MeetingOnly,
    Normal,
    Invisible
}

public enum PlayerNames
{
    Obstructed,
    Visible,
    NotVisible
}

public enum WhoCanSeeFirstKillShield
{
    Everyone,
    PlayerOnly,
    NoOne
}

public enum CustomPlayerOutfitType
{
    Default,
    Shapeshifted,
    HorseWrangler,
    MushroomMixup,
    Morph,
    Camouflage,
    Invis,
    PlayerNameOnly,
    Colorblind,
    NightVision
}

public enum MiscRPC
{
    SetLayer,
    Catch,
    AttemptSound,
    Start,
    SyncCustomSettings,
    SetSettings,
    Notify,
    SubmergedFixOxygen,
    Whisper,
    SetSpawnAirship,
    DoorSyncToilet,
    VersionHandshake,
    ChaosDrive,
    FixLights,
    FixMixup,
    SetFirstKilled,
    SyncPureCrew,
    SyncSummary,
    BodyLocation,
    BastionBomb,
    MoveBody,
    LoadPreset,
    EndRoleGen,
    SyncConvertible,
    RemoveTarget,
    SetTarget,
    ChangeRoles,

    None
}

public enum CustomRPC
{
    Action,
    WinLose,
    Misc,
    Test,
    Vanilla,

    None
}

public enum VanillaRPC
{
    SnapTo,
    SetColor/*,
    SetScanner,
    EndGame,
    StartMeeting,
    UpdateSystem,
    SetName,
    CompleteTask,
    MeetingClose,
    SyncSettings,
    ClimbLadder,
    VotingComplete,
    SendChat,
    EnterVent,
    ExitVent,
    StartCounter*/
}

public enum PlayerLayerEnum
{
    Role,
    Modifier,
    Ability,
    Objectifier,

    None
}

public enum LayerEnum : byte
{
    Altruist,
    Bastion,
    Chameleon,
    Coroner,
    Crewmate,
    Detective,
    Dictator,
    Engineer,
    Escort,
    Mayor,
    Medic,
    Medium,
    Monarch,
    Mystic,
    Operative,
    Retributionist,
    Revealer,
    Seer,
    Sheriff,
    Shifter,
    Tracker,
    Transporter,
    Trapper,
    VampireHunter,
    Veteran,
    Vigilante,

    Actor,
    Amnesiac,
    Arsonist,
    Betrayer,
    BountyHunter,
    Cannibal,
    Cryomaniac,
    Dracula,
    Executioner,
    Glitch,
    GuardianAngel,
    Guesser,
    Jackal,
    Jester,
    Juggernaut,
    Murderer,
    Necromancer,
    Pestilence,
    Phantom,
    Plaguebearer,
    SerialKiller,
    Survivor,
    Thief,
    Troll,
    Werewolf,
    Whisperer,

    Ambusher,
    Blackmailer,
    Camouflager,
    Consigliere,
    Consort,
    Disguiser,
    Enforcer,
    Ghoul,
    Godfather,
    Grenadier,
    Impostor,
    Janitor,
    Mafioso,
    Miner,
    Morphling,
    PromotedGodfather,
    Teleporter,
    Wraith,

    Anarchist,
    Banshee,
    Bomber,
    Collider,
    Concealer,
    Crusader,
    Drunkard,
    Framer,
    Poisoner,
    PromotedRebel,
    Rebel,
    Shapeshifter,
    Sidekick,
    Silencer,
    Spellslinger,
    Stalker,
    Timekeeper,
    Warper,

    Hunter,
    Hunted,

    Runner,

    NoneRole,

    Astral,
    Bait,
    Colorblind,
    Coward,
    Diseased,
    Drunk,
    Dwarf,
    Giant,
    Indomitable,
    Professional,
    Shy,
    VIP,
    Volatile,
    Yeller,
    NoneModifier,

    Allied,
    Corrupted,
    Defector,
    Fanatic,
    Linked,
    Lovers,
    Mafia,
    Overlord,
    Rivals,
    Taskmaster,
    Traitor,
    NoneObjectifier,

    Assassin,
    ButtonBarry,
    CrewAssassin,
    Insider,
    IntruderAssassin,
    Multitasker,
    NeutralAssassin,
    Ninja,
    Politician,
    Radar,
    Ruthless,
    Snitch,
    Swapper,
    SyndicateAssassin,
    Tiebreaker,
    Torch,
    Tunneler,
    Underdog,
    NoneAbility,

    Any,

    RandomCrew,
    CrewSupport,
    CrewInvest,
    CrewProt,
    CrewKill,
    CrewSov,
    CrewAudit,
    CrewUtil,
    RegularCrew,

    RandomIntruder,
    IntruderSupport,
    IntruderConceal,
    IntruderDecep,
    IntruderKill,
    IntruderUtil,
    IntruderHead,
    RegularIntruder,

    RandomNeutral,
    NeutralKill,
    NeutralNeo,
    NeutralEvil,
    NeutralBen,
    NeutralPros,
    NeutralApoc,
    NeutralHarb,
    RegularNeutral,
    HarmfulNeutral,

    RandomSyndicate,
    SyndicateKill,
    SyndicateSupport,
    SyndicateDisrup,
    SyndicatePower,
    SyndicateUtil,
    RegularSyndicate,

    None
}

public enum DeathReasonEnum
{
    Alive,
    Ejected,
    Guessed,
    Killed,
    Revived,
    Suicide,
    Bombed,
    Poisoned,
    Crusaded,
    Mauled,
    Ambushed,
    Failed,
    Trolled,
    Misfire,
    Frozen,
    Ignited,
    Haunted,
    Doomed,
    Dictated,
    Collided,
    Escaped,
    Converted,
    Infected,
    Clicked,

    None
}

public enum WinLoseRPC
{
    JesterWin,
    ExecutionerWin,
    CannibalWin,
    TrollWin,
    PhantomWin,
    GuesserWin,
    ActorWin,
    BountyHunterWin,
    BetrayerWin,

    CrewWin,
    IntruderWin,
    SyndicateWin,
    AllNeutralsWin,

    UndeadWin,
    CabalWin,
    SectWin,
    ReanimatedWin,

    AllNKsWin,
    SoloNKWins,
    SameNKWins,
    ApocalypseWins,

    LoveWin,
    TaskmasterWin,
    RivalWin,
    CorruptedWin,
    OverlordWin,
    MafiaWins,
    DefectorWins,

    NobodyWins,

    TaskRunnerWins,

    HunterWins,
    HuntedWins,

    None
}

public enum DeadRevealed
{
    Oldest,
    Newest,
    Random,
    All
}

public enum VigiOptions
{
    Immediate,
    PreMeeting,
    PostMeeting
}

public enum VigiNotif
{
    Never,
    Message,
    Flash
}

public enum AdminDeadPlayers
{
    Nobody,
    Operative,
    EveryoneButOperative,
    Everyone
}

public enum ShieldOptions
{
    Self,
    Medic,
    SelfAndMedic,
    Everyone,
    Nobody
}

public enum BecomeEnum
{
    Shifter,
    Crewmate
}

public enum RetActionsRPC
{
    Protect,
    Roleblock,
    Transport,
    Mediate,
    Revive,
    Bomb,
    Place,
    Trigger
}

public enum JanitorOptions
{
    Never,
    Body,
    Bodyless,
    Always
}

public enum DisguiserTargets
{
    Everyone,
    Intruders,
    NonIntruders
}

public enum ConsigInfo
{
    Role,
    Faction
}

public enum RevealerCanBeClickedBy
{
    All,
    NonCrew,
    EvilsOnly
}

public enum AlliedFaction
{
    Random,
    Intruder,
    Syndicate,
    Crew
}

public enum GFActionsRPC
{
    Morph,
    Disguise,
    Drag,
    Blackmail,
    Roleblock,
    Ambush
}

public enum SyndicateVentOptions
{
    Always,
    ChaosDrive,
    Never
}

public enum RebActionsRPC
{
    Poison,
    Warp,
    Conceal,
    Shapeshift,
    Frame,
    Crusade,
    Spellbind,
    Confuse,
    Silence
}

public enum ProtectOptions
{
    Self,
    GA,
    SelfAndGA,
    Everyone,
    Nobody
}

public enum NoSolo
{
    Never,
    SameNKs,
    AllNKs,
    AllNeutrals
}

public enum SKVentOptions
{
    Always,
    Bloodlust,
    NoLust,
    Never
}

public enum ChatChannel
{
    Lovers,
    Rivals,
    Linked,
    Intruders,
    Syndicate,
    Undead,
    Sect,
    Cabal,
    Reanimated,
    All
}

public enum DefectorFaction
{
    Random,
    NonFaction,
    NonNeutral,
    NonCrew,
    OpposingEvil,
    Neutral,
    Crew
}

public enum ShowMediumToDead
{
    No,
    Target,
    AllDead
}

public enum CooldownType
{
    Start,
    Meeting,
    Fail,
    Custom,
    Reset
}

public enum AnonVotes
{
    Disabled,
    Enabled,
    NonPolitician,
    PoliticianOnly,
    NotVisible
}

public enum FootprintVisibility
{
    OnlyWhenCamouflaged,
    AlwaysVisible,
    AlwaysCamouflaged
}

public enum RandomSpawning
{
    Disabled,
    GameStart,
    PostMeeting,
    Both
}

public enum AttackEnum
{
    None,
    Basic,
    Powerful,
    Unstoppable
}

public enum DefenseEnum
{
    None,
    Basic,
    Powerful,
    Invincible
}

public enum MedicActionsRPC
{
    Add,
    Remove
}

public enum TrapperActionsRPC
{
    Place,
    Trigger
}

public enum CosmeticTypeEnum
{
    Hat,
    Visor,
    Nameplate
}

public enum SkipEnum
{
    Map,
    RoleCard,
    Zooming,
    Wiki,
    Settings,
    Task,
    Client,
    None
}

public enum WerewolfVentOptions
{
    Allways,
    Attacking,
    NotAttacking,
    Never
}

public enum KeybindType
{
    ActionSecondary,
    Secondary,
    Tertiary,
    Quarternary
}

public enum Format
{
    None,
    Time,
    Distance,
    Percent,
    Multiplier
}

public enum HeaderType
{
    General,
    Layer
}