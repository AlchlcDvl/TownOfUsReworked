namespace TownOfUsReworked.Data;

public enum ActionsRPC
{
    Mine,
    BarryButton,
    Drop,
    BaitReport,
    Convert,
    WarpAll,
    SetUnwarpable,
    Teleport,
    BypassKill,
    FadeBody,
    ForceKill,
    SetUninteractable,
    Burn,
    PlaceHit,
    LayerAction1,
    LayerAction2,

    None
}

public enum DictActionsRPC
{
    SetExiles,
    Reveal
}

public enum GlitchActionsRPC
{
    Mimic,
    Hack
}

public enum ThiefActionsRPC
{
    Steal,
    Guess
}

public enum AbilityTypes
{
    Target,
    Dead,
    Vent,
    Targetless,
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
    Other,

    None
}

public enum TurnRPC
{
    TurnPestilence,
    TurnVigilante,
    TurnGodfather,
    TurnRebel,
    TurnTraitor,
    TurnFanatic,
    TurnSeer,
    TurnSheriff,
    TurnTraitorBetrayer,
    TurnFanaticBetrayer,
    TurnJest,
    TurnSurv,
    TurnAct,
    TurnTroll,
    TurnThief,
    TurnSides,
    TurnRole,

    None
}

public enum Faction
{
    Crew,
    Intruder,
    Neutral,
    Syndicate,

    None
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
    External
}

public enum WhoCanVentOptions
{
    Default,
    Everyone,
    Noone
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
    Custom,
    Vanilla
}

public enum MapEnum
{
    Skeld,
    MiraHQ,
    Polus,
    //dlekS,
    Airship,
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
    Morph,
    Camouflage,
    Invis,
    PlayerNameOnly
}

public enum TargetRPC
{
    SetCouple,
    SetDuo,
    SetLinked,
    SetAlliedFaction,

    SetExeTarget,
    SetGATarget,
    SetGuessTarget,
    SetBHTarget,
    SetActPretendList,

    None
}

public enum MiscRPC
{
    SetLayer,
    SetPhantom,
    CatchPhantom,
    SetRevealer,
    CatchRevealer,
    SetGhoul,
    CatchGhoul,
    SetBanshee,
    CatchBanshee,
    AttemptSound,
    Start,
    SyncCustomSettings,
    SetSettings,
    AddVoteBank,
    MeetingStart,
    Notify,
    SubmergedFixOxygen,
    Whisper,
    SetSpawnAirship,
    DoorSyncToilet,
    //SyncPlatform,
    SetColor,
    VersionHandshake,
    ChaosDrive,
    FixLights,
    SetFirstKilled,
    SyncPureCrew,
    SyncSummary,
    //ShareFriendCode,
    BodyLocation,
    SyncMayorVote,

    None
}

public enum CustomRPC
{
    Action,
    WinLose,
    Change,
    Target,
    Misc,
    Test,

    None
}

public enum PlayerLayerEnum
{
    Role,
    Modifier,
    Ability,
    Objectifier,

    None
}

public enum LayerEnum
{
    Altruist,
    Chameleon,
    Coroner,
    Crewmate,
    Detective,
    Dictator,
    Engineer,
    Escort,
    Inspector,
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

    Any,

    RandomCrew,
    CrewSupport,
    CrewInvest,
    CrewProt,
    CrewKill,
    CrewSov,
    CrewAudit,

    RandomIntruder,
    IntruderSupport,
    IntruderConceal,
    IntruderDecep,
    IntruderKill,

    RandomNeutral,
    NeutralKill,
    NeutralNeo,
    NeutralEvil,
    NeutralBen,
    NeutralPros,
    NeutralApoc,
    NeutralHarb,

    RandomSyndicate,
    SyndicateKill,
    SyndicateSupport,
    SyndicateDisrup,
    SyndicatePower,

    None,

    CrewUtil,
    IntruderUtil,
    SyndicateUtil,

    Astral,
    Bait,
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
    Underdog
}

public enum InspectorResults
{
    DealsWithDead, //Coroner, Amnesiac, Retributionist, Janitor, Cannibal
    PreservesLife, //Medic, Guardian Angel, Altruist, Necromancer, Crusader
    LeadsTheGroup, //Mayor, Godfather, Rebel, Pestilence, Survivor
    BringsChaos, //Shifter, Thief, Camouflager, Whisperer, Jackal
    SeeksToDestroy, //Arsonist, Cryomaniac, Plaguebearer, Spellslinger
    MovesAround, //Transporter, Teleporter, Warper, Timekeeper
    NewLens, //Engineer, Miner, Seer, Dracula, Medium, Monarch
    GainsInfo, //Sheriff, Consigliere, Blackmailer, Detective, Inspector, Silencer
    Manipulative, //Jester, Executioner, Actor, Troll, Framer, Dictator
    Unseen, //Chameleon, Wraith, Concealer, Poisoner, Collider
    IsCold, //Veteran, Vigilante, Sidekick, Guesser, Mafioso
    TracksOthers, //Tracker, Mystic, Vampire Hunter, Bounty Hunter, Stalker
    IsAggressive, //Betrayer, Werewolf, Juggernaut, Serial Killer
    CreatesConfusion, //Morphling, Disguiser, Shapeshifter
    DropsItems, //Bomber, Operative, Grenadier, Enforcer
    HindersOthers, //Escort, Consort, Glitch, Ambusher, Drunkard
    IsBasic, //Crewmate, Impostor, Murderer, Anarchist
    Ghostly, //Revealer, Phantom, Banshee, Ghoul

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

    NobodyWins,

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
    EscRoleblock,
    Transport,
    Mediate,
    RetributionistRevive
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
    ConsRoleblock,
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
    Spell,
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
    OpposingEvil,
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
    Survivor,
    GuardianAngel,
    Reset
}