namespace TownOfUsReworked.Loaders;

public class PresetLoader : AssetLoader<DownloadableAsset>
{
    protected override string DirectoryInfo => TownOfUsReworked.Options;
    protected override bool Downloading => true;
    protected override string Manifest => "Presets";
    protected override string FileExtension => "txt";

    protected override IEnumerator BeginDownload(DownloadableAsset[] response, HashAlgorithm hasher) => CoDownloadAssets(response.Where(x => ShouldDownload(Path.Combine(DirectoryInfo,
        $"{x.ID}.txt"), x.Hash, hasher)) .Select(x => x.ID));

    protected override IEnumerator LoadAssets(DownloadableAsset[] response)
    {
        Message($"Found {response.Length} presets");
        yield return EndFrame();
    }

    protected override IEnumerator GenerateHashes(DownloadableAsset[] response, HashAlgorithm hasher)
    {
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var preset = response[i];
            preset.Hash = GenerateHash(Path.Combine(DirectoryInfo, $"{preset.ID}.txt"), hasher);
            time += Time.deltaTime;

            if (time < 1f)
                continue;

            time = 0f;
            UpdateSplashPatch.SetText($"Generating Preset Hashes ({i + 1}/{response.Length})");
            yield return EndFrame();
        }
    }
}