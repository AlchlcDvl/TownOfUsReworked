namespace TownOfUsReworked.Monos;

public class PortalLoader : AssetLoader
{
    public override string DirectoryInfo => TownOfUsReworked.Portal;
    public override bool Downloading => true;
    public override string FolderDownloadName => "portal";
    public override string ManifestFileName => "Portal";

    [HideFromIl2Cpp]
    public override Type JSONType => typeof(PortalJSON);

    public static PortalLoader Instance { get; private set; }

    public PortalLoader(IntPtr ptr) : base(ptr)
    {
        if (Instance)
            Instance.Destroy();

        Instance = this;
    }

    [HideFromIl2Cpp]
    public override IEnumerator BeginDownload(object response)
    {
        var mainResponse = (PortalJSON)response;
        LogMessage($"Found {mainResponse.Portal.Count} frames");
        var toDownload = mainResponse.Portal.Select(x => x.ID).Where(ShouldDownload);
        LogMessage($"Downloading {toDownload.Count()} frames");

        foreach (var fileName in toDownload)
            yield return CoDownloadAsset(fileName, this, "png");
    }

    [HideFromIl2Cpp]
    public override IEnumerator AfterLoading(object response)
    {
        var portal = (PortalJSON)response;
        var textures = new List<Texture2D>();
        portal.Portal.Select(x => Path.Combine(DirectoryInfo, $"{x.ID}.png")).ForEach(x => textures.Add(LoadDiskTexture(x)));

        for (var i = 0; i < portal.Portal.Count; i++)
        {
            PortalAnimation.Add(CreateSprite(textures[i], portal.Portal[i].ID));
            UpdateSplashPatch.SetText($"Loading Portal Frames ({i + 1}/205)");
            yield return EndFrame();
        }

        yield break;
    }

    private static bool ShouldDownload(string id) => !File.Exists(Path.Combine(TownOfUsReworked.Portal, $"{id}.png"));
}