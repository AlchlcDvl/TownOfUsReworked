using MonoMod.Utils;

namespace TownOfUsReworked.RPCs;

/// <summary>
/// Handler that contains byte data that's either being sent to or being received from an RPC.
/// </summary>
public sealed class NetData : IDisposable, INetSerializable
{
    /// <summary>
    /// Gets the size of the data contained within this instance.
    /// </summary>
    public int DataSize => IsReceived ? ReadBuffer.Length : WriteBuffer.Count;

    /// <summary>
    /// Gets a value indicating whether or not the byte array was received via an RPC, or an array is being constructed for an RPC.
    /// </summary>
    public bool IsReceived { get; }

    /// <summary>
    /// Throws errors to ensure proper flow of reading/writing bytes.
    /// </summary>
    /// <param name="reading">Indicates whether or not it's being read.</param>
    /// <exception cref="InvalidOperationException">Thrown if the instance is in a writing state when reading, or if the instance is in a reading state when writing.</exception>
    /// <inheritdoc cref="EnsureDataReadable"/>
    private void ThrowIfIncorrectState(bool reading, int offset)
    {
        if (reading != IsReceived)
            throw new InvalidOperationException("Tried to send or receive bytes from a data array with the incorrect state");

        EnsureDataReadable(offset);
    }

    /// <summary>
    /// Ensures that data can be read.
    /// </summary>
    /// <param name="offset">The offset of the value being read.</param>
    /// <exception cref="ArgumentException">Thrown when either there was 0 data being attempted to be read, or there was an attempt at reading data from a writing <see cref="NetData"/> instance.</exception>
    /// <exception cref="EndOfStreamException">Thrown if there is insufficient data to read.</exception>
    private void EnsureDataReadable(int offset)
    {
        if (!IsReceived)
        {
            if (offset > 0)
                throw new ArgumentException("This instance of data is for writing, not reading");
            else
                return;
        }

        if (offset == 0)
            throw new ArgumentException("Must gather some byte data");

        if ((Position + offset) >= ReadBuffer.Length)
            throw new EndOfStreamException("Tried to read more data than what was available");

        if (Position >= ReadBuffer.Length)
            throw new EndOfStreamException("No more data to read");
    }

    /// <summary>
    /// Sends the written data to everyone by default, or optionally to only one player.
    /// </summary>
    /// <param name="targetClientId">The player to be sent the RPC to, or <c>-1</c> for everyone.</param>
    /// <exception cref="InvalidOperationException">Thrown if a reader instance is being sent.</exception>
    public void Send(int targetClientId = -1)
    {
        if (IsReceived)
            throw new InvalidOperationException("Cannot send data that was received");

        var writer = AmongUsClient.Instance.StartRpcImmediately(CustomPlayer.Local.NetId, CustomRPCCallID, SendOption.Reliable, targetClientId);
        writer.Serialize(this);
        writer.CloseRpc();
    }

    // This part of the code deals with data being sent for an rpc

    /// <summary>
    /// A list of bytes written to be sent for an RPC.
    /// </summary>
    public List<byte> WriteBuffer { get; }

    /// <summary>
    /// Initialises an instance of <see cref="NetData"/> for writing byte data, and optionally comes with allowing data to be written in during initialisation.
    /// </summary>
    /// <param name="data">The optional data to serialize during initialisation.</param>
    public NetData(params object[] data)
    {
        WriteBuffer = [ .. data.Select(ToBytes).GetAll() ];
        ReadBuffer = [];
        IsReceived = false;
    }

    /// <summary>
    /// Writes the bytes of the value to the data being sent.
    /// </summary>
    /// <param name="value">The value to serialize.</param>
    /// <exception cref="InvalidOperationException">Thrown if the instance is not in a writing state.</exception>
    public void Write(object value)
    {
        ThrowIfIncorrectState(false, 0);
        WriteBuffer.AddRange(ToBytes(value));
    }

    // This part of the code deals with data being received from an rpc

    /// <summary>
    /// The data received.
    /// </summary>
    public byte[] ReadBuffer { get; }

    /// <summary>
    /// Gets the number of bytes remaining to be read.
    /// </summary>
    public int BytesRemaining => IsReceived ? ReadBuffer.Length - Position : 0;

    /// <summary>
    /// The current position of the pointer in the data.
    /// </summary>
    private int Position;

