namespace TownOfUsReworked.IPlayerLayers;

public interface IRole : IPlayerLayer
{
    Faction Faction { get; set; }
    LayerEnum LinkedDisposition { get; set; }
}