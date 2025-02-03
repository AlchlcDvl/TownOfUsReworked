namespace TownOfUsReworked.Loaders;

public class PortalLoader : AssetLoader<DownloadableAsset>
{
    public override string DirectoryInfo => TownOfUsReworked.Portal;
    public override bool Downloading => true;
    public override string Manifest => "Portal";
    public override string FileExtension => "png";

    public static PortalLoader Instance { get; set; }

    public override IEnumerator BeginDownload(DownloadableAsset[] response) => CoDownloadAssets(response.Where(x => ShouldDownload(Path.Combine(DirectoryInfo, $"{x.ID}.png"), x.Hash))
        .Select(x => x.ID));

    public override IEnumerator LoadAssets(DownloadableAsset[] response)
    {
        var time = 0f;
        Message($"Found {response.Length} frames");

        for (var i = 0; i < response.Length; i++)
        {
            PortalAnimation.Add(LoadDiskSprite(Path.Combine(DirectoryInfo, $"{response[i].ID}.png")));
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Loading Portal Frames ({i + 1}/{response.Length})");
                yield return EndFrame();
            }
        }
    }

    public override IEnumerator GenerateHashes(DownloadableAsset[] response)
    {
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var portal = response[i];
            portal.Hash = GenerateHash(Path.Combine(DirectoryInfo, $"{portal.ID}.png"));
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Generating Portal Hashes ({i + 1}/{response.Length})");
                yield return EndFrame();
            }
        }
    }
}