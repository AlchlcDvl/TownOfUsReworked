namespace TownOfUsReworked.Loaders;

public class SoundLoader : AssetLoader<Asset>
{
    public override string DirectoryInfo => TownOfUsReworked.Sounds;
    public override bool Downloading => true;
    public override string Manifest => "Sounds";

    public static SoundLoader Instance { get; set; }

    public override IEnumerator BeginDownload(object response)
    {
        var mainResponse = (List<Asset>)response;
        LogMessage($"Found {mainResponse.Count} sounds");
        var toDownload = mainResponse.Select(x => x.ID).Where(ShouldDownload);
        LogMessage($"Downloading {toDownload.Count()} sounds");

        foreach (var fileName in toDownload)
            yield return CoDownloadAsset(fileName, this, "wav");
    }

    public override IEnumerator AfterLoading(object response)
    {
        var sounds = (List<Asset>)response;
        var time = 0f;

        for (var i = 0; i < sounds.Count; i++)
        {
            var sound = sounds[i];
            SoundEffects[sound.ID] = CreateDiskAudio(sound.ID, File.Open(Path.Combine(DirectoryInfo, $"{sound.ID}.wav"), FileMode.Open));
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Loading Sounds ({i + 1}/{sounds.Count})");
                yield return EndFrame();
            }
        }

        sounds.Clear();
        yield break;
    }

    private static bool ShouldDownload(string id) => !File.Exists(Path.Combine(TownOfUsReworked.Sounds, $"{id}.wav"));
}