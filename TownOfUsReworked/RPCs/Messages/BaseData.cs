namespace TownOfUsReworked.RPCs.Messages;

// Special headerless message to handle pure data
public abstract class BaseDataMessage : BaseReworkedMessage
{
    public override ReworkedRpc RpcCategory => RpcCategory;

    public override void SerializeHeader(RpcWriter writer) => throw new NotSupportedException();
}