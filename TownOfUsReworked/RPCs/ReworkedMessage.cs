using AmongUs.InnerNet.GameDataMessages;

namespace TownOfUsReworked.RPCs;

/// <summary>
/// Base game message entry way for the mod's late RPCs.
/// </summary>
/// <param name="netId">The net id to trigger the rpc in.</param>
/// <param name="targetClientId">The id of the client that the message targets.</param>
/// <param name="payload">The byte data to be networked.</param>
public sealed class ReworkedMessage(uint netId, int targetClientId, byte[] payload) : BaseRpcMessage(netId)
{
    /// <summary>
    /// The The id of the client that the message targets.
    /// </summary>
    private int TargetClientId { get; } = targetClientId;

    /// <summary>
    /// The byte data to be networked.
    /// </summary>
    [HideFromIl2Cpp]
    private byte[] Payload { get; } = payload;

    /// <summary>
    /// Value injector to ensure seamless integration with the base game.
    /// </summary>
    private static readonly EnumInjector<RpcCalls> Injector = new(false);

    /// <summary>
    /// The custom injected enum value that indicates it's a modded rpc.
    /// </summary>
    private static readonly RpcCalls ReworkedType = Injector.InjectAndReturn("ReworkedRpc", 254); // 255 is used by Reactor

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
        msg.Write(true); // Flag for a late message, because AU currently does not have implementations of a targeted late rpc
        msg.WritePacked(TargetClientId); // Target client id
        msg.WriteBytesAndSize(Payload); // Actual rpc stuff
    }
}