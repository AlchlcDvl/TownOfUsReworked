namespace TownOfUsReworked.RPCs.Messages.Actions;

public sealed class ButtonActionMessage(CustomButton button, BaseDataMessage nestedMessage) : BaseActionMessage
{
    private readonly CustomButton Button = button;
    private readonly BaseDataMessage NestedMessage = nestedMessage;

    public override ActionsRpc Rpc => ActionsRpc.ButtonAction;

    public override void SerializeValues(RpcWriter writer)
    {
        writer.WriteNetObject(Button);
        NestedMessage.SerializeValues(writer);
    }
}