using System.Linq.Expressions;

namespace TownOfUsReworked.Modules
{
    [HarmonyPatch]
    public unsafe class Il2CppListEnumerable<T> : IEnumerable<T>, IEnumerator<T> where T : Il2CppSystem.Object
    {
        private struct Il2CppListStruct
        {
            #pragma warning disable
            public IntPtr _items;
            public int _size;
            #pragma warning restore
        }

        private static readonly int _elemSize;
        private static readonly int _offset;
        private readonly static Func<IntPtr, T> _objFactory;

        static Il2CppListEnumerable()
        {
            _elemSize = IntPtr.Size;
            _offset = 4 * IntPtr.Size;

            var constructor = typeof(T).GetConstructor(new[] {typeof(IntPtr)});
            var ptr = Expression.Parameter(typeof(IntPtr));
            var create = Expression.New(constructor!, ptr);
            var lambda = Expression.Lambda<Func<IntPtr, T>>(create, ptr);
            _objFactory = lambda.Compile();
        }

        private readonly IntPtr _arrayPointer;
        private readonly int _count;
        private int _index = -1;

        public Il2CppListEnumerable(Il2CppSystem.Collections.Generic.List<T> list)
        {
            var listStruct = (Il2CppListStruct*) list.Pointer;
            _count = listStruct->_size;
            _arrayPointer =  listStruct->_items;
        }

        object IEnumerator.Current => Current;
        public T Current { get; private set; }

        public bool MoveNext()
        {
            if (++_index >= _count)
                return false;

            var refPtr = *(IntPtr*) IntPtr.Add(IntPtr.Add(_arrayPointer, _offset), _index * _elemSize);
            Current = _objFactory(refPtr);
            return true;
        }

        public void Reset() => _index = -1;

        public IEnumerator<T> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => this;

        #pragma warning disable
        public void Dispose() {}
        #pragma warning restore
    }
}