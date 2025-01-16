namespace TownOfUsReworked.Data;

public static class AllInfo
{
    public static readonly IEnumerable<RoleInfo> AllRoles =
    [
        new(LayerEnum.None, Alignment.None, Faction.None, color: CustomColorManager.Role),

        new(LayerEnum.Altruist, Alignment.CrewProt, Faction.Crew),
        new(LayerEnum.Bastion, Alignment.CrewKill, Faction.Crew),
        new(LayerEnum.Chameleon, Alignment.CrewSupport, Faction.Crew),
        new(LayerEnum.Coroner, Alignment.CrewInvest, Faction.Crew),
        new(LayerEnum.Crewmate, Alignment.CrewUtil, Faction.Crew),
        new(LayerEnum.Detective, Alignment.CrewInvest, Faction.Crew),
        new(LayerEnum.Dictator, Alignment.CrewSov, Faction.Crew),
        new(LayerEnum.Engineer, Alignment.CrewSupport, Faction.Crew),
        new(LayerEnum.Escort, Alignment.CrewSupport, Faction.Crew),
        new(LayerEnum.Mayor, Alignment.CrewSov, Faction.Crew),
        new(LayerEnum.Medic, Alignment.CrewProt, Faction.Crew),
        new(LayerEnum.Medium, Alignment.CrewInvest, Faction.Crew),
        new(LayerEnum.Monarch, Alignment.CrewSov, Faction.Crew),
        new(LayerEnum.Mystic, Alignment.CrewInvest, Faction.Crew),
        new(LayerEnum.Operative, Alignment.CrewInvest, Faction.Crew),
        new(LayerEnum.Retributionist, Alignment.CrewSupport, Faction.Crew),
        new(LayerEnum.Revealer, Alignment.CrewUtil, Faction.Crew),
        new(LayerEnum.Seer, Alignment.CrewInvest, Faction.Crew),
        new(LayerEnum.Sheriff, Alignment.CrewInvest, Faction.Crew),
        new(LayerEnum.Shifter, Alignment.CrewSupport, Faction.Crew),
        new(LayerEnum.Tracker, Alignment.CrewInvest, Faction.Crew),
        new(LayerEnum.Transporter, Alignment.CrewSupport, Faction.Crew),
        new(LayerEnum.Trapper, Alignment.CrewProt, Faction.Crew),
        new(LayerEnum.Trickster, Alignment.CrewProt, Faction.Crew),
        new(LayerEnum.Veteran, Alignment.CrewKill, Faction.Crew),
        new(LayerEnum.Vigilante, Alignment.CrewKill, Faction.Crew, true),

        new(LayerEnum.Actor, Alignment.NeutralEvil, Faction.Neutral),
        new(LayerEnum.Amnesiac, Alignment.NeutralBen, Faction.Neutral),
        new(LayerEnum.Arsonist, Alignment.NeutralKill, Faction.Neutral),
        new(LayerEnum.Betrayer, Alignment.NeutralPros, Faction.Neutral),
        new(LayerEnum.BountyHunter, Alignment.NeutralEvil, Faction.Neutral),
        new(LayerEnum.Cannibal, Alignment.NeutralEvil, Faction.Neutral),
        new(LayerEnum.Cryomaniac, Alignment.NeutralKill, Faction.Neutral),
        new(LayerEnum.Dracula, Alignment.NeutralNeo, Faction.Neutral),
        new(LayerEnum.Executioner, Alignment.NeutralEvil, Faction.Neutral),
        new(LayerEnum.Glitch, Alignment.NeutralKill, Faction.Neutral),
        new(LayerEnum.GuardianAngel, Alignment.NeutralBen, Faction.Neutral),
        new(LayerEnum.Guesser, Alignment.NeutralEvil, Faction.Neutral),
        new(LayerEnum.Jackal, Alignment.NeutralNeo, Faction.Neutral),
        new(LayerEnum.Jester, Alignment.NeutralEvil, Faction.Neutral),
        new(LayerEnum.Juggernaut, Alignment.NeutralKill, Faction.Neutral),
        new(LayerEnum.Murderer, Alignment.NeutralKill, Faction.Neutral),
        new(LayerEnum.Necromancer, Alignment.NeutralNeo, Faction.Neutral),
        new(LayerEnum.Pestilence, Alignment.NeutralApoc, Faction.Neutral),
        new(LayerEnum.Phantom, Alignment.NeutralPros, Faction.Neutral),
        new(LayerEnum.Plaguebearer, Alignment.NeutralHarb, Faction.Neutral),
        new(LayerEnum.SerialKiller, Alignment.NeutralKill, Faction.Neutral),
        new(LayerEnum.Survivor, Alignment.NeutralBen, Faction.Neutral),
        new(LayerEnum.Thief, Alignment.NeutralBen, Faction.Neutral),
        new(LayerEnum.Troll, Alignment.NeutralEvil, Faction.Neutral),
        new(LayerEnum.Werewolf, Alignment.NeutralKill, Faction.Neutral),
        new(LayerEnum.Whisperer, Alignment.NeutralNeo, Faction.Neutral, true),

        new(LayerEnum.Ambusher, Alignment.None, Faction.Intruder),
        new(LayerEnum.Blackmailer, Alignment.None, Faction.Intruder),
        new(LayerEnum.Camouflager, Alignment.None, Faction.Intruder),
        new(LayerEnum.Consigliere, Alignment.None, Faction.Intruder),
        new(LayerEnum.Consort, Alignment.None, Faction.Intruder),
        new(LayerEnum.Disguiser, Alignment.None, Faction.Intruder),
        new(LayerEnum.Enforcer, Alignment.None, Faction.Intruder),
        new(LayerEnum.Ghoul, Alignment.None, Faction.Intruder),
        new(LayerEnum.Godfather, Alignment.None, Faction.Intruder),
        new(LayerEnum.Grenadier, Alignment.None, Faction.Intruder),
        new(LayerEnum.Impostor, Alignment.None, Faction.Intruder),
        new(LayerEnum.Janitor, Alignment.None, Faction.Intruder),
        new(LayerEnum.Mafioso, Alignment.None, Faction.Intruder),
        new(LayerEnum.Miner, Alignment.None, Faction.Intruder),
        new(LayerEnum.Morphling, Alignment.None, Faction.Intruder),
        new(LayerEnum.Teleporter, Alignment.None, Faction.Intruder),
        new(LayerEnum.Wraith, Alignment.None, Faction.Intruder, true),

        new(LayerEnum.Anarchist, Alignment.None, Faction.Syndicate),
        new(LayerEnum.Banshee, Alignment.None, Faction.Syndicate),
        new(LayerEnum.Bomber, Alignment.None, Faction.Syndicate),
        new(LayerEnum.Collider, Alignment.None, Faction.Syndicate),
        new(LayerEnum.Concealer, Alignment.None, Faction.Syndicate),
        new(LayerEnum.Crusader, Alignment.None, Faction.Syndicate),
        new(LayerEnum.Drunkard, Alignment.None, Faction.Syndicate),
        new(LayerEnum.Framer, Alignment.None, Faction.Syndicate),
        new(LayerEnum.Poisoner, Alignment.None, Faction.Syndicate),
        new(LayerEnum.Rebel, Alignment.None, Faction.Syndicate),
        new(LayerEnum.Shapeshifter, Alignment.None, Faction.Syndicate),
        new(LayerEnum.Sidekick, Alignment.None, Faction.Syndicate),
        new(LayerEnum.Silencer, Alignment.None, Faction.Syndicate),
        new(LayerEnum.Spellslinger, Alignment.None, Faction.Syndicate),
        new(LayerEnum.Stalker, Alignment.None, Faction.Syndicate),
        new(LayerEnum.Timekeeper, Alignment.None, Faction.Syndicate),
        new(LayerEnum.Warper, Alignment.None, Faction.Syndicate, true),

        new(LayerEnum.Hunter, Alignment.None, Faction.GameMode),
        new(LayerEnum.Hunted, Alignment.None, Faction.GameMode),

        new(LayerEnum.Runner, Alignment.None, Faction.GameMode, true),
    ];

