namespace TownOfUsReworked.IPlayerLayers;

public interface IPositioner : IPlayerLayer
{
    Vector3 LastPosition { get; set; }
}