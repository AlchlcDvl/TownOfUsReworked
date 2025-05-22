// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace TownOfUsReworked.Modules;

public abstract class Asset
{
    [JsonPropertyName("id")]
    public string ID { get; set; }

    [JsonPropertyName("custom")]
    public bool IsCustom { get; set; }
}

[JsonSerializable(typeof(DownloadableAsset))]
public sealed class DownloadableAsset : Asset
{
    [JsonPropertyName("hash")]
    public string Hash { get; set; }
}

[JsonSerializable(typeof(BundleAsset))]
public sealed class BundleAsset : Asset
{
#if ANDROID
    [JsonPropertyName("andHash")]
#else
    [JsonPropertyName("pcHash")]
#endif
    public string Hash { get; set; }

#if ANDROID
    [JsonPropertyName("pcHash")]
#else
    [JsonPropertyName("andHash")]
#endif
    public string OtherHash { get; set; }
}