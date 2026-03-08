namespace TownOfUsReworked.RPCs.Messages;

public abstract class BaseReworkedMessage<T> : INetworkMessage where T : struct, Enum
{
    public abstract ReworkedRpc RpcCategory { get; }
    public abstract T RpcType { get; }

    public virtual void OnDispose() { }

    public void Dispose()
    {
        OnDispose();
        GC.SuppressFinalize(this);
    }

    public void SerializeTo(RpcWriter writer)
    {
        writer.WriteByte((byte)RpcCategory);
        writer.WriteEnum(RpcType);
        SerializeValues(writer);
    }

    public abstract void SerializeValues(RpcWriter writer);
}