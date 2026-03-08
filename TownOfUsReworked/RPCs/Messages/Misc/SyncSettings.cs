namespace TownOfUsReworked.RPCs.Messages.Misc;

public sealed class SyncSettingsMessage(Option[] options) : BaseMiscMessage
{
    public override MiscRpc RpcType => MiscRpc.SyncCustomSettings;

    private readonly Option[] Options = options;

    public override void SerializeValues(RpcWriter writer) => writer.WriteArray(Options, RpcWriterDels.NetObject<Option>.Writer, CountType.UShort);
}