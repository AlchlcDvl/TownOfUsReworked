namespace TownOfUsReworked.IPlayerLayers;

public interface IPlayerLayer
{
    PlayerControl Player { get; }
    bool Local { get; }
    UColor Color { get; }
    string Name { get; }
    bool Dead { get; }
    bool Disconnected { get; }
}