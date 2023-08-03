namespace TownOfUsReworked.Data
{
    public enum ActionsRPC
    {
        SetExtraVotes,
        SetSwaps,
        Shift,
        Protect,
        Morph,
        Camouflage,
        Mine,
        Swoop,
        Invis,
        Disguise,
        Douse,
        FreezeDouse,
        AltruistRevive,
        NecromancerResurrect,
        BarryButton,
        Drag,
        Drop,
        AssassinKill,
        GuesserKill,
        ThiefKill,
        FlashGrenade,
        Alert,
        Remember,
        BaitReport,
        Transport,
        Mediate,
        Vest,
        GAProtect,
        Blackmail,
        Poison,
        Infect,
        Convert,
        Stake,
        Warp,
        WarpAll,
        SetUnwarpable,
        Teleport,
        Conceal,
        Shapeshift,
        Steal,
        EscRoleblock,
        ConsRoleblock,
        Mimic,
        GlitchRoleblock,
        BypassKill,
        RetributionistAction,
        GodfatherAction,
        Sidekick,
        Declare,
        RebelAction,
        Frame,
        Ambush,
        Crusade,
        Scream,
        Mark,
        FadeBody,
        SetBomb,
        ForceKill,
        SetUninteractable,
        Burn,
        MayorReveal,
        DictatorReveal,
        Spell,
        Knight,
        SetExiles,
        Confuse,
        TimeControl,
        Silence,
        RequestHit,
        PlaceHit,

        None
    }

    public enum AbilityTypes
    {
        Direct,
        Dead,
        Effect,
        Vent,
        Special
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

    public enum RoleAlignment
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
        Nested,
        Entry
    }

    public enum MultiMenu
    {
        main,
        crew,
        neutral,
        intruder,
        syndicate,
        modifier,
        objectifier,
        ability,
        rolelist,
        external
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
        LevelImpostor
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
        CheckMurder,
        RemoveMeetings,
        Notify,
        SubmergedFixOxygen,
        Whisper,
        SetSpawnAirship,
        DoorSyncToilet,
        SyncPlateform,
        SetColor,
        VersionHandshake,
        ChaosDrive,
        FixLights,
        SetFirstKilled,

        None
    }

    public enum CustomRPC
    {
        Action = 200,
        WinLose,
        Change,
        Target,
        Misc,
        Test,

        None
    }

    public enum AbilityEnum
    {
        CrewAssassin,
        IntruderAssassin,
        NeutralAssassin,
        SyndicateAssassin,
        ButtonBarry,
        Insider,
        Multitasker,
        Ninja,
        Politician,
        Radar,
        Ruthless,
        Snitch,
        Swapper,
        Tiebreaker,
        Torch,
        Tunneler,
        Underdog,

        None
    }

    public enum ObjectifierEnum
    {
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

        None
    }

    public enum RoleEnum
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
        Concealer,
        Collider,
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
        TimeKeeper,
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

        None
    }

    public enum ModifierEnum
    {
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
        Betrayer,

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
        Politician,
        PromotedRebel,
        Rebel,
        Shapeshifter,
        Sidekick,
        Silencer,
        Spellslinger,
        Stalker,
        TimeKeeper,
        Warper,

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

        Assassin,
        ButtonBarry,
        Diener,
        Insider,
        Multitasker,
        Ninja,
        Radar,
        Ruthless,
        Snitch,
        Swapper,
        Tiebreaker,
        Torch,
        Tunneler,
        Underdog,

        None
    }

    public enum InspectorResults
    {
        DealsWithDead, //Coroner, Amnesiac, Retributionist, Janitor, Cannibal
        PreservesLife, //Medic, Guardian Angel, Altruist, Necromancer, Crusader
        LeadsTheGroup, //Mayor, Godfather, Rebel, Pestilence, Survivor
        BringsChaos, //Shifter, Thief, Camouflager, Whisperer, Jackal
        SeeksToDestroy, //Arsonist, Cryomaniac, Plaguebearer, Spellslinger
        MovesAround, //Transporter, Teleporter, Warper, Time Keeper
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
        InfectorsWin,

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
        Everyone
    }

    public enum NotificationOptions
    {
        Medic,
        Shielded,
        ShieldedAndMedic,
        Everyone,
        Nobody
    }

    public enum BecomeEnum
    {
        Shifter,
        Crewmate
    }

    public enum RetributionistActionsRPC
    {
        Protect,
        AltruistRevive,
        Alert,
        EscRoleblock,
        Transport,
        Mediate,
        Stake,
        Swoop,
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

    public enum GodfatherActionsRPC
    {
        Morph,
        Camouflage,
        Invis,
        Disguise,
        Drag,
        FlashGrenade,
        Blackmail,
        Infect,
        ConsRoleblock,
        SetBomb,
        Ambush
    }

    public enum SyndicateVentOptions
    {
        Always,
        ChaosDrive,
        Never
    }

    public enum RebelActionsRPC
    {
        Poison,
        Warp,
        Conceal,
        Shapeshift,
        Frame,
        Crusade,
        Spell,
        Confuse,
        TimeControl,
        Silence
    }

    public enum ProtectOptions
    {
        Self,
        GA,
        SelfAndGA,
        Everyone
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

    public enum ModLogType
    {
        Message,
        Fatal,
        Error,
        Debug,
        Info,
        Warning
    }
}