namespace TownOfUsReworked.IPlayerLayers;

public interface IPlayerLayer
{
    public PlayerControl Player { get; set; }

    public bool Local { get; }
    public UColor Color { get; }
    public string Name { get; }
}