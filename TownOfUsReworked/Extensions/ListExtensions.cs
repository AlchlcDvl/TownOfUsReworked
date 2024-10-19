namespace TownOfUsReworked.Extensions;

public static class ListExtensions
{
    public static void Shuffle<T>(this List<T> list)
    {
        if (list.Count is 1 or 0)
            return;

        for (var i = list.Count - 1; i > 0; --i)
        {
            var r = URandom.RandomRangeInt(0, i + 1);
            (list[r], list[i]) = (list[i], list[r]);
        }
    }

    public static T TakeFirst<T>(this List<T> list)
    {
        try
        {
            var item = list[0];
            list.RemoveAt(0);
            return item;
        }
        catch
        {
            return default;
        }
    }

    public static void AddRanges<T>(this List<T> main, params IEnumerable<T>[] items) => items.ForEach(main.AddRange);

    public static List<T> ToSystem<T>(this ISystem.List<T> list) => [ .. list.ToArray() ];

    public static ISystem.List<T> ToIl2Cpp<T>(this List<T> list)
    {
        var newList = new ISystem.List<T>();
        list.ForEach(newList.Add);
        return newList;
    }

    public static T Random<T>(this IEnumerable<T> enumerable, T defaultVal)
    {
        if (!enumerable.Any())
            return defaultVal;
        else
            return enumerable.Random();
    }

    public static T Random<T>(this IEnumerable<T> list, Func<T, bool> predicate, T defaultVal = default) => list.Where(predicate).Random(defaultVal);

    public static int Count<T>(this ISystem.List<T> list, Func<T, bool> predicate) => list.ToSystem().Count(predicate);

    public static bool Any<T>(this ISystem.List<T> list, Func<T, bool> predicate) => list.ToSystem().Any(predicate);

    public static bool All<T>(this ISystem.List<T> list, Func<T, bool> predicate) => list.ToSystem().All(predicate);

    public static void ForEach<T>(this ISystem.List<T> list, Action<T> action) => list.ToSystem().ForEach(action);

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
            action(item);
    }

    public static List<List<T>> Split<T>(this List<T> list, int splitCount)
    {
        var result = new List<List<T>>();
        var temp = new List<T>();

        foreach (var item in list)
        {
            temp.Add(item);

            if (temp.Count == splitCount)
            {
                result.Add(temp);
                temp = [];
            }
        }

        if (temp.Any())
            result.Add(temp);

        return result;
    }

    public static List<T> GetRandomRange<T>(this IEnumerable<T> list, int count)
    {
        var temp = new List<T>();

        if (list.Count() <= count)
            temp.AddRange(list);
        else
        {
            while (temp.Count < count)
                temp.Add(list.Random(x => !temp.Contains(x)));
        }

        temp.Shuffle();
        return temp;
    }

    public static bool AllAnyOrEmpty<T>(this IEnumerable<T> source, Func<T, bool> predicate, bool all = false) => !source.Any() || (all ? source.All(predicate) : source.Any(predicate));

    public static bool TryFinding<T>(this IEnumerable<T> source, Func<T, bool> predicate, out T value)
    {
        try
        {
            value = source.First(predicate);
            return true;
        }
        catch
        {
            value = default;
            return false;
        }
    }

    public static void AddMany<T>(this List<T> list, T item, int count)
    {
        while (count > 0)
        {
            list.Add(item);
            count--;
        }
    }

    public static bool ContainsAny<T>(this IEnumerable<T> source, T[] values) => values.Any(source.Contains);

    public static bool ContainsAnyKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey[] keys) => keys.Any(dict.ContainsKey);

    /*public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        var index = 0;

        foreach (var item in source)
        {
            if (predicate(item))
                return index;

            index++;
        }

        return -1;
    }

    public static List<List<T>> Split<T>(this List<T> list, Func<T, bool> splitCondition, bool includeSatisfier = true)
    {
        var result = new List<List<T>>();
        var temp = new List<T>();

        foreach (var item in list)
        {
            if (splitCondition(item))
            {
                if (includeSatisfier)
                    temp.Add(item);

                result.Add(temp);
                temp = [];
            }
            else
                temp.Add(item);
        }

        if (temp.Any())
            result.Add(temp);

        return result;
    }

    public static Dictionary<int, List<T>> SplitAndGetIndices<T>(this List<T> list, Func<T, int> predicate)
    {
        var result = new Dictionary<int, List<T>>();

        foreach (var item in list)
        {
            var index = predicate(item);

            if (!result.ContainsKey(index))
                result[index] = [];

            result[index].Add(item);
        }

        result.Values.ForEach(x => x.Shuffle());
        return result;
    }

    public static IEnumerable<T> Where<T>(this ISystem.List<T> list, Func<T, bool> predicate) => list.ToSystem().Where(predicate);

    public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> dict, Action<TKey, TValue> action) => dict.ToList().ForEach(pair => action(pair.Key, pair.Value));

    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        var num = 0;
        var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
            action(enumerator.Current, num++);

        enumerator.Dispose();
    }

    public static void Shuffle<T>(this ISystem.List<T> list)
    {
        if (list.Count is 1 or 0)
            return;

        for (var i = list.Count - 1; i > 0; --i)
        {
            var r = URandom.RandomRangeInt(0, i + 1);
            (list[r], list[i]) = (list[i], list[r]);
        }
    }

    public static T TakeFirst<T>(this ISystem.List<T> list) => list.ToSystem().TakeFirst();

    public static int RemoveAsInt<T>(this List<T> list, params T[] items)
    {
        var result = 0;

        foreach (var t in items)
        {
            if (list.Remove(t))
                result++;
        }

        return result;
    }

    public static bool RemoveAsBool<T>(this List<T> list, params T[] items)
    {
        var result = false;

        foreach (var t in items)
        {
            if (list.Remove(t))
                result = true;
        }

        return result;
    }

    public static int RemoveRange<T>(this List<T> list, IEnumerable<T> list2)
    {
        var result = 0;

        foreach (var item in list2)
        {
            if (list.Contains(item))
                result += list.RemoveAll(x => Equals(x, item));
        }

        return result;
    }

    public static int RemoveRanges<T>(this List<T> main, params IEnumerable<T>[] items)
    {
        var result = 0;
        items.ForEach(x => result += main.RemoveRange(x));
        return result;
    }

    public static bool Replace<T>(this List<T> list, T item1, T item2)
    {
        var contains = list.Contains(item1);

        if (contains)
        {
            var index = list.IndexOf(item1);
            list.Remove(item1);
            list.Insert(index, item2);
        }

        return contains;
    }

    public static bool ReplaceAll<T>(this List<T> list, T item1, T item2)
    {
        var contains = list.Contains(item1);

        if (contains)
        {
            var pos = 0;
            var clone = list.Clone();

            foreach (var item in clone)
            {
                if (Equals(item, item1))
                {
                    pos = list.IndexOf(item);
                    list.Remove(item);
                    list.Insert(pos, item2);
                }
            }
        }

        return contains;
    }

    public static T Random<T>(this ISystem.List<T> list, T defaultVal = default) => list.ToSystem().Random(defaultVal);

    public static T Random<T>(this ISystem.List<T> list, Func<T, bool> predicate, T defaultVal = default) => list.ToSystem().Random(predicate, defaultVal);

    public static T Find<T>(this ISystem.List<T> list, Predicate<T> predicate) => list.ToSystem().Find(predicate);

    public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> newValue) where TKey : notnull
    {
        if (dict.TryGetValue(key, out var value))
            return value;

        return dict[key] = newValue();
    }*/
}