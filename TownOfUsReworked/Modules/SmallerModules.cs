namespace TownOfUsReworked.Modules;

public record class SummaryInfo(string PlayerName, string History, string CachedHistory);

public readonly struct PointInTime(Vector3 position)
{
    public readonly Vector3 Position { get; } = position;
}

public record class PlayerVersion(string Version, bool Dev, int DevBuild, bool Stream, Guid Guid, string VersionFinal, Version SVersion)
{
    public bool GuidMatches => TownOfUsReworked.Core.ManifestModule.ModuleVersionId.Equals(Guid);
    public bool DevMatches => Dev == TownOfUsReworked.IsDev;
    public bool StreamMatches => Stream == TownOfUsReworked.IsStream;
    public bool DevBuildMatches => DevBuild == TownOfUsReworked.DevBuild;
    public bool VersionStringMatches => VersionFinal.Replace("_test", "") == TownOfUsReworked.VersionFinal.Replace("_test", "");
    public bool VersionMatches => Diff == 0;
    public int Diff => TownOfUsReworked.Version.CompareTo(SVersion);
    public bool EverythingMatches => GuidMatches && DevMatches && DevBuildMatches && StreamMatches && VersionStringMatches && VersionMatches;
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

public class RoleOptionData(int chance, int count, bool unique, bool active, LayerEnum layer)
{
    public int Chance { get; set; } = chance;
    public int Count { get; set; } = count;
    public bool Unique { get; set; } = unique;
    public bool Active { get; set; } = active;
    public LayerEnum ID { get; set; } = layer;

    public override string ToString() => $"{Chance},{Count},{Unique},{Active},{ID}";

    public static RoleOptionData Parse(string input)
    {
        var parts = input.Split(',');
        return new(int.Parse(parts[0]), int.Parse(parts[1]), bool.Parse(parts[2]), bool.Parse(parts[3]), Enum.Parse<LayerEnum>(parts[4]));
    }

    public RoleOptionData Clone() => new(Chance, Count, Unique, Active, ID);

    public bool IsActive(int? relatedCount = null) => ((Chance > 0 && (IsClassic || IsCustom)) || (Active && IsAA)) && ID.IsValid(relatedCount);
}

public record class LayerDictionaryEntry(Type LayerType, UColor Color, string Name);

// public record class ValuePair(float FloatValue, int IntValue)
// {
//     public static implicit operator float(ValuePair valuePair) => valuePair.FloatValue;

//     public static implicit operator int(ValuePair valuePair) => valuePair.IntValue;
// }