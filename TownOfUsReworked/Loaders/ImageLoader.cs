namespace TownOfUsReworked.Loaders;

public class ImageLoader : AssetLoader<DownloadableAsset>
{
    protected override string DirectoryInfo => TownOfUsReworked.Images;
    protected override bool Downloading => true;
    protected override string Manifest => "Images";
    protected override string FileExtension => "png";

    protected override IEnumerator BeginDownload(DownloadableAsset[] response, HashAlgorithm hasher) => CoDownloadAssets(response.Where(x => ShouldDownload(Path.Combine(DirectoryInfo,
        $"{x.ID}.png"), x.Hash, hasher)).Select(x => x.ID));

    protected override IEnumerator LoadAssets(DownloadableAsset[] response)
    {
        Message($"Found {response.Length} images");
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var image = response[i];
            AddPath(image.ID, Path.Combine(DirectoryInfo, $"{image.ID}.png"));
            time += Time.deltaTime;

            if (time < 1f)
                continue;

            time = 0f;
            UpdateSplashPatch.SetText($"Loading Images ({i + 1}/{response.Length})");
            yield return EndFrame();
        }
    }

    protected override IEnumerator GenerateHashes(DownloadableAsset[] response, HashAlgorithm hasher)
    {
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var image = response[i];
            image.Hash = GenerateHash(Path.Combine(DirectoryInfo, $"{image.ID}.png"), hasher);
            time += Time.deltaTime;

            if (time < 1f)
                continue;

            time = 0f;
            UpdateSplashPatch.SetText($"Generating Image Hashes ({i + 1}/{response.Length})");
            yield return EndFrame();
        }
    }
}