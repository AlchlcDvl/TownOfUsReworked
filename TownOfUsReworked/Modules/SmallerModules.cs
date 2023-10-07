using System.Text.Json.Serialization;

namespace TownOfUsReworked.Modules;

public record GenerationData(int Chance, LayerEnum ID, bool Unique);

public record SummaryInfo(string PlayerName, string History, string CachedHistory);

public class PointInTime
{
    public readonly Vector3 Position;

    public PointInTime(Vector3 position) => Position = position;
}

public record PlayerVersion(Version Version, Guid Guid)
{
    public bool GuidMatches => TownOfUsReworked.Core.ManifestModule.ModuleVersionId.Equals(Guid);
}

public class GitHubApiObject
{
    [JsonPropertyName("tag_name")]
    public string tag_name { get; set; }
    [JsonPropertyName("assets")]
    public GitHubApiAsset[] assets { get; set; }
}

public class GitHubApiAsset
{
    [JsonPropertyName("browser_download_url")]
    public string browser_download_url { get; set; }
}