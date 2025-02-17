namespace TownOfUsReworked.Modules;

[Serializable]
public class MultiSelectValue<T>(params T[] values) : IList<T>, IList, IReadOnlyList<T>, IEquatable<MultiSelectValue<T>> where T : Enum
{
    private readonly List<T> ValueList = [ .. values ];

    public string Values => string.Join(',', ValueList);
    public int Count => ValueList.Count;
    public bool IsReadOnly => false;
    public bool IsFixedSize => ((IList)ValueList).IsFixedSize;
    public bool IsSynchronized => ((ICollection)ValueList).IsSynchronized;
    public object SyncRoot => ((ICollection)ValueList).SyncRoot;

    public static implicit operator string(MultiSelectValue<T> value) => value.Values;

    public static implicit operator List<T>(MultiSelectValue<T> value) => value.ValueList;

    public static implicit operator T[](MultiSelectValue<T> value) => [ .. value.ValueList ];

    public static implicit operator T(MultiSelectValue<T> value)
    {
        if (value.Count == 1)
            return value[0];

        throw new($"Tried to convert multiple or no values ({value.Values}) to a singular one");
    }

    public static implicit operator MultiSelectValue<T>(T value) => new(value);

    public static implicit operator MultiSelectValue<T>(T[] values) => [ .. values ];

    public static implicit operator MultiSelectValue<T>(List<T> values) => [ .. values ];

    public static implicit operator MultiSelectValue<T>(string values)
    {
        var tType = typeof(T);

        if (tType == typeof(Enum))
            throw new("Tried to set values without setting an explicit enum type");

        return [ .. values.TrueSplit(',').Select(x => (T)Enum.Parse(tType, x)) ];
    }

    public T this[int index]
    {
        get => ValueList[index];
        set => ValueList[index] = value;
    }

    object IList.this[int index]
    {
        get => this[index];
        set => this[index] = (T)value;
    }

    public void Add(T item) => ValueList.Add(item);

    public void Clear() => ValueList.Clear();

    public bool Contains(T item) => ValueList.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => ValueList.CopyTo(array, arrayIndex);

    public int IndexOf(T item) => ValueList.IndexOf(item);

    public void Insert(int index, T item) => ValueList.Insert(index, item);

    public bool Remove(T item) => ValueList.Remove(item);

    public void RemoveAt(int index) => ValueList.RemoveAt(index);

    public int Add(object value) => ((IList)ValueList).Add(value);

    public bool Contains(object value) => ((IList)ValueList).Contains(value);

    public int IndexOf(object value) => ((IList)ValueList).IndexOf(value);

    public void Insert(int index, object value) => ((IList)ValueList).Insert(index, value);

    public void Remove(object value) => ((IList)ValueList).Remove(value);

    public void CopyTo(Array array, int index) => ((ICollection)ValueList).CopyTo(array, index);

    public int RemoveAll(Predicate<T> predicate) => ValueList.RemoveAll(predicate);

    public override string ToString() => Values;

    public IEnumerator<T> GetEnumerator() => ValueList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ValueList.GetEnumerator();

    public bool Equals(MultiSelectValue<T> other) => Equals(ValueList, other?.ValueList);

    public override bool Equals(object obj) => obj is MultiSelectValue<T> other && Equals(other);

    public override int GetHashCode() =>  ValueList?.GetHashCode() ?? 0;

    public static bool operator ==(MultiSelectValue<T> left, MultiSelectValue<T> right) => left?.Equals(right) == true;

    public static bool operator !=(MultiSelectValue<T> left, MultiSelectValue<T> right) => left?.Equals(right) == false;

    public static MultiSelectValue<T> operator +(MultiSelectValue<T> left, MultiSelectValue<T> right)
    {
        left.ValueList.AddRange(right.ValueList);
        return left;
    }

    public static MultiSelectValue<T> operator -(MultiSelectValue<T> left, MultiSelectValue<T> right)
    {
        left.ValueList.RemoveRange(right.ValueList);
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

    private interface IYes
    {
        public static bool No() => false;
    }
}