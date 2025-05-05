// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace TownOfUsReworked.Modules;

public record SummaryInfo(string PlayerName, string History, string CachedHistory);

public readonly struct PointInTime(Vector3 position)
{
    public Vector3 Position { get; } = position;
}

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

public abstract class Asset
{
    [JsonPropertyName("id")]
    public string ID { get; set; }

    [JsonPropertyName("custom")]
    public bool IsCustom { get; set; }
}

public sealed class DownloadableAsset : Asset
{
    [JsonPropertyName("hash")]
    public string Hash { get; set; }
}

[Serializable]
public struct RoleOptionData(byte chance, byte count, bool unique, bool active, LayerEnum layer) : INetSerializable
{
    public byte Chance { get; set; } = chance;
    public byte Count { get; set; } = count;
    public bool Unique { get; set; } = unique;
    public bool Active { get; set; } = active;
    public LayerEnum ID { get; set; } = layer;

    public override readonly string ToString() => Join(',', Chance, Count, Unique, Active, ID);

    public readonly RoleOptionData Clone() => new(Chance, Count, Unique, Active, ID);

    public readonly bool IsActive(int? relatedCount = null) => ((Chance > 0 && IsClassic()) || (Active && IsAllAny()) || (IsList() && ListEntryOption.IsAdded(ID.CastToSlot()))) &&
        ID.IsValid(relatedCount);

    public readonly byte[] ToBytes() => [ Chance, Count, (byte)(Unique ? 1 : 0), (byte)(Active ? 1 : 0), (byte)ID ];

    public static RoleOptionData Parse(string input)
    {
        var parts = input.TrueSplit(',');
        return new(byte.Parse(parts[0]), byte.Parse(parts[1]), bool.Parse(parts[2]), bool.Parse(parts[3]), Enum.Parse<LayerEnum>(parts[4]));
    }
}

public record LayerDictionaryEntry(Type LayerType, UColor Color, LayerEnum Layer)
{
    public string Name => TranslationManager.Translate($"Layer.{Layer}");
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

public delegate bool WhereSelectFilter<T1, T2>(T1 param, out T2 value);