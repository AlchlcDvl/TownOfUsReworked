using static TownOfUsReworked.Cosmetics.CustomVisors.CustomVisorManager;
using Cpp2IL.Core.Extensions;

namespace TownOfUsReworked.Loaders;

public class VisorLoader : AssetLoader<CustomVisor>
{
    public override string DirectoryInfo => TownOfUsReworked.Visors;
    public override bool Downloading => true;
    public override string Manifest => "Visors";

    public static VisorLoader Instance { get; set; }

    public override IEnumerator BeginDownload(object response)
    {
        var mainResponse = (List<CustomVisor>)response;
        UnregisteredVisors.AddRange(mainResponse);
        LogMessage($"Found {UnregisteredVisors.Count} visors");
        var toDownload = GenerateDownloadList(UnregisteredVisors);
        LogMessage($"Downloading {toDownload.Count} visor files");

        foreach (var fileName in toDownload)
            yield return CoDownloadAsset(fileName, this, "png");
    }

    public override IEnumerator AfterLoading(object response)
    {
        /*if (TownOfUsReworked.IsStream)
        {
            var filePath = Path.Combine(TownOfUsReworked.Visors, "Stream", "Visors.json");

            if (File.Exists(filePath))
            {
                var data = JsonSerializer.Deserialize<List<CustomVisor>>(File.ReadAllText(filePath));
                data.ForEach(x => x.StreamOnly = true);
                UnregisteredVisors.AddRange(data);
            }
        }*/

        var cache = UnregisteredVisors.Clone();
        var time = 0f;

        for (var i = 0; i < cache.Count; i++)
        {
            var file = cache[i];
            RegisteredVisors.Add(CreateVisorBehaviour(file));
            UnregisteredVisors.Remove(file);
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Loading Visors ({i + 1}/{cache.Count})");
                yield return EndFrame();
            }
        }

        cache.Clear();
        yield break;
    }
}