    public static readonly IEnumerable<ModifierInfo> AllModifiers =
    [
        new(LayerEnum.None, color: CustomColorManager.Modifier),

        new(LayerEnum.Astral),
        new(LayerEnum.Bait),
        new(LayerEnum.Colorblind),
        new(LayerEnum.Coward),
        new(LayerEnum.Diseased),
        new(LayerEnum.Drunk),
        new(LayerEnum.Dwarf),
        new(LayerEnum.Giant),
        new(LayerEnum.Indomitable),
        new(LayerEnum.Professional),
        new(LayerEnum.Shy),
        new(LayerEnum.VIP),
        new(LayerEnum.Volatile),
        new(LayerEnum.Yeller, true)
    ];

    public static readonly IEnumerable<DispositionInfo> AllDispositions =
    [
        new(LayerEnum.None, "φ", color: CustomColorManager.Disposition),
        new(LayerEnum.Allied, "ζ"),
        new(LayerEnum.Corrupted, "δ"),
        new(LayerEnum.Defector, "ε"),
        new(LayerEnum.Fanatic, "♠"),
        new(LayerEnum.Linked, "Ψ"),
        new(LayerEnum.Lovers, "♥"),
        new(LayerEnum.Mafia, "ω"),
        new(LayerEnum.Overlord, "β"),
        new(LayerEnum.Rivals, "α"),
        new(LayerEnum.Taskmaster, "µ"),
        new(LayerEnum.Traitor, "♣", true)
    ];

