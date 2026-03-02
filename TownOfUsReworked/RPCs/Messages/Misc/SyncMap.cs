namespace TownOfUsReworked.RPCs.Messages.Misc;

public sealed class SyncMapMessage : BaseMiscMessage
{
    public override MiscRpc Rpc => MiscRpc.SyncMap;

    public override void SerializeValues(RpcWriter writer) => writer.WriteByte((byte)MapSettings.Map);
}