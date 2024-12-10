namespace TownOfUsReworked.Cosmetics;

public static class CustomVisorManager
{
    public static readonly List<CustomVisor> UnregisteredVisors = [];
    public static readonly List<VisorData> RegisteredVisors = [];
    public static readonly Dictionary<string, VisorExtension> CustomVisorRegistry = [];
    public static readonly Dictionary<string, VisorViewData> CustomVisorViewDatas = [];

    public static VisorData CreateVisorBehaviour(CustomVisor cv)
    {
        var visor = ScriptableObject.CreateInstance<VisorData>().DontDestroy();

        var viewData = ScriptableObject.CreateInstance<VisorViewData>().DontDestroy();
        viewData.IdleFrame = CustomCosmeticsManager.CreateCosmeticSprite(GetPath(cv, cv.ID), CosmeticTypeEnum.Visor);
        viewData.FloorFrame = cv.FloorID != null ? CustomCosmeticsManager.CreateCosmeticSprite(GetPath(cv, cv.FloorID), CosmeticTypeEnum.Visor) : viewData.IdleFrame;
        viewData.LeftIdleFrame = cv.FlipID != null ? CustomCosmeticsManager.CreateCosmeticSprite(GetPath(cv, cv.FlipID), CosmeticTypeEnum.Visor) : null;
        viewData.ClimbFrame = cv.ClimbID != null ? CustomCosmeticsManager.CreateCosmeticSprite(GetPath(cv, cv.ClimbID), CosmeticTypeEnum.Visor) : null;
        viewData.MatchPlayerColor = cv.Adaptive;

        visor.name = cv.Name;
        visor.displayOrder = 99;
        visor.ProductId = "customVisor_" + cv.Name.Replace(' ', '_');
        visor.ChipOffset = new(0f, 0.2f);
        visor.Free = true;
        visor.behindHats = !cv.InFront;
        visor.NotInStore = true;
        visor.ViewDataRef = new(viewData.Pointer);
        visor.CreateAddressableAsset();

        var extend = new VisorExtension()
        {
            Artist = cv.Artist ?? "Unknown",
            StreamOnly = cv.StreamOnly,
            TestOnly = cv.TestOnly,
            FloorImage = viewData.FloorFrame,
            ClimbImage = viewData.ClimbFrame
        };
        CustomVisorRegistry.TryAdd(visor.name, extend);
        CustomVisorViewDatas.TryAdd(visor.ProductId, viewData);
        return visor;
    }

    private static string GetPath(CustomVisor cv, string id)
    {
        var path = Path.Combine(TownOfUsReworked.Visors, $"{id}.png");

        if (cv.StreamOnly)
            path = Path.Combine(TownOfUsReworked.Visors, "Stream", $"{id}.png");
        else if (cv.TestOnly)
            path = Path.Combine(TownOfUsReworked.Visors, "Test", $"{id}.png");

        return path;
    }

    public static List<string> GenerateDownloadList(IEnumerable<CustomVisor> visors)
    {
        var markedfordownload = new List<string>();

        foreach (var visor in visors)
        {
            if (visor.StreamOnly && !TownOfUsReworked.IsStream)
                continue;

            if (!File.Exists(Path.Combine(TownOfUsReworked.Visors, $"{visor.ID}.png")))
                markedfordownload.Add(visor.ID);

            if (visor.FlipID != null && !File.Exists(Path.Combine(TownOfUsReworked.Visors, $"{visor.FlipID}.png")))
                markedfordownload.Add(visor.FlipID);

            if (visor.ClimbID != null && !File.Exists(Path.Combine(TownOfUsReworked.Visors, $"{visor.ClimbID}.png")))
                markedfordownload.Add(visor.ClimbID);

            if (visor.FloorID != null && !File.Exists(Path.Combine(TownOfUsReworked.Visors, $"{visor.FloorID}.png")))
                markedfordownload.Add(visor.FloorID);
        }

        return markedfordownload;
    }

    public static VisorExtension GetExtention(this VisorData visor)
    {
        if (!visor)
            return null;

        CustomVisorRegistry.TryGetValue(visor.name, out var ret);
        return ret;
    }
}