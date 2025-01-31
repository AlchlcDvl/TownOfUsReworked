namespace TownOfUsReworked.Data;

public static class AllInfo
{
    public static readonly IEnumerable<RoleInfo> AllRoles =
    [
        new(LayerEnum.None, Alignment.None, Faction.None, color: CustomColorManager.Role),

        new(LayerEnum.Altruist, Alignment.Protective, Faction.Crew),
        new(LayerEnum.Bastion, Alignment.Killing, Faction.Crew),
        new(LayerEnum.Chameleon, Alignment.Support, Faction.Crew),
        new(LayerEnum.Coroner, Alignment.Investigative, Faction.Crew),
        new(LayerEnum.Crewmate, Alignment.Utility, Faction.Crew),
        new(LayerEnum.Detective, Alignment.Investigative, Faction.Crew),
        new(LayerEnum.Dictator, Alignment.Sovereign, Faction.Crew),
        new(LayerEnum.Engineer, Alignment.Support, Faction.Crew),
        new(LayerEnum.Escort, Alignment.Support, Faction.Crew),
        new(LayerEnum.Mayor, Alignment.Sovereign, Faction.Crew),
        new(LayerEnum.Medic, Alignment.Protective, Faction.Crew),
        new(LayerEnum.Medium, Alignment.Investigative, Faction.Crew),
        new(LayerEnum.Monarch, Alignment.Sovereign, Faction.Crew),
        new(LayerEnum.Mystic, Alignment.Investigative, Faction.Crew),
        new(LayerEnum.Operative, Alignment.Investigative, Faction.Crew),
        new(LayerEnum.Retributionist, Alignment.Support, Faction.Crew),
        new(LayerEnum.Revealer, Alignment.Utility, Faction.Crew),
        new(LayerEnum.Seer, Alignment.Investigative, Faction.Crew),
        new(LayerEnum.Sheriff, Alignment.Investigative, Faction.Crew),
        new(LayerEnum.Shifter, Alignment.Support, Faction.Crew),
        new(LayerEnum.Tracker, Alignment.Investigative, Faction.Crew),
        new(LayerEnum.Transporter, Alignment.Support, Faction.Crew),
        new(LayerEnum.Trapper, Alignment.Protective, Faction.Crew),
        new(LayerEnum.Trickster, Alignment.Protective, Faction.Crew),
        new(LayerEnum.Veteran, Alignment.Killing, Faction.Crew),
        new(LayerEnum.Vigilante, Alignment.Killing, Faction.Crew, true),

        new(LayerEnum.Actor, Alignment.Evil, Faction.Neutral),
        new(LayerEnum.Amnesiac, Alignment.Benign, Faction.Neutral),
        new(LayerEnum.Arsonist, Alignment.Killing, Faction.Neutral),
        new(LayerEnum.Betrayer, Alignment.Proselyte, Faction.Neutral),
        new(LayerEnum.BountyHunter, Alignment.Evil, Faction.Neutral),
        new(LayerEnum.Cannibal, Alignment.Evil, Faction.Neutral),
        new(LayerEnum.Cryomaniac, Alignment.Killing, Faction.Neutral),
        new(LayerEnum.Dracula, Alignment.Neophyte, Faction.Neutral),
        new(LayerEnum.Executioner, Alignment.Evil, Faction.Neutral),
        new(LayerEnum.Glitch, Alignment.Killing, Faction.Neutral),
        new(LayerEnum.GuardianAngel, Alignment.Benign, Faction.Neutral),
        new(LayerEnum.Guesser, Alignment.Evil, Faction.Neutral),
        new(LayerEnum.Jackal, Alignment.Neophyte, Faction.Neutral),
        new(LayerEnum.Jester, Alignment.Evil, Faction.Neutral),
        new(LayerEnum.Juggernaut, Alignment.Killing, Faction.Neutral),
        new(LayerEnum.Murderer, Alignment.Killing, Faction.Neutral),
        new(LayerEnum.Necromancer, Alignment.Neophyte, Faction.Neutral),
        new(LayerEnum.Pestilence, Alignment.Apocalypse, Faction.Neutral),
        new(LayerEnum.Phantom, Alignment.Proselyte, Faction.Neutral),
        new(LayerEnum.Plaguebearer, Alignment.Harbinger, Faction.Neutral),
        new(LayerEnum.SerialKiller, Alignment.Killing, Faction.Neutral),
        new(LayerEnum.Survivor, Alignment.Benign, Faction.Neutral),
        new(LayerEnum.Thief, Alignment.Benign, Faction.Neutral),
        new(LayerEnum.Troll, Alignment.Evil, Faction.Neutral),
        new(LayerEnum.Werewolf, Alignment.Killing, Faction.Neutral),
        new(LayerEnum.Whisperer, Alignment.Neophyte, Faction.Neutral, true),

        new(LayerEnum.Ambusher, Alignment.Killing, Faction.Intruder),
        new(LayerEnum.Blackmailer, Alignment.Concealing, Faction.Intruder),
        new(LayerEnum.Camouflager, Alignment.Concealing, Faction.Intruder),
        new(LayerEnum.Consigliere, Alignment.Support, Faction.Intruder),
        new(LayerEnum.Consort, Alignment.Support, Faction.Intruder),
        new(LayerEnum.Disguiser, Alignment.Deception, Faction.Intruder),
        new(LayerEnum.Enforcer, Alignment.Killing, Faction.Intruder),
        new(LayerEnum.Ghoul, Alignment.Utility, Faction.Intruder),
        new(LayerEnum.Godfather, Alignment.Head, Faction.Intruder),
        new(LayerEnum.Grenadier, Alignment.Concealing, Faction.Intruder),
        new(LayerEnum.Impostor, Alignment.Utility, Faction.Intruder),
        new(LayerEnum.Janitor, Alignment.Concealing, Faction.Intruder),
        new(LayerEnum.Mafioso, Alignment.Utility, Faction.Intruder),
        new(LayerEnum.Miner, Alignment.Support, Faction.Intruder),
        new(LayerEnum.Morphling, Alignment.Deception, Faction.Intruder),
        new(LayerEnum.Teleporter, Alignment.Support, Faction.Intruder),
        new(LayerEnum.Wraith, Alignment.Deception, Faction.Intruder, true),

        new(LayerEnum.Anarchist, Alignment.Utility, Faction.Syndicate),
        new(LayerEnum.Banshee, Alignment.Utility, Faction.Syndicate),
        new(LayerEnum.Bomber, Alignment.Killing, Faction.Syndicate),
        new(LayerEnum.Collider, Alignment.Killing, Faction.Syndicate),
        new(LayerEnum.Concealer, Alignment.Disruption, Faction.Syndicate),
        new(LayerEnum.Crusader, Alignment.Killing, Faction.Syndicate),
        new(LayerEnum.Drunkard, Alignment.Disruption, Faction.Syndicate),
        new(LayerEnum.Framer, Alignment.Disruption, Faction.Syndicate),
        new(LayerEnum.Poisoner, Alignment.Killing, Faction.Syndicate),
        new(LayerEnum.Rebel, Alignment.Power, Faction.Syndicate),
        new(LayerEnum.Shapeshifter, Alignment.Disruption, Faction.Syndicate),
        new(LayerEnum.Sidekick, Alignment.Utility, Faction.Syndicate),
        new(LayerEnum.Silencer, Alignment.Disruption, Faction.Syndicate),
        new(LayerEnum.Spellslinger, Alignment.Power, Faction.Syndicate),
        new(LayerEnum.Stalker, Alignment.Support, Faction.Syndicate),
        new(LayerEnum.Timekeeper, Alignment.Disruption, Faction.Syndicate),
        new(LayerEnum.Warper, Alignment.Support, Faction.Syndicate, true),

        new(LayerEnum.Hunter, Alignment.HideAndSeek, Faction.GameMode),
        new(LayerEnum.Hunted, Alignment.HideAndSeek, Faction.GameMode),

        new(LayerEnum.Runner, Alignment.TaskRace, Faction.GameMode, true),
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
        new(Faction.Compliance),
        new(Faction.Pandorica),
        new(Faction.Illuminati),
        new(Faction.GameMode, true)
    ];

    public static readonly IEnumerable<SubFactionInfo> AllSubFactions =
    [
        new(SubFaction.None),
        new(SubFaction.Cult),
        new(SubFaction.Cabal),
        new(SubFaction.Undead),
        new(SubFaction.Reanimated, true)
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
        new(Alignment.Apocalypse),
        new(Alignment.Disruption),
        new(Alignment.Power),

        new(Alignment.HideAndSeek),
        new(Alignment.TaskRace, true)
    ];

    public static readonly IEnumerable<GameModeInfo> AllModes =
    [
        new(GameMode.None),

        new(GameMode.Vanilla),
        new(GameMode.Classic),
        new(GameMode.TaskRace),
        new(GameMode.RoleList),
        new(GameMode.HideAndSeek, true)
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
        new("Persuaded", "Λ", CustomColorManager.Cult),
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