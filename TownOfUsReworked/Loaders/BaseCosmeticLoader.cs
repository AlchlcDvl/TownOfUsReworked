namespace TownOfUsReworked.Loaders;

public abstract class BaseCosmeticLoader<T> : AssetLoader<T>
    where T : CustomCosmetic
{
    protected override bool HasStreamAssets => true;
    protected override bool Downloading => true;
    protected override string FileExtension => "png";

    private readonly Dictionary<PropertyInfo, PropertyInfo> IDsAndHashes = [];

    protected override void LoadStreamAssets(List<T> response)
    {
        var filePath = Path.Combine(DirectoryInfo, "Stream", $"{Manifest}.json");

        if (!File.Exists(filePath))
            return;

        var data = JsonSerializer.Deserialize<T[]>(File.ReadAllText(filePath));
        data.ForEach(x => x.StreamOnly = true);
        response.AddRange(data);
        Array.Clear(data);
    }

    protected override void BeforeLoading()
    {
        if (IDsAndHashes.Count > 0)
            return;

        var props = typeof(T).GetProperties(AccessTools.all);
        var ids = props.Where(x => x.Name.EndsWith("ID") && x.Name != "ID");
        var hashes = props.Where(x => x.Name.EndsWith("Hash"));

        foreach (var id in ids)
        {
            if (hashes.TryFinding(x => x.Name == id.Name.Replace("ID", "Hash"), out var hash))
                IDsAndHashes.Add(id, hash);
        }
    }

    protected override IEnumerable<string> GenerateDownloadList(T[] response, HashAlgorithm hasher)
    {
        foreach (var item in response)
        {
            if (item.StreamOnly && !TownOfUsReworked.IsStream)
                continue;

            foreach (var (id, hash) in IDsAndHashes)
            {
                var idString = id.GetValue<string>(item);

                if (idString != null && ShouldDownload(Path.Combine(DirectoryInfo, $"{idString}.png"), hash.GetValue<string>(item), hasher))
                    yield return idString;
            }
        }
    }

    protected override void GenerateHash(T item, HashAlgorithm hasher)
    {
        foreach (var (id, hash) in IDsAndHashes)
        {
            var idString = id.GetValue<string>(item);

            if (idString != null)
                hash.SetValue(item, GenerateHash(Path.Combine(DirectoryInfo, $"{idString}.png"), hasher));
        }
    }

    protected override void AfterLoading(List<T> response) => IDsAndHashes.Clear();

    protected static Sprite CreateCosmeticSprite(string dir, string path, CosmeticTypeEnum cosmetic)
    {
        var texture = LoadDiskTexture(Path.Combine(dir, $"{path}.png"));
        return LoadSprite(texture, path.SanitisePath(), cosmetic switch
        {
            CosmeticTypeEnum.Hat or CosmeticTypeEnum.Visor => 100f,
            _ => texture.width * 0.375f
        });
    }
}