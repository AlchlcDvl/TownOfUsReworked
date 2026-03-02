using System.Linq.Expressions;

namespace TownOfUsReworked.Pooling;

public static class RecyclePool<T> where T : class, IPoolable, new()
{
    private static readonly Queue<T> Pool = new();
    private static readonly Func<T> Create = CreateFactory();

    public static T Borrow()
    {
        var result = Pool.TryDequeue(out var item) ? item : Create();
        result.IsPooled = false;
        return result;
    }

    public static void Return(T item)
    {
        if (item == null)
            return;

        if (item.IsPooled)
            throw new InvalidOperationException("Item is already pooled!");

        item.Recycle();
        item.IsPooled = true;
        Pool.Enqueue(item);
    }

    private static Func<T> CreateFactory()
    {
        var newExp = Expression.New(typeof(T));
        var lambda = Expression.Lambda<Func<T>>(newExp);
        return lambda.Compile();
    }
}