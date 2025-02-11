namespace TownOfUsReworked.Loaders;

public class SoundLoader : AssetLoader<DownloadableAsset>
{
    protected override string DirectoryInfo => TownOfUsReworked.Sounds;
    protected override bool Downloading => true;
    protected override string Manifest => "Sounds";
    protected override string FileExtension => "wav";

    protected override IEnumerator BeginDownload(DownloadableAsset[] response, HashAlgorithm hasher) => CoDownloadAssets(response.Where(x => ShouldDownload(Path.Combine(DirectoryInfo,
        $"{x.ID}.wav"), x.Hash, hasher)).Select(x => x.ID));

    protected override IEnumerator LoadAssets(DownloadableAsset[] response)
    {
        Message($"Found {response.Length} sounds");
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var sound = response[i];
            AddPath(sound.ID, Path.Combine(DirectoryInfo, $"{sound.ID}.wav"));
            time += Time.deltaTime;

            if (time < 1f)
                continue;

            time = 0f;
            UpdateSplashPatch.SetText($"Loading Sounds ({i + 1}/{response.Length})");
            yield return EndFrame();
        }
    }

    protected override IEnumerator GenerateHashes(DownloadableAsset[] response, HashAlgorithm hasher)
    {
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var sound = response[i];
            sound.Hash = GenerateHash(Path.Combine(DirectoryInfo, $"{sound.ID}.wav"), hasher);
            time += Time.deltaTime;

            if (time < 1f)
                continue;

            time = 0f;
            UpdateSplashPatch.SetText($"Generating Sound Hashes ({i + 1}/{response.Length})");
            yield return EndFrame();
        }
    }
}