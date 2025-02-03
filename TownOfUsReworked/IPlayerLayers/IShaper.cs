namespace TownOfUsReworked.IPlayerLayers;

public interface IShaper : IPlayerLayer
{
    public PlayerControl ShapeshiftPlayer1 { get; }
    public PlayerControl ShapeshiftPlayer2 { get; }
}