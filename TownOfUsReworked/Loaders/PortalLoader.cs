namespace TownOfUsReworked.Loaders;

public sealed class PortalLoader() : BaseDownloader(TownOfUsReworked.Portal, "Portal", "png")
{
    protected override void LoadAsset(DownloadableAsset item, int i) => PortalPaths.Add(Path.Combine(DirectoryInfo, $"{item.ID}.png"));
}