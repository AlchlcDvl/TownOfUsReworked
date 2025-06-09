namespace TownOfUsReworked.Custom;

public abstract class CustomEnumInjector
{
    public static readonly Dictionary<Type, CustomEnumInjector> Injectors = [];
    protected static readonly object ThreadLock = new();

    public abstract Array Values();

    public abstract string ToString(Enum value);
}

/// <summary>
/// A wrapper class to help with injecting values to enum classes.
/// </summary>
/// <typeparam name="T">The relevant enum Type.</typeparam>
public sealed class EnumInjector<T> : CustomEnumInjector where T : struct, Enum
{
    private readonly List<T> AllValues;
    private readonly List<T> InjectedValues;
    private readonly ValueMap<string, T> NamedValues;
    private readonly ValueMap<dynamic, T> IndexedValues;

    private readonly object Lock;
    private readonly Type Type;
    private readonly bool Il2Cpp;

    private dynamic Last;
    private dynamic First;
    private dynamic Min;
    private dynamic Max;
    private bool MinReached;
    private bool MaxReached;

    public readonly T MaxPossibleValue;
    // public readonly T MinPossibleValue;

    public EnumInjector(bool il2Cpp = true)
    {
        Lock = new();
        Type = typeof(T);
        Il2Cpp = il2Cpp;

        if (Il2Cpp)
            EnumInjector.RegisterEnumInIl2Cpp<T>();

        var underlying = Enum.GetUnderlyingType(Type);
        AllValues = [.. Enum.GetValues<T>().OrderBy(x => x)];
        Last = Convert.ChangeType(AllValues[^1], underlying);
        First = Convert.ChangeType(AllValues[0], underlying);
        Min = Convert.ChangeType(AccessTools.Field(underlying, "MinValue").GetRawConstantValue(), underlying);
        Max = Convert.ChangeType(AccessTools.Field(underlying, "MaxValue").GetRawConstantValue(), underlying);
        MinReached = First == Min;
        MaxReached = Last == Max;
        MaxPossibleValue = (T)Enum.ToObject(Type, Max);
        // MinPossibleValue = (T)Enum.ToObject(Type, Min);
        InjectedValues = [];
        IndexedValues = [];
        NamedValues = [];

        foreach (var value in AllValues)
        {
            IndexedValues.Add(Convert.ChangeType(value, underlying), value);
            NamedValues.Add(EnumPatches.OriginalToString(value), value);
        }

        lock (ThreadLock)
        {
            Injectors[Type] = this;
        }
    }

    public override string ToString(Enum value)
    {
        if (value is not T enumVal)
            throw new InvalidOperationException();

        return NamedValues.TryGetKey(enumVal, out var name) ? name : EnumPatches.OriginalToString(value);
    }

    public override Array Values() => GetValues();

    private T[] GetValues() => [.. AllValues];

    private void InsertValue(T result, string value, dynamic index)
    {
        NamedValues[value] = result;
        IndexedValues[index] = result;

        var index1 = AllValues.BinarySearch(result);

        if (index1 < 0)
            index1 = ~index1;

        AllValues.Insert(index1, result);

        var index2 = InjectedValues.BinarySearch(result);

        if (index2 < 0)
            index2 = ~index2;

        InjectedValues.Insert(index2, result);
    }

    private string Diagnostic(object value) => $"Min = {Min}, Max = {Max}, First = {First}, Last = {Last}, Parameter = {value ?? "None"}";

    // public bool TryInjectAndReturn(string value, out T result, bool decrement = false)
    // {
    //     try
    //     {
    //         result = InjectAndReturn(value, decrement);
    //         return true;
    //     }
    //     catch
    //     {
    //         result = default;
    //         return false;
    //     }
    // }

    public T InjectAndReturn(string value, bool decrement = false)
    {
        lock (Lock)
        {
            if (NamedValues.ContainsKey(value))
                throw new ArgumentException($"{value} has already been injected. {Diagnostic(decrement)}");

            if ((MinReached && decrement) || (MaxReached && !decrement))
                throw new OverflowException($"Overflow for Type {Type.Name} detected for {value}. {Diagnostic(decrement)}");

            var index = decrement ? --First : ++Last;
            MinReached = First == Min;
            MaxReached = Last == Max;

            if (IndexedValues.ContainsKey(index))
                throw new ArgumentException($"{index} has already been injected. {Diagnostic(decrement)}");

            if (Il2Cpp)
                EnumInjector.InjectEnumValues<T>(new(){ { value, (object)index } });

            var result = (T)Enum.ToObject(Type, index);
            InsertValue(result, value, index);
            return result;
        }
    }

