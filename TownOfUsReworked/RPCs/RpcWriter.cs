using System.Buffers;
using AmongUs.InnerNet.GameDataMessages;

namespace TownOfUsReworked.RPCs;

/// <summary>
/// A network writer for creating and sending RPC messages.
/// </summary>
public sealed class RpcWriter : IDisposable
{
    /// <summary>
    /// A list of bytes written to be sent for an RPC. This is the internal buffer.
    /// </summary>
    private byte[] Payload;

    /// <summary>
    /// Gets or sets a flag value that indicates whether or not the writer has been disposed of.
    /// </summary>
    private bool Disposed;

    /// <summary>
    /// Gets or sets the current pointer position in the data stream.
    /// </summary>
    private int Position;

    /// <summary>
    /// Initializes a new instance of the <see cref="RpcWriter"/> class for writing byte data.
    /// </summary>
    /// <param name="rpc">The sub RPC header enum value for this message.</param>
    /// <param name="withTypeCode">A flag indicating whether type codes should be used for initial data serialization.</param>
    /// <param name="data">Optional initial data to serialize into the buffer during initialization.</param>
    public RpcWriter(Enum rpc, bool withTypeCode, params object[] data)
    {
        Payload = ArrayPool<byte>.Shared.Rent(256);
        Position = 0;
        var header = rpc switch
        {
            ActionsRpc => ReworkedRpc.Action,
            MiscRpc => ReworkedRpc.Misc,
            VanillaRpc => ReworkedRpc.Vanilla,
            // TestRpc => ReworkedRpc.Test,
            ReworkedRpc => throw new InvalidOperationException("Can't use ReworkedRpc in this context"),
            _ => throw new InvalidDataException($"Unknown enum {rpc.GetType().Name} is being used")
        };
        Payload[Position++] = (byte)header;
        Payload[Position++] = (byte)Convert.ChangeType(rpc, typeof(byte)); // This is always byte and known so no blind deserialisation until here

        foreach (var b in data.SelectMany(x => GetBytes(x, withTypeCode)))
            Payload[Position++] = b;
    }

    /// <summary>
    /// RpcWriter destructor.
    /// </summary>
    ~RpcWriter() => InternalDispose();

    /// <summary>
    /// Throws errors to ensure proper flow of writing bytes.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the writer has been disposed.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
    /// <exception cref="OverflowException">Thrown if too much data is being packed into the buffer.</exception>
    private void ThrowIfIncorrectState(object value)
    {
        if (Disposed)
            throw new ObjectDisposedException(nameof(RpcWriter));

        if (value is null)
            throw new ArgumentNullException(nameof(value), "Cannot serialize a null value.");
    }

    /// <summary>
    /// Sends the written data to everyone, or optionally to only one player.
    /// </summary>
    /// <param name="targetClientId">The player to be sent the RPC to, or <c>-1</c> for everyone.</param>
    public void Send(int targetClientId = -1)
    {
        if (!TownOfUsReworked.MciActive && LocalPlayer)
            RpcHandler.SendImmediateMessage(ToMessage(targetClientId));
    }

    /// <summary>
    /// Sends the written data to everyone, or optionally to only one player, as a late message.<br/>
    /// Late messages are queued and processed a little later.
    /// </summary>
    /// <inheritdoc cref="Send(int)"/>
    public void SendLate(int targetClientId = -1)
    {
        if (!TownOfUsReworked.MciActive && LocalPlayer)
            Rpc.Instance.QueueLateMessage(ToMessage(targetClientId));
    }

    /// <summary>
    /// Converts the writer into a message to be sent.
    /// </summary>
    /// <param name="targetClientId">The player to be sent the RPC to, or <c>-1</c> for everyone.</param>
    /// <returns>A message instance representing the writer.</returns>
    private ReworkedMessage ToMessage(int targetClientId = -1) => new(targetClientId, Payload[..Position]);

