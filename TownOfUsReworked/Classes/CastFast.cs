using System.Linq.Expressions;

namespace TownOfUsReworked.Classes
{
    public static class FastCast
    {
        private static class CastHelper<T> where T : Il2CppObjectBase
        {
            public static Func<IntPtr, T> Cast;

            static CastHelper()
            {
                var constructor = typeof(T).GetConstructor(new[] { typeof(IntPtr) });
                var ptr = Expression.Parameter(typeof(IntPtr));
                var create = Expression.New(constructor!, ptr);
                var lambda = Expression.Lambda<Func<IntPtr, T>>(create, ptr);
                Cast = lambda.Compile();
            }
        }

        public static T CastFast<T>(this Il2CppObjectBase obj) where T : Il2CppObjectBase
        {
            if (obj is T casted)
                return casted;

            return obj.Pointer.CastFast<T>();
        }

        public static T CastFast<T>(this IntPtr ptr) where T : Il2CppObjectBase => CastHelper<T>.Cast(ptr);
    }
}