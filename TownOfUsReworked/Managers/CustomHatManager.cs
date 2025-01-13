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

        var hat = ScriptableObject.CreateInstance<HatData>().DontDestroy();
        hat.name = ch.Name;
        hat.displayOrder = 99;
        hat.ProductId = "customHat_" + ch.Name.Replace(' ', '_');
        hat.InFront = !ch.Behind;
        hat.NoBounce = ch.NoBounce;
        hat.ChipOffset = new(0f, 0.2f);
        hat.Free = true;
        hat.NotInStore = true;
        hat.PreviewCrewmateColor = ch.Adaptive;

        ch.Artist ??= "Unknown";
        ch.ViewData = viewData;
        ch.CosmeticData = hat;

        CustomHatRegistry[hat.ProductId] = ch;
    }

    public static IEnumerable<string> GenerateDownloadList(IEnumerable<CustomHat> hats)
    {
        foreach (var hat in hats)
        {
            if (hat.StreamOnly && !TownOfUsReworked.IsStream)
                continue;

            if (!File.Exists(Path.Combine(TownOfUsReworked.Hats, $"{hat.ID}.png")))
                yield return hat.ID;

            if (hat.BackID != null && !File.Exists(Path.Combine(TownOfUsReworked.Hats, $"{hat.BackID}.png")))
                yield return hat.BackID;

            if (hat.ClimbID != null && !File.Exists(Path.Combine(TownOfUsReworked.Hats, $"{hat.ClimbID}.png")))
                yield return hat.ClimbID;

            if (hat.FlipID != null && !File.Exists(Path.Combine(TownOfUsReworked.Hats, $"{hat.FlipID}.png")))
                yield return hat.FlipID;

            if (hat.BackFlipID != null && !File.Exists(Path.Combine(TownOfUsReworked.Hats, $"{hat.BackFlipID}.png")))
                yield return hat.BackFlipID;

            if (hat.FloorID != null && !File.Exists(Path.Combine(TownOfUsReworked.Hats, $"{hat.FloorID}.png")))
                yield return hat.FloorID;

            if (hat.FloorFlipID != null && !File.Exists(Path.Combine(TownOfUsReworked.Hats, $"{hat.FloorFlipID}.png")))
                yield return hat.FloorFlipID;

            if (hat.ClimbFlipID != null && !File.Exists(Path.Combine(TownOfUsReworked.Hats, $"{hat.ClimbFlipID}.png")))
                yield return hat.ClimbFlipID;
        }
    }
}