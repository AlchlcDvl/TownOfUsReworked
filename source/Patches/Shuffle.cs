using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs
{
    public static class ListExtensions
    {
        public static void Shuffle<T>(this List<T> list)
        {
            var count = list.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = Random.Range(i, count);
                var tmp = list[i];
                list[i] = list[r];
                list[r] = tmp;
            }
        }

        public static T TakeFirst<T>(this List<T> list)
        {
            var item = list[0];
            list.RemoveAt(0);
            return item;
        }

        public static T Ability<T>(this List<T> list)
        {
            var item = list[0];
            return item;
        }
    }
}