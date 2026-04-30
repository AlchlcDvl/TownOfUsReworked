namespace TownOfUsReworked.RPCs.Messages;

public abstract class BaseReworkedMessage<T> : INetworkMessage where T : struct, Enum
{
    public abstract ReworkedRpc RpcCategory { get; }
    public abstract T RpcType { get; }

    public void Dispose()
    {
        OnDispose();
        GC.SuppressFinalize(this);
    }

    public void SerializeTo(RpcWriter writer)
    {
        writer.WriteEnum(RpcCategory);
        writer.WriteEnum(RpcType);
        SerializeValues(writer);
    }

    public virtual void SerializeValues(RpcWriter writer) { }

    public virtual void OnDispose() { }
}