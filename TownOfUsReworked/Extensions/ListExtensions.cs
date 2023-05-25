namespace TownOfUsReworked.Extensions
{
    [HarmonyPatch]
    public static class ListExtensions
    {
        public static void Shuffle<T>(this List<T> list)
        {
            var count = list.Count;
            var last = count - 1;

            for (var i = 0; i <= last; ++i)
            {
                var r = URandom.RandomRangeInt(i, count);
                (list[r], list[i]) = (list[i], list[r]);
            }
        }

        public static T TakeFirst<T>(this List<T> list)
        {
            var item = list[0];

            while (item == null)
            {
                list.Shuffle();
                item = list[0];
            }

            list.RemoveAt(0);
            return item;
        }

        public static void RemoveRange<T>(this List<T> list, List<T> list2)
        {
            foreach (var item in list2)
            {
                if (list.Contains(item))
                    list.Remove(item);
            }
        }

        public static List<T> Il2CppToSystem<T>(this Il2CppSystem.Collections.Generic.List<T> list) => list.ToArray().ToList();

        public static Il2CppSystem.Collections.Generic.List<T> SystemToIl2Cpp<T>(this List<T> list)
        {
            var newList = new Il2CppSystem.Collections.Generic.List<T>();

            foreach (var item in list)
                newList.Add(item);

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

        public static int Count<T>(this Il2CppSystem.Collections.Generic.List<T> list, Func<T, bool> predicate) => list.Il2CppToSystem().Count(predicate);

        public static bool Any<T>(this Il2CppSystem.Collections.Generic.List<T> list, Func<T, bool> predicate) => list.Il2CppToSystem().Any(predicate);

        public static IEnumerable<T> Where<T>(this Il2CppSystem.Collections.Generic.List<T> list, Func<T, bool> predicate) => list.Il2CppToSystem().Where(predicate);

        public static void ForEach<T>(this Il2CppSystem.Collections.Generic.List<T> list, Action<T> action) => list.Il2CppToSystem().ForEach(action);
    }
}