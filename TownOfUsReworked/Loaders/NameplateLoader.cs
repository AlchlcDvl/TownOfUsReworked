using static TownOfUsReworked.Managers.CustomNameplateManager;

namespace TownOfUsReworked.Loaders;

public class NameplateLoader : AssetLoader<CustomNameplate>
{
    public override string DirectoryInfo => TownOfUsReworked.Nameplates;
    public override bool Downloading => true;
    public override string Manifest => "Nameplates";
    public override string FileExtension => "png";

    public static NameplateLoader Instance { get; set; }

    public override IEnumerator BeginDownload(CustomNameplate[] response) => CoDownloadAssets(GenerateDownloadList(response));

    public override IEnumerator LoadAssets(CustomNameplate[] response)
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

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Loading Nameplates ({i + 1}/{unregistered.Count})");
                yield return EndFrame();
            }
        }

        unregistered.Clear();
    }

    public override IEnumerator GenerateHashes(CustomNameplate[] response)
    {
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var nameplate = response[i];
            nameplate.MainHash = GenerateHash(Path.Combine(DirectoryInfo, $"{nameplate.ID}.png"));

            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Generating Nameplate Hashes ({i + 1}/{response.Length})");
                yield return EndFrame();
            }
        }
    }
}