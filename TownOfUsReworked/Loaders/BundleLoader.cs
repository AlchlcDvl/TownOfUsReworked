namespace TownOfUsReworked.Loaders;

public class BundleLoader : AssetLoader<Asset>
{
    public override string DirectoryInfo => TownOfUsReworked.Misc;
    public override bool Downloading => true;
    public override string Manifest => "MiscAssets";

    public static BundleLoader Instance { get; set; }

    public override IEnumerator BeginDownload(object response)
    {
        var mainResponse = (List<Asset>)response;
        Message($"Found {mainResponse.Count} assets");
        var toDownload = mainResponse.Select(x => x.ID).Where(ShouldDownload);
        Message($"Downloading {toDownload.Count()} assets");
        yield return CoDownloadAsset(toDownload);
    }

    public override IEnumerator AfterLoading(object response)
    {
        var assets = (List<Asset>)response;
        var time = 0f;

        for (var i = 0; i < assets.Count; i++)
        {
            var asset = assets[i];
            var bundle = LoadBundle(File.ReadAllBytes(Path.Combine(DirectoryInfo, asset.ID)));
            Bundles[asset.ID] = bundle;
            bundle.AllAssetNames().ForEach(x => ObjectToBundle[ConvertToBaseName(x)] = asset.ID);
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Loading Assets ({i + 1}/{assets.Count})");
                yield return EndFrame();
            }
        }

        assets.Clear();
        yield break;
    }

    private static bool ShouldDownload(string id) => !File.Exists(Path.Combine(TownOfUsReworked.Misc, id));

    private static string ConvertToBaseName(string name) => name.Split('/').Last().Split('.').First();
}