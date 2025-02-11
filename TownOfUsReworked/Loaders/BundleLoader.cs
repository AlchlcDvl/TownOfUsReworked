namespace TownOfUsReworked.Loaders;

public class BundleLoader : AssetLoader<DownloadableAsset>
{
    protected override string DirectoryInfo => TownOfUsReworked.Bundles;
    protected override bool Downloading => true;
    protected override string Manifest => "Bundles";

#if ANDROID
    protected override IEnumerator BeginDownload(DownloadableAsset[] response, HashAlgorithm hasher) => CoDownloadAssets(response.Where(x => ShouldDownload(Path.Combine(DirectoryInfo, x.ID),
        x.Hash, hasher)).Select(x => $"{x.ID}_android"));
#else
    protected override IEnumerator BeginDownload(DownloadableAsset[] response, HashAlgorithm hasher) => CoDownloadAssets(response.Where(x => ShouldDownload(Path.Combine(DirectoryInfo, x.ID),
        x.Hash, hasher)).Select(x => x.ID));
#endif

    protected override IEnumerator LoadAssets(DownloadableAsset[] response)
    {
        Message($"Found {response.Length} bundles");
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var asset = response[i];
            var bundle = LoadBundle(File.ReadAllBytes(Path.Combine(DirectoryInfo, asset.ID)));
            Bundles[asset.ID] = bundle;
            bundle.GetAllAssetNames().ForEach(x => AssetToBundle[x.SanitisePath()] = asset.ID);
            time += Time.deltaTime;

            if (time < 1f)
                continue;

            time = 0f;
            UpdateSplashPatch.SetText($"Loading Assets ({i + 1}/{response.Length})");
            yield return EndFrame();
        }
    }

    protected override IEnumerator GenerateHashes(DownloadableAsset[] response, HashAlgorithm hasher)
    {
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var bundle = response[i];
            bundle.Hash = GenerateHash(Path.Combine(DirectoryInfo, bundle.ID), hasher);
            time += Time.deltaTime;

            if (time < 1f)
                continue;

            time = 0f;
            UpdateSplashPatch.SetText($"Generating Bundle Hashes ({i + 1}/{response.Length})");
            yield return EndFrame();
        }
    }
}