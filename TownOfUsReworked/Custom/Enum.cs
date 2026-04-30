using System.Runtime.CompilerServices;

namespace TownOfUsReworked.Custom;

public abstract class CustomEnumInjector
{
    public static readonly Dictionary<Type, CustomEnumInjector> Injectors = [];
    protected static readonly object ThreadLock = new();

    public abstract Array Values();

    public abstract string ToString(Enum value);
}

/// <summary>
/// A wrapper class to help with injecting values to enum classes.
/// </summary>
/// <typeparam name="T">The relevant enum Type.</typeparam>
public sealed class EnumInjector<T> : CustomEnumInjector where T : struct, Enum
{
    private static readonly long Min;
    private static readonly long Max;

    private static readonly Type Type;
    private static readonly Type UnderlyingType;
    private static readonly bool IsUnsigned;

    public static readonly T MaxPossibleValue;
    public static readonly T MinPossibleValue;

    static EnumInjector()
    {
        Type = typeof(T);
        UnderlyingType = Enum.GetUnderlyingType(Type);
        IsUnsigned = IsTypeUnsigned(UnderlyingType);

        Min = Convert.ToInt64(AccessTools.Field(UnderlyingType, "MinValue").GetRawConstantValue());

        var maxObj = AccessTools.Field(UnderlyingType, "MaxValue").GetRawConstantValue()!;
        Max = IsUnsigned ? unchecked((long)Convert.ToUInt64(maxObj)) : Convert.ToInt64(maxObj);

        MaxPossibleValue = LongToEnum(Max);
        MinPossibleValue = LongToEnum(Min);
    }

    private readonly List<T> AllValues;
    private readonly List<T> InjectedValues;

    private readonly ValueMap<string, T> NamedValues;
    private readonly ValueMap<long, T> IndexedValues;
    private readonly ValueMap<string, long> PairedValues;

    private readonly object Lock;
    private readonly bool Il2Cpp;

    private long Last;
    private long First;