    /// <summary>
    /// Writes the bytes of a single value to the internal buffer.
    /// </summary>
    /// <param name="value">The value to serialize.</param>
    /// <param name="withTypeCode">Indicates whether the value's type code should also be serialized for blind deserialization.</param>
    /// <inheritdoc cref="ThrowIfIncorrectState(object)"/>
    public void Write(object value, bool withTypeCode = false)
    {
        ThrowIfIncorrectState(value);
        var bytes = GetBytes(value, withTypeCode);
        EnsureCapacity(bytes.Count());

        foreach (var b in bytes)
            Payload[Position++] = b;
    }

    /// <summary>
    /// Writes the bytes of multiple values to the internal buffer.
    /// </summary>
    /// <param name="values">The values to serialize.</param>
    /// <param name="withTypeCode">Indicates whether a value's type code should also be serialized for blind deserialization.</param>
    /// <inheritdoc cref="ThrowIfIncorrectState(object)"/>
    public void Write(bool withTypeCode, params object[] values) => values.Do(x => Write(x, withTypeCode));

    /// <summary>
    /// Ensures there's enough capacity in the internal buffer for the given number of bytes.
    /// </summary>
    /// <param name="additionalAmount">The number of bytes required.</param>
    private void EnsureCapacity(int additionalAmount)
    {
        var requiredCapacity = Position + additionalAmount;

        if (requiredCapacity <= Payload.Length)
            return;

        var newCapacity = Payload.Length;

        while (newCapacity < requiredCapacity)
            newCapacity *= 2;

        var newBuffer = ArrayPool<byte>.Shared.Rent(newCapacity);
        Buffer.BlockCopy(Payload, 0, newBuffer, 0, Position);
        ArrayPool<byte>.Shared.Return(Payload);
        Payload = newBuffer;
    }

    /// <summary>
    /// Internal shared disposal between the destructor and the dispose method.
    /// </summary>
    private void InternalDispose()
    {
        if (Disposed)
            return;

        if (Payload is not null)
        {
            ArrayPool<byte>.Shared.Return(Payload);
            Payload = null;
        }

        Disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        InternalDispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Serializes the value to a sequence of bytes, optionally prepending its type code.<br/>
    /// This is the entry point for general object serialization.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <param name="withTypeCode">If true, prepends the object's type code to the serialized bytes.</param>
    /// <returns>An IEnumerable of bytes representing the serialized value.</returns>
    /// <exception cref="NotSupportedException">Thrown if the value's type cannot be serialized.</exception>
    private static IEnumerable<byte> GetBytes(object value, bool withTypeCode)
    {
        if (withTypeCode)
            yield return value.GetType().GetIdFromType();

        foreach (var b in InternalGetBytes(value, withTypeCode))
            yield return b;
    }

    private static IEnumerable<byte> InternalGetBytes(object value, bool withTypeCode) => value switch
    {
        // Game types
        PlayerControl i => GetBytes(i),
        DeadBody i => GetBytes(i),
        PlayerVoteArea i => GetBytes(i),
        Vent i => GetBytes(i),
        Vector2 i => GetBytes(i),
        Color32 i => GetBytes(i),

        // Custom types
        INetSerializable i => i.GetBytes(),

        // Primitives
        byte i => GetBytes(i),
        bool i => GetBytes(i),
        sbyte i => GetBytes(i),
        char i => GetBytes(i),
        ushort i => GetBytes(i),
        short i => GetBytes(i),
        uint i => GetBytes(i),
        int i => GetBytes(i),
        ulong i => GetBytes(i),
        long i => GetBytes(i),
        Half i => GetBytes(i),
        float i => GetBytes(i),
        double i => GetBytes(i),
        decimal i => GetBytes(i),
        string i => GetBytes(i),
        Type i => GetBytes(i),
        IEnumerable i => GetBytes(i, withTypeCode),
        Enum i => GetBytes(i, withTypeCode),

        // Other
        _ => throw new NotSupportedException($"Type {value.GetType().Name} cannot be serialized to bytes. Ensure it's a known primitive or base game type, or that it implements INetSerializable.")
    };

    private static IEnumerable<byte> GetBytes(byte value)
    {
        yield return value;
    }

    private static IEnumerable<byte> GetBytes(bool value) => GetBytes((byte)(value ? 1 : 0));

    private static IEnumerable<byte> GetBytes(sbyte value) => GetBytes((byte)(value + 128));

    private static IEnumerable<byte> GetBytes(char value) => BitConverter.GetBytes(value);

    public static IEnumerable<byte> GetBytes(ushort value) => BitConverter.GetBytes(value);

    private static IEnumerable<byte> GetBytes(short value) => BitConverter.GetBytes(value);

    private static IEnumerable<byte> GetBytes(uint value) => BitConverter.GetBytes(value);

    private static IEnumerable<byte> GetBytes(int value) => BitConverter.GetBytes(value);

    private static IEnumerable<byte> GetBytes(ulong value) => BitConverter.GetBytes(value);

    private static IEnumerable<byte> GetBytes(long value) => BitConverter.GetBytes(value);

    private static IEnumerable<byte> GetBytes(Half value) => BitConverter.GetBytes(value);

    public static IEnumerable<byte> GetBytes(float value) => BitConverter.GetBytes(value);

    private static IEnumerable<byte> GetBytes(double value) => BitConverter.GetBytes(value);

    private static IEnumerable<byte> GetBytes(decimal value) => decimal.GetBits(value).SelectMany(BitConverter.GetBytes);

    public static IEnumerable<byte> GetBytes(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);

        foreach (var b in GetBytes((ushort)bytes.Length))
            yield return b;

        foreach (var b in bytes)
            yield return b;
    }

