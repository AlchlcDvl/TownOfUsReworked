namespace TownOfUsReworked.Modules;

/// <summary>
/// Represents a collection of enum values that can be stored and manipulated as a comma-separated string.
/// </summary>
/// <typeparam name="T">The enum type that this collection will store.</typeparam>
[Serializable]
public sealed class MultiSelectValue<T> : IDisposable, INetSerializable, INetDeserializable, ICollection<T>, IEquatable<MultiSelectValue<T>>
    where T : struct, Enum
{
    /// <summary>
    /// Gets or sets the underlying comma-separated string representation of enum values.
    /// </summary>
    public string Values => Join(',', values.OrderBy(x => x));

    private readonly HashSet<T> values = [];

    /// <summary>
    /// Initialises a new instance of <see cref="MultiSelectValue{T}"/> with the provided values.
    /// </summary>
    /// <param name="values">One or more enum values to initialize the collection with.</param>
    public MultiSelectValue(params T[] values) => this.values.AddRange(values ?? []);

    /// <summary>
    /// Initialises a new instance of <see cref="MultiSelectValue{T}"/>.
    /// </summary>
    public MultiSelectValue() {}

    /// <inheritdoc/>
    public int Count => values.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    private bool Disposed { get; set; }

    ~MultiSelectValue() => InternalDispose();

    /// <inheritdoc/>
    public void Add(T item) => values.Add(item);

    /// <inheritdoc/>
    public bool Remove(T item) => values.Remove(item);

    /// <summary>
    /// Adds multiple enum values to the end of the collection.
    /// </summary>
    /// <param name="items">The collection of enum values to add.</param>
    public void AddRange(IEnumerable<T> items) => items.Do(Add);

    /// <summary>
    /// Removes multiple enum values from the collection.
    /// </summary>
    /// <param name="items">The collection of enum values to remove.</param>
    /// <returns>The number of items successfully removed.</returns>
    public int RemoveRange(IEnumerable<T> items) => items.Count(Remove);

    /// <summary>
    /// Determines whether the collection contains the specified enum value.
    /// </summary>
    /// <param name="item">The enum value to locate.</param>
    /// <returns>true if the item is found in the collection; otherwise, false.</returns>
    public bool Contains(T item) => values.Contains(item);

    /// <summary>
    /// Determines whether the collection contains the specified string representation of an enum value.
    /// </summary>
    /// <param name="item">The enum value to locate.</param>
    /// <returns>true if the item is found in the collection; otherwise, false.</returns>
    public bool Contains(string item) => Values.Contains(item);

    /// <summary>
    /// Determines whether the collection contains the specified enum values.
    /// </summary>
    /// <param name="items">The enum values to locate.</param>
    /// <returns>true if the items are found in the collection; otherwise, false.</returns>
    public bool ContainsAny(params T[] items) => values.ContainsAny(items);

    /// <summary>
    /// Determines whether the collection contains all of the specified enum values.
    /// </summary>
    /// <param name="items">The enum values to locate.</param>
    /// <returns>true if the items are found in the collection; otherwise, false.</returns>
    public bool ContainsAll(params T[] items) => values.ContainsAll(items);

    /// <summary>
    /// Removes all enum values that match the specified predicate condition.<br/>
    /// Iterates through the collection and removes each matching value.
    /// </summary>
    /// <param name="predicate">A function that defines the condition for removing items.</param>
    /// <returns>The number of items removed from the collection.</returns>
    public int RemoveAll(Func<T, bool> predicate) => values.Where(predicate).Count(Remove);

    /// <summary>
    /// Removes all enum values from the collection.
    /// </summary>
    public void Clear() => values.Clear();

    /// <summary>
    /// Gets the first value in the collection.
    /// </summary>
    /// <returns>Returns the first value.</returns>
    public T First() => values.First();

    /// <inheritdoc cref="Enumerable.Select"/>
    public IEnumerable<E> Select<E>(Func<T, E> predicate) => values.Select(predicate);

    /// <inheritdoc cref="Enumerable.SelectMany"/>
    public IEnumerable<E> SelectMany<E>(Func<T, IEnumerable<E>> predicate) => values.SelectMany(predicate);

    /// <summary>
    /// Converts the collection to its string representation.
    /// </summary>
    /// <returns>A comma-separated string of enum values.</returns>
    public override string ToString() => Values;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is MultiSelectValue<T> other && Equals(other);

    /// <inheritdoc/>
    public bool Equals(MultiSelectValue<T> other) => other is not null && values.SetEquals(other.values);

    /// <inheritdoc/>
    public override int GetHashCode() => Values.GetHashCode();

    private void InternalDispose()
    {
        if (Disposed)
            return;

        values.Clear();
        Disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        InternalDispose();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public IEnumerable<byte> GetBytes()
    {
        // All enums within the code base use byte
        yield return (byte)values.Count;

        var type = typeof(byte);

        foreach (var value in values)
            yield return (byte)Convert.ChangeType(value, type);
    }

    /// <inheritdoc/>
    public void FromBytes(RpcReader reader)
    {
        var count = reader.ReadByte();

        while (count-- > 0)
            values.Add((T)Enum.ToObject(typeof(T), reader.ReadByte()));
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex) => values.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)values).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Converts the current instance to its string representation.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator string(MultiSelectValue<T> value) => value.Values;

    /// <summary>
    /// Converts the current instance to its array representation of values.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator T[](MultiSelectValue<T> value) => [ .. value.values ];

    /// <summary>
    /// Converts the current instance to one singular value.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <exception cref="InvalidOperationException">Thrown if there were either no values, or more than one value.</exception>
    public static implicit operator T(MultiSelectValue<T> value) =>
        value.Count == 1
            ? value.values.First()
            : throw new InvalidOperationException($"Tried to convert multiple or no values ({value.Values}) to a singular one");

    /// <summary>
    /// Converts the value to an instance of MultiSelectValue.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator MultiSelectValue<T>(T value) => new(value);

    /// <summary>
    /// Converts the array of values to an instance of MultiSelectValue.
    /// </summary>
    /// <param name="values">The values to convert.</param>
    public static implicit operator MultiSelectValue<T>(T[] values) => [.. values];

    /// <summary>
    /// Converts the value string to an instance of MultiSelectValue.
    /// </summary>
    /// <param name="values">The values to convert.</param>
    public static implicit operator MultiSelectValue<T>(string values) => new(IsNullEmptyOrWhiteSpace(values) ? [] : [ .. values.TrueSplit(',').Select(Enum.Parse<T>) ]);

    /// <summary>
    /// Equality comparison.
    /// </summary>
    /// <param name="left">Left.</param>
    /// <param name="right">Right.</param>
    public static bool operator ==(MultiSelectValue<T> left, T[] right) => left?.ContainsAll(right) == true;

    /// <summary>
    /// Inequality comparison.
    /// </summary>
    /// <param name="left">Left.</param>
    /// <param name="right">Right.</param>
    public static bool operator !=(MultiSelectValue<T> left, T[] right) => left?.ContainsAll(right) == false;

    /// <summary>
    /// Equality comparison.
    /// </summary>
    /// <param name="left">Left.</param>
    /// <param name="right">Right.</param>
    public static bool operator ==(MultiSelectValue<T> left, T right) => left?.Contains(right) == true;

    /// <summary>
    /// Inequality comparison.
    /// </summary>
    /// <param name="left">Left.</param>
    /// <param name="right">Right.</param>
    public static bool operator !=(MultiSelectValue<T> left, T right) => left?.Contains(right) == false;

    /// <summary>
    /// Adds the elements in the right collection to the left collection.
    /// </summary>
    /// <param name="left">Left.</param>
    /// <param name="right">Right.</param>
    public static MultiSelectValue<T> operator +(MultiSelectValue<T> left, IEnumerable<T> right)
    {
        left.values.AddRange(right);
        return left;
    }

    /// <summary>
    /// Removes the elements in the right collection from the left collection.
    /// </summary>
    /// <param name="left">Left.</param>
    /// <param name="right">Right.</param>
    public static MultiSelectValue<T> operator -(MultiSelectValue<T> left, IEnumerable<T> right)
    {
        left.values.RemoveRange(right);
        return left;
    }
}

