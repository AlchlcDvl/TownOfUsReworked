namespace TownOfUsReworked.Loaders;

public class SoundLoader : AssetLoader<DownloadableAsset>
{
    public override string DirectoryInfo => TownOfUsReworked.Sounds;
    public override bool Downloading => true;
    public override string Manifest => "Sounds";
    public override string FileExtension => "wav";

    public static SoundLoader Instance { get; set; }

    public override IEnumerator BeginDownload(DownloadableAsset[] response) => CoDownloadAssets(response.Where(x => ShouldDownload(Path.Combine(DirectoryInfo, $"{x.ID}.wav"), x.Hash))
        .Select(x => x.ID));

    public override IEnumerator LoadAssets(DownloadableAsset[] response)
    {
        Message($"Found {response.Length} sounds");
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
    }

    public override IEnumerator GenerateHashes(DownloadableAsset[] response)
    {
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var sound = response[i];
            sound.Hash = GenerateHash(Path.Combine(DirectoryInfo, $"{sound.ID}.wav"));
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Generating Sound Hashes ({i + 1}/{response.Length})");
                yield return EndFrame();
            }
        }
    }
}