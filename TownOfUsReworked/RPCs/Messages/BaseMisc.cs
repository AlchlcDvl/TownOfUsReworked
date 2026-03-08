namespace TownOfUsReworked.RPCs.Messages;

public abstract class BaseMiscMessage : BaseReworkedMessage<MiscRpc>
{
    public sealed override ReworkedRpc RpcCategory => ReworkedRpc.Misc;
}