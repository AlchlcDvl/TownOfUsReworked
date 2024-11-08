namespace TownOfUsReworked.Loaders;

public class ImageLoader : AssetLoader<Asset>
{
    public override string DirectoryInfo => TownOfUsReworked.Images;
    public override bool Downloading => true;
    public override string Manifest => "Images";
    public override string FileExtension => "png";

    public static ImageLoader Instance { get; set; }

    public override IEnumerator BeginDownload(Asset[] response)
    {
        Message($"Found {response.Length} assets");
        yield return CoDownloadAssets(response.Select(x => x.ID).Where(ShouldDownload));
    }

    public override IEnumerator AfterLoading(Asset[] response)
    {
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var image = response[i];
            AddPath(image.ID, Path.Combine(DirectoryInfo, $"{image.ID}.png"));
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Loading Images ({i + 1}/{response.Length})");
                yield return EndFrame();
            }
        }

        yield break;
    }

    private static bool ShouldDownload(string id) => !File.Exists(Path.Combine(TownOfUsReworked.Images, $"{id}.png"));
}