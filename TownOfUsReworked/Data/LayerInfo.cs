namespace TownOfUsReworked.Data;

/// <summary>
/// Info class that contains wiki entries.
/// </summary>
public static class AllInfo
{
    /// <summary>
    /// Wiki entries for all of the roles within the mod.
    /// </summary>
    public static readonly IEnumerable<RoleInfo> AllRoles =
    [
        new(Layer.None, Alignment.None, Faction.None, color: CustomColorManager.Role),

        new(Layer.Altruist, Alignment.Protective, Faction.Crew),
        new(Layer.Bastion, Alignment.Killing, Faction.Crew),
        new(Layer.Chameleon, Alignment.Support, Faction.Crew),
        new(Layer.Coroner, Alignment.Investigative, Faction.Crew),
        new(Layer.Crewmate, Alignment.Utility, Faction.Crew),
        new(Layer.Democrat, Alignment.Sovereign, Faction.Crew),
        new(Layer.Detective, Alignment.Investigative, Faction.Crew),
        new(Layer.Dictator, Alignment.Sovereign, Faction.Crew),
        new(Layer.Engineer, Alignment.Support, Faction.Crew),
        new(Layer.Escort, Alignment.Support, Faction.Crew),
        new(Layer.Mayor, Alignment.Sovereign, Faction.Crew),
        new(Layer.Medic, Alignment.Protective, Faction.Crew),
        new(Layer.Medium, Alignment.Investigative, Faction.Crew),
        new(Layer.Monarch, Alignment.Sovereign, Faction.Crew),
        new(Layer.Mystic, Alignment.Investigative, Faction.Crew),
        new(Layer.Operative, Alignment.Investigative, Faction.Crew),
        new(Layer.Retributionist, Alignment.Support, Faction.Crew),
        new(Layer.Revealer, Alignment.Utility, Faction.Crew),
        new(Layer.Seer, Alignment.Investigative, Faction.Crew),
        new(Layer.Sheriff, Alignment.Investigative, Faction.Crew),
        new(Layer.Tracker, Alignment.Investigative, Faction.Crew),
        new(Layer.Transporter, Alignment.Support, Faction.Crew),
        new(Layer.Trapper, Alignment.Protective, Faction.Crew),
        new(Layer.Veteran, Alignment.Killing, Faction.Crew),
        new(Layer.Vigilante, Alignment.Killing, Faction.Crew, true),

        new(Layer.Actor, Alignment.Evil, Faction.Outcast),
        new(Layer.Amnesiac, Alignment.Benign, Faction.Outcast),
        new(Layer.Arsonist, Alignment.Killing, Faction.Outcast),
        new(Layer.Betrayer, Alignment.Proselyte, Faction.Outcast),
        new(Layer.BountyHunter, Alignment.Evil, Faction.Outcast),
        new(Layer.Cannibal, Alignment.Evil, Faction.Outcast),
        new(Layer.Cryomaniac, Alignment.Killing, Faction.Outcast),
        new(Layer.Dracula, Alignment.Neophyte, Faction.Outcast),
        new(Layer.Executioner, Alignment.Evil, Faction.Outcast),
        new(Layer.Glitch, Alignment.Killing, Faction.Outcast),
        new(Layer.GuardianAngel, Alignment.Benign, Faction.Outcast),
        new(Layer.Guesser, Alignment.Evil, Faction.Outcast),
        new(Layer.Jackal, Alignment.Neophyte, Faction.Outcast),
        new(Layer.Jester, Alignment.Evil, Faction.Outcast),
        new(Layer.Juggernaut, Alignment.Killing, Faction.Outcast),
        new(Layer.Murderer, Alignment.Killing, Faction.Outcast),
        new(Layer.Necromancer, Alignment.Neophyte, Faction.Outcast),
        new(Layer.Phantom, Alignment.Proselyte, Faction.Outcast),
        new(Layer.SerialKiller, Alignment.Killing, Faction.Outcast),
        new(Layer.Shifter, Alignment.Evil, Faction.Outcast),
        new(Layer.Survivor, Alignment.Benign, Faction.Outcast),
        new(Layer.Thief, Alignment.Benign, Faction.Outcast),
        new(Layer.Troll, Alignment.Evil, Faction.Outcast),
        new(Layer.Werewolf, Alignment.Killing, Faction.Outcast),
        new(Layer.Whisperer, Alignment.Neophyte, Faction.Outcast, true),

        new(Layer.Ambusher, Alignment.Killing, Faction.Intruder),
        new(Layer.Blackmailer, Alignment.Concealing, Faction.Intruder),
        new(Layer.Camouflager, Alignment.Concealing, Faction.Intruder),
        new(Layer.Consigliere, Alignment.Support, Faction.Intruder),
        new(Layer.Consort, Alignment.Support, Faction.Intruder),
        new(Layer.Disguiser, Alignment.Deception, Faction.Intruder),
        new(Layer.Enforcer, Alignment.Killing, Faction.Intruder),
        new(Layer.Ghoul, Alignment.Utility, Faction.Intruder),
        new(Layer.Godfather, Alignment.Head, Faction.Intruder),
        new(Layer.Grenadier, Alignment.Concealing, Faction.Intruder),
        new(Layer.Impostor, Alignment.Utility, Faction.Intruder),
        new(Layer.Janitor, Alignment.Concealing, Faction.Intruder),
        new(Layer.Mafioso, Alignment.Utility, Faction.Intruder),
        new(Layer.Miner, Alignment.Support, Faction.Intruder),
        new(Layer.Morphling, Alignment.Deception, Faction.Intruder),
        new(Layer.Teleporter, Alignment.Support, Faction.Intruder),
        new(Layer.Wraith, Alignment.Deception, Faction.Intruder, true),

        new(Layer.Anarchist, Alignment.Utility, Faction.Syndicate),
        new(Layer.Banshee, Alignment.Utility, Faction.Syndicate),
        new(Layer.Bomber, Alignment.Killing, Faction.Syndicate),
        new(Layer.Collider, Alignment.Killing, Faction.Syndicate),
        new(Layer.Concealer, Alignment.Disruption, Faction.Syndicate),
        new(Layer.Crusader, Alignment.Killing, Faction.Syndicate),
        new(Layer.Drunkard, Alignment.Disruption, Faction.Syndicate),
        new(Layer.Framer, Alignment.Disruption, Faction.Syndicate),
        new(Layer.Poisoner, Alignment.Killing, Faction.Syndicate),
        new(Layer.Rebel, Alignment.Head, Faction.Syndicate),
        new(Layer.Shapeshifter, Alignment.Disruption, Faction.Syndicate),
        new(Layer.Sidekick, Alignment.Utility, Faction.Syndicate),
        new(Layer.Silencer, Alignment.Disruption, Faction.Syndicate),
        new(Layer.Spellslinger, Alignment.Head, Faction.Syndicate),
        new(Layer.Stalker, Alignment.Support, Faction.Syndicate),
        new(Layer.Timekeeper, Alignment.Disruption, Faction.Syndicate),
        new(Layer.Warper, Alignment.Support, Faction.Syndicate, true),

        new(Layer.Cultist, Alignment.Harbinger, Faction.Apocalypse),
        new(Layer.Pestilence, Alignment.Deity, Faction.Apocalypse),
        new(Layer.Plaguebearer, Alignment.Harbinger, Faction.Apocalypse),
        new(Layer.Void, Alignment.Deity, Faction.Apocalypse, true),

        new(Layer.Hunter, Alignment.HideAndSeek, Faction.GameMode),
        new(Layer.Hunted, Alignment.HideAndSeek, Faction.GameMode),

        new(Layer.Runner, Alignment.TaskRace, Faction.GameMode, true),
    ];

