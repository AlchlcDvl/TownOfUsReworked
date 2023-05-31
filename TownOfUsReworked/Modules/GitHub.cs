using System.Text.Json.Serialization;

namespace TownOfUsReworked.Modules
{
    /*[Serializable]
    public class GHAsset
    {
        public string name { get; set; }
        public int size { get; set; }
        public string browser_download_url { get; set; }
    }

    [Serializable]
    public class GHRelease
    {
        public string tag_name { get; set; }
        public string name { get; set; }
        public string body { get; set; }
        public GHAsset[] assets { get; set; }

        public override string ToString() => name;
    }*/

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