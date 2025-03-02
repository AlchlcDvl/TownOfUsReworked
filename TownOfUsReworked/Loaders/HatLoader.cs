namespace TownOfUsReworked.Loaders;

public class HatLoader : BaseCosmeticLoader<CustomHat>
{
    public static readonly Dictionary<string, CustomHat> CustomHatRegistry = [];

    protected override string DirectoryInfo => TownOfUsReworked.Hats;
    protected override string Manifest => "Hats";
    protected override string FileExtension => "png";

    protected override void LoadAsset(CustomHat item, int i)
    {
        var path = Path.Combine(TownOfUsReworked.Hats, $"{item.ID}.png");

        if (item.StreamOnly)
            path = Path.Combine(TownOfUsReworked.Hats, "Stream", $"{item.ID}.png");
        else if (item.TestOnly)
            path = Path.Combine(TownOfUsReworked.Hats, "Test", $"{item.ID}.png");

        var viewData = ScriptableObject.CreateInstance<HatViewData>().DontDestroy();
        viewData.MainImage = CreateCosmeticSprite(path, CosmeticTypeEnum.Hat);
        viewData.BackImage = item.BackID != null ? CreateCosmeticSprite(path, CosmeticTypeEnum.Hat) : null;
        viewData.ClimbImage = item.ClimbID != null ? CreateCosmeticSprite(path, CosmeticTypeEnum.Hat) : null;
        viewData.LeftBackImage = item.BackFlipID != null ? CreateCosmeticSprite(path, CosmeticTypeEnum.Hat) : viewData.BackImage;
        viewData.LeftClimbImage = item.ClimbFlipID != null ? CreateCosmeticSprite(path, CosmeticTypeEnum.Hat) : viewData.ClimbImage;
        viewData.FloorImage = item.FloorID != null ? CreateCosmeticSprite(path, CosmeticTypeEnum.Hat) : viewData.MainImage;
        viewData.LeftMainImage = item.FlipID != null ? CreateCosmeticSprite(path, CosmeticTypeEnum.Hat) : viewData.MainImage;
        viewData.LeftFloorImage = item.FloorFlipID != null ? CreateCosmeticSprite(path, CosmeticTypeEnum.Hat) : viewData.FloorImage;
        viewData.MatchPlayerColor = item.Adaptive;

        var preview = ScriptableObject.CreateInstance<PreviewViewData>().DontDestroy();
        preview.PreviewSprite = viewData.MainImage;

        var hat = ScriptableObject.CreateInstance<HatData>().DontDestroy();
        hat.name = item.Name;
        hat.displayOrder = 99;
        hat.ProductId = "customHat_" + item.Name.Replace(' ', '_');
        hat.InFront = IsNullEmptyOrWhiteSpace(item.BackID) && IsNullEmptyOrWhiteSpace(item.BackFlipID);
        hat.NoBounce = item.NoBounce;
        hat.ChipOffset = new(0f, 0.2f);
        hat.Free = true;
        hat.NotInStore = true;
        hat.PreviewCrewmateColor = item.Adaptive;
        hat.ViewDataRef = new CustomAddressable<HatViewData>(viewData, hat.ProductId).Ref;
        hat.PreviewData = new CustomAddressable<PreviewViewData>(preview, $"{hat.ProductId}_preview").Ref;

        item.Artist ??= "Unknown";
        item.ViewData = viewData;
        item.CosmeticData = hat;

        CustomHatRegistry[hat.ProductId] = item;
    }
}