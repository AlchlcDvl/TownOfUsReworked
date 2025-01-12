namespace TownOfUsReworked.IPlayerLayers;

public interface IFramer : IPlayerLayer
{
    List<byte> Framed { get; }
}