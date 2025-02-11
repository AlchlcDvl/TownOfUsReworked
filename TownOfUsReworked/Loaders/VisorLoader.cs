using static TownOfUsReworked.Managers.CustomVisorManager;

namespace TownOfUsReworked.Loaders;

public class VisorLoader : AssetLoader<CustomVisor>
{
    protected override string DirectoryInfo => TownOfUsReworked.Visors;
    protected override bool Downloading => true;
    protected override string Manifest => "Visors";
    protected override string FileExtension => "png";

    protected override IEnumerator BeginDownload(CustomVisor[] response, HashAlgorithm hasher) => CoDownloadAssets(GenerateDownloadList(response, hasher));

    protected override IEnumerator LoadAssets(CustomVisor[] response)
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

            if (time < 1f)
                continue;

            time = 0f;
            UpdateSplashPatch.SetText($"Loading Visors ({i + 1}/{unregistered.Count})");
            yield return EndFrame();
        }

        unregistered.Clear();
    }

    protected override IEnumerator GenerateHashes(CustomVisor[] response, HashAlgorithm hasher)
    {
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var visor = response[i];
            visor.MainHash = GenerateHash(Path.Combine(DirectoryInfo, $"{visor.ID}.png"), hasher);

            if (visor.ClimbID != null)
                visor.ClimbHash = GenerateHash(Path.Combine(DirectoryInfo, $"{visor.ClimbID}.png"), hasher);

            if (visor.FlipID != null)
                visor.FlipHash = GenerateHash(Path.Combine(DirectoryInfo, $"{visor.FlipID}.png"), hasher);

            if (visor.FloorID != null)
                visor.FloorHash = GenerateHash(Path.Combine(DirectoryInfo, $"{visor.FloorID}.png"), hasher);

            time += Time.deltaTime;

            if (time < 1f)
                continue;

            time = 0f;
            UpdateSplashPatch.SetText($"Generating Visor Hashes ({i + 1}/{response.Length})");
            yield return EndFrame();
        }
    }
}