using System.Buffers;
using AmongUs.InnerNet.GameDataMessages;

namespace TownOfUsReworked.RPCs;

/// <summary>
/// Base game message entry way for the mod's RPCs.
/// </summary>
public sealed class ReworkedMessage : BaseRpcMessage, IDisposable
{
    /// <summary>
    /// Value injector to ensure seamless integration with the base game.
    /// </summary>
    private static readonly EnumInjector<RpcCalls> Injector = new();

    /// <summary>
    /// The custom injected enum value that indicates it's a modded rpc.
    /// </summary>
    private static readonly RpcCalls ReworkedType = Injector.InjectAndReturn("ReworkedRpc", CustomRPCCallID); // 255 is used by Reactor, so we use 254 instead

    /// <summary>
    /// The id of the client that the message targets.
    /// </summary>
    public readonly int TargetClientId;

    /// <summary>
    /// The byte data to be networked.
    /// </summary>
    private byte[] Payload;

    /// <summary>
    /// The actual size of the payload because of funky ArrayPool behaviour.
    /// </summary>
    private readonly int PayloadSize;

    /// <summary>
    /// The type of the rpc.
    /// </summary>
    public override RpcCalls RpcType => ReworkedType;

    /// <summary>
    /// Initialises a new message;
    /// </summary>
    /// <param name="targetClientId">The id of the client that the message targets.</param>
    /// <param name="payload">The byte data to be networked.</param>
    public ReworkedMessage(int targetClientId, Span<byte> payload) : base(LocalPlayer.NetId)
    {
        TargetClientId = targetClientId;
        PayloadSize = payload.Length;
        Payload = ArrayPool<byte>.Shared.Rent(PayloadSize);
        payload.CopyTo(Payload.AsSpan(0, PayloadSize));
    }

    /// <summary>
    /// Serializes the rpc values to the game's message writer.
    /// </summary>
    /// <param name="msg">The network writer to serialise the data to.</param>
    public override void SerializeRpcValues(MessageWriter msg)
    {
        // The bulk method causes il2cpp casting, so to avoid that I'm doing this
        msg.WritePacked((uint)PayloadSize);

        for (var i = 0; i < PayloadSize; i++)
            msg.Write(Payload[i]);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ArrayPool<byte>.Shared.Return(Payload);
        Payload = null!;
    }
}