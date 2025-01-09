namespace TownOfUsReworked.Managers;

public static class CustomNameplateManager
{
    public static readonly Dictionary<string, CustomNameplate> CustomNameplateRegistry = [];

    public static void CreateNameplateBehaviour(CustomNameplate cn)
    {
        var path = Path.Combine(TownOfUsReworked.Nameplates, $"{cn.ID}.png");

        if (cn.StreamOnly)
            path = Path.Combine(TownOfUsReworked.Nameplates, "Stream", $"{cn.ID}.png");
        else if (cn.TestOnly)
            path = Path.Combine(TownOfUsReworked.Nameplates, "Test", $"{cn.ID}.png");

        var viewData = ScriptableObject.CreateInstance<NamePlateViewData>().DontDestroy();
        viewData.Image = CustomCosmeticsManager.CreateCosmeticSprite(path, CosmeticTypeEnum.Nameplate);

        var nameplate = ScriptableObject.CreateInstance<NamePlateData>().DontDestroy();
        nameplate.PreviewCrewmateColor = false;
        nameplate.name = cn.Name;
        nameplate.displayOrder = 99;
        nameplate.ProductId = "customNameplate_" + cn.Name.Replace(' ', '_');
        nameplate.ChipOffset = new(0f, 0.2f);
        nameplate.Free = true;
        nameplate.NotInStore = true;
        nameplate.ViewDataRef = new(viewData.Pointer);

        cn.Artist ??= "Unknown";
        cn.ViewData = viewData;
        cn.CosmeticData = nameplate;

        CustomNameplateRegistry[nameplate.ProductId] = cn;
    }

    public static IEnumerable<string> GenerateDownloadList(IEnumerable<CustomNameplate> nameplates)
    {
        foreach (var nameplate in nameplates)
        {
            if (nameplate.StreamOnly && !TownOfUsReworked.IsStream)
                continue;

            if (!File.Exists(Path.Combine(TownOfUsReworked.Nameplates, $"{nameplate.ID}.png")))
                yield return nameplate.ID;
        }
    }
}