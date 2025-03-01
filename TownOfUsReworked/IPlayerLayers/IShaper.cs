namespace TownOfUsReworked.IPlayerLayers;

public interface IShaper : IPlayerLayer
{
    PlayerControl ShapeshiftPlayer1 { get; }
    PlayerControl ShapeshiftPlayer2 { get; }
}