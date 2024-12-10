using static TownOfUsReworked.Cosmetics.CustomHatManager;
using Cpp2IL.Core.Extensions;

namespace TownOfUsReworked.Loaders;

public class HatLoader : AssetLoader<CustomHat>
{
    public override string DirectoryInfo => TownOfUsReworked.Hats;
    public override bool Downloading => true;
    public override string Manifest => "Hats";
    public override string FileExtension => "png";

    public static HatLoader Instance { get; set; }

    public override IEnumerator BeginDownload(CustomHat[] response) => CoDownloadAssets(GenerateDownloadList(response));

    public override IEnumerator AfterLoading(CustomHat[] response)
    {
        UnregisteredHats.AddRange(response);
        UnregisteredHats.ForEach(ch => ch.Behind = ch.BackID != null || ch.BackFlipID != null);

        if (TownOfUsReworked.IsStream)
        {
            var filePath = Path.Combine(TownOfUsReworked.Hats, "Stream", "Hats.json");

            if (File.Exists(filePath))
            {
                var data = JsonSerializer.Deserialize<CustomHat[]>(File.ReadAllText(filePath));
                data.ForEach(x => x.StreamOnly = true);
                UnregisteredHats.AddRange(data);
                Array.Clear(data);
            }
        }

        Message($"Found {UnregisteredHats.Count} hats");
        var cache = UnregisteredHats.Clone();
        var time = 0f;

        for (var i = 0; i < cache.Count; i++)
        {
            var file = cache[i];
            RegisteredHats.Add(CreateHatBehaviour(file));
            UnregisteredHats.Remove(file);
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Loading Hats ({i + 1}/{cache.Count})");
                yield return EndFrame();
            }
        }

        cache.Clear();
        yield break;
    }
}