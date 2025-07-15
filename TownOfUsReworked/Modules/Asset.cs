// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace TownOfUsReworked.Modules;

public abstract class Asset
{
    [JsonPropertyName("id")]
    public string ID;

    [JsonPropertyName("custom")]
    public bool IsCustom;
}

public sealed class DownloadableAsset : Asset
{
    [JsonPropertyName("hash")]
    public string Hash;
}

public sealed class BundleAsset : Asset
{
#if ANDROID
    [JsonPropertyName("andHash")]
#else
    [JsonPropertyName("pcHash")]
#endif
    public string Hash;

#if ANDROID
    [JsonPropertyName("pcHash")]
#else
    [JsonPropertyName("andHash")]
#endif
    public string OtherHash;
}