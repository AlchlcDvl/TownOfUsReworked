namespace TownOfUsReworked.RPCs.Messages;

public abstract class BaseVanillaMessage : BaseReworkedMessage
{
    public sealed override ReworkedRpc RpcCategory => ReworkedRpc.Vanilla;

    public abstract VanillaRpc Rpc { get; }

    public sealed override void SerializeHeader(RpcWriter writer) => writer.WriteByte((byte)Rpc);
}