namespace TownOfUsReworked.IPlayerLayers;

public interface IDouser : IPlayerLayer
{
    HashSet<byte> Doused { get; }
}