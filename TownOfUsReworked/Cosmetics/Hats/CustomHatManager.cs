namespace TownOfUsReworked.Cosmetics.CustomHats;

public static class CustomHatManager
{
    public static readonly List<CustomHat> UnregisteredHats = [];
    public static readonly List<HatData> RegisteredHats = [];
    public static readonly Dictionary<string, HatViewData> CustomHatViewDatas = [];
    public static readonly Dictionary<string, HatExtension> CustomHatRegistry = [];

    public static HatData CreateHatBehaviour(CustomHat ch)
    {
        var hat = ScriptableObject.CreateInstance<HatData>().DontDestroy();

        var viewData = ScriptableObject.CreateInstance<HatViewData>().DontDestroy();
        viewData.MainImage = CustomCosmeticsManager.CreateCosmeticSprite(GetPath(ch, ch.ID), CosmeticTypeEnum.Hat);
        viewData.BackImage = ch.BackID != null ? CustomCosmeticsManager.CreateCosmeticSprite(GetPath(ch, ch.BackID), CosmeticTypeEnum.Hat) : null;
        viewData.ClimbImage = ch.ClimbID != null ? CustomCosmeticsManager.CreateCosmeticSprite(GetPath(ch, ch.ClimbID), CosmeticTypeEnum.Hat) : null;
        viewData.LeftBackImage = ch.BackFlipID != null ? CustomCosmeticsManager.CreateCosmeticSprite(GetPath(ch, ch.BackFlipID), CosmeticTypeEnum.Hat) : viewData.BackImage;
        viewData.LeftClimbImage = ch.ClimbFlipID != null ? CustomCosmeticsManager.CreateCosmeticSprite(GetPath(ch, ch.ClimbFlipID), CosmeticTypeEnum.Hat) : viewData.ClimbImage;
        viewData.FloorImage = ch.FloorID != null ? CustomCosmeticsManager.CreateCosmeticSprite(GetPath(ch, ch.FloorID), CosmeticTypeEnum.Hat) : viewData.MainImage;
        viewData.LeftMainImage = ch.FlipID != null ? CustomCosmeticsManager.CreateCosmeticSprite(GetPath(ch, ch.FlipID), CosmeticTypeEnum.Hat) : viewData.MainImage;
        viewData.LeftFloorImage = ch.FloorFlipID != null ? CustomCosmeticsManager.CreateCosmeticSprite(GetPath(ch, ch.FloorFlipID), CosmeticTypeEnum.Hat) : viewData.FloorImage;
        viewData.MatchPlayerColor = ch.Adaptive;

        hat.name = ch.Name;
        hat.displayOrder = 99;
        hat.ProductId = "customHat_" + ch.Name.Replace(' ', '_');
        hat.InFront = !ch.Behind;
        hat.NoBounce = ch.NoBounce;
        hat.ChipOffset = new(0f, 0.2f);
        hat.Free = true;
        hat.NotInStore = true;
        hat.ViewDataRef = new(viewData.Pointer);
        hat.CreateAddressableAsset();

        var extend = new HatExtension()
        {
            Artist = ch.Artist ?? "Unknown",
            FlipImage = viewData.LeftMainImage,
            BackFlipImage = viewData.LeftBackImage,
            StreamOnly = ch.StreamOnly,
            TestOnly = ch.TestOnly
        };
        CustomHatRegistry.TryAdd(hat.name, extend);
        CustomHatViewDatas.TryAdd(hat.ProductId, viewData);
        return hat;
    }

    private static string GetPath(CustomHat ch, string id)
    {
        var path = Path.Combine(TownOfUsReworked.Hats, $"{id}.png");

        if (ch.StreamOnly)
            path = Path.Combine(TownOfUsReworked.Hats, "Stream", $"{id}.png");
        else if (ch.TestOnly)
            path = Path.Combine(TownOfUsReworked.Hats, "Test", $"{id}.png");

        return path;
    }

    public static List<string> GenerateDownloadList(List<CustomHat> hats)
    {
        var markedfordownload = new List<string>();

        foreach (var hat in hats)
        {
            if (hat.StreamOnly && !TownOfUsReworked.IsStream)
                continue;

            if (!File.Exists(Path.Combine(TownOfUsReworked.Hats, $"{hat.ID}.png")))
                markedfordownload.Add(hat.ID);

            if (hat.BackID != null && !File.Exists(Path.Combine(TownOfUsReworked.Hats, $"{hat.BackID}.png")))
                markedfordownload.Add(hat.BackID);

            if (hat.ClimbID != null && !File.Exists(Path.Combine(TownOfUsReworked.Hats, $"{hat.ClimbID}.png")))
                markedfordownload.Add(hat.ClimbID);

            if (hat.FlipID != null && !File.Exists(Path.Combine(TownOfUsReworked.Hats, $"{hat.FlipID}.png")))
                markedfordownload.Add(hat.FlipID);

            if (hat.BackFlipID != null && !File.Exists(Path.Combine(TownOfUsReworked.Hats, $"{hat.BackFlipID}.png")))
                markedfordownload.Add(hat.BackFlipID);

            if (hat.FloorID != null && !File.Exists(Path.Combine(TownOfUsReworked.Hats, $"{hat.FloorID}.png")))
                markedfordownload.Add(hat.FloorID);

            if (hat.FloorFlipID != null && !File.Exists(Path.Combine(TownOfUsReworked.Hats, $"{hat.FloorFlipID}.png")))
                markedfordownload.Add(hat.FloorFlipID);

            if (hat.ClimbFlipID != null && !File.Exists(Path.Combine(TownOfUsReworked.Hats, $"{hat.ClimbFlipID}.png")))
                markedfordownload.Add(hat.ClimbFlipID);
        }

        return markedfordownload;
    }

    public static HatExtension GetExtention(this HatData hat)
    {
        if (!hat)
            return null;

        CustomHatRegistry.TryGetValue(hat.name, out var ret);
        return ret;
    }
}