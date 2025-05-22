
namespace TownOfUsReworked.Loaders;

public sealed class VisorLoader : BaseCosmeticLoader<VisorViewData, VisorData, CustomVisor>
{
    protected override string DirectoryInfo => TownOfUsReworked.Visors;
    protected override string Manifest => "Visors";

    protected override CosmeticsLayer.CosmeticKind CosmeticType => CosmeticsLayer.CosmeticKind.VISOR;

    protected override void LoadData(CustomVisor item, string path, VisorViewData viewData, PreviewViewData preview, VisorData data)
    {
        viewData.IdleFrame = CreateCosmeticSprite(path, item.MainID, CosmeticType);
        viewData.FloorFrame = item.FloorID is not null ? CreateCosmeticSprite(path, item.FloorID, CosmeticType) : viewData.IdleFrame;
        viewData.LeftIdleFrame = item.FlipID is not null ? CreateCosmeticSprite(path, item.FlipID, CosmeticType) : null;
        viewData.ClimbFrame = item.ClimbID is not null ? CreateCosmeticSprite(path, item.ClimbID, CosmeticType) : null;
        viewData.MatchPlayerColor = item.Adaptive;

        preview.PreviewSprite = viewData.IdleFrame;

        data.behindHats = item.Behind;
        data.PreviewCrewmateColor = item.Adaptive;
        data.ViewDataRef = new CustomAddressable<VisorViewData>(viewData, data.ProductId).Ref;
    }
}