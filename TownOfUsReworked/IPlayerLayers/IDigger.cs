namespace TownOfUsReworked.IPlayerLayers;

public interface IDigger : IRole
{
    List<Vent> Vents { get; }
}