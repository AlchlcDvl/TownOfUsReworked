namespace TownOfUsReworked.RPCs.Messages.Actions;

public sealed class CancelMessage(CustomButton button) : BaseActionMessage
{
    private readonly CustomButton Button = button;

    public override ActionsRpc Rpc => ActionsRpc.Cancel;

    public override void SerializeValues(RpcWriter writer) => writer.WriteNetObject(Button);
}