using static TownOfUsReworked.Cosmetics.CustomVisors.CustomVisorManager;
using Cpp2IL.Core.Extensions;

namespace TownOfUsReworked.Monos;

public class VisorLoader : AssetLoader
{
    public override string DirectoryInfo => TownOfUsReworked.Visors;
    public override bool Downloading => true;
    public override string FolderDownloadName => "visors";
    public override string ManifestFileName => "Visors";

    [HideFromIl2Cpp]
    public override Type JSONType => typeof(VisorsJSON);

    public static VisorLoader Instance { get; private set; }

    public VisorLoader(IntPtr ptr) : base(ptr)
    {
        if (Instance)
            Instance.Destroy();

        Instance = this;
    }

    [HideFromIl2Cpp]
    public override IEnumerator BeginDownload(object response)
    {
        var mainResponse = (VisorsJSON)response;
        UnregisteredVisors.AddRange(mainResponse.Visors);
        LogMessage($"Found {UnregisteredVisors.Count} visors");
        var toDownload = GenerateDownloadList(UnregisteredVisors);
        LogMessage($"Downloading {toDownload.Count} visor files");

        foreach (var fileName in toDownload)
            yield return CoDownloadAsset(fileName, this, "png");
    }

    [HideFromIl2Cpp]
    public override IEnumerator AfterLoading(object response)
    {
        /*if (TownOfUsReworked.IsStream)
        {
            var filePath = Path.Combine(TownOfUsReworked.Visors, "Stream", "Visors.json");

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<VisorsJSON>(json);
                data.Visors.ForEach(x => x.StreamOnly = true);
                UnregisteredVisors.AddRange(data.Visors);
            }
        }*/

        var cache = UnregisteredVisors.Clone();

        for (var i = 0; i < cache.Count; i++)
        {
            var file = cache[i];
            RegisteredVisors.Add(CreateVisorBehaviour(file));
            UnregisteredVisors.Remove(file);
            UpdateSplashPatch.SetText($"Loading Visors ({i + 1}/{cache.Count})");
            yield return EndFrame();
        }

        cache.Clear();
        yield break;
    }
}