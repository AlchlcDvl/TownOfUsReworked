using static TownOfUsReworked.Managers.CustomHatManager;

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
        var unregistered = new List<CustomHat>(response);

        if (TownOfUsReworked.IsStream)
        {
            var filePath = Path.Combine(TownOfUsReworked.Hats, "Stream", "Hats.json");

            if (File.Exists(filePath))
            {
                var data = JsonSerializer.Deserialize<CustomHat[]>(File.ReadAllText(filePath));
                data.ForEach(x => x.StreamOnly = true);
                unregistered.AddRange(data);
                Array.Clear(data);
            }
        }

        Message($"Found {unregistered.Count} hats");
        var time = 0f;

        for (var i = 0; i < unregistered.Count; i++)
        {
            var file = unregistered[i];
            CreateHatBehaviour(file);
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Loading Hats ({i + 1}/{unregistered.Count})");
                yield return EndFrame();
            }
        }

        unregistered.Clear();
    }
}