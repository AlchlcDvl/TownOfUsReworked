namespace TownOfUsReworked.Data
{
    public enum ActionsRPC
    {
        FixLights,
        SetExtraVotes,
        SetExtraVotesPol,
        SetSwaps,
        Shift,
        Protect,
        Rewind,
        RewindRevive,
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
        RevealerFinished,
        FlashGrenade,
        Alert,
        Remember,
        BaitReport,
        Transport,
        SetUntransportable,
        Mediate,
        Vest,
        GAProtect,
        Blackmail,
        Poison,
        Infect,
        TimeFreeze,
        Interrogate,
        Convert,
        Stake,
        Warp,
        WarpAll,
        SetUnwarpable,
        Teleport,
        Maul,
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
        Confuse,
        Frame,
        Ambush,
        Crusade,
        Scream,
        Mark,
        FadeBody,
        SetBomb,
        ForceKill,

        None
    }

    public enum AbilityTypes
    {
        Direct,
        Dead,
        Effect,
        Vent,

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

        IntruderSupport,
        IntruderConceal,
        IntruderDecep,
        IntruderKill,
        IntruderUtil,

        NeutralKill,
        NeutralNeo,
        NeutralEvil,
        NeutralBen,
        NeutralPros,

        SyndicateKill,
        SyndicateSupport,
        SyndicateDisruption,
        SyndicatePower,
        SyndicateUtil,

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
        Nested
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
        RandomSynchronized
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
        Custom,
        Vanilla
    }

    public enum Map
    {
        Skeld,
        MiraHQ,
        Polus,
        Airship,
        Submerged
    }

    public enum TaskBarMode
    {
        Normal,
        MeetingOnly,
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
        Morph,
        Camouflage,
        Invis,
        PlayerNameOnly
    }

    public enum LayerRPC
    {
        Role,
        Ability,
        Modifier,
        Objectifier,

        None
    }

    public enum TargetRPC
    {
        SetCouple,
        SetDuo,

        SetAlliedFaction,

        SetGoodRecruit,
        SetEvilRecruit,
        SetExeTarget,
        SetGATarget,
        SetGuessTarget,
        SetBHTarget,
        SetActPretendList,
    }

    public enum CustomRPC
    {
        SetLayer = 100,

        NullModifier,
        NullObjectifier,
        NullAbility,
        NullRole,

        SetPhantom,
        SetRevealer,
        SetBanshee,
        SetGhoul,

        PhantomDied,
        CatchPhantom,

        CatchRevealer,
        RevealerDied,

        CatchGhoul,
        GhoulDied,

        BansheeDied,
        CatchBanshee,

        Action,
        WinLose,
        Change,
        Target,

        AttemptSound,

        Start,
        SyncCustomSettings,
        FixAnimation,
        SetPos,
        SetSettings,

        AddMayorVoteBank,
        AddPoliticianVoteBank,
        MeetingStart,
        CheckMurder,

        SubmergedFixOxygen,

        SendChat,
        Whisper,
        Guess,
        LoveRivalChat,
        FactionComms,
        SubFactionComms,

        SetSpawnAirship,
        DoorSyncToilet,
        SyncPlateform,

        SetColor,
        VersionHandshake,

        ChaosDrive,

        None
    }

    public enum AbilityEnum
    {
        Assassin,
        ButtonBarry,
        Diener,
        Insider,
        Multitasker,
        Ninja,
        Radar,
        Ruthless,
        Snitch,
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
        Fanatic,
        Lovers,
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
        Engineer,
        Escort,
        Inspector,
        Mayor,
        Medic,
        Medium,
        Mystic,
        Operative,
        Retributionist,
        Revealer,
        Seer,
        Sheriff,
        Shifter,
        Swapper,
        TimeLord,
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
        TimeMaster,
        Wraith,

        Anarchist,
        Banshee,
        Bomber,
        Collider,
        Concealer,
        Crusader,
        Drunkard,
        Eraser,
        Framer,
        Poisoner,
        Politician,
        PromotedRebel,
        Rebel,
        Shapeshifter,
        Sidekick,
        Warper,

        None
    }

    public enum ModifierEnum
    {
        Bait,
        Coward,
        Diseased,
        Drunk,
        Dwarf,
        Flincher,
        Giant,
        Indomitable,
        Professional,
        Shy,
        VIP,
        Volatile,

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

    public enum InspectorResults
    {
        MeddlesWithDead, //Janitor, Altruist, Necromancer
        DealsWithDead, //Coroner, Amnesiac, Cannibal, Retributionist
        SeeksToProtect, //Guardian Angel, Survivor, Veteran, Medic, Crusader
        LeadsTheGroup, //Mayor, Godfather, Rebel, Pestilence
        BringsChaos, //Jackal, Swapper, Shifter, Camouflager, Thief
        LikesToExplore, //Teleporter, Warper, Beamer, Transporter
        IsBasic, //Crewmate, Impostor, Murderer, Anarchist
        DifferentLens, //Glitch, Medium, Engineer, Time Lord, Time Master
        HasInformation, //Sheriff, Consigliere, Blackmailer, Detective, Inspector
        MeddlesWithOthers, //Escort, Consort, Jester, Executioner, Actor
        TouchesPeople, //Arsonist, Plaguebearer, Cryomaniac, Framer, Seer
        Unseen, //Chameleon, Wraith, Poisoner, Gorgon, Concealer
        UsesGuns, //Vigilante, Bounty Hunter, Guesser, Mafioso
        TracksOthers, //Tracker, Mystic, Vampire Hunter, Whisperer, Ambusher
        IsAggressive, //Werewolf, Juggernaut, Sidekick, Serial Killer
        CausesConfusion, //Morphling, Disguiser, Shapeshifter, Betrayer, Drunkard
        DropsItems, //Bomber, Operative, Grenadier, Enforcer
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

        NobodyWins,

        None
    }
}