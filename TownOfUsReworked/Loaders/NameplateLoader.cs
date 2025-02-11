using static TownOfUsReworked.Managers.CustomNameplateManager;

namespace TownOfUsReworked.Loaders;

public class NameplateLoader : AssetLoader<CustomNameplate>
{
    protected override string DirectoryInfo => TownOfUsReworked.Nameplates;
    protected override bool Downloading => true;
    protected override string Manifest => "Nameplates";
    protected override string FileExtension => "png";

    protected override IEnumerator BeginDownload(CustomNameplate[] response, HashAlgorithm hasher) => CoDownloadAssets(GenerateDownloadList(response, hasher));

    protected override IEnumerator LoadAssets(CustomNameplate[] response)
    {
        var unregistered = new List<CustomNameplate>(response);

        if (TownOfUsReworked.IsStream)
        {
            var filePath = Path.Combine(TownOfUsReworked.Nameplates, "Stream", "Nameplates.json");

            if (File.Exists(filePath))
            {
                var data = JsonSerializer.Deserialize<CustomNameplate[]>(File.ReadAllText(filePath));
                data.ForEach(x => x.StreamOnly = true);
                unregistered.AddRange(data);
                Array.Clear(data);
            }
        }

        Message($"Found {unregistered.Count} nameplates");
        var time = 0f;

        for (var i = 0; i < unregistered.Count; i++)
        {
            var file = unregistered[i];
            CreateNameplateBehaviour(file);
            time += Time.deltaTime;

            if (time < 1f)
                continue;

            time = 0f;
            UpdateSplashPatch.SetText($"Loading Nameplates ({i + 1}/{unregistered.Count})");
            yield return EndFrame();
        }

        unregistered.Clear();
    }

    protected override IEnumerator GenerateHashes(CustomNameplate[] response, HashAlgorithm hasher)
    {
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var nameplate = response[i];
            nameplate.MainHash = GenerateHash(Path.Combine(DirectoryInfo, $"{nameplate.ID}.png"), hasher);

            time += Time.deltaTime;

            if (time < 1f)
                continue;

            time = 0f;
            UpdateSplashPatch.SetText($"Generating Nameplate Hashes ({i + 1}/{response.Length})");
            yield return EndFrame();
        }
    }
}