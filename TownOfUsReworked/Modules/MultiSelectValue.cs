namespace TownOfUsReworked.Modules;

/// <summary>
/// Represents a collection of enum values that can be stored and manipulated as a comma-separated string.
/// </summary>
/// <typeparam name="T">The enum type that this collection will store.</typeparam>
[Serializable]
public sealed class MultiSelectValue<T> : ICollection<T>, IEquatable<MultiSelectValue<T>>, IDisposable, INetSerializable
    where T : struct, Enum
{
    /// <summary>
    /// Gets or sets the underlying comma-separated string representation of enum values.
    /// </summary>
    public string Values
    {
        get => Join(',', ValuesPriv.OrderBy(x => x));
        set
        {
            ValuesPriv.Clear();
            ValuesPriv.AddRange(value.TrueSplit(',').Select(Enum.Parse<T>).OrderBy(x => x));
        }
    }
    private readonly HashSet<T> ValuesPriv = [];

    /// <summary>
    /// Initialises a new instance of <see cref="MultiSelectValue{T}"/> with the provided values.
    /// </summary>
    /// <param name="values">One or more enum values to initialize the collection with.</param>
    public MultiSelectValue(params T[] values) => ValuesPriv.AddRange(values);

    /// <inheritdoc/>
    public int Count => ValuesPriv.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public void Add(T item) => ValuesPriv.Add(item);

    /// <inheritdoc/>
    public bool Remove(T item) => ValuesPriv.Remove(item);

    /// <summary>
    /// Adds multiple enum values to the end of the collection.
    /// </summary>
    /// <param name="items">The collection of enum values to add.</param>
    public void AddRange(IEnumerable<T> items) => items.ForEach(Add);

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
    public bool Contains(T item) => ValuesPriv.Contains(item);

    /// <summary>
    /// Removes all enum values that match the specified predicate condition.<br/>
    /// Iterates through the collection and removes each matching value.
    /// </summary>
    /// <param name="predicate">A function that defines the condition for removing items.</param>
    /// <returns>The number of items removed from the collection.</returns>
    public int RemoveAll(Func<T, bool> predicate) => ValuesPriv.Where(predicate).Count(Remove);

    /// <summary>
    /// Removes all enum values from the collection.
    /// </summary>
    public void Clear() => ValuesPriv.Clear();

    /// <summary>
    /// Gets the first value in the collection.
    /// </summary>
    /// <returns>Returns the first value.</returns>
    public T First() => ValuesPriv.First();

    /// <summary>
    /// Converts the collection to its string representation.
    /// </summary>
    /// <returns>A comma-separated string of enum values.</returns>
    public override string ToString() => Values;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is MultiSelectValue<T> other && Equals(other);

    /// <inheritdoc/>
    public bool Equals(MultiSelectValue<T> other) => other != null && ValuesPriv.SetEquals(other.ValuesPriv);

    /// <inheritdoc/>
    public override int GetHashCode() => Values.GetHashCode();

    /// <inheritdoc/>
    public void Dispose() => ValuesPriv.Clear();

    /// <inheritdoc/>
    public byte[] ToBytes() => [ (byte)Count, .. ValuesPriv.Select(x => (byte)Convert.ChangeType(x, typeof(byte))) ]; // All enums within the code base use byte

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex) => ValuesPriv.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)ValuesPriv).GetEnumerator();

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
    public static implicit operator T[](MultiSelectValue<T> value) => [ .. value.ValuesPriv ];

    /// <summary>
    /// Converts the current instance to one singular value.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <exception cref="Exception">There were either no values, or more than one value.</exception>
    public static implicit operator T(MultiSelectValue<T> value) =>
        value.Count == 1
            ? value.ValuesPriv.First()
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
    public static implicit operator MultiSelectValue<T>(T[] values) => new(values);

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
    public static bool operator ==(MultiSelectValue<T> left, MultiSelectValue<T> right) => left?.Equals(right) == true;

    /// <summary>
    /// Inequality comparison.
    /// </summary>
    /// <param name="left">Left.</param>
    /// <param name="right">Right.</param>
    public static bool operator !=(MultiSelectValue<T> left, MultiSelectValue<T> right) => left?.Equals(right) == false;

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
}