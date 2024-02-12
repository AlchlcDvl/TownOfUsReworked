using static TownOfUsReworked.Cosmetics.CustomNameplates.CustomNameplateManager;
using Cpp2IL.Core.Extensions;

namespace TownOfUsReworked.Monos;

public class NameplateLoader : AssetLoader
{
    public override string DirectoryInfo => TownOfUsReworked.Nameplates;
    public override bool Downloading => true;
    public override string FolderDownloadName => "nameplates";
    public override string ManifestFileName => "Nameplates";

    [HideFromIl2Cpp]
    public override Type JSONType => typeof(NameplatesJSON);

    public static NameplateLoader Instance { get; private set; }

    public NameplateLoader(IntPtr ptr) : base(ptr)
    {
        if (Instance)
            Instance.Destroy();

        Instance = this;
    }

    [HideFromIl2Cpp]
    public override IEnumerator BeginDownload(object response)
    {
        var mainResponse = (NameplatesJSON)response;
        UnregisteredNameplates.AddRange(mainResponse.Nameplates);
        LogMessage($"Found {UnregisteredNameplates.Count} nameplates");
        var toDownload = GenerateDownloadList(UnregisteredNameplates);
        LogMessage($"Downloading {toDownload.Count} nameplate files");

        foreach (var fileName in toDownload)
            yield return CoDownloadAsset(fileName, this, "png");
    }

    [HideFromIl2Cpp]
    public override IEnumerator AfterLoading(object response)
    {
        /*if (TownOfUsReworked.IsStream)
        {
            var filePath = Path.Combine(TownOfUsReworked.Nameplates, "Stream", "Nameplates.json");

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<NameplatesJSON>(json);
                data.Nameplates.ForEach(x => x.StreamOnly = true);
                UnregisteredNameplates.AddRange(data.Nameplates);
            }
        }*/

        var cache = UnregisteredNameplates.Clone();

        for (var i = 0; i < cache.Count; i++)
        {
            var file = cache[i];
            RegisteredNameplates.Add(CreateNameplateBehaviour(file));
            UnregisteredNameplates.Remove(file);
            UpdateSplashPatch.SetText($"Loading Nameplates ({i + 1}/{cache.Count})");
            yield return EndFrame();
        }

        cache.Clear();
        yield break;
    }
}