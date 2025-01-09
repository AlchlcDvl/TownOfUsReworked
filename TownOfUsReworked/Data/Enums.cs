namespace TownOfUsReworked.Data;

public enum ActionsRPC : byte
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

public enum BomberActionsRPC : byte
{
    DropBomb,
    Explode
}

public enum DouseActionsRPC : byte
{
    Douse,
    UnDouse
}

public enum DictActionsRPC : byte
{
    Tribunal,
    SelectToEject,
}

public enum GlitchActionsRPC : byte
{
    Mimic,
    Hack
}

public enum PoliticianActionsRPC : byte
{
    Add,
    Remove
}

public enum ThiefActionsRPC : byte
{
    Steal,
    Guess
}

[Flags]
public enum AbilityTypes : byte
{
    Targetless = 1 << 0,
    Player = 1 << 1,
    Body = 1 << 2,
    Vent = 1 << 3,
    Console = 1 << 4
}

public enum MeetingTypes : byte
{
    Toggle,
    Click
}

public enum InfoType : byte
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

public enum Faction : byte
{
    None,
    Crew,
    Intruder,
    Syndicate,
    Neutral,
    GameMode
}

public enum Alignment : byte
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

public enum SubFaction : byte
{
    Undead,
    Cabal,
    Reanimated,
    Sect,

    None
}

public enum CustomOptionType : byte
{
    Header,
    Toggle,
    Number,
    String,
    Layer,
    Entry,
    Alignment
}

public enum MultiMenu : byte
{
    Main,
    Layer,
    Presets,
    Client,
    RoleList,
    LayerSubOptions,
    AlignmentSubOptions = 250
}

public enum WhoCanVentOptions : byte
{
    Everyone,
    Default,
    NoOne
}

public enum DisableSkipButtonMeetings : byte
{
    Never,
    Emergency,
    Always
}

public enum RoleFactionReports : byte
{
    Neither,
    Role,
    Faction
}

public enum AirshipSpawnType : byte
{
    Normal,
    Fixed,
    RandomSynchronized,
    Random,
    Meeting
}

public enum MoveAdmin : byte
{
    DontMove,
    Cockpit,
    MainHall
}

public enum MoveElectrical : byte
{
    DontMove,
    Vault,
    Electrical
}

public enum GameMode : byte
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

public enum HnSMode : byte
{
    Classic,
    Infection
}

public enum MapEnum : byte
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

public enum PlayerNames : byte
{
    Obstructed,
    Visible,
    NotVisible
}

public enum WhoCanSeeFirstKillShield : byte
{
    Everyone,
    PlayerOnly,
    NoOne
}

public enum CustomPlayerOutfitType : byte
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

public enum MiscRPC : byte
{
    SetLayer,
    Catch,
    BreakShield,
    Start,
    SyncCustomSettings,
    SetSettings,
    Notify,
    SubmergedFixOxygen,
    Whisper,
    DoorSyncToilet,
    ChaosDrive,
    FixLights,
    FixMixup,
    SetFirstKilled,
    SyncSummary,
    BodyLocation,
    BastionBomb,
    MoveBody,
    LoadPreset,
    EndRoleGen,
    SetTarget,
    ChangeRoles,
    Achievement,
    SyncUses,
    SyncMaxUses,
    SyncMap,

    None
}

public enum CustomRPC : byte
{
    Action,
    WinLose,
    Misc,
    Test,
    Vanilla,

    None
}

public enum TestRPC : byte
{
    Argless,
    Args
}

public enum VanillaRPC : byte
{
    SnapTo,
    SetColor
}

public enum PlayerLayerEnum : byte
{
    Role,
    Modifier,
    Ability,
    Disposition,

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

public enum DeathReasonEnum : byte
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
    Marked,

    None
}

public enum WinLose : byte
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
    HuntedWin,

    EveryoneWins,

    None
}

public enum DeadRevealed : byte
{
    Oldest,
    Newest,
    Random,
    Everyone
}