    public static readonly IEnumerable<ModifierInfo> AllModifiers =
    [
        new(Layer.None, color: CustomColorManager.Modifier),

        new(Layer.Astral),
        new(Layer.Bait),
        new(Layer.Colorblind),
        new(Layer.Coward),
        new(Layer.Diseased),
        new(Layer.Drunk),
        new(Layer.Dwarf),
        new(Layer.Giant),
        new(Layer.Indomitable),
        new(Layer.Shy),
        new(Layer.Vip),
        new(Layer.Volatile),
        new(Layer.Yeller, true)
    ];

    public static readonly IEnumerable<DispositionInfo> AllDispositions =
    [
        new(Layer.None, "φ", color: CustomColorManager.Disposition),
        new(Layer.Allied, "ζ"),
        new(Layer.Corrupted, "δ"),
        new(Layer.Defector, "ε"),
        new(Layer.Fanatic, "♠"),
        new(Layer.Linked, "Ψ"),
        new(Layer.Lovers, "♥"),
        new(Layer.Mafia, "ω"),
        new(Layer.Overlord, "β"),
        new(Layer.Rivals, "α"),
        new(Layer.Taskmaster, "µ"),
        new(Layer.Traitor, "♣", true)
    ];

    public static readonly IEnumerable<AbilityInfo> AllAbilities =
    [
        new(Layer.None, color: CustomColorManager.Ability),

        new(Layer.Assassin),
        new(Layer.Bullseye),
        new(Layer.ButtonBarry),
        new(Layer.Hitman),
        new(Layer.Insider),
        new(Layer.Multitasker),
        new(Layer.Ninja),
        new(Layer.Politician),
        new(Layer.Radar),
        new(Layer.Ruthless),
        new(Layer.Slayer),
        new(Layer.Sniper),
        new(Layer.Snitch),
        new(Layer.Swapper),
        new(Layer.Tiebreaker),
        new(Layer.Torch),
        new(Layer.Tunneler),
        new(Layer.Underdog, true)
    ];

