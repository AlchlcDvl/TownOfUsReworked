namespace TownOfUsReworked.RPCs.Messages;

public abstract class BaseReworkedMessage : IDisposable, INetSerializable
{
    public abstract ReworkedRpc RpcCategory { get; }

    public virtual void OnDispose() { }

    public void Dispose()
    {
        OnDispose();
        GC.SuppressFinalize(this);
    }

    public void SerializeTo(RpcWriter writer)
    {
        writer.WriteByte((byte)RpcCategory);
        SerializeHeader(writer);
    }

    public abstract void SerializeHeader(RpcWriter writer);

    public abstract void SerializeValues(RpcWriter writer);
}