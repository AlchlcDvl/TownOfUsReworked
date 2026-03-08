namespace TownOfUsReworked.RPCs.Messages;

public interface INetworkMessage : INetSerializable, IDisposable
{
    ReworkedRpc RpcCategory { get; }

    void SerializeValues(RpcWriter writer);
}