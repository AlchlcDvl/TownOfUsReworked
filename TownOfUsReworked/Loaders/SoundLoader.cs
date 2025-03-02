namespace TownOfUsReworked.Loaders;

public class SoundLoader : BaseDownloader
{
    protected override string DirectoryInfo => TownOfUsReworked.Sounds;
    protected override string Manifest => "Sounds";
    protected override string FileExtension => "wav";
}