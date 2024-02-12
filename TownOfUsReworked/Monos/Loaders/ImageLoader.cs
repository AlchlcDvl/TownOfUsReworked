namespace TownOfUsReworked.Monos;

public class ImageLoader : AssetLoader
{
    public override string DirectoryInfo => TownOfUsReworked.Images;
    public override bool Downloading => true;
    public override string FolderDownloadName => "modimages";
    public override string ManifestFileName => "Images";

    [HideFromIl2Cpp]
    public override Type JSONType => typeof(ImagesJSON);

    public static ImageLoader Instance { get; private set; }

    public ImageLoader(IntPtr ptr) : base(ptr)
    {
        if (Instance)
            Instance.Destroy();

        Instance = this;
    }

    [HideFromIl2Cpp]
    public override IEnumerator BeginDownload(object response)
    {
        var mainResponse = (ImagesJSON)response;
        LogMessage($"Found {mainResponse.Images.Count} assets");
        var toDownload = mainResponse.Images.Select(x => x.ID).Where(ShouldDownload);
        LogMessage($"Downloading {toDownload.Count()} assets");

        foreach (var fileName in toDownload)
            yield return CoDownloadAsset(fileName, this, "png");
    }

    [HideFromIl2Cpp]
    public override IEnumerator AfterLoading(object response)
    {
        var buttons = (ImagesJSON)response;
        var textures = new List<Texture2D>();
        buttons.Images.Select(x => Path.Combine(TownOfUsReworked.Images, $"{x.ID}.png")).ForEach(x => textures.Add(LoadDiskTexture(x)));

        for (var i = 0; i < buttons.Images.Count; i++)
        {
            var image = buttons.Images[i];
            Sprites[image.ID] = CreateSprite(textures[i], image.ID);
            UpdateSplashPatch.SetText($"Loading Images ({i + 1}/{buttons.Images.Count})");
            yield return EndFrame();
        }

        yield break;
    }

    private static bool ShouldDownload(string id) => !File.Exists(Path.Combine(TownOfUsReworked.Images, $"{id}.png"));
}