namespace TownOfUsReworked.Loaders;

public class BundleLoader : BaseDownloader
{
    protected override string DirectoryInfo => TownOfUsReworked.Bundles;
    protected override string Manifest => "Bundles";

    protected override IEnumerable<string> GenerateDownloadList(DownloadableAsset[] response, HashAlgorithm hasher) => response.Where(x => ShouldDownload(Path.Combine(DirectoryInfo, x.ID),
        x.Hash, hasher)).Select(x =>
#if ANDROID
            $"{x.ID}_android"
#else
            x.ID);
#endif

    protected override void LoadAsset(DownloadableAsset item, int i)
    {
        var bundle = LoadBundle(File.ReadAllBytes(Path.Combine(DirectoryInfo, item.ID)));
        Bundles[item.ID] = bundle;
        bundle.GetAllAssetNames().ForEach(x => AssetToBundle[x.SanitisePath()] = item.ID);
    }
}