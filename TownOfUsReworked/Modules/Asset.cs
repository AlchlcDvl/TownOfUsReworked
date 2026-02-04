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
    public string Hash;

    [JsonPropertyName("pcHash")]
    public string OtherHash;
#else
    [JsonPropertyName("pcHash")]
    public string Hash;

    [JsonPropertyName("andHash")]
    public string OtherHash;
#endif
}