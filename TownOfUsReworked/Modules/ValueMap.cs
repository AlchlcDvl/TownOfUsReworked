namespace TownOfUsReworked.Modules;

/// <summary>
/// Represents a bidirectional dictionary that allows mapping between keys and values in both directions.
/// </summary>
/// <typeparam name="T1">The type of the keys in the forward dictionary.</typeparam>
/// <typeparam name="T2">The type of the values in the forward dictionary.</typeparam>
/// <remarks>Yoinked this lovely piece of code from Daemon at <see href="https://github.com/DaemonBeast/Mitochondria/blob/main/Mitochondria.Core/Utilities/Structures/Map.cs"/> albeit with a few changes of my own.</remarks>
public sealed class ValueMap<T1, T2> : IDictionary<T1, T2>, IReadOnlyDictionary<T1, T2>
    where T1 : notnull
    where T2 : notnull
{
    private readonly Dictionary<T1, T2> Forward;
    private readonly Dictionary<T2, T1> Backward;

    /// <inheritdoc cref="IDictionary{TKey,TValue}.Count"/>
    public int Count => Forward.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public ICollection<T1> Keys => Forward.Keys;

    /// <inheritdoc/>
    public ICollection<T2> Values => Forward.Values;

    /// <inheritdoc/>
    IEnumerable<T1> IReadOnlyDictionary<T1, T2>.Keys => Forward.Keys;

    /// <inheritdoc/>
    IEnumerable<T2> IReadOnlyDictionary<T1, T2>.Values => Forward.Values;

    /// <summary>
    /// Initializes a new instance of the ValueMap class that is empty.
    /// </summary>
    public ValueMap()
    {
        Forward = [];
        Backward = [];
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator() => Forward.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => Forward.GetEnumerator();

    /// <inheritdoc/>
    public void Add(KeyValuePair<T1, T2> item) => Add(item.Key, item.Value);

    /// <inheritdoc/>
    public void Clear()
    {
        Forward.Clear();
        Backward.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<T1, T2> item) => Forward.Contains(item);

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex) => ((ICollection<KeyValuePair<T1, T2>>)Forward).CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<T1, T2> item) => Forward.Remove(item.Key) && Backward.Remove(item.Value);

    /// <inheritdoc/>
    public void Add(T1 key, T2 value)
    {
        if (Forward.ContainsKey(key) || Backward.ContainsKey(value))
            throw new ArgumentException("Key or value already in map");

        Forward.Add(key, value);
        Backward.Add(value, key);
    }

    /// <inheritdoc cref="IDictionary{TKey,TValue}.ContainsKey"/>
    public bool ContainsKey(T1 key) => Forward.ContainsKey(key);

    /// <summary>
    /// Determines whether the ValueMap contains the specified value.
    /// </summary>
    /// <param name="value">The value to locate in the ValueMap.</param>
    /// <returns>true if the value exists; otherwise, false.</returns>
    /// <inheritdoc cref="ContainsKey"/>
    public bool ContainsValue(T2 value) => Backward.ContainsKey(value);

    /// <summary>
    /// Removes the value with the specified key from the ValueMap.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <param name="value">When this method returns, contains the value removed, if the operation was successful.</param>
    /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
    public bool Remove(T1 key, out T2 value) => Forward.Remove(key, out value) && Backward.Remove(value);

    /// <summary>
    /// Removes the key with the specified value from the ValueMap.
    /// </summary>
    /// <param name="value">The value of the element to remove.</param>
    /// <param name="key">When this method returns, contains the key removed, if the operation was successful.</param>
    /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
    public bool Remove(T2 value, out T1 key) => Backward.Remove(value, out key) && Forward.Remove(key);

    /// <inheritdoc/>
    public bool Remove(T1 key) => Forward.Remove(key, out var value) && Backward.Remove(value);

    /// <summary>
    /// Removes the element with the specified value from the ValueMap.
    /// </summary>
    /// <param name="value">The value of the element to remove.</param>
    /// <returns>true if the element is successfully removed; otherwise, false.</returns>
    public bool Remove(T2 value) => Backward.Remove(value, out var key) && Forward.Remove(key);

    /// <inheritdoc cref="IDictionary{TKey,TValue}.TryGetValue"/>
    public bool TryGetValue(T1 key, out T2 value) => Forward.TryGetValue(key, out value);

    /// <summary>
    /// Gets the key associated with the specified value.
    /// </summary>
    /// <param name="value">The value whose key to get.</param>
    /// <param name="key">When this method returns, the key associated with the specified value, if the value is found; otherwise, the default value for the type of the key parameter. This parameter is passed uninitialized.</param>
    /// <returns>true if the object that implements <see cref="IDictionary{T1, T2}"/> contains an element with the specified value; otherwise, false.</returns>
    public bool TryGetKey(T2 value, out T1 key) => Backward.TryGetValue(value, out key);

    /// <inheritdoc cref="IDictionary{TKey,TValue}.this"/>
    public T2 this[T1 key]
    {
        get => Forward[key];
        set
        {
            Forward[key] = value;
            Backward[value] = key;
        }
    }

    /// <summary>
    /// Gets or sets the key associated with the specified value.
    /// </summary>
    /// <param name="val">The value of the element to get or set.</param>
    /// <returns>The element with the specified value.</returns>
    public T1 this[T2 val]
    {
        get => Backward[val];
        set
        {
            Forward[value] = val;
            Backward[val] = value;
        }
    }
}