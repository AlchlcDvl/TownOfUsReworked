namespace TownOfUsReworked.IPlayerLayers;

public interface IShaman : IPlayerLayer
{
    HashSet<byte> MediatedPlayers { get; }
}