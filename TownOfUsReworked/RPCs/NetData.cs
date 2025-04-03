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
    private bool IsReceived { get; }

    /// <summary>
    /// Throws errors to ensure proper flow of reading/writing bytes.
    /// </summary>
    /// <param name="reading">Indicates whether or not it's being read.</param>
    /// <param name="readLength">The length of data being read.</param>
    /// <exception cref="InvalidOperationException">Thrown if the instance is in a writing state when reading, or if the instance is in a reading state when writing.</exception>
    /// <exception cref="ArgumentException">Thrown when either there was 0 data being attempted to be read, or there was an attempt at reading data from a writing <see cref="NetData"/> instance.</exception>
    /// <exception cref="EndOfStreamException">Thrown if there is insufficient data to read.</exception>
    private void ThrowIfIncorrectState(bool reading, int readLength)
    {
        if (reading != IsReceived)
            throw new InvalidOperationException("Tried to send or receive bytes from a data array with the incorrect state");

        if (!IsReceived)
        {
            if (readLength > 0)
                throw new InvalidOperationException("This instance of data is for writing, not reading");

            return;
        }

        if (readLength == 0)
            throw new ArgumentException($"Must gather some byte data. Pos {Position} out of {DataSize}");

        if ((Position + readLength) > ReadBuffer.Length)
            throw new EndOfStreamException($"Tried to read more data than what was available. Pos {Position} out of {DataSize} with read length {readLength}");

        if (Position >= ReadBuffer.Length)
            throw new EndOfStreamException($"No more data to read. Pos {Position} out of {DataSize}");
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

        if (TownOfUsReworked.MciActive || !CustomPlayer.Local)
            return;

        var writer = AmongUsClient.Instance.StartRpcImmediately(CustomPlayer.Local.NetId, CustomRPCCallID, SendOption.Reliable, targetClientId);
        writer.Serialize(this);
        writer.CloseRpc();
    }

    // This part of the code deals with data being sent for an rpc

    /// <summary>
    /// A list of bytes written to be sent for an RPC.
    /// </summary>
    private List<byte> WriteBuffer { get; }

    /// <summary>
    /// Initialises an instance of <see cref="NetData"/> for writing byte data, and optionally comes with allowing data to be written in during initialisation.
    /// </summary>
    /// <param name="rpc">The rpc header.</param>
    /// <param name="data">The optional data to serialize during initialisation.</param>
    public NetData(CustomRPC rpc, params object[] data)
    {
        WriteBuffer = [ (byte)rpc ];
        WriteBuffer.AddRange(data.Select(ToBytes).GetAll());
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
        if (DataSize > 1000)
            throw new InvalidOperationException("Writing too much data!");

        ThrowIfIncorrectState(false, 0);
        WriteBuffer.AddRange(ToBytes(value));
    }

    // This part of the code deals with data being received from an rpc

    /// <summary>
    /// The data received.
    /// </summary>
    private byte[] ReadBuffer { get; }

    /// <summary>
    /// Gets the number of bytes remaining to be read.
    /// </summary>
    private int BytesRemaining => IsReceived ? ReadBuffer.Length - Position : 0;

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
    private object Read(Type type) => type switch
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
        _ when type == typeof(Vector2) => ReadVector2(),
        _ when type.IsEnum => ReadEnum(type),
        _ when typeof(IPlayerLayer).IsAssignableFrom(type) => ReadLayer(),
        _ when typeof(IEnumerable).IsAssignableFrom(type) => type.IsArray
            ? ReadArray(type.GetElementType())
            : ReadValues(type.IsGenericType ? type.GetGenericArguments()[0] : typeof(object)), // Use of generics is enforced in the code base
        _ when typeof(INetSerializable).IsAssignableFrom(type) => // Allowing the classes to implement their own deserialization methods
            AccessTools.Method(type, "Read")?.Invoke(null, [ this ]) ??
            AccessTools.Method(typeof(RpcManager), $"Read{type.Name}")?.Invoke(null, [ this ]) ??
            throw new ArgumentOutOfRangeException($"Cannot read a {type.Name} from the data and the provided type"),
        _ => throw new NotSupportedException($"{type.Name} cannot be read")
    };

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
        var count = ReadUInt();

        while (count-- > 0)
            yield return Read(type);
    }

    /// <summary>
    /// Reads a casted collection of values from the data, prefixed by a count.
    /// </summary>
    /// <typeparam name="T">The type to cast the collection to.</typeparam>
    /// <returns>A collection of values, casted to <typeparamref name="T"/>.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public T[] ReadArray<T>() => (T[])ReadArray(typeof(T));

    /// <summary>
    /// Reads a collection of values from the data, prefixed by a count.
    /// </summary>
    /// <returns>A collection of values.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private Array ReadArray(Type type)
    {
        var count = ReadUInt();
        var array = Array.CreateInstance(type, count);

        for (var i = 0; i < count; i++)
            array.SetValue(Read(type), i);

        return array;
    }

    /// <summary>
    /// Reads a boolean value from the data.
    /// </summary>
    /// <returns>true if the read byte is non-zero; otherwise, false.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public bool ReadBool()
    {
        ThrowIfIncorrectState(true, 1);
        var result = ReadBuffer[Position] != 0;
        Position++;
        return result;
    }

    /// <summary>
    /// Reads a byte from the data.
    /// </summary>
    /// <returns>The deserialized byte value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    public byte ReadByte()
    {
        ThrowIfIncorrectState(true, 1);
        var result = ReadBuffer[Position];
        Position++;
        return result;
    }

    /// <summary>
    /// Reads a signed byte (sbyte) from the data.
    /// </summary>
    /// <returns>The deserialized signed byte value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private sbyte ReadSByte()
    {
        ThrowIfIncorrectState(true, 1);
        var result = (sbyte)(ReadBuffer[Position] - sbyte.MaxValue);
        Position++;
        return result;
    }

    /// <summary>
    /// Reads an unsigned 16-bit integer (ushort) from the data.
    /// </summary>
    /// <returns>The deserialized unsigned short value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private ushort ReadUShort()
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
    private short ReadShort()
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
    private ulong ReadULong()
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
    private long ReadLong()
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
        var length = (int)ReadUShort();
        ThrowIfIncorrectState(true, length);
        var result = Encoding.UTF8.GetString(ReadBuffer, Position, length);
        Position += length;
        return result;
    }

    // /// <summary>
    // /// Reads a sequence of bytes from the data.
    // /// </summary>
    // /// <param name="length">The number of bytes to read.</param>
    // /// <returns>The deserialized byte array.</returns>
    // /// <inheritdoc cref="ThrowIfIncorrectState"/>
    // public byte[] ReadBytes(int length)
    // {
    //     ThrowIfIncorrectState(true, length);
    //     var result = ReadBuffer[Position..(Position + length)];
    //     Position += length;
    //     return result;
    // }

    // No need for the ThrowIfIncorrectState for the following methods because it's nested

    /// <summary>
    /// Reads an enum value of the provided type from the data.
    /// </summary>
    /// <returns>The deserialized enum value.</returns>
    /// <exception cref="InvalidDataException">Thrown if <paramref name="type"/> is not an enum.</exception>
    private object ReadEnum(Type type) => type.IsEnum ? Enum.ToObject(type, Read(Enum.GetUnderlyingType(type))) : throw new InvalidDataException($"{type.Name} is not an enum");

    /// <summary>
    /// Reads a player layer from the data by its player ID and layer type.
    /// </summary>
    /// <returns>The matching <see cref="IPlayerLayer"/> instance, or null if not found.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private IPlayerLayer ReadLayer()
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
    private Vent ReadVent() => VentById(ReadInt());

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
        var count = ReadByte();
        var result = new T[count];

        for (var i = 0; i < count; i++)
            result[i] = Read<T>();

        return result;
    }

    /// <summary>
    /// Reads a <see cref="CustomButton"/> by its ID from the data.
    /// </summary>
    /// <returns>The button with the deserialized ID.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private CustomButton ReadButton()
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
        return new(Mathf.Lerp(MinPos, MaxPos, x), Mathf.Lerp(MinPos, MaxPos, y));
    }

    // Some stuff to serialize Vector2 using the game's bounds
    private const float MinPos = -50f;
    private const float MaxPos = +50f;

    private static float ReverseLerp(float t) => Mathf.Clamp((t - MinPos) / (MaxPos - MinPos), 0f, 1f);

    // Interface stuff

    /// <inheritdoc/>
    public void Dispose()
    {
        if (BytesRemaining > 0 && IsReceived)
            Warning($"There were {BytesRemaining} bytes of unread data in the rpc {(CustomRPC)ReadBuffer[0]} {ReadBuffer[1]}");

        Array.Clear(ReadBuffer); // Explicitly clearing a byte[] just for the ease of my mind
        WriteBuffer.Clear();
    }

    /// <inheritdoc/>
    public byte[] ToBytes() => IsReceived ? ReadBuffer : [ .. WriteBuffer ];

    // Static method helpers

    /// <inheritdoc cref="ToBytes(string)"/>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> was null.</exception>
    /// <exception cref="NotSupportedException">Thrown of the value can't be serialized.</exception>
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
        float i => BitConverter.GetBytes(i),
        string i => ToBytes(i), // String needs a custom method because BitConverter apparently can't support it
        Enum i => ToBytes(Convert.ChangeType(i, Enum.GetUnderlyingType(i.GetType()))),

        // Types from the base game
        PlayerControl i => [ i.PlayerId ],
        DeadBody i => [ i.ParentId ],
        PlayerVoteArea i => [ i.TargetPlayerId ],
        Vent i => BitConverter.GetBytes(i.Id),
        Vector2 i => ToBytes(i),

        // Custom types using the interface
        INetSerializable i => i.ToBytes(),

        // Special cases
        Array i => ToBytes(i),
        // IDictionary i => ToBytes(i), WIP
        IEnumerable i => ToBytes(i),

        // Edge cases
        null => throw new ArgumentNullException(nameof(value)),
        _ => throw new NotSupportedException($"Either {value.GetType().Name} does not extend INetSerializable, or cannot be serialized to bytes")
    };

    /// <summary>
    /// Serializes the value to an array of bytes.
    /// </summary>
    /// <param name="value">The value to serialize.</param>
    /// <returns>An array of bytes representing the value.</returns>
    public static byte[] ToBytes(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        return [ .. ToBytes((ushort)bytes.Length), .. bytes ];
    }

    /// <summary>
    /// Serializes a collection of values to an array of bytes, prefixed by the collection's length.
    /// </summary>
    /// <param name="values">The values to serialize.</param>
    /// <returns>An array of bytes representing the values.</returns>
    private static byte[] ToBytes(IEnumerable values)
    {
        var result = new List<byte>();
        var i = 0u;

        foreach (var obj in values)
        {
            result.AddRange(ToBytes(obj));
            i++;
        }

        result.InsertRange(0, BitConverter.GetBytes(i));
        return [ .. result ];
    }

    /// <summary>
    /// Serializes an array of values to an array of bytes, prefixed by the collection's length.
    /// </summary>
    /// <param name="values">The values to serialize.</param>
    /// <returns>An array of bytes representing the values.</returns>
    private static byte[] ToBytes(Array values)
    {
        var result = new List<byte>();

        foreach (var obj in values)
            result.AddRange(ToBytes(obj));

        result.InsertRange(0, BitConverter.GetBytes((ushort)values.Length));
        return [ .. result ];
    }

    /// <summary>
    /// Serializes a 2d vector to an array of bytes, prefixed by the collection's length.
    /// </summary>
    /// <param name="value">The value to serialize.</param>
    /// <returns>An array of bytes representing the value.</returns>
    private static byte[] ToBytes(Vector2 value)
    {
        var x = (ushort)(ReverseLerp(value.x) * ushort.MaxValue);
        var y = (ushort)(ReverseLerp(value.y) * ushort.MaxValue);
        return [ .. BitConverter.GetBytes(x), .. BitConverter.GetBytes(y), ];
    }
}