namespace TownOfUsReworked.Loaders;

public class ImageLoader : BaseDownloader
{
    protected override string DirectoryInfo => TownOfUsReworked.Images;
    protected override string Manifest => "Images";
    protected override string FileExtension => "png";
}