namespace TownOfUsReworked.Monos;

public class SoundLoader : AssetLoader
{
    public override string DirectoryInfo => TownOfUsReworked.Sounds;
    public override bool Downloading => true;
    public override string FolderDownloadName => "sounds";
    public override string ManifestFileName => "Sounds";

    [HideFromIl2Cpp]
    public override Type JSONType => typeof(SoundsJSON);

    public static SoundLoader Instance { get; private set; }

    public SoundLoader(IntPtr ptr) : base(ptr)
    {
        if (Instance)
            Instance.Destroy();

        Instance = this;
    }

    [HideFromIl2Cpp]
    public override IEnumerator BeginDownload(object response)
    {
        var mainResponse = (SoundsJSON)response;
        LogMessage($"Found {mainResponse.Sounds.Count} sounds");
        var toDownload = mainResponse.Sounds.Select(x => x.ID).Where(ShouldDownload);
        LogMessage($"Downloading {toDownload.Count()} sounds");

        foreach (var fileName in toDownload)
            yield return CoDownloadAsset(fileName, this, "wav");
    }

    [HideFromIl2Cpp]
    public override IEnumerator AfterLoading(object response)
    {
        var sounds = ((SoundsJSON)response).Sounds;

        for (var i = 0; i < sounds.Count; i++)
        {
            var sound = sounds[i];
            SoundEffects[sound.ID] = CreateDiskAudio(sound.ID, File.Open(Path.Combine(DirectoryInfo, $"{sound.ID}.wav"), FileMode.Open));
            UpdateSplashPatch.SetText($"Loading Sounds ({i + 1}/{sounds.Count})");
            yield return EndFrame();
        }

        yield break;
    }

    private static bool ShouldDownload(string id) => !File.Exists(Path.Combine(TownOfUsReworked.Sounds, $"{id}.wav"));
}