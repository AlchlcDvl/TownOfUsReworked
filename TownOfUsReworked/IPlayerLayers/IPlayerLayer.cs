namespace TownOfUsReworked.IPlayerLayers;

public interface IPlayerLayer
{
    PlayerControl Player { get; }
    bool Local { get; }
    UColor Color { get; }
    bool Disconnected { get; }

    void ReadRPC(RpcReader reader);

    void PerformRpcAction(params object[] args);
}