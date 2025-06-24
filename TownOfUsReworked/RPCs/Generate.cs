namespace TownOfUsReworked;

public static partial class Generate
{
    public static ValueMap<byte, Type> TypeIdMap = [];

    private static void GenerateIDs()
    {
        byte id = 0;

        // C# primitive types (these never change...hopefully)
        var primitives = new[] { typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(Half), typeof(float), typeof(double),
            typeof(decimal), typeof(bool), typeof(char), typeof(string), typeof(Enum), typeof(Type), typeof(IEnumerable) };

        foreach (var type in primitives)
            TypeIdMap[id++] = type;

        // Base game types (ideally this array also never changes, unless I want to start serialising another base game type)
        var gameTypes = new[] { typeof(PlayerControl), typeof(DeadBody), typeof(Vent), typeof(PlayerVoteArea), typeof(Vector2), typeof(Color32) };

        foreach (var type in gameTypes)
            TypeIdMap[id++] = type;

        // I don't actually know if using typeof introduces overhead by being repeatedly used in a delegate, but I'd rather be safe than sorry lol
        var iNetSerializable = typeof(INetSerializable);
        var objectType = typeof(object);

        var relevantTypes = AccessTools.GetTypesFromAssembly(TownOfUsReworked.Core)
            .Where(x => iNetSerializable.IsAssignableFrom(x) && (x.BaseType == objectType || x.IsValueType) && !x.IsEnum)
            .OrderBy(x => x.Name);

        foreach (var type in relevantTypes)
            TypeIdMap[id++] = type;
    }

    // Serialisation helpers
    private const float MinPos = -50f;
    private const float MaxPos = +50f;
    private const float Diff = MaxPos - MinPos;

    public static float ReverseLerp(float t) => Mathf.Clamp01((t - MinPos) / Diff);

    public static float Lerp(float t) => Mathf.Lerp(MinPos, MaxPos, t);
}