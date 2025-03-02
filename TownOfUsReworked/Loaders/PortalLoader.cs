namespace TownOfUsReworked.Loaders;

public class PortalLoader : BaseDownloader
{
    protected override string DirectoryInfo => TownOfUsReworked.Portal;
    protected override string Manifest => "Portal";
    protected override string FileExtension => "png";

    protected override void LoadAsset(DownloadableAsset item, int i) => PortalPaths.Add(Path.Combine(DirectoryInfo, $"{item.ID}.png"));
}