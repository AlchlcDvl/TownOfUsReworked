namespace TownOfUsReworked.Data.Enums;

public enum Faction : byte
{
    None,

    // Natural
    Crew,
    Intruder,
    Syndicate,
    Apocalypse,
    Outcast,

    // Special
    Pandorica, // Int + Syn
    Compliance, // NK + NN
    Illuminati, // Pand + Comp
    GameMode,

    // NKs
    Arsonist,
    Cryomaniac,
    Glitch,
    Juggernaut,
    Murderer,
    SerialKiller,
    Werewolf,

    // Disposition
    Mafia,

    // Converters
    Cabal,
    Cult,
    Followers,
    Reanimated,
    Undead
}