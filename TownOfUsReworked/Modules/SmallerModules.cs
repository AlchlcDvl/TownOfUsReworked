// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace TownOfUsReworked.Modules;

public record SummaryInfo(string PlayerName, string History, string CachedHistory);

public readonly struct PointInTime(Vector3 position)
{
    public Vector3 Position { get; } = position;
}

public class GitHubApiObject
{
    [JsonPropertyName("tag_name")]
    public string Tag { get; set; }

    [JsonPropertyName("body")]
    public string Description { get; set; }

    [JsonPropertyName("assets")]
    public GitHubApiAsset[] Assets { get; set; }
}

public class GitHubApiAsset
{
    [JsonPropertyName("browser_download_url")]
    public string URL { get; set; }
}

public class Asset
{
    [JsonPropertyName("id")]
    public string ID { get; set; }
}

public class DownloadableAsset : Asset
{
    [JsonPropertyName("hash")]
    public string Hash { get; set; }
}

[Serializable]
public struct RoleOptionData(int chance, int count, bool unique, bool active, LayerEnum layer)
{
    public int Chance { get; set; } = chance;
    public int Count { get; set; } = count;
    public bool Unique { get; set; } = unique;
    public bool Active { get; set; } = active;
    public LayerEnum ID { get; set; } = layer;

    public override readonly string ToString() => $"{Chance},{Count},{Unique},{Active},{ID}";

    public static RoleOptionData Parse(string input)
    {
        var parts = input.TrueSplit(',');
        return new(int.Parse(parts[0]), int.Parse(parts[1]), bool.Parse(parts[2]), bool.Parse(parts[3]), Enum.Parse<LayerEnum>(parts[4]));
    }

    public readonly RoleOptionData Clone() => new(Chance, Count, Unique, Active, ID);

    public readonly bool IsActive(int? relatedCount = null) => ((Chance > 0 && IsClassic()) || (Active && IsAllAny()) || (IsRoleList() && ListEntryAttribute.IsAdded(ID))) &&
        ID.IsValid(relatedCount);

    public readonly byte[] Serialize() => [ (byte)Chance, (byte)(Chance >> 8), (byte)(Chance >> 16), (byte)(Chance >> 24), (byte)Count, (byte)(Count >> 8), (byte)(Count >> 16), (byte)(Count >>
        24), (byte)(Unique ? 1 : 0), (byte)(Active ? 1 : 0), (byte)ID ];

    public static RoleOptionData Deserialize(byte[] data) => new(BitConverter.ToInt32(data, 0), BitConverter.ToInt32(data, 4), BitConverter.ToBoolean(data, 8), BitConverter.ToBoolean(data, 9),
        (LayerEnum)data[^1]);
}

public record LayerDictionaryEntry(Type LayerType, UColor Color, LayerEnum Layer)
{
    public string Name => TranslationManager.Translate($"Layer.{Layer}");
}

public class Achievement(string name, bool unlocked = false, bool hidden = false, bool eog = false, string icon = "Placeholder")
{
    public string Name { get; } = name;
    public bool Hidden { get; } = hidden;
    public bool EndOfGame { get; } = eog;
    public string Icon { get; } = icon;
    public bool Unlocked { get; set; } = unlocked;
}