namespace TownOfUsReworked.IPlayerLayers;

public interface IBlocker : IPlayerLayer
{
    PlayerControl BlockTarget { get; }
}