namespace TownOfUsReworked.Loaders;

public class PresetLoader : BaseDownloader
{
    protected override string DirectoryInfo => TownOfUsReworked.Options;
    protected override string Manifest => "Presets";
    protected override string FileExtension => "txt";
}