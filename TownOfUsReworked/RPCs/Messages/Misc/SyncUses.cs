namespace TownOfUsReworked.RPCs.Messages.Misc;

public abstract class BaseSyncUsesMessage(CustomButton button, int uses) : BaseMiscMessage
{
    private readonly CustomButton Button = button;
    private readonly int Uses = uses;

    public sealed override void SerializeValues(RpcWriter writer)
    {
        writer.WriteNetObject(Button);
        writer.WritePackedInt(Uses);
    }
}

public sealed class SyncUsesMessage(CustomButton button, int uses) : BaseSyncUsesMessage(button, uses)
{
    public override MiscRpc Rpc => MiscRpc.SyncUses;
}

public sealed class SyncMaxUsesMessage(CustomButton button, int uses) : BaseSyncUsesMessage(button, uses)
{
    public override MiscRpc Rpc => MiscRpc.SyncMaxUses;
}