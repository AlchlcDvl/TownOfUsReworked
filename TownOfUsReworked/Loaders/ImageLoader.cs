namespace TownOfUsReworked.Loaders;

public class ImageLoader : AssetLoader<Asset>
{
    public override string DirectoryInfo => TownOfUsReworked.Images;
    public override bool Downloading => true;
    public override string Manifest => "Images";

    public static ImageLoader Instance { get; set; }

    public override IEnumerator BeginDownload(object response)
    {
        var mainResponse = (List<Asset>)response;
        LogMessage($"Found {mainResponse.Count} assets");
        var toDownload = mainResponse.Select(x => x.ID).Where(ShouldDownload);
        LogMessage($"Downloading {toDownload.Count()} assets");

        foreach (var fileName in toDownload)
            yield return CoDownloadAsset(fileName, this, "png");
    }

    public override IEnumerator AfterLoading(object response)
    {
        var images = (List<Asset>)response;
        var textures = new List<Texture2D>();
        images.Select(x => Path.Combine(TownOfUsReworked.Images, $"{x.ID}.png")).ForEach(x => textures.Add(LoadDiskTexture(x)));
        var time = 0f;

        for (var i = 0; i < images.Count; i++)
        {
            var image = images[i];
            AddAsset(image.ID, CreateSprite(textures[i], image.ID));
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Loading Images ({i + 1}/{images.Count})");
                yield return EndFrame();
            }
        }

        images.Clear();
        yield break;
    }

    private static bool ShouldDownload(string id) => !File.Exists(Path.Combine(TownOfUsReworked.Images, $"{id}.png"));
}