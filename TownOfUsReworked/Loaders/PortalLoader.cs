namespace TownOfUsReworked.Loaders;

public class PortalLoader : AssetLoader<DownloadableAsset>
{
    protected override string DirectoryInfo => TownOfUsReworked.Portal;
    protected override bool Downloading => true;
    protected override string Manifest => "Portal";
    protected override string FileExtension => "png";

    protected override IEnumerator BeginDownload(DownloadableAsset[] response, HashAlgorithm hasher) => CoDownloadAssets(response.Where(x => ShouldDownload(Path.Combine(DirectoryInfo,
        $"{x.ID}.png"), x.Hash, hasher)).Select(x => x.ID));

    protected override IEnumerator LoadAssets(DownloadableAsset[] response)
    {
        var time = 0f;
        Message($"Found {response.Length} frames");

        for (var i = 0; i < response.Length; i++)
        {
            PortalAnimation.Add(LoadDiskSprite(Path.Combine(DirectoryInfo, $"{response[i].ID}.png")));
            time += Time.deltaTime;

            if (time < 1f)
                continue;

            time = 0f;
            UpdateSplashPatch.SetText($"Loading Portal Frames ({i + 1}/{response.Length})");
            yield return EndFrame();
        }
    }

    protected override IEnumerator GenerateHashes(DownloadableAsset[] response, HashAlgorithm hasher)
    {
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var portal = response[i];
            portal.Hash = GenerateHash(Path.Combine(DirectoryInfo, $"{portal.ID}.png"), hasher);
            time += Time.deltaTime;

            if (time < 1f)
                continue;

            time = 0f;
            UpdateSplashPatch.SetText($"Generating Portal Hashes ({i + 1}/{response.Length})");
            yield return EndFrame();
        }
    }
}