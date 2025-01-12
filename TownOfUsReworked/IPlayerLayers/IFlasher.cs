namespace TownOfUsReworked.IPlayerLayers;

public interface IFlasher : IPlayerLayer
{
    IEnumerable<byte> FlashedPlayers { get; set; }
}