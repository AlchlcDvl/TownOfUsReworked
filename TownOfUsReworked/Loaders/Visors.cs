namespace TownOfUsReworked.Loaders;

public sealed class VisorLoader : BaseCosmeticLoader<CustomVisor>
{
    public static readonly Dictionary<string, CustomVisor> CustomVisorRegistry = [];

    protected override string DirectoryInfo => TownOfUsReworked.Visors;
    protected override string Manifest => "Visors";

    protected override void LoadAsset(CustomVisor item, int i)
    {
        var path = DirectoryInfo;

        if (item.StreamOnly)
            path = Path.Combine(DirectoryInfo, "Stream");
        else if (item.TestOnly)
            path = Path.Combine(DirectoryInfo, "Test");

        var viewData = ScriptableObject.CreateInstance<VisorViewData>().DontDestroy();
        viewData.IdleFrame = CreateCosmeticSprite(path, item.MainID, CosmeticTypeEnum.Visor);
        viewData.FloorFrame = item.FloorID != null ? CreateCosmeticSprite(path, item.FloorID, CosmeticTypeEnum.Visor) : viewData.IdleFrame;
        viewData.LeftIdleFrame = item.FlipID != null ? CreateCosmeticSprite(path, item.FlipID, CosmeticTypeEnum.Visor) : null;
        viewData.ClimbFrame = item.ClimbID != null ? CreateCosmeticSprite(path, item.ClimbID, CosmeticTypeEnum.Visor) : null;
        viewData.MatchPlayerColor = item.Adaptive;

        var preview = ScriptableObject.CreateInstance<PreviewViewData>().DontDestroy();
        preview.PreviewSprite = viewData.IdleFrame;

        var visor = ScriptableObject.CreateInstance<VisorData>().DontDestroy();
        visor.name = item.Name;
        visor.displayOrder = 99;
        visor.ProductId = "customVisor_" + item.Name.Replace(' ', '_');
        visor.ChipOffset = new(0f, 0.2f);
        visor.Free = true;
        visor.behindHats = !item.InFront;
        visor.NotInStore = true;
        visor.PreviewCrewmateColor = item.Adaptive;
        visor.ViewDataRef = new CustomAddressable<VisorViewData>(viewData, visor.ProductId).Ref;
        visor.PreviewData = new CustomAddressable<PreviewViewData>(preview, $"{visor.ProductId}_preview").Ref;

        item.Artist ??= "Unknown";
        item.ViewData = viewData;
        item.CosmeticData = visor;

        CustomVisorRegistry[visor.ProductId] = item;
    }
}