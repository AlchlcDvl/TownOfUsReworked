using System.Buffers;
using System.Buffers.Binary;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using TownOfUsReworked.Pooling;

namespace TownOfUsReworked.RPCs;

/// <summary>
/// A network writer for creating and sending RPC messages.
/// </summary>
public sealed class RpcWriter() : RpcBuffer(0)
{
    private int size;
    private int targetId = -1;

    public override int DataSize => size;

    // State

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureCapacity(int bytesToAdd)
    {
        ThrowIfPooled();

        if (currentBitIndex > 0)
            FlushPackedByte();

        if (position + bytesToAdd > buffer.Length)
            ResizeBuffer(bytesToAdd);
    }

    private void ResizeBuffer(int bytesToAdd)
    {
        var newSize = Math.Max(position + bytesToAdd, buffer.Length * 2);
        var newBuffer = ArrayPool<byte>.Shared.Rent(newSize);
        buffer.AsSpan(0, position).CopyTo(newBuffer);
        ArrayPool<byte>.Shared.Return(buffer);
        buffer = newBuffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FlushPackedByte()
    {
        var packed = currentPackedByte;
        currentPackedByte = 0;
        currentBitIndex = 0;

        if (position + 1 > buffer.Length)
            ResizeBuffer(1);

        buffer[position] = packed;
        Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Advance(int count)
    {
        position += count;
        size += count;
    }

    private void WriteVarInt(ulong value, int maxSize)
    {
        EnsureCapacity(maxSize);

        while (value >= 0x80)
        {
            buffer[position++] = (byte)(value | 0x80);
            value >>= 7;
        }

        buffer[position++] = (byte)value;

        if (position > size)
            size = position;
    }

    private void Send(bool immediate)
    {
        if (currentBitIndex > 0)
            FlushPackedByte();

        if (TownOfUsReworked.MciActive || !LocalPlayer)
            return;

        var message = new ReworkedMessage(targetId, buffer.AsSpan(0, position));

        if (immediate)
            RpcHandler.SendImmediateMessage(message);
        else
            Rpc.Instance.QueueLateMessage(message);
    }

    public void SendImmediate() => Send(true);

    public void SendLate() => Send(false);

    public void SetTargetId(int targetId) => this.targetId = targetId;

    protected override void OnRecycle() => ArrayPool<byte>.Shared.Return(buffer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Span<byte> WriteAlloc(int count)
    {
        EnsureCapacity(count);
        var span = buffer.AsSpan(position, count);
        span.Clear();
        Advance(count);
        return span;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteFloats(ReadOnlySpan<float> values)
    {
        var span = WriteAlloc(values.Length * 4);

        for (var i = 0; i < values.Length; i++)
            BinaryPrimitives.WriteSingleLittleEndian(span[(i * 4)..], values[i]);
    }

    // 1 byte

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteByte(byte value) => WriteAlloc(1)[0] = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBool(bool value) => WriteByte((byte)(value ? 1 : 0));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSByte(sbyte value) => WriteByte((byte)value);

    // Technically 1 byte

    public void WritePackedBool(bool value)
    {
        if (value)
            currentPackedByte |= (byte)(1 << currentBitIndex);

        currentBitIndex++;

        if (currentBitIndex == 8)
            EnsureCapacity(0);
    }

    // 2 bytes

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteShort(short value) => BinaryPrimitives.WriteInt16LittleEndian(WriteAlloc(2), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUShort(ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(WriteAlloc(2), value);

    // 4 bytes

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt(int value) => BinaryPrimitives.WriteInt32LittleEndian(WriteAlloc(4), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt(uint value) => BinaryPrimitives.WriteUInt32LittleEndian(WriteAlloc(4), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteFloat(float value) => BinaryPrimitives.WriteSingleLittleEndian(WriteAlloc(4), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteVector2(Vector2 value)
    {
        var span = WriteAlloc(4);
        BinaryPrimitives.WriteUInt16LittleEndian(span, ReverseLerp(value.x));
        BinaryPrimitives.WriteUInt16LittleEndian(span[2..], ReverseLerp(value.y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteColor32(Color32 value)
    {
        var span = WriteAlloc(4);
        span[0] = value.r;
        span[1] = value.g;
        span[2] = value.b;
        span[3] = value.a;
    }

    // 1 to 5 bytes

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WritePackedInt(int value) => WritePackedUInt((uint)((value << 1) ^ (value >> 31)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WritePackedUInt(uint value) => WriteVarInt(value, 5);

    // 8 bytes

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteLong(long value) => BinaryPrimitives.WriteInt64LittleEndian(WriteAlloc(8), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteULong(ulong value) => BinaryPrimitives.WriteUInt64LittleEndian(WriteAlloc(8), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteDouble(double value) => BinaryPrimitives.WriteDoubleLittleEndian(WriteAlloc(8), value);

    // 1 to 10 bytes

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WritePackedLong(long value) => WritePackedULong((ulong)((value << 1) ^ (value >> 63)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WritePackedULong(ulong value) => WriteVarInt(value, 10);

    // 12 bytes

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteVector3(Vector3 value) => WriteFloats(stackalloc float[3] { value.x, value.y, value.z });

    // 16 bytes

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteColor(Color value) => WriteFloats(stackalloc float[4] { value.r, value.g, value.b, value.a });

    // Variable bytes

    public void WriteString(string? value, CountType countType = CountType.Byte)
    {
        if (string.IsNullOrEmpty(value))
        {
            WriteCount(0, countType);
            return;
        }

        var maxByteCount = Encoding.UTF8.GetMaxByteCount(value.Length);

        var headerSize = countType == CountType.Byte ? 1 : 2;
        EnsureCapacity(headerSize + maxByteCount);

        var lengthIndex = position;
        Advance(headerSize);

        var actualCount = Encoding.UTF8.GetBytes(value.AsSpan(), buffer.AsSpan(position));

        if (countType == CountType.Byte)
            buffer[lengthIndex] = (byte)actualCount;
        else
            BinaryPrimitives.WriteUInt16LittleEndian(buffer.AsSpan(lengthIndex), (ushort)actualCount);

        Advance(actualCount);
    }

    public void WriteStringWithoutSize(string value)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("Value cannot be null or empty");

        var maxByteCount = Encoding.UTF8.GetMaxByteCount(value.Length);
        EnsureCapacity(maxByteCount);
        Advance(Encoding.UTF8.GetBytes(value.AsSpan(), buffer.AsSpan(position)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteEnum<T>(T value) where T : struct, Enum => RpcWriterDels.Enum<T>.Writer(this, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteNetObject<T>(T value) where T : INetSerializable => RpcWriterDels.NetObject<T>.Writer(this, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteTuple<T1, T2>((T1, T2) value) => RpcWriterDels.Tuple<T1, T2>.Writer(this, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteGeneric<T>(T value) => RpcWriterDels.Type<T>.Writer(this, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteNullable<T>(T? value) where T : struct
    {
        var hasValue = value.HasValue;
        WriteBool(hasValue);

        if (hasValue)
            WriteGeneric(value!.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WritePackedEnum<T>(T value) where T : struct, Enum => RpcWriterDels.PackedEnum<T>.Writer(this, value);

    private void WriteCollection<T>(int count, IEnumerable<T> items, Action<RpcWriter, T> writer, CountType countType = CountType.Byte)
    {
        WriteCount(count, countType);

        foreach (var item in items)
            writer(this, item);
    }

    public void WriteArray<T>(T[]? array, Action<RpcWriter, T> writer, CountType countType = CountType.Byte)
        => WriteCollection(array?.Length ?? 0, array ?? Enumerable.Empty<T>(), writer, countType);

    public void WriteList<T>(List<T>? list, Action<RpcWriter, T> writer, CountType countType = CountType.Byte)
        => WriteCollection(list?.Count ?? 0, list ?? Enumerable.Empty<T>(), writer, countType);

    public void WriteSet<T>(HashSet<T>? set, Action<RpcWriter, T> writer, CountType countType = CountType.Byte)
        => WriteCollection(set?.Count ?? 0, set ?? Enumerable.Empty<T>(), writer, countType);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCount(int count, CountType countType)
    {
        if (countType == CountType.Byte)
            WriteByte((byte)count);
        else
            WriteUShort((ushort)count);
    }

    public void WriteDictionary<TKey, TValue>(Dictionary<TKey, TValue>? dict, Action<RpcWriter, TKey> keyWriter, Action<RpcWriter, TValue> valueWriter, CountType countType = CountType.UShort) where TKey : notnull
    {
        WriteCount(dict?.Count ?? 0, countType);

        if (dict == null)
            return;

        foreach (var (key, value) in dict)
        {
            keyWriter(this, key);
            valueWriter(this, value);
        }
    }

    public override void Clear()
    {
        base.Clear();
        size = 0;
        targetId = -1;
    }

    public void Reset(int initialCapacity = 256)
    {
        buffer = ArrayPool<byte>.Shared.Rent(initialCapacity);
        Clear();
    }

    private static ushort ReverseLerp(float t) => (ushort)(Mathf.Clamp01((t - MinPos) / Diff) * ushort.MaxValue);

    public static RpcWriter Borrow(int initialCapacity = 256)
    {
        var writer = RecyclePool<RpcWriter>.Borrow();
        writer.Reset(initialCapacity);
        return writer;
    }

    public static void Return(RpcWriter writer) => RecyclePool<RpcWriter>.Return(writer);
}

public static class RpcWriterDels
{
    public static class NetObject<T> where T : INetSerializable
    {
        public static readonly Action<RpcWriter, T> Writer = (writer, value) => value.SerializeTo(writer);
    }

    public static class Tuple<T1, T2>
    {
        public static readonly Action<RpcWriter, (T1, T2)> Writer = CreateTupleWriter<(T1, T2)>(typeof(T1), typeof(T2));
    }

    public static class Type<T>
    {
        public static readonly Action<RpcWriter, T> Writer = (Action<RpcWriter, T>)Delegate.CreateDelegate(typeof(Action<RpcWriter, T>), GetWriteExpression(typeof(T)));
    }

    public static class Enum<T> where T : struct, Enum
    {
        public static readonly Action<RpcWriter, T> Writer = CreateWriter();

        private static Action<RpcWriter, T> CreateWriter()
        {
            var size = Unsafe.SizeOf<T>();
            return size switch
            {
                1 => (writer, value) => writer.WriteByte(Unsafe.As<T, byte>(ref value)),
                2 => (writer, value) => writer.WriteUShort(Unsafe.As<T, ushort>(ref value)),
                4 => (writer, value) => writer.WriteUInt(Unsafe.As<T, uint>(ref value)),
                8 => (writer, value) => writer.WriteULong(Unsafe.As<T, ulong>(ref value)),
                _ => throw new ArgumentException($"Enum size {size} not supported")
            };
        }
    }

    public static class PackedEnum<T> where T : struct, Enum
    {
        public static readonly Action<RpcWriter, T> Writer = CreateWriter();

        private static Action<RpcWriter, T> CreateWriter()
        {
            var size = Unsafe.SizeOf<T>();
            var underlying = Enum.GetUnderlyingType(typeof(T));
            return size switch
            {
                1 => (writer, value) => writer.WriteByte(Unsafe.As<T, byte>(ref value)),
                2 => (writer, value) => writer.WriteUShort(Unsafe.As<T, ushort>(ref value)),
                4 when underlying == typeof(int) => (writer, value) => writer.WritePackedInt(Unsafe.As<T, int>(ref value)),
                4 => (writer, value) => writer.WritePackedUInt(Unsafe.As<T, uint>(ref value)),
                8 when underlying == typeof(long) => (writer, value) => writer.WritePackedLong(Unsafe.As<T, long>(ref value)),
                8 => (writer, value) => writer.WritePackedULong(Unsafe.As<T, ulong>(ref value)),
                _ => throw new ArgumentException($"Enum size {size} not supported")
            };
        }
    }

    private static Action<RpcWriter, TTuple> CreateTupleWriter<TTuple>(params Type[] componentTypes)
    {
        var writerParam = Expression.Parameter(typeof(RpcWriter), "writer");
        var tupleParam = Expression.Parameter(typeof(TTuple), "value");

        var writeCalls = new Expression[componentTypes.Length];

        for (var i = 0; i < componentTypes.Length; i++)
            writeCalls[i] = Expression.Call(writerParam, GetWriteExpression(componentTypes[i]), Expression.Field(tupleParam, $"Item{i + 1}"));

        var block = Expression.Block(writeCalls);
        return Expression.Lambda<Action<RpcWriter, TTuple>>(block, writerParam, tupleParam).Compile();
    }

    private static readonly Dictionary<Type, MethodInfo> TypeWriteCache = [];
    public static readonly ReadOnlyDictionary<Type, string> WriterMethodNames = new(new Dictionary<Type, string>
    {
        { typeof(byte), nameof(RpcWriter.WriteByte) },
        { typeof(int), nameof(RpcWriter.WriteInt) },
        { typeof(uint), nameof(RpcWriter.WriteUInt) },
        { typeof(long), nameof(RpcWriter.WriteLong) },
        { typeof(bool), nameof(RpcWriter.WriteBool) },
        { typeof(short), nameof(RpcWriter.WriteShort) },
        { typeof(ulong), nameof(RpcWriter.WriteULong) },
        { typeof(sbyte), nameof(RpcWriter.WriteSByte) },
        { typeof(float), nameof(RpcWriter.WriteFloat) },
        { typeof(Color), nameof(RpcWriter.WriteColor) },
        { typeof(ushort), nameof(RpcWriter.WriteUShort) },
        { typeof(double), nameof(RpcWriter.WriteDouble) },
        { typeof(string), nameof(RpcWriter.WriteString) },
        { typeof(Vector3), nameof(RpcWriter.WriteVector3) },
        { typeof(Vector2), nameof(RpcWriter.WriteVector2) },
        { typeof(Color32), nameof(RpcWriter.WriteColor32) }
    });

    private static MethodInfo GetWriteExpression(Type type)
    {
        if (TypeWriteCache.TryGetValue(type, out var method))
            return method;

        if (WriterMethodNames.TryGetValue(type, out var name))
            method = Method(name);
        else if (type.IsEnum)
            method = Method(nameof(RpcWriter.WriteEnum)).MakeGenericMethod(type);
        else if (typeof(INetSerializable).IsAssignableFrom(type))
            method = Method(nameof(RpcWriter.WriteNetObject)).MakeGenericMethod(type);

        TypeWriteCache[type] = method ?? throw new NotSupportedException($"Type {type.Name} is not supported in automatic serialization.");
        return method;
    }

    private static MethodInfo Method(string name) =>
        typeof(RpcWriter).GetMethod(name, BindingFlags.Instance | BindingFlags.Public)
        ?? throw new MissingMethodException($"RpcWriter missing method: {name}");
}