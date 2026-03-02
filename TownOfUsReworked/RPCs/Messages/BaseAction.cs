namespace TownOfUsReworked.RPCs.Messages;

public abstract class BaseActionMessage : BaseReworkedMessage
{
    public sealed override ReworkedRpc RpcCategory => ReworkedRpc.Action;

    public abstract ActionsRpc Rpc { get; }

    public sealed override void SerializeHeader(RpcWriter writer) => writer.WriteByte((byte)Rpc);
}