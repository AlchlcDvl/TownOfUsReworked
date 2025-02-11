namespace TownOfUsReworked.Managers;

public static class CustomHatManager
{
    public static readonly Dictionary<string, CustomHat> CustomHatRegistry = [];

    public static void CreateHatBehaviour(CustomHat ch)
    {
        var path = Path.Combine(TownOfUsReworked.Hats, $"{ch.ID}.png");

        if (ch.StreamOnly)
            path = Path.Combine(TownOfUsReworked.Hats, "Stream", $"{ch.ID}.png");
        else if (ch.TestOnly)
            path = Path.Combine(TownOfUsReworked.Hats, "Test", $"{ch.ID}.png");

        var viewData = ScriptableObject.CreateInstance<HatViewData>().DontDestroy();
        viewData.MainImage = CustomCosmeticsManager.CreateCosmeticSprite(path, CosmeticTypeEnum.Hat);
        viewData.BackImage = ch.BackID != null ? CustomCosmeticsManager.CreateCosmeticSprite(path, CosmeticTypeEnum.Hat) : null;
        viewData.ClimbImage = ch.ClimbID != null ? CustomCosmeticsManager.CreateCosmeticSprite(path, CosmeticTypeEnum.Hat) : null;
        viewData.LeftBackImage = ch.BackFlipID != null ? CustomCosmeticsManager.CreateCosmeticSprite(path, CosmeticTypeEnum.Hat) : viewData.BackImage;
        viewData.LeftClimbImage = ch.ClimbFlipID != null ? CustomCosmeticsManager.CreateCosmeticSprite(path, CosmeticTypeEnum.Hat) : viewData.ClimbImage;
        viewData.FloorImage = ch.FloorID != null ? CustomCosmeticsManager.CreateCosmeticSprite(path, CosmeticTypeEnum.Hat) : viewData.MainImage;
        viewData.LeftMainImage = ch.FlipID != null ? CustomCosmeticsManager.CreateCosmeticSprite(path, CosmeticTypeEnum.Hat) : viewData.MainImage;
        viewData.LeftFloorImage = ch.FloorFlipID != null ? CustomCosmeticsManager.CreateCosmeticSprite(path, CosmeticTypeEnum.Hat) : viewData.FloorImage;
        viewData.MatchPlayerColor = ch.Adaptive;

        var preview = ScriptableObject.CreateInstance<PreviewViewData>().DontDestroy();
        preview.PreviewSprite = viewData.MainImage;

        var hat = ScriptableObject.CreateInstance<HatData>().DontDestroy();
        hat.name = ch.Name;
        hat.displayOrder = 99;
        hat.ProductId = "customHat_" + ch.Name.Replace(' ', '_');
        hat.InFront = IsNullEmptyOrWhiteSpace(ch.BackID) && IsNullEmptyOrWhiteSpace(ch.BackFlipID);
        hat.NoBounce = ch.NoBounce;
        hat.ChipOffset = new(0f, 0.2f);
        hat.Free = true;
        hat.NotInStore = true;
        hat.PreviewCrewmateColor = ch.Adaptive;
        hat.ViewDataRef = new CustomAddressable<HatViewData>(viewData, hat.ProductId).Ref;
        hat.PreviewData = new CustomAddressable<PreviewViewData>(preview, $"{hat.ProductId}_preview").Ref;

        ch.Artist ??= "Unknown";
        ch.ViewData = viewData;
        ch.CosmeticData = hat;

        CustomHatRegistry[hat.ProductId] = ch;
    }

    public static IEnumerable<string> GenerateDownloadList(IEnumerable<CustomHat> hats, HashAlgorithm hasher)
    {
        foreach (var hat in hats)
        {
            if (hat.StreamOnly && !TownOfUsReworked.IsStream)
                continue;

            if (AssetLoader.ShouldDownload(Path.Combine(TownOfUsReworked.Hats, $"{hat.ID}.png"), hat.MainHash, hasher))
                yield return hat.ID;

            if (hat.FlipID != null && AssetLoader.ShouldDownload(Path.Combine(TownOfUsReworked.Hats, $"{hat.FlipID}.png"), hat.FlipHash, hasher))
                yield return hat.FlipID;

            if (hat.ClimbID != null && AssetLoader.ShouldDownload(Path.Combine(TownOfUsReworked.Hats, $"{hat.ClimbID}.png"), hat.ClimbHash, hasher))
                yield return hat.ClimbID;

            if (hat.FloorID != null && AssetLoader.ShouldDownload(Path.Combine(TownOfUsReworked.Hats, $"{hat.FloorID}.png"), hat.FloorHash, hasher))
                yield return hat.FloorID;

            if (hat.BackFlipID != null && AssetLoader.ShouldDownload(Path.Combine(TownOfUsReworked.Hats, $"{hat.BackFlipID}.png"), hat.BackFlipHash, hasher))
                yield return hat.BackFlipID;

            if (hat.BackID != null && AssetLoader.ShouldDownload(Path.Combine(TownOfUsReworked.Hats, $"{hat.BackID}.png"), hat.BackHash, hasher))
                yield return hat.BackID;

            if (hat.FloorFlipID != null && AssetLoader.ShouldDownload(Path.Combine(TownOfUsReworked.Hats, $"{hat.FloorFlipID}.png"), hat.FloorFlipHash, hasher))
                yield return hat.FloorFlipID;

            if (hat.ClimbFlipID != null && AssetLoader.ShouldDownload(Path.Combine(TownOfUsReworked.Hats, $"{hat.ClimbFlipID}.png"), hat.ClimbFlipHash, hasher))
                yield return hat.ClimbFlipID;
        }
    }
}