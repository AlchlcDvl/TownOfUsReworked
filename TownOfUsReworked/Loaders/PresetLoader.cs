namespace TownOfUsReworked.Loaders;

public class PresetLoader : AssetLoader<Asset>
{
    public override string DirectoryInfo => TownOfUsReworked.Options;
    public override bool Downloading => true;
    public override string Manifest => "Presets";
    public override string FileExtension => "txt";

    public static PresetLoader Instance { get; set; }

    public override IEnumerator BeginDownload(object response)
    {
        var mainResponse = (List<Asset>)response;
        Message($"Found {mainResponse.Count} presets");
        var toDownload = mainResponse.Select(x => x.ID);
        Message($"Downloading {toDownload.Count()} presets");
        yield return CoDownloadAsset(toDownload);
        mainResponse.Clear();
    }
}