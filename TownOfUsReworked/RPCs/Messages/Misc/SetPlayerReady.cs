namespace TownOfUsReworked.RPCs.Messages.Misc;

public sealed class SetPlayerReadyMessage(PlayerControl player) : BaseMiscMessage
{
    public override MiscRpc Rpc => MiscRpc.SyncCustomSettings;

    private readonly byte PlayerId = player.PlayerId;

    public override void SerializeValues(RpcWriter writer) => writer.WriteByte(PlayerId);
}