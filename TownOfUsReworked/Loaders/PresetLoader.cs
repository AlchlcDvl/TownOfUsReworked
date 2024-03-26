namespace TownOfUsReworked.Loaders;

public class PresetLoader : AssetLoader<Asset>
{
    public override string DirectoryInfo => TownOfUsReworked.Options;
    public override bool Downloading => true;
    public override string Manifest => "Presets";

    public static PresetLoader Instance { get; set; }

    public override IEnumerator BeginDownload(object response)
    {
        var mainResponse = (List<Asset>)response;
        LogMessage($"Found {mainResponse.Count} presets");
        var toDownload = mainResponse.Select(x => x.ID);

        foreach (var fileName in toDownload)
            yield return CoDownloadAsset(fileName, this, "txt");
    }
}