namespace TownOfUsReworked.Loaders;

public sealed class BundleLoader : AssetLoader<BundleAsset>
{
    private static readonly int GlitchTimer = Shader.PropertyToID("_GlitchTimer");

    protected override string DirectoryInfo => TownOfUsReworked.Bundles;
    protected override string Manifest => "Bundles";
    protected override bool Downloading => true;
    protected override string FileExtension => "bundle_" +
#if ANDROID
        "android";
#else
        "pc";
#endif

    private const string OtherFileExtension = "bundle_" +
#if ANDROID
        "pc";
#else
        "android";
#endif

    protected override IEnumerable<string> GenerateDownloadList(BundleAsset[] response, HashAlgorithm hasher) => response.Where(x => !x.IsCustom && ShouldDownload(Path.Combine(DirectoryInfo,
        $"{x.ID}.{FileExtension}"), x.Hash, hasher)).Select(x => x.ID);

    protected override void GenerateHash(BundleAsset item, HashAlgorithm hasher)
    {
        item.Hash = GenerateHash(Path.Combine(DirectoryInfo, $"{item.ID}.{FileExtension}"), hasher);
        item.OtherHash = GenerateHash(Path.Combine(DirectoryInfo, $"{item.ID}.{OtherFileExtension}"), hasher);
    }

    protected override void LoadAsset(BundleAsset item, int i)
    {
        var bundle = LoadBundle(File.ReadAllBytes(Path.Combine(DirectoryInfo, $"{item.ID}.{FileExtension}")));
        Bundles[item.ID] = bundle;
        bundle.GetAllAssetNames().Do(x => AssetToBundle[x.SanitisePath()] = item.ID);
    }

    protected override void AfterLoading(List<BundleAsset> response) => (UpdateSplashPatch.Rend.material = new(GetMaterial("GlitchedMaterial"))).SetFloat(GlitchTimer, 1);
}