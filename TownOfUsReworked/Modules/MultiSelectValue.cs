namespace TownOfUsReworked.Modules;

/// <summary>
/// Represents a collection of enum values that can be stored and manipulated as a comma-separated string.
/// </summary>
/// <typeparam name="T">The enum type that this collection will store.</typeparam>
[Serializable]
public struct MultiSelectValue<T> : IComparable, IConvertible, ISpanFormattable, IComparable<T>, IEnumerable<T>, IEquatable<T>, IComparable<MultiSelectValue<T>>, IEquatable<MultiSelectValue<T>>
    where T : struct, Enum
{
    /// <summary>
    /// Gets or sets the underlying comma-separated string representation of enum values.
    /// </summary>
    public string Values
    {
        readonly get => ValuesPriv;
        set
        {
            if (value.Length > 0)
            {
                value = value.TrueReplace(",,", ",");

                if (value.StartsWith(','))
                    value = value[1..];

                if (value.EndsWith(','))
                    value = value[..^1];
            }

            ValuesPriv = value;
        }
    }
    private string ValuesPriv;

    /// <summary>
    /// Gets the number of enum values currently stored in the collection.
    /// </summary>
    public readonly int Count => Values.TrueSplit(',').Length;

    /// <summary>
    /// Initializes a new instance of MultiSelectValue with the specified enum values.
    /// </summary>
    /// <param name="values">One or more enum values to initialize the collection with.</param>
    public MultiSelectValue(params T[] values) => Values = Join(',', values);

    public static implicit operator string(MultiSelectValue<T> value) => value.Values;

    public static implicit operator T[](MultiSelectValue<T> value) => [ .. value.Values.TrueSplit(',').Select(Enum.Parse<T>) ];

    public static implicit operator T(MultiSelectValue<T> value) => value.Count == 1 ? value[0] : throw new($"Tried to convert multiple or no values ({value.Values}) to a singular one");

    public static implicit operator MultiSelectValue<T>(T value) => new(value);

    public static implicit operator MultiSelectValue<T>(T[] values) => [ .. values ];

    public static implicit operator MultiSelectValue<T>(string values) => [ .. values.TrueSplit(',').Select(Enum.Parse<T>) ];

    /// <summary>
    /// Gets or sets the enum value at the specified index position.
    /// </summary>
    /// <param name="index">The zero-based index of the value to access.</param>
    /// <returns>The enum value at the specified position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">When index is outside the valid range.</exception>
    public T this[int index]
    {
        readonly get => Enum.Parse<T>(Values.TrueSplit(',')[index]);
        set
        {
            var values = Values.TrueSplit(',');
            values[index] = $"{value}";
            Values = Join(',', values);
        }
    }

    /// <summary>
    /// Adds a single enum value to the end of the collection.
    /// </summary>
    /// <param name="item">The enum value to add.</param>
    public void Add(T item) => Values += $",{item}";

    /// <summary>
    /// Removes the first occurrence of the specified enum value.<br></br>
    /// Returns <c>true</c> if the value was found and removed, <c>false</c> otherwise.
    /// </summary>
    /// <param name="item">The enum value to remove.</param>
    /// <returns><c>true</c> if successfully removed; otherwise, <c>false</c>.</returns>
    public bool Remove(T item)
    {
        var former = $"{item}";
        var contains = Values.Contains(former);
        Values = Values.TrueReplace(former, "");
        return contains;
    }

    /// <summary>
    /// Adds multiple enum values to the end of the collection.
    /// </summary>
    /// <param name="items">The collection of enum values to add.</param>
    public void AddRange(IEnumerable<T> items) => Values = Join(',', [ Values, Join(',', items) ]);

    /// <summary>
    /// Removes multiple enum values from the collection.
    /// </summary>
    /// <param name="items">The enum values to remove.</param>
    /// <returns>The number of items successfully removed.</returns>
    public int RemoveRange(params T[] items) => RemoveRange((IEnumerable<T>)items);

    /// <summary>
    /// Removes multiple enum values from the collection.
    /// </summary>
    /// <param name="items">The collection of enum values to remove.</param>
    /// <returns>The number of items successfully removed.</returns>
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

    /// <summary>
    /// Determines whether the collection contains the specified enum value.
    /// </summary>
    /// <param name="item">The enum value to locate.</param>
    /// <returns>true if the item is found in the collection; otherwise, false.</returns>
    public readonly bool Contains(T item) => Values.Contains($"{item}");

    /// <summary>
    /// Removes all enum values that match the specified predicate condition.<br></br>
    /// Iterates through the collection and removes each matching value.
    /// </summary>
    /// <param name="predicate">A function that defines the condition for removing items.</param>
    /// <returns>The number of items removed from the collection.</returns>
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

    /// <summary>
    /// Removes all enum values from the collection.
    /// </summary>
    public void Clear() => Values = "";

    /// <summary>
    /// Converts the collection to its string representation.
    /// </summary>
    /// <returns>A comma-separated string of enum values.</returns>
    public override readonly string ToString() => Values;

    public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider provider)
    {
        var str = ToString(provider);

        if (str.AsSpan().TryCopyTo(destination))
        {
            charsWritten = str.Length;
            return true;
        }

        charsWritten = 0;
        return false;
    }

    public readonly string ToString(string format, IFormatProvider formatProvider) => ToString();

    public readonly bool Equals(MultiSelectValue<T> other) => Values == Values;

    public readonly override bool Equals(object obj) => obj is MultiSelectValue<T> other && Equals(other);

    public readonly override int GetHashCode() =>  Values?.GetHashCode() ?? 0;

    public readonly IEnumerator<T> GetEnumerator() => (IEnumerator<T>)((IEnumerable)this).GetEnumerator();

    readonly IEnumerator IEnumerable.GetEnumerator() => ((T[])this).GetEnumerator();

    public readonly bool Equals(MultiSelectValue<T> x, MultiSelectValue<T> y) => x.Equals(y);

    public readonly TypeCode GetTypeCode() => TypeCode.Object;

    public readonly bool ToBoolean(IFormatProvider provider) => Convert.ToBoolean(Values, provider);

    public readonly byte ToByte(IFormatProvider provider) => Convert.ToByte(Values, provider);

    public readonly char ToChar(IFormatProvider provider) => Convert.ToChar(Values, provider);

    public readonly DateTime ToDateTime(IFormatProvider provider) => Convert.ToDateTime(Values, provider);

    public readonly decimal ToDecimal(IFormatProvider provider) => Convert.ToDecimal(Values, provider);

    public readonly double ToDouble(IFormatProvider provider) => Convert.ToDouble(Values, provider);

    public readonly short ToInt16(IFormatProvider provider) => Convert.ToInt16(Values, provider);

    public readonly int ToInt32(IFormatProvider provider) => Convert.ToInt32(Values, provider);

    public readonly long ToInt64(IFormatProvider provider) => Convert.ToInt64(Values, provider);

    public readonly sbyte ToSByte(IFormatProvider provider) => Convert.ToSByte(Values, provider);

    public readonly float ToSingle(IFormatProvider provider) => Convert.ToSingle(Values, provider);

    public readonly string ToString(IFormatProvider provider) => Convert.ToString(Values, provider);

    public readonly object ToType(Type conversionType, IFormatProvider provider) => Convert.ChangeType(Values, conversionType, provider);

    public readonly ushort ToUInt16(IFormatProvider provider) => Convert.ToUInt16(Values, provider);

    public readonly uint ToUInt32(IFormatProvider provider) => Convert.ToUInt32(Values, provider);

    public readonly ulong ToUInt64(IFormatProvider provider) => Convert.ToUInt64(Values, provider);

    public readonly int GetHashCode(MultiSelectValue<T> obj) => obj.GetHashCode();

    public readonly int CompareTo(MultiSelectValue<T> other) => string.CompareOrdinal(Values, other.Values);

    public readonly bool Equals(T other) => Values.Contains(other.ToString());

    public readonly int CompareTo(T other) => string.CompareOrdinal(Values, other.ToString());

    public readonly int CompareTo(object obj) => obj is MultiSelectValue<T> other ? CompareTo(other) : throw new ArgumentException($"Object is not a MultiSelectValue<{typeof(T).Name}>");

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