    /// <summary>
    /// Initialises an instance of <see cref="NetData"/> for reading byte data.
    /// </summary>
    /// <param name="data">The data to deserialize from during initialisation.</param>
    public NetData(params byte[] data)
    {
        ReadBuffer = data;
        WriteBuffer = [];
        IsReceived = true;
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
    /// <returns>The deserialized value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public object Read(Type type) => type switch
    {
        _ when type == typeof(byte) => ReadByte(),
        _ when type == typeof(bool) => ReadBool(),
        _ when type == typeof(sbyte) => ReadSByte(),
        _ when type == typeof(ushort) => ReadUShort(),
        _ when type == typeof(short) => ReadShort(),
        _ when type == typeof(uint) => ReadUInt(),
        _ when type == typeof(int) => ReadInt(),
        _ when type == typeof(ulong) => ReadULong(),
        _ when type == typeof(long) => ReadLong(),
        _ when type == typeof(float) => ReadFloat(),
        _ when type == typeof(string) => ReadString(),
        _ when type == typeof(PlayerControl) => ReadPlayer(),
        _ when type == typeof(PlayerVoteArea) => ReadVoteArea(),
        _ when type == typeof(DeadBody) => ReadBody(),
        _ when type == typeof(Vent) => ReadVent(),
        _ when type == typeof(RoleOptionData) => ReadRoleOptionData(),
        _ when type == typeof(CustomButton) => ReadButton(),
        _ when typeof(IPlayerLayer).IsAssignableFrom(type) => ReadLayer(),
        _ when typeof(IEnumerable<IPlayerLayer>).IsAssignableFrom(type) => ReadLayers(),
        _ when typeof(IEnumerable<byte>).IsAssignableFrom(type) => ReadBytes(),
        _ when type.IsEnum => ReadEnum(type),
        _ => throw new ArgumentOutOfRangeException($"Cannot read a {type.Name} from the data and the provided type")
    };

    /// <summary>
    /// Reads a boolean value from the data.
    /// </summary>
    /// <returns>true if the read byte is non-zero; otherwise, false.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public bool ReadBool()
    {
        ThrowIfIncorrectState(true, 1);
        return ReadBuffer[Position++] != 0;
    }

    /// <summary>
    /// Reads a byte from the data.
    /// </summary>
    /// <returns>The deserialized byte value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public byte ReadByte()
    {
        ThrowIfIncorrectState(true, 1);
        return ReadBuffer[Position++];
    }

    /// <summary>
    /// Reads a signed byte (sbyte) from the data.
    /// </summary>
    /// <returns>The deserialized signed byte value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public sbyte ReadSByte()
    {
        ThrowIfIncorrectState(true, 1);
        return (sbyte)(ReadBuffer[Position++] - sbyte.MaxValue);
    }

    /// <summary>
    /// Reads an unsigned 16-bit integer (ushort) from the data.
    /// </summary>
    /// <returns>The deserialized unsigned short value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public ushort ReadUShort()
    {
        ThrowIfIncorrectState(true, 2);
        var result = BitConverter.ToUInt16(ReadBuffer, Position);
        Position += 2;
        return result;
    }

    /// <summary>
    /// Reads a signed 16-bit integer (short) from the data.
    /// </summary>
    /// <returns>The deserialized signed short value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public short ReadShort()
    {
        ThrowIfIncorrectState(true, 2);
        var result = BitConverter.ToInt16(ReadBuffer, Position);
        Position += 2;
        return result;
    }

    /// <summary>
    /// Reads an unsigned 32-bit integer (uint) from the data.
    /// </summary>
    /// <returns>The deserialized unsigned int value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public uint ReadUInt()
    {
        ThrowIfIncorrectState(true, 4);
        var result = BitConverter.ToUInt32(ReadBuffer, Position);
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
        ThrowIfIncorrectState(true, 4);
        var result = BitConverter.ToInt32(ReadBuffer, Position);
        Position += 4;
        return result;
    }

    /// <summary>
    /// Reads an unsigned 64-bit integer (ulong) from the data.
    /// </summary>
    /// <returns>The deserialized unsigned long value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public ulong ReadULong()
    {
        ThrowIfIncorrectState(true, 8);
        var result = BitConverter.ToUInt64(ReadBuffer, Position);
        Position += 8;
        return result;
    }

    /// <summary>
    /// Reads a signed 64-bit integer (long) from the data.
    /// </summary>
    /// <returns>The deserialized signed long value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public long ReadLong()
    {
        ThrowIfIncorrectState(true, 8);
        var result = BitConverter.ToInt64(ReadBuffer, Position);
        Position += 8;
        return result;
    }

    /// <summary>
    /// Reads a single floating point precision value (float) from the data.
    /// </summary>
    /// <returns>The deserialized float value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public float ReadFloat()
    {
        ThrowIfIncorrectState(true, 4);
        var result = BitConverter.ToSingle(ReadBuffer, Position);
        Position += 4;
        return result;
    }

    /// <summary>
    /// Reads a UTF-8 encoded string from the data.
    /// </summary>
    /// <returns>The deserialized string.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public string ReadString()
    {
        ThrowIfIncorrectState(true, 4);
        var length = (int)ReadUInt();
        EnsureDataReadable(length);
        var result = Encoding.UTF8.GetString(ReadBuffer, Position, length);
        Position += length;
        return result;
    }

    /// <summary>
    /// Reads a sequence of bytes from the data.
    /// </summary>
    /// <param name="length">The number of bytes to read.</param>
    /// <returns>The deserialized byte array.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public byte[] ReadBytes(int length)
    {
        ThrowIfIncorrectState(true, length);
        var result = ReadBuffer[Position..(Position + length)];
        Position += length;
        return result;
    }

    // No need for the ThrowIfIncorrectState for the following methods because it's nested

    /// <summary>
    /// Reads a collection of bytes from the data, prefixed by a count.
    /// </summary>
    /// <returns>An enumerable of deserialized bytes.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public IEnumerable<byte> ReadBytes()
    {
        var count = ReadUInt();

        while (count-- > 0)
            yield return ReadByte();
    }

    /// <summary>
    /// Reads an enum value of the provided type from the data.
    /// </summary>
    /// <returns>The deserialized enum value.</returns>
    /// <exception cref="InvalidDataException">Thrown if <paramref name="type"/> is not an enum.</exception>
    public object ReadEnum(Type type)
    {
        if (!type.IsEnum)
            throw new InvalidDataException($"{type.Name} is not an enum");

        var underlyingType = Enum.GetUnderlyingType(type);
        ThrowIfIncorrectState(true, underlyingType.GetManagedSize());
        return Enum.ToObject(type, Read(underlyingType));
    }

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
    /// Reads multiple player layers from the data, prefixed by a count and casts them to <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to cast to.</typeparam>
    /// <inheritdoc cref="ReadLayers"/>
    public IEnumerable<T> ReadLayers<T>() where T : IPlayerLayer => ReadLayers().Cast<T>();

    /// <summary>
    /// Reads multiple player layers from the data, prefixed by a count.
    /// </summary>
    /// <returns>An enumerable of deserialized player layers.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public IEnumerable<IPlayerLayer> ReadLayers()
    {
        var count = ReadUInt();

        while (count-- > 0)
            yield return ReadLayer();
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
    /// Reads a <see cref="RoleOptionData"/> from the data.
    /// </summary>
    /// <returns>The option data with the deserialized properties.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public RoleOptionData ReadRoleOptionData() => new(ReadByte(), ReadByte(), ReadBool(), ReadBool(), Read<LayerEnum>());

    /// <summary>
    /// Reads a <see cref="MultiSelectValue{T}"/> containing enum values from the data.
    /// </summary>
    /// <typeparam name="T">The enum type contained in the collection.</typeparam>
    /// <returns>A collection of deserialized enum values.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public MultiSelectValue<T> ReadMultiSelectValue<T>() where T : struct, Enum
    {
        var count = ReadUInt();
        var result = new MultiSelectValue<T>();

        while (count-- > 0)
            result.Add(Read<T>());

        return result;
    }

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
    public Vector2 ReadVector2() => new(ReadFloat(), ReadFloat());

    // Interface stuff

    /// <inheritdoc/>
    public void Dispose()
    {
        if (BytesRemaining > 0)
            Warning("There was unread data");

        Array.Clear(ReadBuffer); // Explicitly clearing a byte[] just for the ease of my mind
        WriteBuffer.Clear();
    }

    /// <inheritdoc/>
    public byte[] ToBytes() => IsReceived ? ReadBuffer : WriteBuffer.ToArray();

    // Static method helpers

    /// <inheritdoc cref="ToBytes(string)"/>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> was null.</exception>
    /// <exception cref="NotImplementedException">Thrown of the value couldn't be serialized.</exception>
    public static byte[] ToBytes(object value) => value switch
    {
        // Base C# types
        bool i => [ (byte)(i ? 1 : 0) ],
        byte i => [ i ],
        sbyte i => [ (byte)(i + sbyte.MaxValue) ],
        ushort i => BitConverter.GetBytes(i),
        short i => BitConverter.GetBytes(i),
        int i => BitConverter.GetBytes(i),
        uint i => BitConverter.GetBytes(i),
        ulong i => BitConverter.GetBytes(i),
        long i => BitConverter.GetBytes(i),
        string i => ToBytes(i), // String needs a custom method because BitConverter apparently can't support it
        Enum i => ToBytes(Convert.ChangeType(i, Enum.GetUnderlyingType(i.GetType()))),

        // Types from the base game
        PlayerControl i => [ i.PlayerId ],
        DeadBody i => [ i.ParentId ],
        PlayerVoteArea i => [ i.TargetPlayerId ],
        Vent i => ToBytes(i.Id),
        Vector2 i => [ .. ToBytes(i.x), .. ToBytes(i.y) ],

        // Custom types using the interface
        INetSerializable i => i.ToBytes(),

        // Special cases
        IEnumerable<byte> i => [ .. ToBytes((uint)i.Count()), .. i ],
        IEnumerable<IPlayerLayer> i => [ .. ToBytes((uint)i.Count()), .. i.Select(ToBytes).GetAll() ],
        object[] i => [ .. ToBytes((uint)i.Length), .. i.Select(ToBytes).GetAll() ],

        // Edge cases
        _ when value == null => throw new ArgumentNullException(nameof(value)),
        _ => throw new NotImplementedException($"Either {value.GetType().Name} does not extend INetSerializable, or cannot be serialized to bytes")
    };

    /// <summary>
    /// Serializes the value to an array of bytes.
    /// </summary>
    /// <param name="value">The value to serialize.</param>
    /// <returns>An array of bytes representing the value.</returns>
    public static byte[] ToBytes(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        return [ .. ToBytes((uint)bytes.Length), .. bytes ];
    }
}