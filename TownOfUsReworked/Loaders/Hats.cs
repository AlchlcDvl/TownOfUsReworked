namespace TownOfUsReworked.Loaders;

public sealed class HatLoader : BaseCosmeticLoader<HatViewData, HatData, CustomHat>
{
    protected override string DirectoryInfo => TownOfUsReworked.Hats;
    protected override string Manifest => "Hats";

    protected override CosmeticTypeEnum CosmeticType => CosmeticTypeEnum.Hat;

    protected override void LoadData(CustomHat item, string path, HatViewData viewData, PreviewViewData preview, HatData data)
    {
        viewData.MainImage = CreateCosmeticSprite(path, item.MainID, CosmeticTypeEnum.Hat);
        viewData.BackImage = item.BackID is not null ? CreateCosmeticSprite(path, item.BackID, CosmeticTypeEnum.Hat) : null;
        viewData.ClimbImage = item.ClimbID is not null ? CreateCosmeticSprite(path, item.ClimbID, CosmeticTypeEnum.Hat) : null;
        viewData.LeftBackImage = item.BackFlipID is not null ? CreateCosmeticSprite(path, item.BackFlipID, CosmeticTypeEnum.Hat) : viewData.BackImage;
        viewData.LeftClimbImage = item.ClimbFlipID is not null ? CreateCosmeticSprite(path, item.ClimbFlipID, CosmeticTypeEnum.Hat) : viewData.ClimbImage;
        viewData.FloorImage = item.FloorID is not null ? CreateCosmeticSprite(path, item.FloorID, CosmeticTypeEnum.Hat) : viewData.MainImage;
        viewData.LeftMainImage = item.FlipID is not null ? CreateCosmeticSprite(path, item.FlipID, CosmeticTypeEnum.Hat) : viewData.MainImage;
        viewData.LeftFloorImage = item.FloorFlipID is not null ? CreateCosmeticSprite(path, item.FloorFlipID, CosmeticTypeEnum.Hat) : viewData.FloorImage;
        viewData.MatchPlayerColor = item.Adaptive;

        preview.PreviewSprite = viewData.MainImage;

        data.NoBounce = item.NoBounce;
        data.InFront = IsNullEmptyOrWhiteSpace(item.BackID) && IsNullEmptyOrWhiteSpace(item.BackFlipID);
        data.PreviewCrewmateColor = item.Adaptive;
        data.ViewDataRef = new CustomAddressable<HatViewData>(viewData, data.ProductId).Ref;
    }
}