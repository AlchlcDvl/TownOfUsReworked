namespace TownOfUsReworked.Loaders;

public class PortalLoader : AssetLoader<Asset>
{
    public override string DirectoryInfo => TownOfUsReworked.Portal;
    public override bool Downloading => true;
    public override string Manifest => "Portal";
    public override string FileExtension => "png";

    public static PortalLoader Instance { get; set; }

    public override IEnumerator BeginDownload(Asset[] response)
    {
        Message($"Found {response.Length} frames");
        yield return CoDownloadAssets(response.Select(x => x.ID).Where(ShouldDownload));
    }

    public override IEnumerator AfterLoading(Asset[] response)
    {
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            PortalAnimation.Add(LoadDiskSprite(Path.Combine(DirectoryInfo, $"{response[i].ID}.png")));
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Loading Portal Frames ({i + 1}/205)");
                yield return EndFrame();
            }
        }

        yield break;
    }

    private static bool ShouldDownload(string id) => !File.Exists(Path.Combine(TownOfUsReworked.Portal, $"{id}.png"));
}