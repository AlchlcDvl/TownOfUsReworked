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

    public override IEnumerator LoadAssets(CustomVisor[] response)
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

    public override IEnumerator GenerateHashes(CustomVisor[] response)
    {
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var visor = response[i];
            visor.MainHash = GenerateHash(Path.Combine(DirectoryInfo, $"{visor.ID}.png"));

            if (visor.ClimbID != null)
                visor.ClimbHash = GenerateHash(Path.Combine(DirectoryInfo, $"{visor.ClimbID}.png"));

            if (visor.FlipID != null)
                visor.FlipHash = GenerateHash(Path.Combine(DirectoryInfo, $"{visor.FlipID}.png"));

            if (visor.FloorID != null)
                visor.FloorHash = GenerateHash(Path.Combine(DirectoryInfo, $"{visor.FloorID}.png"));

            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Generating Visor Hashes ({i + 1}/{response.Length})");
                yield return EndFrame();
            }
        }
    }
}