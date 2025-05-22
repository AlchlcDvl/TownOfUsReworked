
namespace TownOfUsReworked.Loaders;

public sealed class HatLoader : BaseCosmeticLoader<HatViewData, HatData, CustomHat>
{
    protected override string DirectoryInfo => TownOfUsReworked.Hats;
    protected override string Manifest => "Hats";

    protected override CosmeticsLayer.CosmeticKind CosmeticType => CosmeticsLayer.CosmeticKind.HAT;

    protected override void LoadData(CustomHat item, string path, HatViewData viewData, PreviewViewData preview, HatData data)
    {
        viewData.MainImage = CreateCosmeticSprite(path, item.MainID, CosmeticType);
        viewData.BackImage = item.BackID is not null ? CreateCosmeticSprite(path, item.BackID, CosmeticType) : null;
        viewData.ClimbImage = item.ClimbID is not null ? CreateCosmeticSprite(path, item.ClimbID, CosmeticType) : null;
        viewData.LeftBackImage = item.BackFlipID is not null ? CreateCosmeticSprite(path, item.BackFlipID, CosmeticType) : viewData.BackImage;
        viewData.LeftClimbImage = item.ClimbFlipID is not null ? CreateCosmeticSprite(path, item.ClimbFlipID, CosmeticType) : viewData.ClimbImage;
        viewData.FloorImage = item.FloorID is not null ? CreateCosmeticSprite(path, item.FloorID, CosmeticType) : viewData.MainImage;
        viewData.LeftMainImage = item.FlipID is not null ? CreateCosmeticSprite(path, item.FlipID, CosmeticType) : viewData.MainImage;
        viewData.LeftFloorImage = item.FloorFlipID is not null ? CreateCosmeticSprite(path, item.FloorFlipID, CosmeticType) : viewData.FloorImage;
        viewData.MatchPlayerColor = item.Adaptive;

        preview.PreviewSprite = viewData.MainImage;

        data.NoBounce = item.NoBounce;
        data.InFront = item.Behind && IsNullEmptyOrWhiteSpace(item.BackID) && IsNullEmptyOrWhiteSpace(item.BackFlipID);
        data.PreviewCrewmateColor = item.Adaptive;
        data.ViewDataRef = new CustomAddressable<HatViewData>(viewData, data.ProductId).Ref;
    }
}