    public EnumInjector(Il2CppState state = Il2CppState.AlreadyInIl2Cpp)
    {
        Lock = new();

        if (state == Il2CppState.RegisterToIl2Cpp)
            EnumInjector.RegisterEnumInIl2Cpp<T>();

        Il2Cpp = state != Il2CppState.NonIl2Cpp;

        AllValues = [.. Enum.GetValues<T>().OrderBy(x => x)];

        Last = EnumToLong(AllValues[^1]);
        First = EnumToLong(AllValues[0]);

        InjectedValues = [];
        IndexedValues = [];
        NamedValues = [];
        PairedValues = [];

        foreach (var value in AllValues)
        {
            var valLong = EnumToLong(value);
            IndexedValues.Add(valLong, value);
            NamedValues.Add(EnumPatches.OriginalToString(value), value);
        }

        lock (ThreadLock)
            Injectors[Type] = this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsTypeUnsigned(Type t) => t == typeof(byte) || t == typeof(ushort) || t == typeof(uint) || t == typeof(ulong);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long EnumToLong(T val) => Unsafe.SizeOf<T>() switch
    {
        1 => IsUnsigned ? Unsafe.As<T, byte>(ref val) : Unsafe.As<T, sbyte>(ref val),
        2 => IsUnsigned ? Unsafe.As<T, ushort>(ref val) : Unsafe.As<T, short>(ref val),
        4 => IsUnsigned ? Unsafe.As<T, uint>(ref val) : Unsafe.As<T, int>(ref val),
        8 => Unsafe.As<T, long>(ref val),
        _ => throw new NotSupportedException("Unsupported enum size.")
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T LongToEnum(long val) => Unsafe.As<long, T>(ref val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool CheckOverflow(long index, bool decrement)
    {
        if (IsUnsigned)
        {
            var uIndex = (ulong)index;
            var uMin = (ulong)Min;
            var uMax = (ulong)Max;
            var uFirst = (ulong)First;
            var uLast = (ulong)Last;

            if ((uFirst == uMin && decrement) || (uLast == uMax && !decrement))
                return true;

            return uIndex < uMin || uIndex > uMax;
        }
        else
        {
            if ((First == Min && decrement) || (Last == Max && !decrement))
                return true;

            return index < Min || index > Max;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private long Increment(long val) => IsUnsigned ? (long)((ulong)val + 1) : (val + 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private long Decrement(long val) => IsUnsigned ? (long)((ulong)val - 1) : (val - 1);

    public override string ToString(Enum value)
    {
        if (value is not T enumVal)
            throw new InvalidOperationException();

        return NamedValues.TryGetKey(enumVal, out var name) ? name : EnumPatches.OriginalToString(value);
    }

    public override Array Values() => AllValues.ToArray();

    private string Diagnostic(object? value) => $"Min = {Min}, Max = {Max}, First = {First}, Last = {Last}, Parameter = {value ?? "None"}";

    public bool TryInjectAndReturn(string value, out T result, bool decrement = false)
    {
        try
        {
            result = InjectAndReturn(value, decrement);
            return true;
        }
        catch (Exception ex) when (ex is ArgumentException or OverflowException)
        {
            result = default;
            return false;
        }
    }

    public T InjectAndReturn(string value, bool decrement = false)
    {
        lock (Lock)
        {
            if (NamedValues.ContainsKey(value))
                throw new ArgumentException($"{value} already injected. {Diagnostic(decrement)}");

            if (CheckOverflow(decrement ? First : Last, decrement))
                throw new OverflowException($"Overflow detected. {Diagnostic(decrement)}");

            var index = decrement ? (First = Decrement(First)) : (Last = Increment(Last));
            return ProcessSingleInjection(value, index, decrement);
        }
    }

    public bool TryInjectAndReturn(string[] values, out IEnumerable<T>? result, bool[]? decrements = null)
    {
        try
        {
            result = InjectAndReturn(values, decrements);
            return true;
        }
        catch (Exception ex) when (ex is ArgumentException or OverflowException)
        {
            result = null;
            return false;
        }
    }

    public bool TryInjectAndReturn(string value, long index, out T result)
    {
        try
        {
            result = InjectAndReturn(value, index);
            return true;
        }
        catch (Exception ex) when (ex is ArgumentException or OverflowException)
        {
            result = default;
            return false;
        }
    }

    public T InjectAndReturn(string value, long index)
    {
        lock (Lock)
        {
            if (NamedValues.ContainsKey(value))
                throw new ArgumentException($"{value} already injected. {Diagnostic(index)}");

            var outOfBounds = IsUnsigned
                ? (ulong)index < (ulong)Min || (ulong)index > (ulong)Max
                : index < Min || index > Max;

            if (outOfBounds)
                throw new OverflowException($"Index {index} out of bounds for {Type.Name}. {Diagnostic(index)}");

            return ProcessSingleInjection(value, index, index);
        }
    }

    public bool TryInjectAndReturn(string[] values, long[] indices, out IEnumerable<T>? result)
    {
        try
        {
            result = InjectAndReturn(values, indices);
            return true;
        }
        catch (Exception ex) when (ex is ArgumentException or OverflowException)
        {
            result = null;
            return false;
        }
    }

    private IEnumerable<T> InjectAndReturn(string[] values, long[] indices)
    {
        if (indices.Length != values.Length)
            throw new InvalidOperationException($"Not all string names have a corresponding index. {Diagnostic(null)}");

        lock (Lock)
        {
            foreach (var index in indices)
            {
                bool outOfBounds = IsUnsigned
                    ? (ulong)index < (ulong)Min || (ulong)index > (ulong)Max
                    : index < Min || index > Max;

                if (outOfBounds)
                    throw new OverflowException($"Index {index} out of bounds for {Type.Name}. {Diagnostic(index)}");
            }

            return ProcessBulkInjection(values, indices);
        }
    }

    private IEnumerable<T> InjectAndReturn(string[] values, bool[]? decrements = null)
    {
        decrements ??= new bool[values.Length];

        if (decrements.Length != values.Length)
            Array.Resize(ref decrements, values.Length);

        var indices = new long[values.Length];

        lock (Lock)
        {
            for (var i = 0; i < values.Length; i++)
            {
                var dec = decrements[i];

                if (CheckOverflow(dec ? First : Last, dec))
                    throw new OverflowException($"Overflow detected for {values[i]}. {Diagnostic(dec)}");

                indices[i] = dec ? (First = Decrement(First)) : (Last = Increment(Last));
            }

            return ProcessBulkInjection(values, indices);
        }
    }

    private T ProcessSingleInjection(string value, long index, object diagnosticState)
    {
        if (IndexedValues.ContainsKey(index))
            throw new ArgumentException($"{index} already injected. {Diagnostic(diagnosticState)}");

        if (Il2Cpp)
        {
            var boxedIl2CppVal = IsUnsigned
                ? Convert.ChangeType((ulong)index, UnderlyingType)
                : Convert.ChangeType(index, UnderlyingType);

            EnumInjector.InjectEnumValues<T>(new() { { value, boxedIl2CppVal } });
        }

        var result = LongToEnum(index);

        NamedValues[value] = result;
        IndexedValues[index] = result;
        PairedValues[value] = index;

        InsertSorted(AllValues, result);
        InsertSorted(InjectedValues, result);

        return result;
    }

    private IEnumerable<T> ProcessBulkInjection(string[] values, long[] indices)
    {
        var results = new List<T>(values.Length);
        var valuesToAdd = new Dictionary<string, object>();
        var listsModified = false;

        for (int i = 0; i < values.Length; i++)
        {
            var value = values[i];
            var index = indices[i];

            if (NamedValues.ContainsKey(value) || IndexedValues.ContainsKey(index) || valuesToAdd.ContainsKey(value))
                continue;

            var boxedVal = IsUnsigned
                ? Convert.ChangeType((ulong)index, UnderlyingType)
                : Convert.ChangeType(index, UnderlyingType);

            valuesToAdd.Add(value, boxedVal);
        }

        if (valuesToAdd.Count == 0)
            return results;

        if (Il2Cpp)
            EnumInjector.InjectEnumValues<T>(valuesToAdd);

        foreach (var (name, value) in valuesToAdd)
        {
            var index = IsUnsigned
                ? unchecked((long)Convert.ToUInt64(value))
                : Convert.ToInt64(value);
            var result = LongToEnum(index);

            NamedValues[name] = result;
            IndexedValues[index] = result;
            PairedValues[name] = index;

            AllValues.Add(result);
            InjectedValues.Add(result);
            results.Add(result);
            listsModified = true;
        }

        if (listsModified)
        {
            AllValues.Sort();
            InjectedValues.Sort();
        }

        return results;
    }

    private void InsertSorted(List<T> list, T item)
    {
        var idx = list.BinarySearch(item);

        if (idx < 0)
            idx = ~idx;

        list.Insert(idx, item);
    }
}

public enum Il2CppState : byte
{
    NonIl2Cpp,
    RegisterToIl2Cpp,
    AlreadyInIl2Cpp
}