namespace TownOfUsReworked.Managers;

public static class CustomVisorManager
{
    public static readonly Dictionary<string, CustomVisor> CustomVisorRegistry = [];

    public static void CreateVisorBehaviour(CustomVisor cv)
    {
        var path = Path.Combine(TownOfUsReworked.Visors, $"{cv.ID}.png");

        if (cv.StreamOnly)
            path = Path.Combine(TownOfUsReworked.Visors, "Stream", $"{cv.ID}.png");
        else if (cv.TestOnly)
            path = Path.Combine(TownOfUsReworked.Visors, "Test", $"{cv.ID}.png");

        var viewData = ScriptableObject.CreateInstance<VisorViewData>().DontDestroy();
        viewData.IdleFrame = CustomCosmeticsManager.CreateCosmeticSprite(path, CosmeticTypeEnum.Visor);
        viewData.FloorFrame = cv.FloorID != null ? CustomCosmeticsManager.CreateCosmeticSprite(path, CosmeticTypeEnum.Visor) : viewData.IdleFrame;
        viewData.LeftIdleFrame = cv.FlipID != null ? CustomCosmeticsManager.CreateCosmeticSprite(path, CosmeticTypeEnum.Visor) : null;
        viewData.ClimbFrame = cv.ClimbID != null ? CustomCosmeticsManager.CreateCosmeticSprite(path, CosmeticTypeEnum.Visor) : null;
        viewData.MatchPlayerColor = cv.Adaptive;

        var visor = ScriptableObject.CreateInstance<VisorData>().DontDestroy();
        visor.name = cv.Name;
        visor.displayOrder = 99;
        visor.ProductId = "customVisor_" + cv.Name.Replace(' ', '_');
        visor.ChipOffset = new(0f, 0.2f);
        visor.Free = true;
        visor.behindHats = !cv.InFront;
        visor.NotInStore = true;
        visor.PreviewCrewmateColor = cv.Adaptive;

        cv.Artist ??= "Unknown";
        cv.ViewData = viewData;
        cv.CosmeticData = visor;

        CustomVisorRegistry[visor.ProductId] = cv;
    }

    public static IEnumerable<string> GenerateDownloadList(IEnumerable<CustomVisor> visors)
    {
        foreach (var visor in visors)
        {
            if (visor.StreamOnly && !TownOfUsReworked.IsStream)
                continue;

            if (AssetLoader.ShouldDownload(Path.Combine(TownOfUsReworked.Visors, $"{visor.ID}.png"), visor.MainHash))
                yield return visor.ID;

            if (visor.FlipID != null && AssetLoader.ShouldDownload(Path.Combine(TownOfUsReworked.Visors, $"{visor.FlipID}.png"), visor.FlipHash))
                yield return visor.FlipID;

            if (visor.ClimbID != null && AssetLoader.ShouldDownload(Path.Combine(TownOfUsReworked.Visors, $"{visor.ClimbID}.png"), visor.ClimbHash))
                yield return visor.ClimbID;

            if (visor.FloorID != null && AssetLoader.ShouldDownload(Path.Combine(TownOfUsReworked.Visors, $"{visor.FloorID}.png"), visor.FloorHash))
                yield return visor.FloorID;
        }
    }
}