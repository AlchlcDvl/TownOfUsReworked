namespace TownOfUsReworked.RPCs.Messages.Misc;

public sealed class LoadPresetMessage(string presetName) : BaseMiscMessage
{
    public override MiscRpc RpcType => MiscRpc.SyncCustomSettings;

    private readonly string PresetName = presetName;

    public override void SerializeValues(RpcWriter writer) => writer.WriteString(PresetName);
}