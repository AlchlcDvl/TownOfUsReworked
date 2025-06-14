// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace TownOfUsReworked.Modules;

public record struct PointInTime(Vector3 Position);

public sealed class GitHubApiObject
{
    [JsonPropertyName("tag_name")]
    public string Tag { get; set; }

    [JsonPropertyName("body")]
    public string Description { get; set; }

    [JsonPropertyName("assets")]
    public GitHubApiAsset[] Assets { get; set; }
}

public sealed class GitHubApiAsset
{
    [JsonPropertyName("browser_download_url")]
    public string URL { get; set; }
}

[Serializable]
public struct RoleOptionData(byte chance, byte count, bool unique, bool active, LayerEnum layer) : INetSerializable, INetDeserializable
{
    public RoleOptionData() : this(0, 0, false, false, LayerEnum.None) { }

    public byte Chance { get; set; } = chance;
    public byte Count { get; set; } = count;
    public bool Unique { get; set; } = unique;
    public bool Active { get; set; } = active;
    public LayerEnum ID { get; set; } = layer;

    public override readonly string ToString() => Join(',', Chance, Count, Unique, Active, ID);

    public readonly RoleOptionData Clone() => new(Chance, Count, Unique, Active, ID);

    public readonly bool IsActive(int? relatedCount = null) => ((Chance > 0 && IsClassic()) || (Active && IsAllAny()) || (IsList() && ListEntryOption.IsAdded(ID.CastToSlot()))) &&
        ID.IsValid(relatedCount);

    public readonly IEnumerable<byte> GetBytes() => [ Chance, Count, (byte)(Unique ? 1 : 0), (byte)(Active ? 1 : 0), (byte)ID ];

    public void FromBytes(RpcReader reader)
    {
        Chance = reader.ReadByte();
        Count = reader.ReadByte();
        Unique = reader.ReadBool();
        Active = reader.ReadBool();
        ID = (LayerEnum)reader.ReadByte();
    }

    public static RoleOptionData Parse(string input)
    {
        var parts = input.TrueSplit(',');
        return new(byte.Parse(parts[0]), byte.Parse(parts[1]), bool.Parse(parts[2]), bool.Parse(parts[3]), Enum.Parse<LayerEnum>(parts[4]));
    }
}

public readonly record struct LayerDictionaryEntry(Type LayerType, UColor Color, LayerEnum Layer, string Symbol = null)
{
    public string Name => TranslationManager.Translate($"Layer.{Layer}");
}

public readonly record struct FactionDictionaryEntry(Faction Faction, UColor Color)
{
    public string Name => TranslationManager.Translate($"Faction.{Faction}");
}

public sealed class Achievement(string name, bool unlocked = false, bool hidden = false, bool eog = false, string icon = "Placeholder")
{
    public string Name { get; } = name;
    public string Icon { get; } = icon;
    public bool Hidden { get; } = hidden;
    public bool EndOfGame { get; } = eog;
    public bool Unlocked { get; set; } = unlocked;
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
public sealed class SortedAttribute(int order) : Attribute
{
    public int Order { get; } = order;
}

public delegate bool WhereSelectFilter<in T1, T2>(T1 param, out T2 value);

// /// <summary>
// /// Wrapper to act as an out parameter for coroutines.
// /// </summary>
// /// <typeparam name="T">The type of the value being set.</typeparam>
// public sealed class Out<T>(T value = default)
// {
//     public T Value = value;
//
//     public static implicit operator T(Out<T> outVal) => outVal.Value;
//
//     public static implicit operator Out<T>(T val) => new(val);
// }

/// <summary>
/// A struct that signifies a pair of colors. Used to simplify some code regarding player material colors.
/// </summary>
/// <param name="color1">The first color.</param>
/// <param name="color2">The second color.</param>
public readonly struct ColorPair(UColor color1, UColor color2)
{
    public UColor Color1 { get; } = color1;
    public UColor Color2 { get; } = color2;

    // public UColor Lerp(float t) => UColor.Lerp(Color1, Color2, t);

    public static ColorPair Lerp(ColorPair a, ColorPair b, float t) => new(UColor.Lerp(a.Color1, b.Color1, t), UColor.Lerp(a.Color2, b.Color2, t));
}

public sealed class VersionData
{
    [JsonPropertyName("gameVers")]
    // ReSharper disable once CollectionNeverUpdated.Global
    public Dictionary<int, string> GameVersions { get; set; }

    [JsonPropertyName("modVers")]
    public string[] ModVersions { get; set; }
}