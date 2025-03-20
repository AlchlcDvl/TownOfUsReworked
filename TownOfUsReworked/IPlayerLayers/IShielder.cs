namespace TownOfUsReworked.IPlayerLayers;

public interface IShielder : IPlayerLayer
{
    PlayerControl ShieldedPlayer { get; set; }
    bool ShieldBroken { get; set; }
}