namespace TownOfUsReworked.Loaders;

public class SoundLoader : AssetLoader<Asset>
{
    public override string DirectoryInfo => TownOfUsReworked.Sounds;
    public override bool Downloading => true;
    public override string Manifest => "Sounds";
    public override string FileExtension => "wav";

    public static SoundLoader Instance { get; set; }

    public override IEnumerator BeginDownload(Asset[] response)
    {
        Message($"Found {response.Length} sounds");
        yield return CoDownloadAssets(response.Select(x => x.ID).Where(ShouldDownload));
    }

    public override IEnumerator AfterLoading(Asset[] response)
    {
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var sound = response[i];
            AddPath(sound.ID, Path.Combine(DirectoryInfo, $"{sound.ID}.wav"));
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Loading Sounds ({i + 1}/{response.Length})");
                yield return EndFrame();
            }
        }

        yield break;
    }

    private static bool ShouldDownload(string id) => !File.Exists(Path.Combine(TownOfUsReworked.Sounds, $"{id}.wav"));
}