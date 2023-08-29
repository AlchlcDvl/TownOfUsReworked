namespace TownOfUsReworked.Extensions;

[HarmonyPatch]
public static class ListExtensions
{
    public static void Shuffle<T>(this List<T> list)
    {
        if (list.Count is 1 or 0)
            return;

        var count = list.Count;

        for (var i = 0; i <= count - 1; ++i)
        {
            var r = URandom.RandomRangeInt(i, count);
            (list[r], list[i]) = (list[i], list[r]);
        }
    }

    public static void Shuffle<T>(this ISystem.List<T> list)
    {
        if (list.Count is 1 or 0)
            return;

        var count = list.Count;

        for (var i = 0; i <= count - 1; ++i)
        {
            var r = URandom.RandomRangeInt(i, count);
            (list[r], list[i]) = (list[i], list[r]);
        }
    }

    public static T TakeFirst<T>(this List<T> list)
    {
        if (list.Count == 0)
            return default;

        list.Shuffle();
        var item = list[0];

        while (item == null)
        {
            list.Shuffle();
            item = list[0];
        }

        list.RemoveAt(0);
        list.Shuffle();
        return item;
    }

    public static T TakeFirst<T>(this ISystem.List<T> list) => list.Il2CppToSystem().TakeFirst();

    public static void RemoveRange<T>(this List<T> list, List<T> list2)
    {
        foreach (var item in list2)
        {
            if (list.Contains(item))
                list.Remove(item);
        }
    }

    public static void AddRanges<T>(this List<T> main, params List<T>[] items) => items.ToList().ForEach(main.AddRange);

    public static void RemoveRanges<T>(this List<T> main, params List<T>[] items) => items.ToList().ForEach(main.RemoveRange);

    public static bool Replace<T>(this List<T> list, T item1, T item2)
    {
        var contains = false;

        if (list.Contains(item1))
        {
            var index = list.IndexOf(item1);
            list.Remove(item1);
            list.Insert(index, item2);
            contains = true;
        }

        return contains;
    }

    public static List<T> Il2CppToSystem<T>(this ISystem.List<T> list) => list.ToArray().ToList();

    public static ISystem.List<T> SystemToIl2Cpp<T>(this List<T> list)
    {
        var newList = new ISystem.List<T>();
        list.ForEach(newList.Add);
        return newList;
    }

    public static T Random<T>(this List<T> list, T defaultVal = default)
    {
        if (list.Count == 0)
            return defaultVal;
        else if (list.Count == 1)
            return list[0];
        else
            return list[URandom.RandomRangeInt(0, list.Count)];
    }

    public static T Random<T>(this List<T> list, Func<T, bool> predicate, T defaultVal = default) => list.Where(predicate).ToList().Random(defaultVal);

    public static int Count<T>(this ISystem.List<T> list, Func<T, bool> predicate) => list.Il2CppToSystem().Count(predicate);

    public static bool Any<T>(this ISystem.List<T> list, Func<T, bool> predicate) => list.Il2CppToSystem().Any(predicate);

    public static bool All<T>(this ISystem.List<T> list, Func<T, bool> predicate) => list.Il2CppToSystem().All(predicate);

    public static IEnumerable<T> Where<T>(this ISystem.List<T> list, Func<T, bool> predicate) => list.Il2CppToSystem().Where(predicate);

    public static void ForEach<T>(this ISystem.List<T> list, Action<T> action) => list.Il2CppToSystem().ForEach(action);

    public static T Random<T>(this ISystem.List<T> list, T defaultVal = default) => list.Il2CppToSystem().Random(defaultVal);

    public static T Random<T>(this ISystem.List<T> list, Func<T, bool> predicate, T defaultVal = default) => list.Il2CppToSystem().Random(predicate, defaultVal);

    public static T Find<T>(this ISystem.List<T> list, Predicate<T> predicate) => list.Il2CppToSystem().Find(predicate);
}