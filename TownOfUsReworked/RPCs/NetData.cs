namespace TownOfUsReworked.RPCs;

// Recreated the MessageReader/Writer class so that I have more control over this and because making extensions is boring

/// <summary>
/// Handler that contains byte data that's either being sent to or being received from an RPC.
/// </summary>
public sealed class NetData : IDisposable, INetSerializable
{
    /// <summary>
    /// Gets the size of the data contained within this instance.
    /// </summary>
    public int DataSize => IsReceived ? ReadBuffer.Length : WriteBuffer.Count;

    /// <inheritdoc/>
    public CustomTypeCode TypeCode => CustomTypeCode.NetData;

    /// <summary>
    /// Gets a value indicating whether or not the byte array was received via an RPC, or an array is being constructed for an RPC.
    /// </summary>
    private bool IsReceived { get; }

    private bool Disposed { get; set; }
    private bool Exceeded { get; set; }

    /// <summary>
    /// NetData destructor.
    /// </summary>
    ~NetData() => InternalDispose();

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
        if (Disposed)
            throw new ObjectDisposedException(nameof(NetData));

        if (!reading && DataSize > 1000 && Exceeded)
            throw new OverflowException("Writing too much data!");

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
    /// Sends the written data to everyone, or optionally to only one player.
    /// </summary>
    /// <param name="targetClientId">The player to be sent the RPC to, or <c>-1</c> for everyone.</param>
    /// <exception cref="InvalidOperationException">Thrown if a reader instance is being sent.</exception>
    public void Send(int targetClientId = -1)
    {
        if (IsReceived)
            throw new InvalidOperationException("Cannot send data that was received");

        if (TownOfUsReworked.MciActive || !LocalPlayer)
            return;

        var writer = AmongUsClient.Instance.StartRpcImmediately(LocalPlayer.NetId, CustomRPCCallID, SendOption.Reliable, targetClientId);
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
    /// <param name="withTypeCode">The rpc header.</param>
    /// <param name="data">The optional data to serialize during initialisation.</param>
    public NetData(CustomRPC rpc, bool withTypeCode, params object[] data)
    {
        WriteBuffer = [(byte)rpc, .. data.Select(x => ToBytes(x, withTypeCode)).GetAll()];
        ReadBuffer = [];
        IsReceived = false;
    }

    // /// <summary>
    // /// Converts the current writer instance to a reader instance.
    // /// </summary>
    // /// <returns>A reader instance of the current instance with the same byte data.</returns>
    // public NetData ToReadState()
    // {
    //     if (Disposed)
    //         throw new ObjectDisposedException(nameof(NetData));
    //
    //     return IsReceived ? throw new InvalidOperationException("The current instance is already in read state!") : new(ToBytes());
    // }

    /// <summary>
    /// Writes the bytes of the value to the data being sent.
    /// </summary>
    /// <param name="value">The value to serialize.</param>
    /// <param name="withTypeCode">Indicates whether or not the value's type code should also be serialised for blind serialisation.</param>
    /// <exception cref="InvalidOperationException">Thrown if the instance is not in a writing state or too much data is being packed.</exception>
    public void Write(object value, bool withTypeCode = false)
    {
        ThrowIfIncorrectState(false, 0);
        WriteBuffer.AddRange(ToBytes(value, withTypeCode));
        Exceeded = DataSize > 1000;
    }

    // /// <summary>
    // /// Writes the bytes of a collection of values to the data being sent.
    // /// </summary>
    // /// <param name="values">The values to serialize.</param>
    // /// <exception cref="InvalidOperationException">Thrown if the instance is not in a writing state or too much data is being packed.</exception>
    // public void Write(params object[] values) => values.Do(x => Write(x));

    // /// <summary>
    // /// Writes the bytes of a collection of values along with their relevant type codes to the data being sent.
    // /// </summary>
    // /// <param name="values">The values to serialize.</param>
    // /// <exception cref="InvalidOperationException">Thrown if the instance is not in a writing state or too much data is being packed.</exception>
    // public void WriteWithTypeCode(params object[] values) => values.Do(x => Write(x, true));

    // This part of the code deals with data being received from an rpc

    /// <summary>
    /// The data received.
    /// </summary>
    private byte[] ReadBuffer { get; }

    /// <summary>
    /// Gets the number of bytes remaining to be read.
    /// </summary>
    public int BytesRemaining => IsReceived ? (ReadBuffer.Length - Position) : (1000 - WriteBuffer.Count);

    /// <summary>
    /// The current index pointer in the data.
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

    // /// <summary>
    // /// Converts the current reader instance to a writer instance.
    // /// </summary>
    // /// <returns>A writer instance of the current instance with the same byte data.</returns>
    // public NetData ToWriteState()
    // {
    //     if (Disposed)
    //         throw new ObjectDisposedException(nameof(NetData));
    //
    //     return IsReceived ? new(Read<CustomRPC>(), false, [.. ReadBuffer[1..]]) : throw new InvalidOperationException("The current instance is already in write state!");
    // }

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
        _ when type == typeof(RoleOptionData) => ReadRoleOptionData(),
        _ when type == typeof(CustomButton) => ReadButton(),
        _ when type == typeof(Vector2) => ReadVector2(),
        _ when type == typeof(NetData) => ReadNetData(),
        _ when type == typeof(Number) => ReadNumber(),
        _ when type == typeof(Type) => ReadType(),
        _ when type == typeof(Enum) => ReadEnum(ReadType()),
        _ when type.IsEnum => ReadEnum(type),
        _ when typeof(IPlayerLayer).IsAssignableFrom(type) => ReadLayer(),
        _ when typeof(IEnumerable).IsAssignableFrom(type) => type.IsArray
            ? ReadArray(type.GetElementType())
            : ReadValues(type.IsGenericType ? type.GetGenericArguments()[0] : typeof(object)), // Use of generics is enforced in the code base
        _ => throw new NotSupportedException($"{type.Name} cannot be read")
    };

