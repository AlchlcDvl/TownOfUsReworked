using static TownOfUsReworked.Cosmetics.CustomHats.CustomHatManager;
using System.Text.Json;
using Cpp2IL.Core.Extensions;

namespace TownOfUsReworked.Monos;

public class HatsLoader : AssetLoader
{
    public override string DirectoryInfo => TownOfUsReworked.Hats;
    public override bool Downloading => true;
    public override string FolderDownloadName => "hats";
    public override string ManifestFileName => "Hats";

    [HideFromIl2Cpp]
    public override Type JSONType => typeof(HatsJSON);

    public static HatsLoader Instance { get; private set; }

    public HatsLoader(IntPtr ptr) : base(ptr)
    {
        if (Instance)
            Instance.Destroy();

        Instance = this;
    }

    [HideFromIl2Cpp]
    public override IEnumerator BeginDownload(object response)
    {
        var mainResponse = (HatsJSON)response;
        UnregisteredHats.AddRange(mainResponse.Hats);
        LogMessage($"Found {UnregisteredHats.Count} hats");
        var toDownload = GenerateDownloadList(UnregisteredHats);
        LogMessage($"Downloading {toDownload.Count} hat files");

        foreach (var fileName in toDownload)
            yield return CoDownloadAsset(fileName, this);

        yield return new WaitForEndOfFrame();
    }

    [HideFromIl2Cpp]
    public override void AfterLoading(object response)
    {
        UnregisteredHats.ForEach(ch => ch.Behind = ch.BackID != null || ch.BackFlipID != null);

        if (TownOfUsReworked.IsStream)
        {
            var filePath = Path.Combine(TownOfUsReworked.Hats, "Stream", "Hats.json");

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<HatsJSON>(json, new JsonSerializerOptions() { AllowTrailingCommas = true });
                data.Hats.ForEach(x => x.StreamOnly = true);
                UnregisteredHats.AddRange(data.Hats);
            }
        }

        var cache = UnregisteredHats.Clone();

        foreach (var file in cache)
        {
            try
            {
                RegisteredHats.Add(CreateHatBehaviour(file));
                UnregisteredHats.Remove(file);
            } catch { /*file doesn't exist yet*/ }
        }

        cache.Clear();
    }
}