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

    public override IEnumerator LoadAssets(CustomHat[] response)
    {
        var unregistered = new List<CustomHat>(response);

        if (TownOfUsReworked.IsStream)
        {
            var filePath = Path.Combine(DirectoryInfo, "Stream", "Hats.json");

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

    public override IEnumerator GenerateHashes(CustomHat[] response)
    {
        var time = 0f;

        for (var i = 0; i < response.Length; i++)
        {
            var hat = response[i];
            hat.MainHash = GenerateHash(Path.Combine(DirectoryInfo, $"{hat.ID}.png"));

            if (hat.BackID != null)
                hat.BackHash = GenerateHash(Path.Combine(DirectoryInfo, $"{hat.BackID}.png"));

            if (hat.ClimbID != null)
                hat.ClimbHash = GenerateHash(Path.Combine(DirectoryInfo, $"{hat.ClimbID}.png"));

            if (hat.FlipID != null)
                hat.FlipHash = GenerateHash(Path.Combine(DirectoryInfo, $"{hat.FlipID}.png"));

            if (hat.BackFlipID != null)
                hat.BackFlipHash = GenerateHash(Path.Combine(DirectoryInfo, $"{hat.BackFlipID}.png"));

            if (hat.FloorID != null)
                hat.FloorHash = GenerateHash(Path.Combine(DirectoryInfo, $"{hat.FloorID}.png"));

            if (hat.FloorFlipID != null)
                hat.FloorFlipHash = GenerateHash(Path.Combine(DirectoryInfo, $"{hat.FloorFlipID}.png"));

            if (hat.ClimbFlipID != null)
                hat.ClimbFlipHash = GenerateHash(Path.Combine(DirectoryInfo, $"{hat.ClimbFlipID}.png"));

            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0f;
                UpdateSplashPatch.SetText($"Generating Hat Hashes ({i + 1}/{response.Length})");
                yield return EndFrame();
            }
        }
    }
}