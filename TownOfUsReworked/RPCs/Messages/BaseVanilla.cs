namespace TownOfUsReworked.RPCs.Messages;

public abstract class BaseVanillaMessage : BaseReworkedMessage<VanillaRpc>
{
    public sealed override ReworkedRpc RpcCategory => ReworkedRpc.Vanilla;
}