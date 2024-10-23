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

[Flags]
public enum AbilityType
{
    None = 0,
    Alive = 1 << 0,
    Dead = 1 << 1,
    Vent = 1 << 2,
    Targetless = 1 << 3,
    Console = 1 << 4
}

public enum MeetingTypes
{
    Toggle,
    Click
}

public enum InfoType
{
    Role,
    Disposition,
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

    Ability,
    Modifier,
    Disposition,

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
    Layers,
    Entry,
    Alignment
}

public enum MultiMenu
{
    Main,
    Layer,
    Presets,
    Client,
    RoleList,
    LayerSubOptions,
    AlignmentSubOptions = 250
}

public enum WhoCanVentOptions
{
    Everyone,
    Default,
    NoOne
}

public enum DisableSkipButtonMeetings
{
    Never,
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
    Random,
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
    SetColor
}

public enum PlayerLayerEnum
{
    Role,
    Modifier,
    Ability,
    Disposition,

    None
}

public enum LayerEnum
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
    NoneDisposition,

    Assassin,
    Bullseye,
    ButtonBarry,
    Hitman,
    Insider,
    Multitasker,
    Ninja,
    Politician,
    Radar,
    Ruthless,
    Slayer,
    Sniper,
    Snitch,
    Swapper,
    Tiebreaker,
    Torch,
    Tunneler,
    Underdog,
    NoneAbility,

    Undead,
    Sect,
    Cabal,
    Reanimated,

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

    NonCrew,
    NonIntruder,
    NonSyndicate,
    NonNeutral,

    FactionedEvil,

    Ability,
    Modifier,
    Disposition,

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

public enum WinLose
{
    JesterWins,
    ExecutionerWins,
    CannibalWins,
    TrollWins,
    PhantomWins,
    GuesserWins,
    ActorWins,
    BountyHunterWins,
    BetrayerWins,

    CrewWins,
    IntrudersWin,
    SyndicateWins,
    AllNeutralsWin,

    UndeadWins,
    CabalWins,
    SectWins,
    ReanimatedWins,

    AllNKsWin,
    ApocalypseWins,

    ArsonistWins,
    CryomaniacWins,
    GlitchWins,
    JuggernautWins,
    MurdererWins,
    SerialKillerWins,
    WerewolfWins,

    LoveWins,
    TaskmasterWins,
    RivalWins,
    CorruptedWins,
    OverlordWins,
    MafiaWins,
    DefectorWins,

    NobodyWins,

    TaskRunnerWins,

    HunterWins,
    HuntedWins,

    EveryoneWins,

    None
}

public enum DeadRevealed
{
    Oldest,
    Newest,
    Random,
    Everyone
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
    Shielded,
    Medic,
    ShieldedAndMedic,
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
    ProtectAdd,
    ProtectRemove,
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
    Everyone,
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
    Protected,
    GA,
    ProtectedAndGA,
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

    Dead,

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
    Never,
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
    Task,
    Settings,
    None
}

public enum WerewolfVentOptions
{
    Always,
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

public enum CrewVenting
{
    Never,
    OnTasksDone,
    Always
}

public enum TempLocation
{
    DontMove,
    DeathValley,
    SwappedWithVitals
}