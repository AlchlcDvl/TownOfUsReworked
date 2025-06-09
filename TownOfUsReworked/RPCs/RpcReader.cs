using System.Buffers;
using AmongUs.InnerNet.GameDataMessages;

namespace TownOfUsReworked.RPCs;

/// <summary>
/// A network reader for reading from RPC messages.
/// </summary>
public sealed class RpcReader : IDisposable
{
    /// <summary>
    /// A list of bytes written to be sent for an RPC. This is the internal buffer.
    /// </summary>
    private byte[] Payload { get; set; }

    /// <summary>
    /// Gets or sets a flag value that indicates whether or not the writer has been disposed of.
    /// </summary>
    private bool Disposed { get; set; }

    /// <summary>
    /// Gets or sets the current pointer position in the data stream.
    /// </summary>
    private int Position { get; set; }

    /// <summary>
    /// Gets the size of the networked payload.
    /// </summary>
    public int DataSize { get; }

    /// <summary>
    /// Gets the remaining unread bytes from the payload.
    /// </summary>
    public int BytesRemaining => DataSize - Position;

    /// <summary>
    /// Initializes a new instance of the <see cref="RpcReader"/> class for reading byte data from a network message.
    /// </summary>
    /// <param name="data">The networked byte stream containing rpc data.</param>
    public RpcReader(byte[] data)
    {
        DataSize = data.Length;
        Payload = ArrayPool<byte>.Shared.Rent(DataSize);
        Buffer.BlockCopy(data, 0, Payload, 0, DataSize);
        Position = 0;
    }

    /// <summary>
    /// RpcReader destructor.
    /// </summary>
    ~RpcReader() => InternalDispose();

    /// <summary>
    /// Throws errors to ensure proper flow of reading bytes.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the writer has been disposed.</exception>
    /// <exception cref="ArgumentException">Thrown when either there was 0 data being attempted to be read.</exception>
    /// <exception cref="EndOfStreamException">Thrown if there is insufficient data to read.</exception>
    private void ThrowIfIncorrectState(int readLength)
    {
        if (Disposed)
            throw new ObjectDisposedException(nameof(RpcReader));

        if (readLength == 0)
            throw new ArgumentException($"Must gather some byte data. Pos {Position} out of {DataSize}");

        if ((Position + readLength) > DataSize)
            throw new EndOfStreamException($"Tried to read more data than what was available. Pos {Position} out of {DataSize} with read length {readLength}");

        if (Position >= DataSize)
            throw new EndOfStreamException($"No more data to read. Pos {Position} out of {DataSize}");
    }

