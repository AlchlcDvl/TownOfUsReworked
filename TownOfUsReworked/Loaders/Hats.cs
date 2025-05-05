namespace TownOfUsReworked.Loaders;

public sealed class HatLoader : BaseCosmeticLoader<CustomHat>
{
    public static readonly Dictionary<string, CustomHat> CustomHatRegistry = [];

    protected override string DirectoryInfo => TownOfUsReworked.Hats;
    protected override string Manifest => "Hats";

    protected override void LoadAsset(CustomHat item, int i)
    {
        var path = DirectoryInfo;

        if (item.StreamOnly)
            path = Path.Combine(DirectoryInfo, "Stream");
        else if (item.TestOnly)
            path = Path.Combine(DirectoryInfo, "Test");

        var viewData = ScriptableObject.CreateInstance<HatViewData>();
        viewData.MainImage = CreateCosmeticSprite(path, item.MainID, CosmeticTypeEnum.Hat);
        viewData.BackImage = item.BackID != null ? CreateCosmeticSprite(path, item.BackID, CosmeticTypeEnum.Hat) : null;
        viewData.ClimbImage = item.ClimbID != null ? CreateCosmeticSprite(path, item.ClimbID, CosmeticTypeEnum.Hat) : null;
        viewData.LeftBackImage = item.BackFlipID != null ? CreateCosmeticSprite(path, item.BackFlipID, CosmeticTypeEnum.Hat) : viewData.BackImage;
        viewData.LeftClimbImage = item.ClimbFlipID != null ? CreateCosmeticSprite(path, item.ClimbFlipID, CosmeticTypeEnum.Hat) : viewData.ClimbImage;
        viewData.FloorImage = item.FloorID != null ? CreateCosmeticSprite(path, item.FloorID, CosmeticTypeEnum.Hat) : viewData.MainImage;
        viewData.LeftMainImage = item.FlipID != null ? CreateCosmeticSprite(path, item.FlipID, CosmeticTypeEnum.Hat) : viewData.MainImage;
        viewData.LeftFloorImage = item.FloorFlipID != null ? CreateCosmeticSprite(path, item.FloorFlipID, CosmeticTypeEnum.Hat) : viewData.FloorImage;
        viewData.MatchPlayerColor = item.Adaptive;

        var preview = ScriptableObject.CreateInstance<PreviewViewData>();
        preview.PreviewSprite = viewData.MainImage;

        var hat = ScriptableObject.CreateInstance<HatData>();
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