    /// <summary>
    /// Reads a value based on its custom type code from the data.
    /// </summary>
    /// <returns>The deserialized value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private object Read(CustomTypeCode? type = null) => (type ??= Read<CustomTypeCode>()) switch
    {
        CustomTypeCode.PlayerControl => ReadPlayer(),
        CustomTypeCode.DeadBody => ReadBody(),
        CustomTypeCode.Vent => ReadVent(),
        CustomTypeCode.PlayerVoteArea => ReadVoteArea(),
        CustomTypeCode.Vector2 => ReadVector2(),
        CustomTypeCode.Char => ReadChar(),
        CustomTypeCode.Boolean => ReadBool(),
        CustomTypeCode.Byte => ReadByte(),
        CustomTypeCode.SByte => ReadSByte(),
        CustomTypeCode.UShort => ReadUShort(),
        CustomTypeCode.Short => ReadShort(),
        CustomTypeCode.UInt => ReadUInt(),
        CustomTypeCode.Int => ReadInt(),
        CustomTypeCode.Long => ReadLong(),
        CustomTypeCode.ULong => ReadULong(),
        CustomTypeCode.Float => ReadFloat(),
        CustomTypeCode.Double => ReadDouble(),
        CustomTypeCode.Half => ReadHalf(),
        CustomTypeCode.Decimal => ReadDecimal(),
        CustomTypeCode.String => ReadString(),
        CustomTypeCode.Enum => ReadEnum(Read<Type>()),
        CustomTypeCode.Type => ReadType(),
        CustomTypeCode.Array => ReadArray(),
        CustomTypeCode.IEnumerable => ReadValues(),
        CustomTypeCode.NetData => ReadNetData(),
        CustomTypeCode.Button => ReadButton(),
        CustomTypeCode.Number => ReadNumber(),
        CustomTypeCode.RoleOptionData => ReadRoleOptionData(),
        CustomTypeCode.PlayerLayer => ReadLayer(),
        CustomTypeCode.Object => Read(),
        _ => throw new NotSupportedException($"Custom type code {type} cannot be read")
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
            var code = Read<CustomTypeCode>();

            while (count-- > 0)
                yield return Read(code);
        }
        else
        {
            while (count-- > 0)
                yield return Read();
        }
    }

    // /// <summary>
    // /// Reads a casted array of values from the data, prefixed by a count.
    // /// </summary>
    // /// <typeparam name="T">The type to cast the collection to.</typeparam>
    // /// <returns>An array of values, casted to <typeparamref name="T"/>.</returns>
    // /// <inheritdoc cref="ThrowIfIncorrectState"/>
    // public T[] ReadArray<T>() => ReadArray(typeof(T)) as T[];

    /// <summary>
    /// Reads an array of values from the data, prefixed by a count.
    /// </summary>
    /// <returns>An array of values.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private Array ReadArray(Type type)
    {
        var count = ReadUShort();
        var array = Array.CreateInstance(type, count);

        for (var i = 0; i < count; i++)
            array.SetValue(Read(type), i);

        return array;
    }

    /// <inheritdoc cref="ReadArray(Type)"/>
    private Array ReadArray()
    {
        var flag = ReadBool();

        if (flag)
            return ReadArray(Read<CustomTypeCode>().ToType());
        else
        {
            var count = ReadUShort();
            var array = new object[count];

            for (var i = 0; i < count; i++)
                array[i] = Read();

            return array;
        }
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
    /// Reads a boolean value from the data.
    /// </summary>
    /// <returns>true if the read byte is non-zero; otherwise, false.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private char ReadChar()
    {
        ThrowIfIncorrectState(true, 2);
        var result = BitConverter.ToChar(ReadBuffer, Position);
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
        var result = (sbyte)(ReadBuffer[Position] - 128);
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
    /// Reads a half floating point precision value (float) from the data.
    /// </summary>
    /// <returns>The deserialized float value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private Half ReadHalf()
    {
        ThrowIfIncorrectState(true, 2);
        var result = BitConverter.ToHalf(ReadBuffer, Position);
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
        ThrowIfIncorrectState(true, 4);
        var result = BitConverter.ToSingle(ReadBuffer, Position);
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
        ThrowIfIncorrectState(true, 8);
        var result = BitConverter.ToDouble(ReadBuffer, Position);
        Position += 8;
        return result;
    }

    /// <summary>
    /// Reads a decimal value from the data.
    /// </summary>
    /// <returns>The deserialized double value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private decimal ReadDecimal()
    {
        var bytes = ReadBytes(16);
        return new([BitConverter.ToInt32(bytes, 0), BitConverter.ToInt32(bytes, 4), BitConverter.ToInt32(bytes, 8), BitConverter.ToInt32(bytes, 12)]);
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

    /// <summary>
    /// Reads a sequence of bytes from the data.
    /// </summary>
    /// <param name="length">The number of bytes to read.</param>
    /// <returns>The deserialized byte array.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private byte[] ReadBytes(int length)
    {
        ThrowIfIncorrectState(true, length);
        var result = ReadBuffer[Position..(Position + length)];
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
        return new(Mathf.Lerp(MinPos, MaxPos, x), Mathf.Lerp(MinPos, MaxPos, y));
    }

    /// <summary>
    /// Reads a chunked <see cref="NetData"/> from the data.
    /// </summary>
    /// <returns>The deserialized net data.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private NetData ReadNetData() => new(ReadBytes(ReadUShort()));

    /// <summary>
    /// Reads a numbered value from the data.
    /// </summary>
    /// <returns>The deserialized number value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private Number ReadNumber() => ReadFloat();

    /// <summary>
    /// Reads a type from the data.
    /// </summary>
    /// <returns>The deserialized type value.</returns>
    /// <inheritdoc cref="ThrowIfIncorrectState"/>
    private Type ReadType() => Type.GetType(ReadString());

    // Some stuff to serialize Vector2 using the game's bounds
    private const float MinPos = -50f;
    private const float MaxPos = +50f;
    private const float Diff = MaxPos - MinPos;

    private static float ReverseLerp(float t) => Mathf.Clamp01((t - MinPos) / Diff);

    // Interface stuff

    private void InternalDispose()
    {
        if (Disposed)
            return;

        if (BytesRemaining > 0 && IsReceived)
            Warning($"There were {BytesRemaining} bytes of unread data in the rpc {(CustomRPC)ReadBuffer[0]} {ReadBuffer[1]}");

        if (ReadBuffer is not null)
            Array.Clear(ReadBuffer); // Explicitly clearing a byte[] just for the ease of my mind

        WriteBuffer?.Clear();
        Disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        InternalDispose();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public byte[] ToBytes() => [.. ToBytes((ushort)DataSize), .. (IEnumerable<byte>)(IsReceived ? ReadBuffer : WriteBuffer)];

    // Static method helpers

    /// <summary>
    /// Serializes the value to an array of bytes.
    /// </summary>
    /// <param name="value">The value to serialize.</param>
    /// <param name="withTypeCode">Indicates whether or not the type code of the value should be included or not.</param>
    /// <returns>An array of bytes representing the value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> was null.</exception>
    /// <exception cref="NotSupportedException">Thrown of the value can't be serialized.</exception>
    public static byte[] ToBytes(object value, bool withTypeCode = false) => value switch
    {
        // Types from the base game
        PlayerControl i => ToBytes(i, withTypeCode),
        DeadBody i => ToBytes(i, withTypeCode),
        PlayerVoteArea i => ToBytes(i, withTypeCode),
        Vent i => ToBytes(i, withTypeCode),
        Vector2 i => ToBytes(i, withTypeCode),

        // Custom types using the interface
        INetSerializable i => ToBytes(i, withTypeCode),

        // Base C# types
        char i => ToBytes(i, withTypeCode),
        bool i => ToBytes(i, withTypeCode),
        byte i => ToBytes(i, withTypeCode),
        sbyte i => ToBytes(i, withTypeCode),
        ushort i => ToBytes(i, withTypeCode),
        short i => ToBytes(i, withTypeCode),
        int i => ToBytes(i, withTypeCode),
        uint i => ToBytes(i, withTypeCode),
        ulong i => ToBytes(i, withTypeCode),
        long i => ToBytes(i, withTypeCode),
        float i => ToBytes(i, withTypeCode),
        double i => ToBytes(i, withTypeCode),
        Half i => ToBytes(i, withTypeCode),
        decimal i => ToBytes(i, withTypeCode), // BitConverter please have more conversion methods I beg you
        string i => ToBytes(i, withTypeCode), // String needs a custom method because BitConverter apparently can't support it
        Enum i => ToBytes(i, withTypeCode),
        Type i => ToBytes(i, withTypeCode),

        // WIP
        // IDictionary i => ToBytes(i, withTypeCode),
        // ICollection i => ToBytes(i, withTypeCode),

        // Special cases
        Array i => ToBytes(i, withTypeCode),
        IEnumerable i => ToBytes(i, withTypeCode),

        // Edge cases
        null => throw new ArgumentNullException(nameof(value)),
        _ => throw new NotSupportedException($"Either {value.GetType().Name} does not extend INetSerializable, or cannot be serialized to bytes")
    };

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(byte value, bool withTypeCode = false) => withTypeCode ? [ (byte)CustomTypeCode.Byte, value] : [value];

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(bool value, bool withTypeCode = false) => withTypeCode ? [ (byte)CustomTypeCode.Boolean, (byte)(value ? 1 : 0)] : [(byte)(value ? 1 : 0)];

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(sbyte value, bool withTypeCode = false) => withTypeCode ? [ (byte)CustomTypeCode.SByte, (byte)(value + 128)] : [(byte)(value + 128)];

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(PlayerControl value, bool withTypeCode = false) => withTypeCode ? [ (byte)CustomTypeCode.PlayerControl, value.PlayerId] : [value.PlayerId];

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(DeadBody value, bool withTypeCode = false) => withTypeCode ? [ (byte)CustomTypeCode.DeadBody, value.ParentId] : [value.ParentId];

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(PlayerVoteArea value, bool withTypeCode = false) => withTypeCode ? [ (byte)CustomTypeCode.PlayerVoteArea, value.TargetPlayerId] : [value.TargetPlayerId];

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(Vent value, bool withTypeCode = false)
    {
        var bytes = BitConverter.GetBytes(value.Id);
        return withTypeCode ? [ (byte)CustomTypeCode.Vent, .. bytes] : bytes;
    }

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(Type value, bool withTypeCode = false)
    {
        var bytes = ToBytes(value.AssemblyQualifiedName);
        return withTypeCode ? [ (byte)CustomTypeCode.Type, .. bytes] : bytes;
    }

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(string value, bool withTypeCode = false)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        return withTypeCode ? [ (byte)CustomTypeCode.String, .. ToBytes((ushort)bytes.Length), .. bytes] : [.. ToBytes((ushort)bytes.Length), .. bytes];
    }

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(decimal value, bool withTypeCode = false)
    {
        var bytes = decimal.GetBits(value).SelectMany(BitConverter.GetBytes);
        return withTypeCode ? [ (byte)CustomTypeCode.Decimal, .. bytes] : [.. bytes];
    }

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(ushort value, bool withTypeCode = false)
    {
        var bytes = BitConverter.GetBytes(value);
        return withTypeCode ? [ (byte)CustomTypeCode.UShort, .. bytes] : bytes;
    }

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(short value, bool withTypeCode = false)
    {
        var bytes = BitConverter.GetBytes(value);
        return withTypeCode ? [ (byte)CustomTypeCode.Short, .. bytes] : bytes;
    }

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(uint value, bool withTypeCode = false)
    {
        var bytes = BitConverter.GetBytes(value);
        return withTypeCode ? [ (byte)CustomTypeCode.UInt, .. bytes] : bytes;
    }

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(int value, bool withTypeCode = false)
    {
        var bytes = BitConverter.GetBytes(value);
        return withTypeCode ? [ (byte)CustomTypeCode.Int, .. bytes] : bytes;
    }

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(ulong value, bool withTypeCode = false)
    {
        var bytes = BitConverter.GetBytes(value);
        return withTypeCode ? [ (byte)CustomTypeCode.ULong, .. bytes] : bytes;
    }

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(long value, bool withTypeCode = false)
    {
        var bytes = BitConverter.GetBytes(value);
        return withTypeCode ? [ (byte)CustomTypeCode.Long, .. bytes] : bytes;
    }

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(Half value, bool withTypeCode = false)
    {
        var bytes = BitConverter.GetBytes(value);
        return withTypeCode ? [ (byte)CustomTypeCode.Half, .. bytes] : bytes;
    }

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(float value, bool withTypeCode = false)
    {
        var bytes = BitConverter.GetBytes(value);
        return withTypeCode ? [ (byte)CustomTypeCode.Float, .. bytes] : bytes;
    }

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(double value, bool withTypeCode = false)
    {
        var bytes = BitConverter.GetBytes(value);
        return withTypeCode ? [ (byte)CustomTypeCode.Double, .. bytes] : bytes;
    }

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(char value, bool withTypeCode = false)
    {
        var bytes = BitConverter.GetBytes(value);
        return withTypeCode ? [ (byte)CustomTypeCode.Char, .. bytes] : bytes;
    }

    /// <inheritdoc cref="ToBytes(object, bool)"/>
    public static byte[] ToBytes(INetSerializable value, bool withTypeCode = false)
    {
        var bytes = value.ToBytes();
        return withTypeCode ? [ (byte)value.TypeCode, .. bytes] : bytes;
    }

    /// <summary>
    /// Serializes a collection of values to an array of bytes, prefixed by the collection's length.
    /// </summary>
    /// <param name="values">The values to serialize.</param>
    /// <param name="withTypeCode">Indicates whether or not the typ code of the value should be included or not.</param>
    /// <returns>An array of bytes representing the values.</returns>
    private static byte[] ToBytes(IEnumerable values, bool withTypeCode = false)
    {
        var result = new List<byte>();

        if (!withTypeCode)
        {
            ushort count = 0;

            foreach (var obj in values)
            {
                result.AddRange(ToBytes(obj, false));
                count++;
            }

            result.InsertRange(0, BitConverter.GetBytes(count));
            return [.. result];
        }

        Type commonType = null;
        var elements = new List<object>();
        var allSameType = true;

        foreach (var obj in values)
        {
            elements.Add(obj);

            if (!allSameType || obj == null)
                continue;

            var currentType = obj.GetType();

            if (commonType == null)
                commonType = currentType;
            else if (commonType != currentType)
                allSameType = false;
        }

        result.AddRange(BitConverter.GetBytes((ushort)elements.Count));
        result.Add((byte)(allSameType ? 1 : 0));

        if (allSameType && commonType != null)
        {
            result.Add((byte)commonType.GetCustomTypeCode());
            elements.ForEach(x => result.AddRange(ToBytes(x, false)));
        }
        else
            elements.ForEach(x => result.AddRange(ToBytes(x, true)));

        return [.. result];
    }

    // /// <summary>
    // /// Serializes an array of values to an array of bytes, prefixed by the collection's length.
    // /// </summary>
    // /// <param name="values">The values to serialize.</param>
    // /// <returns>An array of bytes representing the values.</returns>
    // private static byte[] ToBytes(ICollection values)
    // {
    //     var result = new List<byte>(BitConverter.GetBytes((ushort)values.Count));

    //     foreach (var obj in values)
    //         result.AddRange(ToBytes(obj));

    //     return [ .. result ];
    // }

    /// <summary>
    /// Serializes an array of values to an array of bytes, prefixed by the collection's length.
    /// </summary>
    /// <param name="values">The values to serialize.</param>
    /// <param name="withTypeCode">Indicates whether or not the typ code of the value should be included or not.</param>
    /// <returns>An array of bytes representing the values.</returns>
    private static byte[] ToBytes(Array values, bool withTypeCode = false)
    {
        var result = new List<byte>();

        if (!withTypeCode)
        {
            result.AddRange(BitConverter.GetBytes((ushort)values.Length));

            foreach (var obj in values)
                result.AddRange(ToBytes(obj, false));
        }
        else
        {
            Type commonType = null;
            var elements = new List<object>();
            var allSameType = true;

            foreach (var obj in values)
            {
                elements.Add(obj);

                if (!allSameType)
                    continue;

                if (obj == null)
                {
                    allSameType = false;
                    continue;
                }

                var currentType = obj.GetType();

                if (commonType == null)
                    commonType = currentType;
                else if (commonType != currentType)
                    allSameType = false;
            }

            result.Add((byte)(allSameType ? 1 : 0));
            result.AddRange(BitConverter.GetBytes((ushort)values.Length));

            if (allSameType && commonType != null)
            {
                result.Add((byte)commonType.GetCustomTypeCode());
                elements.ForEach(x => result.AddRange(ToBytes(x, false)));
            }
            else
                elements.ForEach(x => result.AddRange(ToBytes(x, true)));
        }

        return [.. result];
    }

    /// <summary>
    /// Serializes a 2d vector to an array of bytes.
    /// </summary>
    /// <param name="value">The value to serialize.</param>
    /// <param name="withTypeCode">Indicates whether or not the typ code of the value should be included or not.</param>
    /// <returns>An array of bytes representing the value.</returns>
    private static byte[] ToBytes(Vector2 value, bool withTypeCode = false)
    {
        var x = BitConverter.GetBytes((ushort)(ReverseLerp(value.x) * ushort.MaxValue));
        var y = BitConverter.GetBytes((ushort)(ReverseLerp(value.y) * ushort.MaxValue));
        return withTypeCode ? [(byte)CustomTypeCode.Vector2, .. x, .. y] : [.. x, .. y];
    }

    /// <summary>
    /// Serializes an enum to an array of bytes.
    /// </summary>
    /// <param name="value">The value to serialize.</param>
    /// <param name="withTypeCode">Indicates whether or not the typ code of the value should be included or not.</param>
    /// <returns>An array of bytes representing the value.</returns>
    private static byte[] ToBytes(Enum value, bool withTypeCode = false)
    {
        var type = value.GetType();
        var bytes = ToBytes(Convert.ChangeType(value, Enum.GetUnderlyingType(type)));
        return withTypeCode ? [(byte)CustomTypeCode.Enum, .. ToBytes(type), .. bytes] : bytes;
    }
}