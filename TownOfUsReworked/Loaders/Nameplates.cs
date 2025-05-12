namespace TownOfUsReworked.Loaders;

public sealed class NameplateLoader : BaseCosmeticLoader<NamePlateViewData, NamePlateData, CustomNameplate>
{
    protected override string DirectoryInfo => TownOfUsReworked.Nameplates;
    protected override string Manifest => "Nameplates";

    protected override CosmeticTypeEnum CosmeticType => CosmeticTypeEnum.Nameplate;

    protected override void LoadData(CustomNameplate item, string path, NamePlateViewData viewData, PreviewViewData preview, NamePlateData data)
    {
        viewData.Image = CreateCosmeticSprite(path, item.MainID, CosmeticTypeEnum.Nameplate);

        preview.PreviewSprite = viewData.Image;

        data.PreviewCrewmateColor = false;
        data.ViewDataRef = new CustomAddressable<NamePlateViewData>(viewData, data.ProductId).Ref;
    }
}