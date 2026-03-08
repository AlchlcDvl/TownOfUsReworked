namespace TownOfUsReworked.RPCs.Messages;

public abstract class BaseActionMessage : BaseReworkedMessage<ActionsRpc>
{
    public sealed override ReworkedRpc RpcCategory => ReworkedRpc.Action;
}