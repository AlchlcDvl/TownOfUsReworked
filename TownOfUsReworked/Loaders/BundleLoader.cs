namespace TownOfUsReworked.Loaders;

public class BundleLoader : AssetLoader<Asset>
{
    public override string DirectoryInfo => TownOfUsReworked.Misc;
    public override bool Downloading => true;
    public override string Manifest => "MiscAssets";

    public static BundleLoader Instance { get; set; }

    public override IEnumerator BeginDownload(Asset[] response) => CoDownloadAssets(response.Select(x => x.ID).Where(ShouldDownload));

    public override IEnumerator AfterLoading(Asset[] response)
    {
        Message($"Found {response.Length} bundles");
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var asset = response[i];
            var bundle = LoadBundle(File.ReadAllBytes(Path.Combine(DirectoryInfo, asset.ID)));
            Bundles[asset.ID] = bundle;
            bundle.AllAssetNames().ForEach(x => ObjectToBundle[ConvertToBaseName(x)] = asset.ID);
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Loading Assets ({i + 1}/{response.Length})");
                yield return EndFrame();
            }
        }
    }

    private static bool ShouldDownload(string id) => !File.Exists(Path.Combine(TownOfUsReworked.Misc, id));

    private static string ConvertToBaseName(string name) => name.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Last().Split('.',
        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).First();
}