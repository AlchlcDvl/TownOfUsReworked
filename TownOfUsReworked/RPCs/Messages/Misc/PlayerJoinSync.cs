namespace TownOfUsReworked.RPCs.Messages.Misc;

public sealed class PlayerJoinSyncMessage : BaseMiscMessage
{
    public override MiscRpc Rpc => MiscRpc.PlayerJoinSync;

    public override void SerializeValues(RpcWriter writer)
    {
        writer.WriteByte((byte)MapSettings.Map);

        var cache = Summary is not null;
        writer.WritePackedBool(cache);

        var cache2 = CachedFirstDead is not null;
        writer.WritePackedBool(cache2);

        if (cache)
            writer.WriteNetObject(Summary);

        if (cache2)
            writer.WriteString(CachedFirstDead);
    }
}