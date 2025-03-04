namespace TownOfUsReworked.Loaders;

#if ANDROID
public sealed class BundleLoader() : BaseDownloader(TownOfUsReworked.Bundles, "Bundles", "bundle_android")
#else
public sealed class BundleLoader() : BaseDownloader(TownOfUsReworked.Bundles, "Bundles", "bundle_pc")
#endif
{
    protected override void LoadAsset(DownloadableAsset item, int i)
    {
        var bundle = LoadBundle(File.ReadAllBytes(Path.Combine(DirectoryInfo, $"{item.ID}.{FileExtension}")));
        Bundles[item.ID] = bundle;
        bundle.GetAllAssetNames().ForEach(x => AssetToBundle[x.SanitisePath()] = item.ID);
    }
}