    public static readonly IEnumerable<AbilityInfo> AllAbilities =
    [
        new(LayerEnum.None, color: CustomColorManager.Ability),

        new(LayerEnum.Assassin),
        new(LayerEnum.Bullseye),
        new(LayerEnum.ButtonBarry),
        new(LayerEnum.Hitman),
        new(LayerEnum.Insider),
        new(LayerEnum.Multitasker),
        new(LayerEnum.Ninja),
        new(LayerEnum.Politician),
        new(LayerEnum.Radar),
        new(LayerEnum.Ruthless),
        new(LayerEnum.Slayer),
        new(LayerEnum.Sniper),
        new(LayerEnum.Snitch),
        new(LayerEnum.Swapper),
        new(LayerEnum.Tiebreaker),
        new(LayerEnum.Torch),
        new(LayerEnum.Tunneler),
        new(LayerEnum.Underdog, true)
    ];

    public static readonly IEnumerable<FactionInfo> AllFactions =
    [
        new(Faction.None),
        new(Faction.Crew),
        new(Faction.Intruder),
        new(Faction.Neutral),
        new(Faction.Syndicate),
        new(Faction.GameMode, true)
    ];

    public static readonly IEnumerable<SubFactionInfo> AllSubFactions =
    [
        new(SubFaction.None),
        new(SubFaction.Sect),
        new(SubFaction.Cabal),
        new(SubFaction.Undead),
        new(SubFaction.Reanimated, true)
    ];

    public static readonly IEnumerable<AlignmentInfo> AllAlignments =
    [
        new(Alignment.None),
        new(Alignment.CrewSupport),
        new(Alignment.CrewInvest),
        new(Alignment.CrewProt),
        new(Alignment.CrewKill),
        new(Alignment.CrewUtil),
        new(Alignment.CrewSov),
        new(Alignment.CrewConceal),
        new(Alignment.CrewDecep),
        new(Alignment.CrewPower),
        new(Alignment.CrewDisrup),
        new(Alignment.CrewHead),
        new(Alignment.IntruderSupport),
        new(Alignment.IntruderConceal),
        new(Alignment.IntruderDecep),
        new(Alignment.IntruderKill),
        new(Alignment.IntruderUtil),
        new(Alignment.IntruderInvest),
        new(Alignment.IntruderProt),
        new(Alignment.IntruderSov),
        new(Alignment.IntruderPower),
        new(Alignment.IntruderDisrup),
        new(Alignment.IntruderHead),
        new(Alignment.NeutralKill),
        new(Alignment.NeutralNeo),
        new(Alignment.NeutralEvil),
        new(Alignment.NeutralBen),
        new(Alignment.NeutralPros),
        new(Alignment.NeutralApoc),
        new(Alignment.NeutralHarb),
        new(Alignment.NeutralInvest),
        new(Alignment.NeutralSov),
        new(Alignment.NeutralProt),
        new(Alignment.NeutralSupport),
        new(Alignment.NeutralUtil),
        new(Alignment.NeutralConceal),
        new(Alignment.NeutralDecep),
        new(Alignment.NeutralDisrup),
        new(Alignment.NeutralPower),
        new(Alignment.NeutralHead),
        new(Alignment.SyndicateKill),
        new(Alignment.SyndicateSupport),
        new(Alignment.SyndicateDisrup),
        new(Alignment.SyndicatePower),
        new(Alignment.SyndicateUtil),
        new(Alignment.SyndicateInvest),
        new(Alignment.SyndicateProt),
        new(Alignment.SyndicateSov),
        new(Alignment.SyndicateConceal),
        new(Alignment.SyndicateDecep),
        new(Alignment.SyndicateHead),
        new(Alignment.GameModeHideAndSeek),
        new(Alignment.GameModeTaskRace, true)
    ];

