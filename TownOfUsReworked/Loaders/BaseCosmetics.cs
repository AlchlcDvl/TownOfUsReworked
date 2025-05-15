// ReSharper disable HeuristicUnreachableCode
#pragma warning disable CS0162 // Unreachable code detected

namespace TownOfUsReworked.Loaders;

public abstract class BaseCosmeticLoader<T> : AssetLoader<T>
    where T : CustomCosmetic
{
    protected override bool HasStreamAssets => true;

    protected override void LoadStreamAssets(List<T> response)
    {
        var filePath = Path.Combine(DirectoryInfo, "Stream", $"{Manifest}.json");

        if (!File.Exists(filePath))
            return;

        var data = JsonSerializer.Deserialize<T[]>(File.ReadAllBytes(filePath));
        data.Do(x => x.StreamOnly = true);
        response.AddRange(data);
        Array.Clear(data);
    }
}