namespace TownOfUsReworked.IPlayerLayers;

public interface IDigger : IPlayerLayer
{
    List<Vent> Vents { get; set; }
}