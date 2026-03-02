using System.Buffers.Binary;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using TownOfUsReworked.Pooling;

namespace TownOfUsReworked.RPCs;

/// <summary>
/// A network reader for reading from RPC messages.
/// </summary>
public sealed class RpcReader() : RpcBuffer(8)
{
    public int BytesRemaining => dataSize - position;

    public override int DataSize => dataSize;
    private int dataSize;

    // State

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureReadable(int bytesToRead)
    {
        ThrowIfPooled();
        ThrowIfTooLong(bytesToRead);
        currentBitIndex = 8;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ThrowIfTooLong(int bytesToRead)
    {
        if (position + bytesToRead > dataSize)
            throw new EndOfStreamException($"Attempted to read {bytesToRead} bytes, but only {BytesRemaining} remain.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ReadOnlySpan<byte> ReadRequest(int size)
    {
        EnsureReadable(size);
        var span = buffer.AsSpan(position, size);
        position += size;
        return span;
    }

    public void SetBuffer(byte[] data)
    {
        buffer = data;
        dataSize = data.Length;
        Clear();
    }

    public static void Return(RpcReader reader) => RecyclePool<RpcReader>.Return(reader);

    private ulong ReadVarInt(int maxShift)
    {
        EnsureReadable(1);

        var result = 0ul;
        var shift = 0;

        while (true)
        {
            if (position >= dataSize)
                throw new EndOfStreamException("Unexpected end of stream during VarInt.");

            var b = buffer[position++];
            result |= (ulong)(b & 0x7F) << shift;

            if ((b & 0x80) == 0)
                break;

            shift += 7;

            if (shift >= maxShift)
                throw new InvalidDataException("VarInt too long");
        }

        return result;
    }

    // 1 byte

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadByte()
    {
        EnsureReadable(1);
        return buffer[position++];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBool() => ReadByte() != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte ReadSByte() => (sbyte)ReadByte();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PlayerControl ReadPlayer() => PlayerById(ReadByte());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PlayerVoteArea ReadVoteArea() => VoteAreaById(ReadByte());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DeadBody ReadDeadBody() => BodyById(ReadByte());

    // Technically 1 byte

    public bool ReadPackedBool()
    {
        if (currentBitIndex >= 8)
        {
            currentPackedByte = ReadByte();
            currentBitIndex = 0;
        }

        var value = (currentPackedByte & (1 << currentBitIndex)) != 0;
        currentBitIndex++;
        return value;
    }

    // 2 bytes

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadShort() => BinaryPrimitives.ReadInt16LittleEndian(ReadRequest(2));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadUShort() => BinaryPrimitives.ReadUInt16LittleEndian(ReadRequest(2));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CustomButton ReadButton() => CustomButton.ButtonLookup[ReadUShort()];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IPlayerLayer ReadLayer()
    {
        var playerId = ReadByte();
        var type = ReadEnum<Layer>();
        return PlayerLayer.LayerLookup[((byte)type << 8) | playerId];
    }

    // 4 bytes

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt() => BinaryPrimitives.ReadInt32LittleEndian(ReadRequest(4));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUInt() => BinaryPrimitives.ReadUInt32LittleEndian(ReadRequest(4));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vent ReadVent() => VentById(ReadInt());

    // 1 to 5 bytes

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadPackedInt()
    {
        var val = ReadPackedUInt();
        return (int)(val >> 1) ^ -(int)(val & 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadPackedUInt() => (uint)ReadVarInt(35);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ReadFloat() => BinaryPrimitives.ReadSingleLittleEndian(ReadRequest(4));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 ReadVector2()
    {
        var span = ReadRequest(4);
        var xNorm = BinaryPrimitives.ReadUInt16LittleEndian(span);
        var yNorm = BinaryPrimitives.ReadUInt16LittleEndian(span[2..]);
        return new(Lerp(xNorm), Lerp(yNorm));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Color32 ReadColor32()
    {
        var span = ReadRequest(4);
        return new(span[0], span[1], span[2], span[3]);
    }

    // 8 bytes

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadLong() => BinaryPrimitives.ReadInt64LittleEndian(ReadRequest(8));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadULong() => BinaryPrimitives.ReadUInt64LittleEndian(ReadRequest(8));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ReadDouble() => BinaryPrimitives.ReadDoubleLittleEndian(ReadRequest(8));

    // 1 to 10 bytes

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadPackedLong()
    {
        var val = ReadPackedULong();
        return (long)(val >> 1) ^ -(long)(val & 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadPackedULong() => ReadVarInt(70);

    // 12 bytes

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadFloats(Span<float> values)
    {
        var span = ReadRequest(values.Length * 4);

        for (var i = 0; i < values.Length; i++)
            values[i] = BinaryPrimitives.ReadSingleLittleEndian(span[(i * 4)..]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 ReadVector3()
    {
        Span<float> v = stackalloc float[3];
        ReadFloats(v);
        return new(v[0], v[1], v[2]);
    }

    // 16 bytes

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Color ReadColor()
    {
        Span<float> v = stackalloc float[4];
        ReadFloats(v);
        return new(v[0], v[1], v[2], v[3]);
    }

    // Variable bytes

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadString(CountType countType = CountType.Byte) => ReadStringWithSize(ReadCount(countType))!;

    public string? ReadStringWithSize(int len)
    {
        if (len < 0)
            return null;

        if (len == 0)
            return string.Empty;

        EnsureReadable(len);
        var s = Encoding.UTF8.GetString(buffer.AsSpan(position, len));
        position += len;
        return s;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read<T>() => RpcReaderDels.Type<T>.Reader(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? ReadNullable<T>() where T : struct => ReadBool() ? Read<T>() : null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T ReadPackedEnum<T>() where T : struct, Enum => RpcReaderDels.PackedEnum<T>.Reader(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T ReadEnum<T>() where T : struct, Enum => RpcReaderDels.Enum<T>.Reader(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T ReadNetObject<T>() where T : INetDeserializable, new()
    {
        var obj = RpcReaderDels.NetObjectFactory<T>.Create();
        obj.DeserializeFrom(this);
        return obj;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int ReadCount(CountType countType) => countType == CountType.Byte ? ReadByte() : ReadUShort();

    private TCollection ReadCollection<TCollection, T>(Func<int, TCollection> factory, Action<TCollection, T> add, Func<RpcReader, T> reader, CountType countType = CountType.Byte)
    {
        var count = ReadCount(countType);
        var collection = factory(count);

        for (var i = 0; i < count; i++)
            add(collection, reader(this));

        return collection;
    }

    public T[] ReadArray<T>(Func<RpcReader, T> itemReader, CountType countType = CountType.Byte)
    {
        var count = ReadCount(countType);

        if (count == 0)
            return [];

        var array = new T[count];

        for (var i = 0; i < count; i++)
            array[i] = itemReader(this);

        return array;
    }

    public List<T> ReadList<T>(Func<RpcReader, T> reader, CountType countType = CountType.Byte)
        => ReadCollection(RpcReaderDels.ListDelegates<T>.Create, RpcReaderDels.ListDelegates<T>.Add, reader, countType);

    public HashSet<T> ReadSet<T>(Func<RpcReader, T> reader, CountType countType = CountType.Byte)
        => ReadCollection(RpcReaderDels.HashSetDelegates<T>.Create, RpcReaderDels.HashSetDelegates<T>.Add, reader, countType);

    public Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>(Func<RpcReader, TKey> keyReader, Func<RpcReader, TValue> valueReader, CountType countType = CountType.Byte) where TKey : notnull
    {
        var count = ReadCount(countType);

        if (count == 0)
            return [];

        var dict = new Dictionary<TKey, TValue>(count);

        while (count-- > 0)
            dict[keyReader(this)] = valueReader(this);

        return dict;
    }

    public void PopulateArray<T>(T[] array, Func<RpcReader, T> itemReader, CountType countType = CountType.Byte)
    {
        Array.Clear(array);
        var count = ReadCount(countType);

        if (count == 0)
            return;

        for (var i = 0; i < count; i++)
            array[i] = itemReader(this);
    }

    private void PopulateCollection<TCollection, T>(TCollection collection, Action<TCollection> clear, Action<TCollection, T> add, Func<RpcReader, T> reader, CountType countType = CountType.Byte)
    {
        clear(collection);
        var count = ReadCount(countType);

        if (count == 0)
            return;

        while (count-- > 0)
            add(collection, reader(this));
    }

    public void PopulateList<T>(List<T> list, Func<RpcReader, T> itemReader, CountType countType = CountType.Byte)
        => PopulateCollection(list, RpcReaderDels.ListDelegates<T>.Clear, RpcReaderDels.ListDelegates<T>.Add, itemReader, countType);

    public void PopulateSet<T>(HashSet<T> set, Func<RpcReader, T> itemReader, CountType countType = CountType.Byte)
        => PopulateCollection(set, RpcReaderDels.HashSetDelegates<T>.Clear, RpcReaderDels.HashSetDelegates<T>.Add, itemReader, countType);

    public void PopulateDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, Func<RpcReader, TKey> keyReader, Func<RpcReader, TValue> valueReader, CountType countType = CountType.Byte) where TKey : notnull
    {
        dict.Clear();
        var count = ReadCount(countType);

        if (count == 0)
            return;

        while (count-- > 0)
            dict[keyReader(this)] = valueReader(this);
    }

    private static float Lerp(ushort t) => Mathf.Lerp(MinPos, MaxPos, t / (float)ushort.MaxValue);

    public static RpcReader Borrow(byte[] data)
    {
        var reader = RecyclePool<RpcReader>.Borrow();
        reader.SetBuffer(data);
        return reader;
    }
}

public static class RpcReaderDels
{
    public static class Type<T>
    {
        public static readonly Func<RpcReader, T> Reader = (Func<RpcReader, T>)Delegate.CreateDelegate(typeof(Func<RpcReader, T>), GetReadExpression(typeof(T)));
    }

    public static class Tuple<T1, T2>
    {
        public static readonly Func<RpcReader, (T1, T2)> Reader = CreateTupleReader<(T1, T2)>(typeof(T1), typeof(T2));
    }

    public static class NetObject<T> where T : INetDeserializable, new()
    {
        public static readonly Func<RpcReader, T> Reader = reader => reader.ReadNetObject<T>();
    }

    public static class Enum<T> where T : struct, Enum
    {
        public static readonly Func<RpcReader, T> Reader = CreateEnumReader();

        private static Func<RpcReader, T> CreateEnumReader()
        {
            var size = Unsafe.SizeOf<T>();
            return size switch
            {
                1 => reader =>
                {
                    var v = reader.ReadByte();
                    return Unsafe.As<byte, T>(ref v);
                },
                2 => reader =>
                {
                    var v = reader.ReadUShort();
                    return Unsafe.As<ushort, T>(ref v);
                },
                4 => reader =>
                {
                    var v = reader.ReadUInt();
                    return Unsafe.As<uint, T>(ref v);
                },
                8 => reader =>
                {
                    var v = reader.ReadULong();
                    return Unsafe.As<ulong, T>(ref v);
                },
                _ => throw new NotSupportedException($"Enum size {size} not supported")
            };
        }
    }

    public static class PackedEnum<T> where T : struct, Enum
    {
        public static readonly Func<RpcReader, T> Reader = CreateReader();

        private static Func<RpcReader, T> CreateReader()
        {
            var size = Unsafe.SizeOf<T>();
            var underlying = Enum.GetUnderlyingType(typeof(T));
            return size switch
            {
                1 => reader =>
                {
                    var v = reader.ReadByte();
                    return Unsafe.As<byte, T>(ref v);
                },
                2 => reader =>
                {
                    var v = reader.ReadUShort();
                    return Unsafe.As<ushort, T>(ref v);
                },
                4 when underlying == typeof(int) => reader =>
                {
                    var v = reader.ReadPackedInt();
                    return Unsafe.As<int, T>(ref v);
                },
                4 => reader =>
                {
                    var v = reader.ReadPackedUInt();
                    return Unsafe.As<uint, T>(ref v);
                },
                8 when underlying == typeof(long) => reader =>
                {
                    var v = reader.ReadPackedLong();
                    return Unsafe.As<long, T>(ref v);
                },
                8 => reader =>
                {
                    var v = reader.ReadPackedULong();
                    return Unsafe.As<ulong, T>(ref v);
                },
                _ => throw new NotSupportedException($"Enum size {size} not supported")
            };
        }
    }

    private static Func<RpcReader, TTuple> CreateTupleReader<TTuple>(params Type[] componentTypes)
    {
        var readerParam = Expression.Parameter(typeof(RpcReader), "reader");
        var readCalls = new Expression[componentTypes.Length];

        for (var i = 0; i < componentTypes.Length; i++)
            readCalls[i] = Expression.Call(readerParam, GetReadExpression(componentTypes[i]));

        var constructor = typeof(TTuple).GetConstructor(componentTypes) ?? throw new InvalidOperationException($"Could not find constructor for tuple {typeof(TTuple)}");
        var newTuple = Expression.New(constructor, readCalls);
        return Expression.Lambda<Func<RpcReader, TTuple>>(newTuple, readerParam).Compile();
    }

    private static readonly Dictionary<Type, MethodInfo> TypeReadCache = [];
    public static readonly ReadOnlyDictionary<Type, string> ReaderMethodNames = new(new Dictionary<Type, string>
    {
        { typeof(int), nameof(RpcReader.ReadInt) },
        { typeof(byte), nameof(RpcReader.ReadByte) },
        { typeof(uint), nameof(RpcReader.ReadUInt) },
        { typeof(long), nameof(RpcReader.ReadLong) },
        { typeof(bool), nameof(RpcReader.ReadBool) },
        { typeof(Vent), nameof(RpcReader.ReadVent) },
        { typeof(sbyte), nameof(RpcReader.ReadSByte) },
        { typeof(short), nameof(RpcReader.ReadShort) },
        { typeof(ulong), nameof(RpcReader.ReadULong) },
        { typeof(float), nameof(RpcReader.ReadFloat) },
        { typeof(Color), nameof(RpcReader.ReadColor) },
        { typeof(ushort), nameof(RpcReader.ReadUShort) },
        { typeof(double), nameof(RpcReader.ReadDouble) },
        { typeof(string), nameof(RpcReader.ReadString) },
        { typeof(Vector2), nameof(RpcReader.ReadVector2) },
        { typeof(Vector3), nameof(RpcReader.ReadVector3) },
        { typeof(Color32), nameof(RpcReader.ReadColor32) },
        { typeof(DeadBody), nameof(RpcReader.ReadDeadBody) },
        { typeof(CustomButton), nameof(RpcReader.ReadButton) },
        { typeof(PlayerControl), nameof(RpcReader.ReadPlayer) },
        { typeof(PlayerVoteArea), nameof(RpcReader.ReadVoteArea) }
    });

    private static MethodInfo GetReadExpression(Type type)
    {
        if (TypeReadCache.TryGetValue(type, out var method))
            return method;

        if (ReaderMethodNames.TryGetValue(type, out var name))
            method = Method(name);
        else if (type.IsEnum)
            method = Method(nameof(RpcReader.ReadEnum)).MakeGenericMethod(type);
        else if (typeof(IPlayerLayer).IsAssignableFrom(type))
            method = Method(nameof(RpcReader.ReadLayer));
        else if (typeof(INetDeserializable).IsAssignableFrom(type))
            method = Method(nameof(RpcReader.ReadNetObject)).MakeGenericMethod(type);

        TypeReadCache[type] = method ?? throw new NotSupportedException($"Type {type.Name} is not supported in automatic deserialization.");
        return method;
    }

    private static MethodInfo Method(string name) =>
        typeof(RpcReader).GetMethod(name, BindingFlags.Instance | BindingFlags.Public)
        ?? throw new MissingMethodException($"RpcReader missing method: {name}");

    public static class NetObjectFactory<T> where T : INetDeserializable, new()
    {
        public static readonly Func<T> Create = CreateFactory();

        private static Func<T> CreateFactory()
        {
            var newExp = Expression.New(typeof(T));
            var lambda = Expression.Lambda<Func<T>>(newExp);
            return lambda.Compile();
        }
    }

    public static class ListDelegates<T>
    {
        public static readonly Func<int, List<T>> Create = count => new List<T>(count);
        public static readonly Action<List<T>, T> Add = (list, item) => list.Add(item);
        public static readonly Action<List<T>> Clear = list => list.Clear();
    }

    public static class HashSetDelegates<T>
    {
        public static readonly Func<int, HashSet<T>> Create = count => new HashSet<T>(count);
        public static readonly Action<HashSet<T>, T> Add = (set, item) => set.Add(item);
        public static readonly Action<HashSet<T>> Clear = list => list.Clear();
    }
}