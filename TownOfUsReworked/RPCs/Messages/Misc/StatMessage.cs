
namespace TownOfUsReworked.RPCs.Messages.Misc;

public sealed class StatMessage(StringNames stat) : BaseMiscMessage
{
    public override MiscRpc RpcType => MiscRpc.Stat;

    private readonly StringNames Stat = stat;

    public override void SerializeValues(RpcWriter writer) => writer.WriteEnum(Stat);
}