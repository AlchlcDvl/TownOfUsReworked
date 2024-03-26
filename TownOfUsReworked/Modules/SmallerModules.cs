namespace TownOfUsReworked.Modules;

public record class GenerationData(int Chance, LayerEnum ID, bool Unique);

public record class SummaryInfo(string PlayerName, string History, string CachedHistory);

public readonly struct PointInTime(Vector3 position)
{
    public readonly Vector3 Position { get; } = position;
}

public record class PlayerVersion(Version Version, bool Dev, int DevBuild, bool Stream, Guid Guid, string VersionString)
{
    public bool GuidMatches => TownOfUsReworked.Core.ManifestModule.ModuleVersionId.Equals(Guid);
    public bool DevMatches => Dev == TownOfUsReworked.IsDev;
    public bool StreamMatches => Stream == TownOfUsReworked.IsStream;
    public bool DevBuildMatches => DevBuild == TownOfUsReworked.DevBuild;
    public bool VersionStringMatches => VersionString.Replace("_test", "") == TownOfUsReworked.VersionFinal.Replace("_test", "");
    public bool VersionMatches => Diff == 0;
    public int Diff => TownOfUsReworked.Version.CompareTo(Version);
    public bool EverythingMatches => GuidMatches && DevMatches && DevBuildMatches && StreamMatches && VersionStringMatches && VersionMatches;
}

public class GitHubApiObject
{
    [JsonPropertyName("tag_name")]
    public string Tag { get; set; }

    [JsonPropertyName("body")]
    public string Description { get; set; }

    [JsonPropertyName("assets")]
    public List<GitHubApiAsset> Assets { get; set; }
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