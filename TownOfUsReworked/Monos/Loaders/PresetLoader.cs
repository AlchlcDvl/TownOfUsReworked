namespace TownOfUsReworked.Monos;

public class PresetLoader : AssetLoader
{
    public override string DirectoryInfo => TownOfUsReworked.Options;
    public override bool Downloading => true;
    public override string FolderDownloadName => "presets";
    public override string ManifestFileName => "Presets";

    [HideFromIl2Cpp]
    public override Type JSONType => typeof(PresetsJSON);

    public static PresetLoader Instance { get; private set; }

    public PresetLoader(IntPtr ptr) : base(ptr)
    {
        if (Instance)
            Instance.Destroy();

        Instance = this;
    }

    [HideFromIl2Cpp]
    public override IEnumerator BeginDownload(object response)
    {
        var mainResponse = (PresetsJSON)response;
        LogMessage($"Found {mainResponse.Presets.Count} presets");
        var toDownload = mainResponse.Presets.Select(x => x.ID);

        foreach (var fileName in toDownload)
            yield return CoDownloadAsset(fileName, this, "txt");
    }
}