[Serializable]
public struct RoleOptionData(byte chance, byte count, bool unique, bool active, Layer layer) : INetSerializable, INetDeserializable, IEquatable<RoleOptionData>
{
    public RoleOptionData() : this(0, 0, false, false, Layer.None) {}

    public byte Chance { get; set; } = chance;
    public byte Count { get; set; } = count;
    public bool Unique { get; set; } = unique;
    public bool Active { get; set; } = active;
    public Layer ID { get; set; } = layer;

    public override readonly string ToString() => Join(',', Chance, Count, Unique, Active, ID);

    public readonly RoleOptionData Clone() => new(Chance, Count, Unique, Active, ID);

    public readonly bool IsActive(int? relatedCount = null) => ((Chance > 0 && Count > 0 && IsClassic()) || (Active && IsAllAny()) || (IsList() && ListEntryOption.IsAdded(ID.CastToSlot()))) &&
        ID.IsValid(relatedCount);

    public readonly IEnumerable<byte> GetBytes() => [ Chance, Count, (byte)(Unique ? 1 : 0), (byte)(Active ? 1 : 0), (byte)ID ];

    public void FromBytes(RpcReader reader)
    {
        Chance = reader.ReadByte();
        Count = reader.ReadByte();
        Unique = reader.ReadBool();
        Active = reader.ReadBool();
        ID = (Layer)reader.ReadByte();
    }

    public readonly bool Equals(RoleOptionData other) => other.Chance == Chance && other.Count == Count && other.Unique == Unique && other.Active == Active && other.ID == ID;

    public override readonly bool Equals(object obj) => obj is RoleOptionData data && Equals(data);

    public override readonly int GetHashCode() => HashCode.Combine(Chance, Count, Unique, Active, ID);