    /// <summary>
    /// Internal shared disposal between the destructor and the dispose method.
    /// </summary>
    private void InternalDispose()
    {
        if (Disposed)
            return;

        if (BytesRemaining > 0)
            Warning($"There were {BytesRemaining} bytes of unread data in the rpc {(CustomRPC)Payload[0]} {Payload[1]}");

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
    /// Reads a value of the specified type from the data.
    /// </summary>
    /// <typeparam name="T">The type of value to read.</typeparam>
    /// <returns>The deserialized value of type <typeparamref name="T"/>.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public T Read<T>() => (T)Read(typeof(T));

    /// <summary>
    /// Reads a value of the specified type from the data.
    /// </summary>
    /// <typeparam name="T">The type of value to read.</typeparam>
    /// <returns>The deserialized value of type <typeparamref name="T"/>.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public T ReadFromTypeCode<T>() => (T)Read();

    /// <summary>
    /// Reads a value of the specified type from the data.
    /// </summary>
    /// <returns>The deserialized value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private object Read(Type type) => type switch
    {
        null => throw new NullReferenceException(nameof(type)),
        _ when type == typeof(byte) => ReadByte(),
        _ when type == typeof(char) => ReadChar(),
        _ when type == typeof(bool) => ReadBool(),
        _ when type == typeof(sbyte) => ReadSByte(),
        _ when type == typeof(ushort) => ReadUShort(),
        _ when type == typeof(short) => ReadShort(),
        _ when type == typeof(uint) => ReadUInt(),
        _ when type == typeof(int) => ReadInt(),
        _ when type == typeof(ulong) => ReadULong(),
        _ when type == typeof(long) => ReadLong(),
        _ when type == typeof(float) => ReadFloat(),
        _ when type == typeof(double) => ReadDouble(),
        _ when type == typeof(decimal) => ReadDecimal(),
        _ when type == typeof(string) => ReadString(),
        _ when type == typeof(Half) => ReadHalf(),
        _ when type == typeof(PlayerControl) => ReadPlayer(),
        _ when type == typeof(PlayerVoteArea) => ReadVoteArea(),
        _ when type == typeof(DeadBody) => ReadBody(),
        _ when type == typeof(Vent) => ReadVent(),
        _ when type == typeof(CustomButton) => ReadButton(),
        _ when type == typeof(Vector2) => ReadVector2(),
        _ when type == typeof(Type) => ReadType(),
        _ when type == typeof(Enum) => ReadEnum(ReadType()),
        _ when type == typeof(IEnumerable) => ReadValues(),
        _ when type.IsEnum => ReadEnum(type),
        _ when typeof(IPlayerLayer).IsAssignableFrom(type) => ReadLayer(),
        _ when typeof(INetDeserializable).IsAssignableFrom(type) => ReadDeserializable(type),
        _ when typeof(IEnumerable).IsAssignableFrom(type) => ReadValues(type.GetGenericArguments()[0]),
        _ => throw new NotSupportedException($"{type.Name} cannot be read")
    };

    /// <summary>
    /// Reads a value based on its custom type code from the data.
    /// </summary>
    /// <returns>The deserialized value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private object Read(byte? typeId = null)
    {
        typeId ??= ReadByte();
        return Read(typeId.Value.GetTypeFromId());
    }

    /// <summary>
    /// Reads a casted collection of values from the data, prefixed by a count.
    /// </summary>
    /// <typeparam name="T">The type to cast the collection to.</typeparam>
    /// <returns>A collection of values, casted to <typeparamref name="T"/>.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public IEnumerable<T> ReadValues<T>() => ReadValues(typeof(T)).Cast<T>();

    /// <summary>
    /// Reads a collection of values from the data, prefixed by a count.
    /// </summary>
    /// <returns>A collection of values.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private IEnumerable ReadValues(Type type)
    {
        var count = ReadUShort();

        while (count-- > 0)
            yield return Read(type);
    }

    /// <inheritdoc cref="ReadValues(Type)"/>
    private IEnumerable ReadValues()
    {
        var count = ReadUShort();

        if (ReadBool())
        {
            var code = ReadByte();

            while (count-- > 0)
                yield return Read(code);
        }
        else while (count-- > 0)
            yield return Read();
    }

    /// <summary>
    /// Reads a boolean value from the data.
    /// </summary>
    /// <returns>true if the read byte is non-zero; otherwise, false.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public bool ReadBool()
    {
        ThrowIfIncorrectState(1);
        return Payload[Position++] != 0;
    }

    /// <summary>
    /// Reads a boolean value from the data.
    /// </summary>
    /// <returns>true if the read byte is non-zero; otherwise, false.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private char ReadChar()
    {
        ThrowIfIncorrectState(2);
        var result = BitConverter.ToChar(Payload, Position);
        Position += 2;
        return result;
    }

    /// <summary>
    /// Reads a byte from the data.
    /// </summary>
    /// <returns>The deserialized byte value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public byte ReadByte()
    {
        ThrowIfIncorrectState(1);
        return Payload[Position++];
    }

    /// <summary>
    /// Reads a signed byte (sbyte) from the data.
    /// </summary>
    /// <returns>The deserialized signed byte value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private sbyte ReadSByte()
    {
        ThrowIfIncorrectState(1);
        return (sbyte)(Payload[Position++] - 128);
    }

    /// <summary>
    /// Reads an unsigned 16-bit integer (ushort) from the data.
    /// </summary>
    /// <returns>The deserialized unsigned short value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private ushort ReadUShort()
    {
        ThrowIfIncorrectState(2);
        var result = BitConverter.ToUInt16(Payload, Position);
        Position += 2;
        return result;
    }

    /// <summary>
    /// Reads a signed 16-bit integer (short) from the data.
    /// </summary>
    /// <returns>The deserialized signed short value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private short ReadShort()
    {
        ThrowIfIncorrectState(2);
        var result = BitConverter.ToInt16(Payload, Position);
        Position += 2;
        return result;
    }

    /// <summary>
    /// Reads an unsigned 32-bit integer (uint) from the data.
    /// </summary>
    /// <returns>The deserialized unsigned int value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private uint ReadUInt()
    {
        ThrowIfIncorrectState(4);
        var result = BitConverter.ToUInt32(Payload, Position);
        Position += 4;
        return result;
    }

    /// <summary>
    /// Reads a signed 32-bit integer (int) from the data.
    /// </summary>
    /// <returns>The deserialized signed int value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public int ReadInt()
    {
        ThrowIfIncorrectState(4);
        var result = BitConverter.ToInt32(Payload, Position);
        Position += 4;
        return result;
    }

    /// <summary>
    /// Reads an unsigned 64-bit integer (ulong) from the data.
    /// </summary>
    /// <returns>The deserialized unsigned long value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private ulong ReadULong()
    {
        ThrowIfIncorrectState(8);
        var result = BitConverter.ToUInt64(Payload, Position);
        Position += 8;
        return result;
    }

    /// <summary>
    /// Reads a signed 64-bit integer (long) from the data.
    /// </summary>
    /// <returns>The deserialized signed long value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private long ReadLong()
    {
        ThrowIfIncorrectState(8);
        var result = BitConverter.ToInt64(Payload, Position);
        Position += 8;
        return result;
    }

    /// <summary>
    /// Reads a half floating point precision value (float) from the data.
    /// </summary>
    /// <returns>The deserialized float value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private Half ReadHalf()
    {
        ThrowIfIncorrectState(2);
        var result = BitConverter.ToHalf(Payload, Position);
        Position += 2;
        return result;
    }

    /// <summary>
    /// Reads a single floating point precision value (float) from the data.
    /// </summary>
    /// <returns>The deserialized float value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public float ReadFloat()
    {
        ThrowIfIncorrectState(4);
        var result = BitConverter.ToSingle(Payload, Position);
        Position += 4;
        return result;
    }

    /// <summary>
    /// Reads a double floating point precision value from the data.
    /// </summary>
    /// <returns>The deserialized double value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private double ReadDouble()
    {
        ThrowIfIncorrectState(8);
        var result = BitConverter.ToDouble(Payload, Position);
        Position += 8;
        return result;
    }

    /// <summary>
    /// Reads a decimal value from the data.
    /// </summary>
    /// <returns>The deserialized double value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private decimal ReadDecimal() => new([ReadInt(), ReadInt(), ReadInt(), ReadInt()]);

    /// <summary>
    /// Reads a UTF-8 encoded string from the data.
    /// </summary>
    /// <returns>The deserialized string.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public string ReadString()
    {
        var length = (int)ReadUShort();
        ThrowIfIncorrectState(length);
        var result = Encoding.UTF8.GetString(Payload, Position, length);
        Position += length;
        return result;
    }

    // No need for the ThrowIfIncorrectState for the following methods because it's nested

    /// <summary>
    /// Reads an enum value of the provided type from the data.
    /// </summary>
    /// <returns>The deserialized enum value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private object ReadEnum(Type type) => Enum.ToObject(type, Read(Enum.GetUnderlyingType(type)));

    /// <summary>
    /// Reads a player layer from the data by its player ID and layer type.
    /// </summary>
    /// <returns>The matching <see cref="IPlayerLayer"/> instance, or null if not found.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public IPlayerLayer ReadLayer()
    {
        var player = ReadByte();
        var type = Read<LayerEnum>();
        return PlayerLayer.AllLayers.Find(x => x.PlayerId == player && x.Type == type);
    }

    /// <summary>
    /// Reads a <see cref="PlayerControl"/> by its player ID from the data.
    /// </summary>
    /// <returns>The player with the deserialized ID.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public PlayerControl ReadPlayer() => PlayerById(ReadByte());

    /// <summary>
    /// Reads a <see cref="PlayerVoteArea"/> by its player ID from the data.
    /// </summary>
    /// <returns>The vote area with the deserialized ID.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public PlayerVoteArea ReadVoteArea() => VoteAreaById(ReadByte());

    /// <summary>
    /// Reads a <see cref="DeadBody"/> by its player ID from the data.
    /// </summary>
    /// <returns>The body with the deserialized ID.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public DeadBody ReadBody() => BodyById(ReadByte());

    /// <summary>
    /// Reads a <see cref="Vent"/> by its ID from the data.
    /// </summary>
    /// <returns>The vent with the deserialized ID.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public Vent ReadVent() => VentById(ReadInt());

    /// <summary>
    /// Reads a <see cref="CustomButton"/> by its ID from the data.
    /// </summary>
    /// <returns>The button with the deserialized ID.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public CustomButton ReadButton()
    {
        var id = ReadString();
        return CustomButton.AllButtons.Find(x => x.ID == id);
    }

    /// <summary>
    /// Reads a <see cref="Vector2"/> from the data.
    /// </summary>
    /// <returns>The deserialized vector 2.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public Vector2 ReadVector2()
    {
        var x = ReadUShort() / (float)ushort.MaxValue;
        var y = ReadUShort() / (float)ushort.MaxValue;
        return new(Generate.Lerp(x), Generate.Lerp(y));
    }

    /// <summary>
    /// Reads a type from the data.
    /// </summary>
    /// <returns>The deserialized type value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private Type ReadType() => Type.GetType(ReadString());

    /// <summary>
    /// Creates and initialises an new deserialized object from the data.
    /// </summary>
    /// <returns>The deserialized summary info.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private INetDeserializable ReadDeserializable(Type type)
    {
        var obj = Activator.CreateInstance(type) as INetDeserializable;
        obj!.FromBytes(this);
        return obj;
    }
}