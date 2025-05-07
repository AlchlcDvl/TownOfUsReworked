namespace TownOfUsReworked.Loaders;

public sealed class NameplateLoader : BaseCosmeticLoader<CustomNameplate>
{
    public static readonly Dictionary<string, CustomNameplate> CustomNameplateRegistry = [];

    protected override string DirectoryInfo => TownOfUsReworked.Nameplates;
    protected override string Manifest => "Nameplates";

    protected override void LoadAsset(CustomNameplate item, int i)
    {
        var path = DirectoryInfo;

        if (item.StreamOnly)
            path = Path.Combine(DirectoryInfo, "Stream");
        else if (item.TestOnly)
            path = Path.Combine(DirectoryInfo, "Test");

        var viewData = ScriptableObject.CreateInstance<NamePlateViewData>().DontDestroy();
        viewData.Image = CreateCosmeticSprite(path, item.MainID, CosmeticTypeEnum.Nameplate);

        var preview = ScriptableObject.CreateInstance<PreviewViewData>().DontDestroy();
        preview.PreviewSprite = viewData.Image;

        var nameplate = ScriptableObject.CreateInstance<NamePlateData>().DontDestroy();
        nameplate.PreviewCrewmateColor = false;
        nameplate.name = item.Name;
        nameplate.displayOrder = 99;
        nameplate.ProductId = "customNameplate_" + item.Name.Replace(' ', '_');
        nameplate.ChipOffset = new(0f, 0.2f);
        nameplate.Free = true;
        nameplate.NotInStore = true;
        nameplate.ViewDataRef = new CustomAddressable<NamePlateViewData>(viewData, nameplate.ProductId).Ref;
        nameplate.PreviewData = new CustomAddressable<PreviewViewData>(preview, $"{nameplate.ProductId}_preview").Ref;

        item.Artist ??= "Unknown";
        item.ViewData = viewData;
        item.CosmeticData = nameplate;

        CustomNameplateRegistry[nameplate.ProductId] = item;
    }
}