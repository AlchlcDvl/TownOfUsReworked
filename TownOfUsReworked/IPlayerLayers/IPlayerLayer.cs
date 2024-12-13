namespace TownOfUsReworked.IPlayerLayers;

public interface IPlayerLayer
{
    PlayerControl Player { get; set; }

    bool Local { get; }
    UColor Color { get; }
    string Name { get; }
}