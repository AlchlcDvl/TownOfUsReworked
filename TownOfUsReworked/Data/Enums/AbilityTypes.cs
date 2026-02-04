namespace TownOfUsReworked.Data.Enums;

[Flags]
public enum ReworkedAbilityTypes : byte
{
    None = 0,
    Targetless = 1 << 0,
    Player = 1 << 1,
    Body = 1 << 2,
    Vent = 1 << 3,
    Console = 1 << 4
}