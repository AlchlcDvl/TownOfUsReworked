namespace TownOfUsReworked.IPlayerLayers;

public interface IShaman : IPlayerLayer
{
    List<byte> MediatedPlayers { get; }
}