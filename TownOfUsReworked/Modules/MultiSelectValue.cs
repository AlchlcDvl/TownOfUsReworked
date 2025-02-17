namespace TownOfUsReworked.Modules;

[Serializable]
public struct MultiSelectValue<T> : IComparable, IConvertible, ISpanFormattable, IComparable<T>, IEnumerable<T>, IEquatable<T>, IComparable<MultiSelectValue<T>>,
    IEquatable<MultiSelectValue<T>> where T : struct, Enum
{
    private string ValuesPriv;
    public string Values
    {
        get => ValuesPriv;
        set
        {
            if (value.Length == 0)
            {
                ValuesPriv = value;
                return;
            }

            value = value.TrueReplace(",,", ",");

            if (value.StartsWith(','))
                value = value[1..];

            if (value.EndsWith(','))
                value = value[..^1];

            ValuesPriv = value;
        }
    }

    public int Count => Values.TrueSplit(',').Length;

    public MultiSelectValue(params T[] values) => Values = Join(',', values);

    public static implicit operator string(MultiSelectValue<T> value) => value.Values;

    public static implicit operator T[](MultiSelectValue<T> value) => [ .. value.Values.TrueSplit(',').Select(Enum.Parse<T>) ];

    public static implicit operator T(MultiSelectValue<T> value) => value.Count == 1 ? value[0] : throw new($"Tried to convert multiple or no values ({value.Values}) to a singular one");

    public static implicit operator MultiSelectValue<T>(T value) => new(value);

    public static implicit operator MultiSelectValue<T>(T[] values) => [ .. values ];

    public static implicit operator MultiSelectValue<T>(string values) => [ .. values.TrueSplit(',').Select(Enum.Parse<T>) ];

    public T this[int index]
    {
        get => Enum.Parse<T>(Values.TrueSplit(',')[index]);
        set
        {
            var values = Values.TrueSplit(',');
            values[index] = $"{value}";
            Values = Join(',', values);
        }
    }

    public void Add(T item) => Values += $",{item}";

    public bool Remove(T item)
    {
        var former = $"{item}";
        var contains = Values.Contains(former);
        Values = Values.TrueReplace(former, "");
        return contains;
    }

    public void AddRange(IEnumerable<T> items) => Values = Join(',', [ Values, Join(',', items) ]);

    public int RemoveRange(params T[] items) => RemoveRange((IEnumerable<T>)items);

    public int RemoveRange(IEnumerable<T> items)
    {
        var count = 0;

        foreach (var item in items)
        {
            if (Remove(item))
                count++;
        }

        return count;
    }

    public bool Contains(T item) => Values.Contains($"{item}");

    public int RemoveAll(Func<T, bool> predicate)
    {
        var count = 0;

        foreach (var item in this)
        {
            if (predicate(item) && Remove(item))
                count++;
        }

        return count;
    }

    public void Clear() => Values = "";

    public override string ToString() => Values;

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider provider)
    {
        var str = ToString();

        if (str.AsSpan().TryCopyTo(destination))
        {
            charsWritten = str.Length;
            return true;
        }

        charsWritten = 0;
        return false;
    }

    public string ToString(string format, IFormatProvider formatProvider) => ToString();

    public bool Equals(MultiSelectValue<T> other) => Values == Values;

    public override bool Equals(object obj) => obj is MultiSelectValue<T> other && Equals(other);

    public override int GetHashCode() =>  Values?.GetHashCode() ?? 0;

    public IEnumerator<T> GetEnumerator() => (IEnumerator<T>)((IEnumerable)this).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((T[])this).GetEnumerator();

    public bool Equals(MultiSelectValue<T> x, MultiSelectValue<T> y) => x.Equals(y);

    public TypeCode GetTypeCode() => TypeCode.Object;

    public bool ToBoolean(IFormatProvider provider) => Convert.ToBoolean(Values, provider);

    public byte ToByte(IFormatProvider provider) => Convert.ToByte(Values, provider);

    public char ToChar(IFormatProvider provider) => Convert.ToChar(Values, provider);

    public DateTime ToDateTime(IFormatProvider provider) => Convert.ToDateTime(Values, provider);

    public decimal ToDecimal(IFormatProvider provider) => Convert.ToDecimal(Values, provider);

    public double ToDouble(IFormatProvider provider) => Convert.ToDouble(Values, provider);

    public short ToInt16(IFormatProvider provider) => Convert.ToInt16(Values, provider);

    public int ToInt32(IFormatProvider provider) => Convert.ToInt32(Values, provider);

    public long ToInt64(IFormatProvider provider) => Convert.ToInt64(Values, provider);

    public sbyte ToSByte(IFormatProvider provider) => Convert.ToSByte(Values, provider);

    public float ToSingle(IFormatProvider provider) => Convert.ToSingle(Values, provider);

    public string ToString(IFormatProvider provider) => Convert.ToString(Values, provider);

    public object ToType(Type conversionType, IFormatProvider provider) => Convert.ChangeType(Values, conversionType, provider);

    public ushort ToUInt16(IFormatProvider provider) => Convert.ToUInt16(Values, provider);

    public uint ToUInt32(IFormatProvider provider) => Convert.ToUInt32(Values, provider);

    public ulong ToUInt64(IFormatProvider provider) => Convert.ToUInt64(Values, provider);

    public int GetHashCode(MultiSelectValue<T> obj) => obj.GetHashCode();

    public int CompareTo(MultiSelectValue<T> other) => string.CompareOrdinal(Values, other.Values);

    public bool Equals(T other) => Values.Contains(other.ToString());

    public int CompareTo(T other) => string.CompareOrdinal(Values, other.ToString());

    public int CompareTo(object obj) => obj is MultiSelectValue<T> other ? CompareTo(other) : throw new ArgumentException($"Object is not a MultiSelectValue<{typeof(T).Name}>");

    public static bool operator ==(MultiSelectValue<T> left, MultiSelectValue<T> right) => left.Equals(right);

    public static bool operator !=(MultiSelectValue<T> left, MultiSelectValue<T> right) => left.Equals(right);

    public static MultiSelectValue<T> operator +(MultiSelectValue<T> left, IEnumerable<T> right)
    {
        left.AddRange(right);
        return left;
    }

    public static MultiSelectValue<T> operator -(MultiSelectValue<T> left, IEnumerable<T> right)
    {
        left.RemoveRange(right);
        return left;
    }

    public static MultiSelectValue<T> operator +(MultiSelectValue<T> left, T right)
    {
        left.Add(right);
        return left;
    }

    public static MultiSelectValue<T> operator -(MultiSelectValue<T> left, T right)
    {
        left.Remove(right);
        return left;
    }
}