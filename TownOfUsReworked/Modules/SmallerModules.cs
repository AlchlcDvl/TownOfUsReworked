using System.Text.Json.Serialization;

namespace TownOfUsReworked.Modules;

public record class GenerationData(int Chance, LayerEnum ID, bool Unique);

public record class SummaryInfo(string PlayerName, string History, string CachedHistory);

public class PointInTime
{
    public readonly Vector3 Position;

    public PointInTime(Vector3 position) => Position = position;
}

public record class PlayerVersion(Version Version, Guid Guid)
{
    public bool GuidMatches => TownOfUsReworked.Core.ManifestModule.ModuleVersionId.Equals(Guid);
}

public class GitHubApiObject
{
    [JsonPropertyName("tag_name")]
    public string Tag { get; set; }
    [JsonPropertyName("assets")]
    public GitHubApiAsset[] Assets { get; set; }
}

public class GitHubApiAsset
{
    [JsonPropertyName("browser_download_url")]
    public string URL { get; set; }
}