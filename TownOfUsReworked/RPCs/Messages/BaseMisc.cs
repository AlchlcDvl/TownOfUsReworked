namespace TownOfUsReworked.RPCs.Messages;

public abstract class BaseMiscMessage : BaseReworkedMessage
{
    public sealed override ReworkedRpc RpcCategory => ReworkedRpc.Misc;

    public abstract MiscRpc Rpc { get; }

    public sealed override void SerializeHeader(RpcWriter writer) => writer.WriteByte((byte)Rpc);
}