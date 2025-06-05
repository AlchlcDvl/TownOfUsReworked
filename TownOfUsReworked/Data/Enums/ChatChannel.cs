namespace TownOfUsReworked.Data.Enums;

[Flags]
public enum ChatChannel : byte
{
    None = 0,
    Lovers = 1 << 0,
    Rivals = 1 << 1,
    Linked = 1 << 2,
    Mafia = 1 << 3,
    Dead = 1 << 4,
    Meeting = 1 << 5,
    All = Lovers | Rivals | Linked | Mafia | Dead | Meeting
}