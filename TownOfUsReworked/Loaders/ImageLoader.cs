namespace TownOfUsReworked.Loaders;

public class ImageLoader : AssetLoader<DownloadableAsset>
{
    public override string DirectoryInfo => TownOfUsReworked.Images;
    public override bool Downloading => true;
    public override string Manifest => "Images";
    public override string FileExtension => "png";

    public static ImageLoader Instance { get; set; }

    public override IEnumerator BeginDownload(DownloadableAsset[] response) => CoDownloadAssets(response.Where(x => ShouldDownload(Path.Combine(DirectoryInfo, $"{x.ID}.png"), x.Hash))
        .Select(x => x.ID));

    public override IEnumerator LoadAssets(DownloadableAsset[] response)
    {
        Message($"Found {response.Length} images");
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
    }

    public override IEnumerator GenerateHashes(DownloadableAsset[] response)
    {
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var image = response[i];
            image.Hash = GenerateHash(Path.Combine(DirectoryInfo, $"{image.ID}.png"));
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Generating Image Hashes ({i + 1}/{response.Length})");
                yield return EndFrame();
            }
        }
    }
}