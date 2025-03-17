using static TownOfUsReworked.Managers.CustomColorManager;
// ReSharper disable HeuristicUnreachableCode
#pragma warning disable CS0162 // Unreachable code detected

namespace TownOfUsReworked.Loaders;

public sealed class ColorLoader : BaseCosmeticLoader<CustomColor>
{
    protected override string DirectoryInfo => TownOfUsReworked.Colors;
    protected override string Manifest => "Colors";
    protected override bool Downloading => false;
    protected override string FileExtension => "";

    protected override void BeforeLoading() {} // I don't want the type checking to occur for custom colors because they don't have hashes to compare with

    protected override IEnumerable<string> GenerateDownloadList(CustomColor[] response, HashAlgorithm hasher) => []; // Same here since there's nothing to download

    protected override void LoadAsset(CustomColor item, int i)
    {
        if (item.StreamOnly && !TownOfUsReworked.IsStream)
            return;

        item.ColorID = i;

        if (item.MainColorValues != null)
            item.MainColors = [ .. item.MainColorValues.Select(FromHex) ];

        if (item.ShadowColorValues != null)
            item.ShadowColors = [ .. item.ShadowColorValues.Select(FromHex) ];

        item.TimeSpeed = item.TimeSpeed == 0f ? 1f : item.TimeSpeed;

        if (!item.Default)
            item.StringID = TranslationManager.GetOrAddName($"Colors.{item.Name}");

        AllColors[item.ColorID] = item;
    }

    protected override void AfterLoading(List<CustomColor> response)
    {
        Palette.ColorNames = response.Select(x => x.StringID).ToArray();
        Palette.TextColors = Palette.PlayerColors = response.Select(x => (Color32)x.GetMainColor()).ToArray();
        Palette.ShadowColors = response.Select(x => (Color32)x.GetShadowColor()).ToArray();
        Palette.TextOutlineColors = Palette.PlayerColors.Select(x => x.Alternate()).ToArray();
    }
}