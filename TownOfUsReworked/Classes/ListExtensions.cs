using Random = UnityEngine.Random;

namespace TownOfUsReworked.Classes
{
    public static class ListExtensions
    {
        public static void Shuffle<T>(this System.Collections.Generic.List<T> list)
        {
            var count = list.Count;
            var last = count - 1;

            for (var i = 0; i <= last; ++i)
            {
                var r = Random.Range(i, count);
                var tmp = list[i];
                list[i] = list[r];
                list[r] = tmp;
            }
        }

        public static T TakeFirst<T>(this System.Collections.Generic.List<T> list)
        {
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

        public static void Shuffle<T>(this Il2CppSystem.Collections.Generic.List<T> list)
        {
            var count = list.Count;
            var last = count - 1;

            for (var i = 0; i <= last; ++i)
            {
                var r = Random.Range(i, count);
                var tmp = list[i];
                list[i] = list[r];
                list[r] = tmp;
            }
        }

        public static T TakeFirst<T>(this Il2CppSystem.Collections.Generic.List<T> list)
        {
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

        public static void Replace<T>(this System.Collections.Generic.List<T> list, T item1, T item2)
        {
            var item = list.Find(x => x.GetType() == item1.GetType());

            if (item == null)
                return;

            var index = list.IndexOf(item1);
            list[index] = item2;
        }

        public static void RemoveRange<T>(this System.Collections.Generic.List<T> list, System.Collections.Generic.List<T> list2)
        {
            foreach (var item in list2)
            {
                if (list.Contains(item))
                    list.Remove(item);
            }
        }
    }
}