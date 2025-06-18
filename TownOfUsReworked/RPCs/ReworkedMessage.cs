using AmongUs.InnerNet.GameDataMessages;

namespace TownOfUsReworked.RPCs;

/// <summary>
/// Base game message entry way for the mod's late RPCs.
/// </summary>
/// <param name="targetClientId">The id of the client that the message targets.</param>
/// <param name="payload">The byte data to be networked.</param>
public sealed class ReworkedMessage(int targetClientId, Il2CppStructArray<byte> payload) : BaseRpcMessage(LocalPlayer.NetId)
{
    /// <summary>
    /// Value injector to ensure seamless integration with the base game.
    /// </summary>
    private static readonly EnumInjector<RpcCalls> Injector = new(true, true);

    /// <summary>
    /// The custom injected enum value that indicates it's a modded rpc.
    /// </summary>
    private static readonly RpcCalls ReworkedType = Injector.InjectAndReturn("ReworkedRpc", CustomRPCCallID); // 255 is used by Reactor

    /// <summary>
    /// The id of the client that the message targets.
    /// </summary>
    private readonly int TargetClientId = targetClientId;

    /// <summary>
    /// The byte data to be networked.
    /// </summary>
    private readonly Il2CppStructArray<byte> Payload = payload;

    /// <summary>
    /// The type of the rpc.
    /// </summary>
    public override RpcCalls RpcType => ReworkedType;

    /// <summary>
    /// Serializes the rpc values to the game's message writer.
    /// </summary>
    /// <param name="msg">The network writer to serialise the data to.</param>
    public override void SerializeRpcValues(MessageWriter msg)
    {
        var flag = TargetClientId != -1;
        msg.Write(flag); // Flag for a late message, because AU currently does not have implementations of a targeted late rpc

        if (flag)
            msg.WritePacked(TargetClientId); // Target client id

        msg.WriteBytesAndSize(Payload); // Actual rpc stuff
    }
}