    public static bool operator ==(RoleOptionData left, RoleOptionData right) => left.Equals(right);

    public static bool operator !=(RoleOptionData left, RoleOptionData right) => !(left == right);

    public static RoleOptionData Parse(string input)
    {
        var parts = input.TrueSplit(',');
        return new(byte.Parse(parts[0]), byte.Parse(parts[1]), bool.Parse(parts[2]), bool.Parse(parts[3]), Enum.Parse<Layer>(parts[4]));
    }
}

/// <summary>
/// A wrapper for handling quick value conversions between <see cref="byte"/>, <see cref="int"/> and <see cref="float"/>.
/// </summary>
/// <param name="num">The value to be set.</param>
[Serializable]
public struct Number(float num) : IComparable, IFormattable, INetSerializable, INetDeserializable, IEquatable<Number>, IComparable<Number>
{
    public Number() : this(0) {}

    /// <summary>
    /// Gets the value.
    /// </summary>
    public float Value { get; private set; } = num;

    /// <summary>
    /// Implicitly converts to float.
    /// </summary>
    /// <param name="number">The number to convert.</param>
    /// <returns>A float value.</returns>
    public static implicit operator float(Number number) => number.Value;

    /// <summary>
    /// Implicitly converts to int.
    /// </summary>
    /// <param name="number">The number to convert.</param>
    /// <returns>An int value.</returns>
    public static implicit operator int(Number number) => (int)number.Value;

    /// <summary>
    /// Implicitly converts to byte.
    /// </summary>
    /// <param name="number">The number to convert.</param>
    /// <returns>A byte value.</returns>
    public static implicit operator byte(Number number) => (byte)number.Value;

    /// <summary>
    /// Implicitly converts to Number.
    /// </summary>
    /// <param name="num">The number to convert.</param>
    /// <returns>A Number instance.</returns>
    public static implicit operator Number(float num) => new(num);

    /// <inheritdoc cref="op_Implicit(float)"/>
    public static implicit operator Number(int num) => new(num);

    /// <summary>
    /// Equality check.
    /// </summary>
    /// <param name="a">Left.</param>
    /// <param name="b">Right.</param>
    /// <returns><c>true</c> if both values match</returns>
    public static bool operator ==(Number a, Number b) => a.Value == b.Value;

    /// <summary>
    /// Inequality check.
    /// </summary>
    /// <param name="a">Left.</param>
    /// <param name="b">Right.</param>
    /// <returns><c>true</c> if neither values match</returns>
    public static bool operator !=(Number a, Number b) => a.Value != b.Value;

    /// <summary>
    /// Order check.
    /// </summary>
    /// <param name="a">Left.</param>
    /// <param name="b">Right.</param>
    /// <returns><c>true</c> if <paramref name="a"/> is greater than <paramref name="b"/>.</returns>
    public static bool operator >(Number a, Number b) => a.Value > b.Value;

    /// <summary>
    /// Order check.
    /// </summary>
    /// <param name="a">Left.</param>
    /// <param name="b">Right.</param>
    /// <returns><c>true</c> if <paramref name="a"/> is greater than or equal <paramref name="b"/>.</returns>
    public static bool operator >=(Number a, Number b) => a.Value >= b.Value;

    /// <summary>
    /// Order check.
    /// </summary>
    /// <param name="a">Left.</param>
    /// <param name="b">Right.</param>
    /// <returns><c>true</c> if <paramref name="a"/> is lesser than <paramref name="b"/>.</returns>
    public static bool operator <(Number a, Number b) => a.Value < b.Value;

    /// <summary>
    /// Order check.
    /// </summary>
    /// <param name="a">Left.</param>
    /// <param name="b">Right.</param>
    /// <returns><c>true</c> if both values match</returns>
    /// <returns><c>true</c> if <paramref name="a"/> is lesser than or equal <paramref name="b"/>.</returns>
    public static bool operator <=(Number a, Number b) => a.Value <= b.Value;

    /// <inheritdoc/>
    public readonly IEnumerable<byte> GetBytes() => RpcWriter.GetBytes(Value);

    /// <inheritdoc/>
    public override readonly int GetHashCode() => Value.GetHashCode();

    /// <inheritdoc/>
    public readonly bool Equals(Number other) => Value == other.Value;

    /// <inheritdoc/>
    public override readonly bool Equals(object obj) => obj is Number number && Value == number.Value;

    /// <inheritdoc/>
    public override readonly string ToString() => Value.ToString();

    /// <inheritdoc/>
    public readonly string ToString(string format, IFormatProvider formatProvider) => Value.ToString(format, formatProvider);

    /// <inheritdoc/>
    public readonly int CompareTo(object obj) => obj switch
    {
        Number i => CompareTo(i),
        _ => Value.CompareTo(obj)
    };

    /// <inheritdoc/>
    public readonly int CompareTo(Number other) => Value.CompareTo(other.Value);

    /// <inheritdoc/>
    public void FromBytes(RpcReader reader) => Value = reader.ReadFloat();
}