    public static readonly IEnumerable<FactionInfo> AllFactions =
    [
        new(Faction.None),

        new(Faction.Crew),
        new(Faction.Intruder),
        new(Faction.Syndicate),
        new(Faction.Apocalypse),
        new(Faction.Outcast),
        new(Faction.Compliance),
        new(Faction.Pandorica),
        new(Faction.Illuminati),
        new(Faction.GameMode, true)
    ];

    public static readonly IEnumerable<AlignmentInfo> AllAlignments =
    [
        new(Alignment.None),

        new(Alignment.Support),
        new(Alignment.Investigative),
        new(Alignment.Protective),
        new(Alignment.Killing),
        new(Alignment.Utility),
        new(Alignment.Sovereign),
        new(Alignment.Concealing),
        new(Alignment.Deception),
        new(Alignment.Head),
        new(Alignment.Neophyte),
        new(Alignment.Evil),
        new(Alignment.Benign),
        new(Alignment.Proselyte),
        new(Alignment.Harbinger),
        new(Alignment.Deity),
        new(Alignment.Disruption),

        new(Alignment.HideAndSeek),
        new(Alignment.TaskRace, true)
    ];

    public static readonly IEnumerable<GameModeInfo> AllModes =
    [
        new(Mode.None),

        new(Mode.Vanilla),
        new(Mode.Classic),
        new(Mode.AllAny),
        new(Mode.TaskRace),
        new(Mode.List),
        new(Mode.HideAndSeek, true)
    ];

    public static readonly IEnumerable<OtherInfo> AllOthers =
    [
        new("ChaosDrive", CustomColorManager.Syndicate, "Drive", true)
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
        new("Tormented", "§", CustomColorManager.Executioner),
        new("Watched", "★", UColor.white),
        new("Protected", "η", UColor.white),
        new("Agonised", "π", CustomColorManager.Guesser),
        new("Infected", "ρ", CustomColorManager.Plaguebearer),
        new("Plague", "米", CustomColorManager.Pestilence),
        new("Vested", "υ", CustomColorManager.Survivor),
        new("Framed", "ς", CustomColorManager.Framer),
        new("Spellbound", "ø", CustomColorManager.Spellslinger),
        new("Blackmailed", "Φ", CustomColorManager.Blackmailer),
        new("Silenced", "乂", CustomColorManager.Silencer),
        new("Flashed", "ㅇ", CustomColorManager.Grenadier),
        new("Campaigned", "°", CustomColorManager.Democrat),
        new("Driven", "Δ", CustomColorManager.Syndicate),
        new("Friend", "ξ", CustomColorManager.Faction),
        new("Marked", "χ", CustomColorManager.Ghoul),
        new("FirstDead", "Γ", CustomColorManager.FirstShield),
        new("Alerting", "σ", CustomColorManager.Veteran),
        new("Crusaded", "τ", CustomColorManager.Crusader),
        new("Positive", "+", CustomColorManager.Collider),
        new("Negative", "-", CustomColorManager.Collider),
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