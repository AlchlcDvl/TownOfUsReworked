namespace TownOfUsReworked.RPCs.Messages;

// Special headerless message to handle pure data
public abstract class BaseDataMessage : BaseReworkedMessage<ReworkedRpc>
{
    public sealed override ReworkedRpc RpcCategory => throw new NotSupportedException();
    public sealed override ReworkedRpc RpcType => throw new NotSupportedException();
}