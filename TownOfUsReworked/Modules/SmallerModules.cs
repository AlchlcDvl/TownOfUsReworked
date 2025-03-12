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
}

public sealed class DownloadableAsset : Asset
{
    [JsonPropertyName("hash")]
    public string Hash { get; set; }
}

[Serializable]
public struct RoleOptionData(byte chance, byte count, bool unique, bool active, LayerEnum layer)
{
    public byte Chance { get; set; } = chance;
    public byte Count { get; set; } = count;
    public bool Unique { get; set; } = unique;
    public bool Active { get; set; } = active;
    public LayerEnum ID { get; set; } = layer;

    public override readonly string ToString() => Join(',', Chance, Count, Unique, Active, ID);

    public static RoleOptionData Parse(string input)
    {
        var parts = input.TrueSplit(',');
        return new(byte.Parse(parts[0]), byte.Parse(parts[1]), bool.Parse(parts[2]), bool.Parse(parts[3]), Enum.Parse<LayerEnum>(parts[4]));
    }

    public readonly RoleOptionData Clone() => new(Chance, Count, Unique, Active, ID);

    public readonly bool IsActive(int? relatedCount = null) => ((Chance > 0 && IsClassic()) || (Active && IsAllAny()) || (IsRoleList() && ListEntryAttribute.IsAdded(ID.CastToSlot()))) &&
        ID.IsValid(relatedCount);

    public readonly void Serialize(MessageWriter writer)
    {
        writer.Write(Chance);
        writer.Write(Count);
        writer.Write(Unique);
        writer.Write(Active);
        writer.Write(ID);
    }

    public static RoleOptionData Deserialize(MessageReader reader) => new(reader.ReadByte(), reader.ReadByte(), reader.ReadBoolean(), reader.ReadBoolean(), reader.ReadEnum<LayerEnum>());
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

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class SortedAttribute(int order) : Attribute
{
    public int Order { get; } = order;
}