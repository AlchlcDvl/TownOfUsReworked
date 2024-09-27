namespace TownOfUsReworked.Loaders;

public class PortalLoader : AssetLoader<Asset>
{
    public override string DirectoryInfo => TownOfUsReworked.Portal;
    public override bool Downloading => true;
    public override string Manifest => "Portal";
    public override string FileExtension => "png";

    public static PortalLoader Instance { get; set; }

    public override IEnumerator BeginDownload(object response)
    {
        var mainResponse = (List<Asset>)response;
        Message($"Found {mainResponse.Count} frames");
        var toDownload = mainResponse.Select(x => x.ID).Where(ShouldDownload);
        Message($"Downloading {toDownload.Count()} frames");
        yield return CoDownloadAsset(toDownload);
    }

    public override IEnumerator AfterLoading(object response)
    {
        var portal = (List<Asset>)response;
        var textures = new List<Texture2D>();
        portal.Select(x => Path.Combine(DirectoryInfo, $"{x.ID}.png")).ForEach(x => textures.Add(LoadDiskTexture(x)));
        var time = 0f;

        for (var i = 0; i < portal.Count; i++)
        {
            PortalAnimation.Add(CreateSprite(textures[i], portal[i].ID));
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Loading Portal Frames ({i + 1}/205)");
                yield return EndFrame();
            }
        }

        portal.Clear();
        yield break;
    }

    private static bool ShouldDownload(string id) => !File.Exists(Path.Combine(TownOfUsReworked.Portal, $"{id}.png"));
}