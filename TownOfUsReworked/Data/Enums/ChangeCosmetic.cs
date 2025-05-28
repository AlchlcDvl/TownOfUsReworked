namespace TownOfUsReworked.Data.Enums;

[Flags]
public enum ChangeCosmetics
{
    None = 0,
    Color = 1 << 0,
    Hat = 1 << 1,
    Visor = 1 << 2,
    Skin = 1 << 3,
    Pet = 1 << 4,
    Name = 1 << 5
}