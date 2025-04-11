namespace TownOfUsReworked.Data.Enums;

public enum Faction : byte
{
    None,
    Crew,
    Intruder,
    Syndicate,
    Apocalypse,
    Neutral,
    Pandorica, // Int + Syn
    Compliance, // NK + NN
    Illuminati, // Pand + Comp
    GameMode
}