    public static readonly IEnumerable<GameModeInfo> AllModes =
    [
        new(GameMode.None),
        new(GameMode.Classic),
        new(GameMode.Vanilla),
        new(GameMode.AllAny),
        new(GameMode.KillingOnly),
        new(GameMode.TaskRace),
        new(GameMode.RoleList),
        new(GameMode.HideAndSeek),
        new(GameMode.Custom, true)
    ];

    public static readonly IEnumerable<OtherInfo> AllOthers =
    [
    ];

    public static readonly IEnumerable<SymbolInfo> AllSymbols =
    [
        new("Invalid", "Invalid", UColor.red),

        new("Null", "φ", CustomColorManager.Status),
        new("Shield", "✚", CustomColorManager.Medic),
        new("Knighted", "κ", CustomColorManager.Monarch),
        new("Trapped", "∮", CustomColorManager.Trapper),
        new("ArsoDoused", "Ξ", CustomColorManager.Arsonist),
        new("Bounty", "Θ", CustomColorManager.BountyHunter),
        new("CryoDoused", "λ", CustomColorManager.Cryomaniac),
        new("Bitton", "γ", CustomColorManager.Undead),
        new("Tormented", "§", CustomColorManager.Executioner),
        new("Watched", "★", UColor.white),
        new("Protected", "η", UColor.white),
        new("Agonised", "π", CustomColorManager.Guesser),
        new("Recruited", "$", CustomColorManager.Cabal),
        new("Resurrected", "Σ", CustomColorManager.Reanimated),
        new("Infected", "ρ", CustomColorManager.Plaguebearer),
        new("Plague", "米", CustomColorManager.Pestilence),
        new("Vested", "υ", CustomColorManager.Survivor),
        new("Persuaded", "Λ", CustomColorManager.Sect),
        new("Framed", "ς", CustomColorManager.Framer),
        new("Spellbound", "ø", CustomColorManager.Spellslinger),
        new("Blackmailed", "Φ", CustomColorManager.Blackmailer),
        new("Silenced", "乂", CustomColorManager.Silencer),
        new("Flashed", "ㅇ", CustomColorManager.Grenadier),
        new("Drived", "Δ", CustomColorManager.Syndicate),
        new("Friend", "ξ", CustomColorManager.Faction),
        new("Marked", "χ", CustomColorManager.Ghoul),
        new("First Dead", "Γ", CustomColorManager.FirstShield),
        new("Alerting", "σ", CustomColorManager.Veteran),
        new("Crusaded", "τ", CustomColorManager.Crusader),
        new("Ambushed", "人", CustomColorManager.Ambusher),
        new("Allied", "ζ", CustomColorManager.Allied),
        new("Corrupted", "δ", CustomColorManager.Corrupted),
        new("Defector", "ε", CustomColorManager.Defector),
        new("Fanatic", "♠", CustomColorManager.Fanatic),
        new("Linked", "Ψ", CustomColorManager.Linked),
        new("Lovers", "♥", CustomColorManager.Lovers),
        new("Mafia", "ω", CustomColorManager.Mafia),
        new("Overlord", "β", CustomColorManager.Overlord),
        new("Rivals", "α", CustomColorManager.Rivals),
        new("Taskmaster", "µ", CustomColorManager.Taskmaster),
        new("Traitor", "♣", CustomColorManager.Traitor, true)
    ];
}