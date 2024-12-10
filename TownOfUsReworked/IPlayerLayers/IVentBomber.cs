namespace TownOfUsReworked.IPlayerLayers;

public interface IVentBomber : IPlayerLayer
{
    public List<int> BombedIDs { get; set; }
}