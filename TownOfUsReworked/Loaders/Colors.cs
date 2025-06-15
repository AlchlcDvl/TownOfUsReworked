// ReSharper disable HeuristicUnreachableCode
using static TownOfUsReworked.Managers.CustomColorManager;
#pragma warning disable CS0162 // Unreachable code detected

namespace TownOfUsReworked.Loaders;

public sealed class ColorLoader : BaseCosmeticLoader<CustomColor>
{
    protected override string DirectoryInfo => TownOfUsReworked.Colors;
    protected override string Manifest => "Colors";

    protected override void LoadAsset(CustomColor item, int i)
    {
        if (item.StreamOnly && !TownOfUsReworked.IsStream)
            return;

        item.ColorID = i;

        if (item.MainColorValues is not null)
            item.MainColors = [ .. item.MainColorValues.Select(FromHex) ];

        if (item.ShadowColorValues is not null)
            item.ShadowColors = [ .. item.ShadowColorValues.Select(FromHex) ];

        if (!item.Default)
        {
            var id = $"Colors.{item.Name}";
            item.StringID = TranslationManager.GetOrAddName(id);

            if (TownOfUsReworked.IsDev)
                TranslationManager.DebugId(id);
        }

        AllColors[item.ColorID] = item;
    }

    protected override void AfterLoading(List<CustomColor> response)
    {
        Palette.ColorNames = response.Select(x => x.StringID).ToArray();
        Palette.TextColors = Palette.PlayerColors = response.Select(x => (Color32)x.GetMainColor()).ToArray();
        Palette.ShadowColors = response.Select(x => (Color32)x.GetShadowColor()).ToArray();
        Palette.TextOutlineColors = Palette.PlayerColors.Select(x => x.Alternate()).ToArray();

        if (TownOfUsReworked.IsDev)
        {
            foreach (var id in TranslationManager.AllTranslations.Keys.Where(x => x.StartsWith("Colors")))
            {
                if (!AllColors.Values.Any(x => !x.Default && id == $"Colors.{x.Name}"))
                    Fatal(id);
            }
        }
    }
}