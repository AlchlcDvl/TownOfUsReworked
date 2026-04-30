namespace TownOfUsReworked.RPCs.Messages;

public abstract class BaseMiscMessage(MiscRpc rpc) : BaseReworkedMessage<MiscRpc>
{
    public sealed override ReworkedRpc RpcCategory => ReworkedRpc.Misc;
    public sealed override MiscRpc RpcType => rpc;
}