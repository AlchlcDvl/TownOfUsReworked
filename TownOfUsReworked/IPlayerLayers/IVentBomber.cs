namespace TownOfUsReworked.IPlayerLayers;

public interface IVentBomber : IRole
{
    List<int> BombedIDs { get; }
}