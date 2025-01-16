using System.Diagnostics.CodeAnalysis;

namespace TownOfUsReworked.Modules;

// Yoinked this lovely piece of code from Daemon at https://github.com/DaemonBeast/Mitochondria/blob/main/Mitochondria.Core/Utilities/Structures/Map.cs albeit with a few changes of my own
public class ValueMap<T1, T2> : IDictionary<T1, T2> where T1: notnull where T2 : notnull
{
    public ValueMap<T2, T1> Reverse { get; }

    private readonly Dictionary<T1, T2> Forward;
    private readonly Dictionary<T2, T1> Backward;

    public int Count => Forward.Count;
    public bool IsReadOnly => false;
    public ICollection<T1> Keys => Forward.Keys;
    public ICollection<T2> Values => Forward.Values;

    public ValueMap()
    {
        Forward = [];
        Backward = [];

        Reverse = new(this, Backward, Forward);
    }

    public ValueMap(IEnumerable<KeyValuePair<T1, T2>> collection)
    {
        Forward = collection.ToDictionary(p => p.Key, p => p.Value);
        Backward = collection.ToDictionary(p => p.Value, p => p.Key);

        Reverse = new(this, Backward, Forward);
    }

    private ValueMap(ValueMap<T2, T1> reverse, Dictionary<T1, T2> forward, Dictionary<T2, T1> back)
    {
        Reverse = reverse;

        Forward = forward;
        Backward = back;
    }

    public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator() => Forward.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Forward.GetEnumerator();

    public void Add(KeyValuePair<T1, T2> item) => Add(item.Key, item.Value);

    public void Clear()
    {
        Forward.Clear();
        Backward.Clear();
    }

    public bool Contains(KeyValuePair<T1, T2> item) => Forward.Contains(item);

    public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex) => ((ICollection<KeyValuePair<T1, T2>>)Forward).CopyTo(array, arrayIndex);

    public bool Remove(KeyValuePair<T1, T2> item) => Forward.Remove(item.Key) && Backward.Remove(item.Value);

    public void Add(T1 key, T2 value)
    {
        if (Forward.ContainsKey(key) || Backward.ContainsKey(value))
            throw new ArgumentException("Key or value already in map");

        Forward.Add(key, value);
        Backward.Add(value, key);
    }

    public bool ContainsKey(T1 key) => Forward.ContainsKey(key);

    public bool ContainsValue(T2 value) => Backward.ContainsKey(value);

    public bool Remove(T1 key) => Forward.TryGetValue(key, out var value) && Forward.Remove(key) && Backward.Remove(value);

    public bool Remove(T2 value) => Backward.TryGetValue(value, out var key) && Backward.Remove(value) && Forward.Remove(key);

    public bool TryGetValue(T1 key, [MaybeNullWhen(false)] out T2 value) => Forward.TryGetValue(key, out value);

    public bool TryGetKey(T2 value, [MaybeNullWhen(false)] out T1 key) => Backward.TryGetValue(value, out key);

    public T2 this[T1 key]
    {
        get => Forward[key];
        set
        {
            Forward[key] = value;
            Backward[value] = key;
        }
    }

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