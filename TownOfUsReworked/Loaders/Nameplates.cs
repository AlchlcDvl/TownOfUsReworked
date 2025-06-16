namespace TownOfUsReworked.Loaders;

public sealed class NameplateLoader : BaseCosmeticLoader<NamePlateViewData, NamePlateData, CustomNameplate>
{
    private static EnumInjector<CosmeticsLayer.CosmeticKind> Injector = new(true, true);

    public static CosmeticsLayer.CosmeticKind Nameplate = Injector.InjectAndReturn("NAMEPLATE");

    protected override string DirectoryInfo => TownOfUsReworked.Nameplates;
    protected override string Manifest => "Nameplates";

    protected override CosmeticsLayer.CosmeticKind CosmeticType => Nameplate;

    protected override void LoadData(CustomNameplate item, string path, NamePlateViewData viewData, PreviewViewData preview, NamePlateData data)
    {
        viewData.Image = CreateCosmeticSprite(path, item.MainID, CosmeticType);

        preview.PreviewSprite = viewData.Image;

        data.PreviewCrewmateColor = false;
        data.ViewDataRef = new CustomAddressable<NamePlateViewData>(viewData, data.ProductId).Ref;
    }
}