    // public bool TryInjectAndReturn(string[] values, out IEnumerable<T> result, bool[] decrements = null)
    // {
    //     try
    //     {
    //         result = InjectAndReturn(values, decrements);
    //         return true;
    //     }
    //     catch
    //     {
    //         result = null;
    //         return false;
    //     }
    // }
    //
    // private IEnumerable<T> InjectAndReturn(string[] values, bool[] decrements = null)
    // {
    //     lock (Lock)
    //     {
    //         decrements ??= new bool[values.Length];
    //
    //         if (decrements.Length != values.Length)
    //             Array.Resize(ref decrements, values.Length);
    //
    //         var valuesToAdd = new Dictionary<string, object>();
    //
    //         foreach (var (value, decrement) in (values, decrements).Zip())
    //         {
    //             if (NamedValues.ContainsKey(value) || valuesToAdd.ContainsKey(value))
    //             {
    //                 Warning($"{value} has already been injected, skipping injection... {Diagnostic(decrement)}");
    //                 continue;
    //             }
    //
    //             if ((MinReached && decrement) || (MaxReached && !decrement))
    //             {
    //                 Warning($"Overflow detected for {value}, skipping injection... {Diagnostic(decrement)}");
    //                 continue;
    //             }
    //
    //             var index = decrement ? --First : ++Last;
    //             MinReached = First == Min;
    //             MaxReached = Last == Max;
    //
    //             if (IndexedValues.ContainsKey(index))
    //             {
    //                 Warning($"{index} is already injected, skipping injection... {Diagnostic(decrement)}");
    //                 continue;
    //             }
    //
    //             valuesToAdd.Add(value, (object)index);
    //         }
    //
    //         EnumInjector.InjectEnumValues<T>(valuesToAdd);
    //
    //         foreach (var (value, index) in valuesToAdd)
    //         {
    //             var result = (T)Enum.ToObject(Type, index);
    //             InsertValue(result, value, index);
    //             yield return result;
    //         }
    //     }
    // }

    // public bool TryInjectAndReturn(string value, dynamic index , out T result)
    // {
    //     try
    //     {
    //         result = InjectAndReturn(value, index);
    //         return true;
    //     }
    //     catch
    //     {
    //         result = default;
    //         return false;
    //     }
    // }

    public T InjectAndReturn(string value, dynamic index)
    {
        lock (Lock)
        {
            if (NamedValues.ContainsKey(value))
                throw new ArgumentException($"{value} has already been injected. {Diagnostic(index)}");

            if (IndexedValues.ContainsKey(index))
                throw new ArgumentException($"{index} has already been injected. {Diagnostic(index)}");

            if (index < Min || index > Max)
                throw new OverflowException($"Overflow for Type {Type.Name} detected");

            EnumInjector.InjectEnumValues<T>(new(){ { value, (object)index } });
            var result = (T)Enum.ToObject(Type, index);
            InsertValue(result, value, index);
            return result;
        }
    }

    // public bool TryInjectAndReturn(string[] values, dynamic[] indices, out IEnumerable<T> result)
    // {
    //     try
    //     {
    //         result = InjectAndReturn(values, indices);
    //         return true;
    //     }
    //     catch
    //     {
    //         result = null;
    //         return false;
    //     }
    // }

    // private IEnumerable<T> InjectAndReturn(string[] values, dynamic[] indices)
    // {
    //     lock (Lock)
    //     {
    //         if (indices.Length != values.Length)
    //             throw new InvalidOperationException($"Not all string names have a corresponding index. {Diagnostic(null)}");
    //
    //         var valuesToAdd = new Dictionary<string, object>();
    //
    //         foreach (var (value, index) in (values, indices).Zip())
    //         {
    //             if (NamedValues.ContainsKey(value) || valuesToAdd.ContainsKey(value))
    //             {
    //                 Warning($"{value} has already been injected, skipping injection... {Diagnostic(index)}");
    //                 continue;
    //             }
    //
    //             if (IndexedValues.ContainsKey(index))
    //             {
    //                 Warning($"{index} is already injected, skipping injection... {Diagnostic(index)}");
    //                 continue;
    //             }
    //
    //             if (index < Min || index > Max)
    //             {
    //                 Warning($"Overflow detected for {value}, skipping injection... {Diagnostic(index)}");
    //                 continue;
    //             }
    //
    //             valuesToAdd.Add(value, (object)index);
    //         }
    //
    //         EnumInjector.InjectEnumValues<T>(valuesToAdd);
    //
    //         foreach (var (value, index) in valuesToAdd)
    //         {
    //             var result = (T)Enum.ToObject(Type, index);
    //             InsertValue(result, value, index);
    //             yield return result;
    //         }
    //     }
    // }
}