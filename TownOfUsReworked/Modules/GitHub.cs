using System.Text.Json.Serialization;

namespace TownOfUsReworked.Modules
{
    public class GitHubApiObject
    {
        [JsonPropertyName("tag_name")]
        public string tag_name;
        [JsonPropertyName("assets")]
        public GitHubApiAsset[] assets;
    }

    public class GitHubApiAsset
    {
        [JsonPropertyName("browser_download_url")]
        public string browser_download_url;
    }
}