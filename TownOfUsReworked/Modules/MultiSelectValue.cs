namespace TownOfUsReworked.Modules;

/// <summary>
/// Represents a collection of enum values that can be stored and manipulated as a comma-separated string.
/// </summary>
/// <typeparam name="T">The enum type that this collection will store.</typeparam>
/// <param name="values">One or more enum values to initialize the collection with.</param>
[Serializable]
public struct MultiSelectValue<T>(params T[] values) : IEnumerable<T>, IEquatable<MultiSelectValue<T>> where T : struct, Enum
{
    /// <summary>
    /// Gets or sets the underlying comma-separated string representation of enum values.
    /// </summary>
    public string Values
    {
        readonly get => Join(',', ValuesPriv);
        set => ValuesPriv = [ .. value.TrueSplit(',').Select(Enum.Parse<T>) ];
    }
    private HashSet<T> ValuesPriv = [ .. values ];

    /// <summary>
    /// Gets the number of values contained within.
    /// </summary>
    public readonly int Count => ValuesPriv.Count;

    public static implicit operator string(MultiSelectValue<T> value) => value.Values;

    public static implicit operator T[](MultiSelectValue<T> value) => [ .. value.ValuesPriv ];

    public static implicit operator T(MultiSelectValue<T> value) => value.Count == 1 ? value.ValuesPriv.First() :
        throw new($"Tried to convert multiple or no values ({value.Values}) to a singular one");

    public static implicit operator MultiSelectValue<T>(T value) => new(value);

    public static implicit operator MultiSelectValue<T>(T[] values) => new(values);

    public static implicit operator MultiSelectValue<T>(string values) => new([ .. values.TrueSplit(',').Select(Enum.Parse<T>) ]);

    /// <summary>
    /// Adds a single enum value to the end of the collection.
    /// </summary>
    /// <param name="item">The enum value to add.</param>
    public readonly bool Add(T item) => ValuesPriv.Add(item);

    /// <summary>
    /// Removes the first occurrence of the specified enum value.<br/>
    /// Returns <c>true</c> if the value was found and removed, <c>false</c> otherwise.
    /// </summary>
    /// <param name="item">The enum value to remove.</param>
    /// <returns><c>true</c> if successfully removed; otherwise, <c>false</c>.</returns>
    public readonly bool Remove(T item) => ValuesPriv.Remove(item);

    /// <summary>
    /// Adds multiple enum values to the end of the collection.
    /// </summary>
    /// <param name="items">The collection of enum values to add.</param>
    public readonly void AddRange(IEnumerable<T> items)
    {
        foreach (var item in items)
            ValuesPriv.Add(item);
    }

    /// <summary>
    /// Removes multiple enum values from the collection.
    /// </summary>
    /// <param name="items">The enum values to remove.</param>
    /// <returns>The number of items successfully removed.</returns>
    public readonly int RemoveRange(params T[] items) => RemoveRange((IEnumerable<T>)items);

    /// <summary>
    /// Removes multiple enum values from the collection.
    /// </summary>
    /// <param name="items">The collection of enum values to remove.</param>
    /// <returns>The number of items successfully removed.</returns>
    public readonly int RemoveRange(IEnumerable<T> items)
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
    public readonly bool Contains(T item) => ValuesPriv.Contains(item);

    /// <summary>
    /// Removes all enum values that match the specified predicate condition.<br/>
    /// Iterates through the collection and removes each matching value.
    /// </summary>
    /// <param name="predicate">A function that defines the condition for removing items.</param>
    /// <returns>The number of items removed from the collection.</returns>
    public readonly int RemoveAll(Func<T, bool> predicate)
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
    public readonly void Clear() => ValuesPriv.Clear();

    /// <summary>
    /// Gets the first value in the collection.
    /// </summary>
    public readonly T First() => ValuesPriv.First();

    /// <summary>
    /// Converts the collection to its string representation.
    /// </summary>
    /// <returns>A comma-separated string of enum values.</returns>
    public override readonly string ToString() => Values;

    public override readonly bool Equals(object obj) => obj is MultiSelectValue<T> other && Equals(other);

    public override readonly int GetHashCode() =>  Values?.GetHashCode() ?? 0;

    public readonly IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)ValuesPriv).GetEnumerator();

    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static bool operator ==(MultiSelectValue<T> left, MultiSelectValue<T> right) => left.Equals(right);

    public static bool operator !=(MultiSelectValue<T> left, MultiSelectValue<T> right) => !left.Equals(right);

    public readonly bool Equals(MultiSelectValue<T> other) => ValuesPriv.SetEquals(other.ValuesPriv);
}