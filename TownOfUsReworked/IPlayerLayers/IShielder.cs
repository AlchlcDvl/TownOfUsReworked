namespace TownOfUsReworked.IPlayerLayers;

public interface IShielder : IPlayerLayer
{
    public PlayerControl ShieldedPlayer { get; set; }
    public bool ShieldBroken { get; set; }
}