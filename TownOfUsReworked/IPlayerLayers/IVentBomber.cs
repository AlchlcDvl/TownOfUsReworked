namespace TownOfUsReworked.IPlayerLayers;

public interface IVentBomber : IPlayerLayer
{
    List<int> BombedIDs { get; set; }
}