    private static IEnumerable<byte> GetBytes(PlayerControl value) => GetBytes(value.PlayerId);

    private static IEnumerable<byte> GetBytes(DeadBody value) => GetBytes(value.ParentId);

    private static IEnumerable<byte> GetBytes(PlayerVoteArea value) => GetBytes(value.TargetPlayerId);

    private static IEnumerable<byte> GetBytes(Vent value) => BitConverter.GetBytes(value.Id);

    private static IEnumerable<byte> GetBytes(Vector2 value)
    {
        foreach (var b in GetBytes((ushort)(Generate.ReverseLerp(value.x) * ushort.MaxValue)))
            yield return b;

        foreach (var b in GetBytes((ushort)(Generate.ReverseLerp(value.y) * ushort.MaxValue)))
            yield return b;
    }

    private static IEnumerable<byte> GetBytes(Type value) => GetBytes(value.AssemblyQualifiedName);

    public static IEnumerable<byte> GetBytes(Enum value, bool withTypeCode)
    {
        var type = value.GetType();

        if (withTypeCode)
        {
            foreach (var b in GetBytes(type))
                yield return b;
        }

        foreach (var b in GetBytes(Convert.ChangeType(value, Enum.GetUnderlyingType(type)), false))
            yield return b;
    }

    public static IEnumerable<byte> GetBytes(Color32 value)
    {
        yield return value.r;
        yield return value.g;
        yield return value.b;
        yield return value.a;
    }

    private static IEnumerable<byte> GetBytes(IEnumerable values, bool withTypeCode)
    {
        if (!withTypeCode)
        {
            var elementObjs = values.Cast<object>();

            foreach (var b in GetBytes((ushort)elementObjs.Count()))
                yield return b;

            foreach (var obj in elementObjs)
            {
                foreach (var b in InternalGetBytes(obj, false))
                    yield return b;
            }

            yield break;
        }

        var elements = new List<object>();
        Type commonType = null;
        var allSameType = true;
        var firstElement = true;

        foreach (var obj in values)
        {
            elements.Add(obj);

            if (!allSameType)
                continue;

            if (obj is null)
            {
                allSameType = false;
                continue;
            }

            var currentType = obj.GetType();

            if (firstElement)
            {
                commonType = currentType;
                firstElement = false;
            }
            else if (commonType != currentType)
                allSameType = false;
        }

        foreach (var b in GetBytes((ushort)elements.Count))
            yield return b;

        yield return (byte)(allSameType ? 1 : 0);

        if (allSameType && commonType is not null)
            yield return commonType.GetIdFromType();

        foreach (var b in elements.SelectMany(obj => GetBytes(obj, !allSameType)))
            yield return b;
    }
}