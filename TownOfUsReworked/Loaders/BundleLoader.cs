namespace TownOfUsReworked.Loaders;

#if ANDROID
public sealed class BundleLoader() : BaseDownloader(TownOfUsReworked.Bundles, "Bundles", "bundle_android")
#else
public sealed class BundleLoader() : BaseDownloader(TownOfUsReworked.Bundles, "Bundles", "bundle_pc")
#endif
{
    private static readonly int GlitchTimer = Shader.PropertyToID("_GlitchTimer");

    protected override void LoadAsset(DownloadableAsset item, int i)
    {
        var bundle = LoadBundle(File.ReadAllBytes(Path.Combine(DirectoryInfo, $"{item.ID}.{FileExtension}")));
        Bundles[item.ID] = bundle;
        bundle.GetAllAssetNames().ForEach(x => AssetToBundle[x.SanitisePath()] = item.ID);
    }

    protected override void AfterLoading(List<DownloadableAsset> response)
    {
        var mat = UpdateSplashPatch.Rend.material = new(GetMaterial("GlitchedMaterial"));
        mat.SetFloat(GlitchTimer, 1);
    }
}