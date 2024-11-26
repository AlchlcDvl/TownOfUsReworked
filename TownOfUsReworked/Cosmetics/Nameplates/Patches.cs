using Innersloth.Assets;
using static TownOfUsReworked.Cosmetics.CustomNameplates.CustomNameplateManager;

namespace TownOfUsReworked.Cosmetics.CustomNameplates;

[HarmonyPatch(typeof(HatManager), nameof(HatManager.GetNamePlateById))]
public static class HatManagerPatch
{
    private static bool IsLoaded;

    public static void Prefix(HatManager __instance)
    {
        if (IsLoaded)
            return;

        var allPlates = __instance.allNamePlates.ToList();
        allPlates.AddRange(RegisteredNameplates);
        __instance.allNamePlates = allPlates.ToArray();
        RegisteredNameplates.Clear();
        IsLoaded = true;
    }
}

[HarmonyPatch(typeof(NameplatesTab), nameof(NameplatesTab.OnEnable))]
public static class NameplatesTabOnEnablePatch
{
    private static TMP_Text Template;

    private static float CreateNameplatePackage(List<NamePlateData> nameplates, string packageName, float YStart, NameplatesTab __instance)
    {
        var isDefaultPackage = "Innersloth" == packageName;

        if (!isDefaultPackage)
            nameplates = [ .. nameplates.OrderBy(x => x.name) ];

        var offset = YStart;

        if (Template)
        {
            var title = UObject.Instantiate(Template, __instance.scroller.Inner);
            var material = title.GetComponent<MeshRenderer>().material;
            material.SetFloat("_StencilComp", 4f);
            material.SetFloat("_Stencil", 1f);
            title.transform.localPosition = new(2.25f, YStart, -1f);
            title.transform.localScale = Vector3.one * 1.5f;
            title.fontSize *= 0.5f;
            title.enableAutoSizing = false;
            Coroutines.Start(PerformTimedAction(0.1f, _ => title.SetText(packageName, true)));
            offset -= 0.8f * __instance.YOffset;
        }

        for (var i = 0; i < nameplates.Count; i++)
        {
            var nameplate = nameplates[i];
            var xpos = __instance.XRange.Lerp(i % __instance.NumPerRow / (__instance.NumPerRow - 1f));
            var ypos = offset - (i / __instance.NumPerRow * __instance.YOffset);
            var colorChip = UObject.Instantiate(__instance.ColorTabPrefab, __instance.scroller.Inner);

            if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
            {
                colorChip.Button.OverrideOnMouseOverListeners(() => __instance.SelectNameplate(nameplate));
                colorChip.Button.OverrideOnMouseOutListeners(() => __instance.SelectNameplate(HatManager.Instance.GetNamePlateById(DataManager.Player.Customization.NamePlate)));
                colorChip.Button.OverrideOnClickListeners(__instance.ClickEquip);
            }
            else
                colorChip.Button.OverrideOnClickListeners(() => __instance.SelectNameplate(nameplate));

            colorChip.Button.ClickMask = __instance.scroller.Hitbox;
            colorChip.transform.localPosition = new(xpos, ypos, -1f);
            colorChip.ProductId = nameplate.ProductId;
            colorChip.Tag = nameplate;
            colorChip.SelectionHighlight.gameObject.SetActive(false);

            if (CustomNameplateViewDatas.TryGetValue(colorChip.ProductId, out var viewData))
                colorChip.GetComponent<NameplateChip>().image.sprite = viewData.Image;
            else
                DefaultNameplateCoro(__instance, colorChip.GetComponent<NameplateChip>());

            __instance.ColorChips.Add(colorChip);
        }

        return offset - ((nameplates.Count - 1) / __instance.NumPerRow * __instance.YOffset) - 1.5f;
    }

    private static void DefaultNameplateCoro(NameplatesTab __instance, NameplateChip chip) => __instance.StartCoroutine(__instance.CoLoadAssetAsync<NamePlateViewData>(HatManager.Instance
        .GetNamePlateById(chip.ProductId).ViewDataRef, (Action<NamePlateViewData>)(viewData => chip.image.sprite = viewData?.Image)));

    public static bool Prefix(NameplatesTab __instance)
    {
        for (var i = 0; i < __instance.scroller.Inner.childCount; i++)
            __instance.scroller.Inner.GetChild(i).gameObject.Destroy();

        __instance.ColorChips = new();
        var array = HatManager.Instance.GetUnlockedNamePlates();
        var packages = new Dictionary<string, List<NamePlateData>>();

        foreach (var data in array)
        {
            var ext = data.GetExtention();
            var package = "Innersloth";

            if (ext != null)
                package = ext.StreamOnly ? "Stream" : ext.Artist;

            if (IsNullEmptyOrWhiteSpace(package))
                package = "Misc";

            if (!packages.ContainsKey(package))
                packages[package] = [];

            packages[package].Add(data);
        }

        var YOffset = __instance.YStart;
        Template = __instance.transform.FindChild("Text").GetComponent<TMP_Text>();
        var keys = packages.Keys.OrderBy(x => x switch
        {
            "Innersloth" => 4,
            "Stream" => 1,
            "Misc" => 3,
            _ => 2
        });
        keys.ForEach(key => YOffset = CreateNameplatePackage(packages[key], key, YOffset, __instance));

        if (array.Length != 0)
            __instance.GetDefaultSelectable().PlayerEquippedForeground.SetActive(true);

        __instance.plateId = DataManager.Player.Customization.NamePlate;
        __instance.currentNameplateIsEquipped = true;
        __instance.SetScrollerBounds();
        __instance.scroller.ContentYBounds.max = -(YOffset + 3.8f);
        return false;
    }
}

[HarmonyPatch(typeof(CosmeticsCache), nameof(CosmeticsCache.GetNameplate))]
public static class CosmeticsCacheGetNameplatePatch
{
    public static bool Prefix(CosmeticsCache __instance, string id, ref NamePlateViewData __result)
    {
        if (!CustomNameplateViewDatas.TryGetValue(id, out __result))
            return true;

        return !(__result ??= __instance.nameplates["nameplate_NoPlate"].GetAsset());
    }
}

[HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.PreviewNameplate))]
public static class PreviewNameplatesPatch
{
    public static void Postfix(PlayerVoteArea __instance, string plateID)
    {
        if (CustomNameplateViewDatas.TryGetValue(plateID, out var viewData))
            __instance.Background.sprite = viewData?.Image;
    }
}