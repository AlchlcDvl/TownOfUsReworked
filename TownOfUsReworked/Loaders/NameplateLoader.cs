using static TownOfUsReworked.Cosmetics.CustomNameplates.CustomNameplateManager;
using Cpp2IL.Core.Extensions;

namespace TownOfUsReworked.Loaders;

public class NameplateLoader : AssetLoader<CustomNameplate>
{
    public override string DirectoryInfo => TownOfUsReworked.Nameplates;
    public override bool Downloading => true;
    public override string Manifest => "Nameplates";
    public override string FileExtension => "png";

    public static NameplateLoader Instance { get; set; }

    public override IEnumerator BeginDownload(CustomNameplate[] response)
    {
        UnregisteredNameplates.AddRange(response);
        Message($"Found {UnregisteredNameplates.Count} nameplates");
        yield return CoDownloadAssets(GenerateDownloadList(UnregisteredNameplates));
    }

    public override IEnumerator AfterLoading(CustomNameplate[] response)
    {
        if (TownOfUsReworked.IsStream)
        {
            var filePath = Path.Combine(TownOfUsReworked.Nameplates, "Stream", "Nameplates.json");

            if (File.Exists(filePath))
            {
                var data = JsonSerializer.Deserialize<CustomNameplate[]>(File.ReadAllText(filePath));
                data.ForEach(x => x.StreamOnly = true);
                UnregisteredNameplates.AddRange(data);
                Array.Clear(data);
            }
        }

        var cache = UnregisteredNameplates.Clone();
        var time = 0f;

        for (var i = 0; i < cache.Count; i++)
        {
            var file = cache[i];
            RegisteredNameplates.Add(CreateNameplateBehaviour(file));
            UnregisteredNameplates.Remove(file);
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Loading Nameplates ({i + 1}/{cache.Count})");
                yield return EndFrame();
            }
        }

        cache.Clear();
        yield break;
    }
}