public enum VigiOptions : byte
{
    Immediate,
    PreMeeting,
    PostMeeting
}

public enum VigiNotif : byte
{
    Never,
    Message,
    Flash
}

public enum AdminDeadPlayers : byte
{
    Nobody,
    Operative,
    EveryoneButOperative,
    Everyone
}

public enum ShieldOptions : byte
{
    Shielded,
    Medic,
    ShieldedAndMedic,
    Everyone,
    Nobody
}

public enum BecomeEnum : byte
{
    Shifter,
    Crewmate
}

public enum RetActionsRPC : byte
{
    Shield,
    Roleblock,
    Transport,
    Mediate,
    Revive,
    Bomb,
    Place,
    Trigger,
    AltRevive
}

public enum JanitorOptions : byte
{
    Never,
    Body,
    Bodyless,
    Always
}

public enum DisguiserTargets : byte
{
    Everyone,
    Intruders,
    NonIntruders
}

public enum ConsigInfo : byte
{
    Role,
    Faction
}

public enum RevealerCanBeClickedBy : byte
{
    Everyone,
    NonCrew,
    EvilsOnly
}

public enum AlliedFaction : byte
{
    Random,
    Crew,
    Intruder,
    Syndicate
}

public enum GFActionsRPC : byte
{
    Morph,
    Disguise,
    Drag,
    Blackmail,
    Roleblock,
    Ambush
}

public enum SyndicateVentOptions : byte
{
    Always,
    ChaosDrive,
    Never
}

public enum RebActionsRPC : byte
{
    Poison,
    Warp,
    Conceal,
    Shapeshift,
    Frame,
    Crusade,
    Spellbind,
    Confuse,
    DropBomb,
    Explode,
    Silence
}

public enum ProtectOptions : byte
{
    Protected,
    GA,
    ProtectedAndGA,
    Everyone,
    Nobody
}

public enum NoSolo : byte
{
    Never,
    SameNKs,
    AllNKs,
    AllNeutrals
}

public enum SKVentOptions : byte
{
    Always,
    Bloodlust,
    NoLust,
    Never
}

public enum ChatChannel : byte
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

public enum DefectorFaction : byte
{
    Random,
    NonFaction,
    NonNeutral,
    NonCrew,
    OpposingEvil,
    Neutral,
    Crew
}

public enum ShowMediumToDead : byte
{
    Never,
    Target,
    AllDead
}

public enum CooldownType : byte
{
    Start,
    Meeting,
    Fail,
    Custom,
    Reset
}

public enum AnonVotes : byte
{
    Disabled,
    Enabled,
    NonPolitician,
    PoliticianOnly,
    NotVisible
}

public enum FootprintVisibility : byte
{
    OnlyWhenCamouflaged,
    AlwaysVisible,
    AlwaysCamouflaged
}

public enum RandomSpawning : byte
{
    Disabled,
    GameStart,
    PostMeeting,
    Both
}

public enum AttackEnum : byte
{
    None,
    Basic,
    Powerful,
    Unstoppable
}

public enum DefenseEnum : byte
{
    None,
    Basic,
    Powerful,
    Invincible
}

public enum TrapperActionsRPC : byte
{
    Place,
    Trigger
}

public enum CosmeticTypeEnum : byte
{
    Hat,
    Visor,
    Nameplate
}

public enum SkipEnum : byte
{
    Map,
    RoleCard,
    Zooming,
    Wiki,
    Task,
    Settings,
    None
}

public enum WerewolfVentOptions : byte
{
    Always,
    Attacking,
    NotAttacking,
    Never
}

public enum KeybindType : byte
{
    ActionSecondary,
    Secondary,
    Tertiary,
    Quarternary
}

public enum Format : byte
{
    None,
    Time,
    Distance,
    Percent,
    Multiplier
}

public enum CrewVenting : byte
{
    Never,
    OnTasksDone,
    Always
}

public enum TempLocation : byte
{
    DontMove,
    DeathValley,
    SwappedWithVitals
}

public enum ReworkedLogLevel : byte
{
    Critical
}