namespace TownOfUsReworked.Data.Enums;

public enum Faction : byte
{
    None,
    Crew,
    Intruder,
    Syndicate,
    Neutral,
    Pandorica, // Int + Syn
    Compliance, // NK + NH + NA + NN
    Illuminati, // Pand + Comp
    GameMode
}