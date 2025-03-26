namespace TownOfUsReworked.IPlayerLayers;

public interface IPlayerLayer
{
    PlayerControl Player { get; }
    bool Local { get; }
    UColor Color { get; }
    string Name { get; }
    bool Disconnected { get; }
    bool Winner { get; set; }

    void ReadRPC(NetData reader);
}