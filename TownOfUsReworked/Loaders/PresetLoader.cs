namespace TownOfUsReworked.Loaders;

public class PresetLoader : AssetLoader<Asset>
{
    public override string DirectoryInfo => TownOfUsReworked.Options;
    public override bool Downloading => true;
    public override string Manifest => "Presets";
    public override string FileExtension => "txt";

    public static PresetLoader Instance { get; set; }

    public override IEnumerator BeginDownload(Asset[] response)
    {
        Message($"Found {response.Length} presets");
        yield return CoDownloadAssets(response.Select(x => x.ID));
    }
}