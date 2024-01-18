using static TownOfUsReworked.Cosmetics.CustomVisors.CustomVisorManager;
using System.Text.Json;
using Cpp2IL.Core.Extensions;

namespace TownOfUsReworked.Monos;

public class VisorsLoader : AssetLoader
{
    public override string DirectoryInfo => TownOfUsReworked.Visors;
    public override bool Downloading => true;
    public override string FolderDownloadName => "visors";
    public override string ManifestFileName => "Visors";

    [HideFromIl2Cpp]
    public override Type JSONType => typeof(VisorsJSON);

    public static VisorsLoader Instance { get; private set; }

    public VisorsLoader(IntPtr ptr) : base(ptr)
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
            yield return CoDownloadAsset(fileName, this);

        yield return new WaitForEndOfFrame();
    }

    [HideFromIl2Cpp]
    public override void AfterLoading(object response)
    {
        if (TownOfUsReworked.IsStream)
        {
            var filePath = Path.Combine(TownOfUsReworked.Visors, "Stream", "Visors.json");

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<VisorsJSON>(json, new JsonSerializerOptions() { AllowTrailingCommas = true });
                data.Visors.ForEach(x => x.StreamOnly = true);
                UnregisteredVisors.AddRange(data.Visors);
            }
        }

        var cache = UnregisteredVisors.Clone();

        foreach (var file in cache)
        {
            try
            {
                RegisteredVisors.Add(CreateVisorBehaviour(file));
                UnregisteredVisors.Remove(file);
            } catch { /*file doesn't exist yet*/ }
        }

        cache.Clear();
    }
}