namespace TownOfUsReworked.Loaders;

public sealed class VisorLoader : BaseCosmeticLoader<VisorViewData, VisorData, CustomVisor>
{
    protected override string DirectoryInfo => TownOfUsReworked.Visors;
    protected override string Manifest => "Visors";

    protected override CosmeticTypeEnum CosmeticType => CosmeticTypeEnum.Visor;

    protected override void LoadData(CustomVisor item, string path, VisorViewData viewData, PreviewViewData preview, VisorData data)
    {
        viewData.IdleFrame = CreateCosmeticSprite(path, item.MainID, CosmeticTypeEnum.Visor);
        viewData.FloorFrame = item.FloorID is not null ? CreateCosmeticSprite(path, item.FloorID, CosmeticTypeEnum.Visor) : viewData.IdleFrame;
        viewData.LeftIdleFrame = item.FlipID is not null ? CreateCosmeticSprite(path, item.FlipID, CosmeticTypeEnum.Visor) : null;
        viewData.ClimbFrame = item.ClimbID is not null ? CreateCosmeticSprite(path, item.ClimbID, CosmeticTypeEnum.Visor) : null;
        viewData.MatchPlayerColor = item.Adaptive;

        preview.PreviewSprite = viewData.IdleFrame;

        data.behindHats = !item.InFront;
        data.PreviewCrewmateColor = item.Adaptive;
        data.ViewDataRef = new CustomAddressable<VisorViewData>(viewData, data.ProductId).Ref;
    }
}