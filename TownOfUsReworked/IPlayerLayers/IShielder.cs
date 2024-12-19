namespace TownOfUsReworked.IPlayerLayers;

public interface IShielder : IRole
{
    PlayerControl ShieldedPlayer { get; set; }
    bool ShieldBroken { get; set; }
}