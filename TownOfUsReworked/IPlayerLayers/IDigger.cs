namespace TownOfUsReworked.IPlayerLayers;

public interface IDigger : IPlayerLayer
{
    public List<Vent> Vents { get; set; }
}