// ReSharper disable HeuristicUnreachableCode
#pragma warning disable CS0162 // Unreachable code detected

namespace TownOfUsReworked.Loaders;

public abstract class BaseCosmeticLoader<TView, TData, TAsset> : BaseCosmeticLoader<TAsset>
    where TView : ScriptableObject
    where TData : CosmeticData
    where TAsset : CustomCosmetic<TView, TData>
{
    public static readonly Dictionary<string, TAsset> CustomCosmeticRegistry = [];

    protected override bool Downloading => true;
    protected override string FileExtension => "png";

    protected abstract CosmeticTypeEnum CosmeticType { get; }

    private readonly Dictionary<PropertyInfo, PropertyInfo> IDsAndHashes = [];

    protected abstract void LoadData(TAsset item, string path, TView viewData, PreviewViewData preview, TData data);

    protected override void BeforeLoading()
    {
        if (IDsAndHashes.Count > 0)
            return;

        var props = typeof(TAsset).GetProperties(AccessTools.all);
        var ids = props.Where(x => x.Name.EndsWith("ID") && x.Name != "ID");
        var hashes = props.Where(x => x.Name.EndsWith("Hash"));

        foreach (var id in ids)
        {
            if (hashes.TryFinding(x => x.Name == id.Name.Replace("ID", "Hash"), out var hash))
                IDsAndHashes.Add(id, hash);
        }
    }

    protected override IEnumerable<string> GenerateDownloadList(TAsset[] response, HashAlgorithm hasher)
    {
        foreach (var item in response)
        {
            if ((item.StreamOnly && !TownOfUsReworked.IsStream) || item.IsCustom)
                continue;

            foreach (var (id, hash) in IDsAndHashes)
            {
                var idString = id.GetValue<string>(item);

                if (idString is not null && ShouldDownload(Path.Combine(DirectoryInfo, $"{idString}.png"), hash.GetValue<string>(item), hasher))
                    yield return idString;
            }
        }
    }

    protected override void LoadAsset(TAsset item, int i)
    {
        item.Artist ??= "Unknown";
        var path = DirectoryInfo;

        if (item.StreamOnly)
            path = Path.Combine(DirectoryInfo, "Stream");
        else if (item.TestOnly)
            path = Path.Combine(DirectoryInfo, "Test");

        var data = ScriptableObject.CreateInstance<TData>().DontDestroy();
        var viewData = ScriptableObject.CreateInstance<TView>().DontDestroy();
        var preview = ScriptableObject.CreateInstance<PreviewViewData>().DontDestroy();

        data.name = item.Name;
        data.ProductId = $"custom{CosmeticType}_" + item.ID.Replace(' ', '_');
        data.ChipOffset = new(0f, 0.2f);
        data.Free = true;
        data.NotInStore = true;

        LoadData(item, path, viewData, preview, data);

        data.PreviewData = new CustomAddressable<PreviewViewData>(preview, $"{item.ID}_preview").Ref;

        item.ViewData = viewData;
        item.CosmeticData = data;
        item.PreviewData = preview;

        CustomCosmeticRegistry[data.ProductId] = item;
    }

    protected override void GenerateHash(TAsset item, HashAlgorithm hasher)
    {
        foreach (var (id, hash) in IDsAndHashes)
        {
            var idString = id.GetValue<string>(item);

            if (idString is not null)
                hash.SetValue(item, GenerateHash(Path.Combine(DirectoryInfo, $"{idString}.png"), hasher));
        }
    }

    protected override void AfterLoading(List<TAsset> response) => IDsAndHashes.Clear();

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