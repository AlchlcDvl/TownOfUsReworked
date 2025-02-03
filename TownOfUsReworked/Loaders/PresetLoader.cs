namespace TownOfUsReworked.Loaders;

public class PresetLoader : AssetLoader<DownloadableAsset>
{
    public override string DirectoryInfo => TownOfUsReworked.Options;
    public override bool Downloading => true;
    public override string Manifest => "Presets";
    public override string FileExtension => "txt";

    public static PresetLoader Instance { get; set; }

    public override IEnumerator BeginDownload(DownloadableAsset[] response) => CoDownloadAssets(response.Where(x => ShouldDownload(Path.Combine(DirectoryInfo, $"{x.ID}.txt"), x.Hash))
        .Select(x => x.ID));

    public override IEnumerator LoadAssets(DownloadableAsset[] response)
    {
        Message($"Found {response.Length} presets");
        yield return EndFrame();
    }

    public override IEnumerator GenerateHashes(DownloadableAsset[] response)
    {
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var preset = response[i];
            preset.Hash = GenerateHash(Path.Combine(DirectoryInfo, $"{preset.ID}.txt"));
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Generating Preset Hashes ({i + 1}/{response.Length})");
                yield return EndFrame();
            }
        }
    }
}