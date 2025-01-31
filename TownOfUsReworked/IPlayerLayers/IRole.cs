namespace TownOfUsReworked.IPlayerLayers;

public interface IRole : IPlayerLayer
{
    Faction Faction { get; }
    LayerEnum LinkedDisposition { get; }
}