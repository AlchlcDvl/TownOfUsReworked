using static TownOfUsReworked.Cosmetics.CustomNameplates.CustomNameplateManager;
using System.Text.Json;
using Cpp2IL.Core.Extensions;

namespace TownOfUsReworked.Monos;

public class NameplatesLoader : AssetLoader
{
    public override string DirectoryInfo => TownOfUsReworked.Nameplates;
    public override bool Downloading => true;
    public override string FolderDownloadName => "nameplates";
    public override string ManifestFileName => "Nameplates";

    [HideFromIl2Cpp]
    public override Type JSONType => typeof(NameplatesJSON);

    public static NameplatesLoader Instance { get; private set; }

    public NameplatesLoader(IntPtr ptr) : base(ptr)
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
            yield return CoDownloadAsset(fileName, this);

        yield return new WaitForEndOfFrame();
    }

    [HideFromIl2Cpp]
    public override void AfterLoading(object response)
    {
        if (TownOfUsReworked.IsStream)
        {
            var filePath = Path.Combine(TownOfUsReworked.Nameplates, "Stream", "Nameplates.json");

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<NameplatesJSON>(json, new JsonSerializerOptions() { AllowTrailingCommas = true });
                data.Nameplates.ForEach(x => x.StreamOnly = true);
                UnregisteredNameplates.AddRange(data.Nameplates);
            }
        }

        var cache = UnregisteredNameplates.Clone();

        foreach (var file in cache)
        {
            try
            {
                RegisteredNameplates.Add(CreateNameplateBehaviour(file));
                UnregisteredNameplates.Remove(file);
            } catch { /*file doesn't exist yet*/ }
        }

        cache.Clear();
    }
}