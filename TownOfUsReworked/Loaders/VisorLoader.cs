using static TownOfUsReworked.Managers.CustomVisorManager;

namespace TownOfUsReworked.Loaders;

public class VisorLoader : AssetLoader<CustomVisor>
{
    public override string DirectoryInfo => TownOfUsReworked.Visors;
    public override bool Downloading => true;
    public override string Manifest => "Visors";
    public override string FileExtension => "png";

    public static VisorLoader Instance { get; set; }

    public override IEnumerator BeginDownload(CustomVisor[] response) => CoDownloadAssets(GenerateDownloadList(response));

    public override IEnumerator AfterLoading(CustomVisor[] response)
    {
        var unregistered = new List<CustomVisor>(response);

        if (TownOfUsReworked.IsStream)
        {
            var filePath = Path.Combine(TownOfUsReworked.Visors, "Stream", "Visors.json");

            if (File.Exists(filePath))
            {
                var data = JsonSerializer.Deserialize<CustomVisor[]>(File.ReadAllText(filePath));
                data.ForEach(x => x.StreamOnly = true);
                unregistered.AddRange(data);
                Array.Clear(data);
            }
        }

        Message($"Found {unregistered.Count} visors");
        var time = 0f;

        for (var i = 0; i < unregistered.Count; i++)
        {
            var file = unregistered[i];
            CreateVisorBehaviour(file);
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Loading Visors ({i + 1}/{unregistered.Count})");
                yield return EndFrame();
            }
        }

        unregistered.Clear();
    }
}