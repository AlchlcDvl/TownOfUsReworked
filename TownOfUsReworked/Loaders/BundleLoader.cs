namespace TownOfUsReworked.Loaders;

public class BundleLoader : AssetLoader<DownloadableAsset>
{
    public override string DirectoryInfo => TownOfUsReworked.Bundles;
    public override bool Downloading => true;
    public override string Manifest => "Bundles";

    public static BundleLoader Instance { get; set; }

    public override IEnumerator BeginDownload(DownloadableAsset[] response) => CoDownloadAssets(response.Where(x => ShouldDownload(Path.Combine(DirectoryInfo, x.ID), x.Hash)).Select(x => x.ID));

    public override IEnumerator LoadAssets(DownloadableAsset[] response)
    {
        Message($"Found {response.Length} bundles");
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var asset = response[i];
            var bundle = LoadBundle(File.ReadAllBytes(Path.Combine(DirectoryInfo, asset.ID)));
            Bundles[asset.ID] = bundle;
            bundle.AllAssetNames().ForEach(x => AssetToBundle[ConvertToBaseName(x)] = asset.ID);
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Loading Assets ({i + 1}/{response.Length})");
                yield return EndFrame();
            }
        }
    }

    public override IEnumerator GenerateHashes(DownloadableAsset[] response)
    {
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var bundle = response[i];
            bundle.Hash = GenerateHash(Path.Combine(DirectoryInfo, bundle.ID));
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Generating Bundle Hashes ({i + 1}/{response.Length})");
                yield return EndFrame();
            }
        }
    }

    private static string ConvertToBaseName(string name) => name.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Last().Split('.',
        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).First();
}