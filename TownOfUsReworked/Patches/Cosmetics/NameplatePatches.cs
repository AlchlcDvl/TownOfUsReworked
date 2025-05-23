using Innersloth.Assets;

namespace TownOfUsReworked.Patches.Cosmetics;

[HarmonyPatch(typeof(NameplatesTab), nameof(NameplatesTab.OnEnable))]
public static class NameplatesTabOnEnablePatch
{
    public static bool Prefix(NameplatesTab __instance)
    {
        __instance.BaseOnEnable();
		__instance.PlayerPreview.gameObject.SetActive(false);
        __instance.StartCoroutine(__instance.CoLoadNameplatePreview());
        var array = HatManager.Instance.GetUnlockedNamePlates();
        var packages = CosmeticTabPatches.GeneratePackages<NamePlateData, CustomNameplate, NamePlateViewData>(array, NameplateLoader.CustomCosmeticRegistry);
        __instance.CreatePackages(packages, true, __instance.SelectNameplate, () => __instance.SelectNameplate(HatManager.Instance.GetNamePlateById(DataManager.Player.Customization.NamePlate)),
            out var yOffset, UpdateChip);
        __instance.plateId = DataManager.Player.Customization.NamePlate;
        __instance.currentNameplateIsEquipped = true;
        __instance.EndOnEnable(yOffset, array);
        return false;
    }

    private static void UpdateChip(ColorChip colorChip, NamePlateData nameplate, InventoryTab __instance)
    {
        var chip = colorChip.Cast<NameplateChip>();

        if (NameplateLoader.CustomCosmeticRegistry.TryGetValue(colorChip.ProductId, out var cn))
            chip.image.sprite = cn.ViewData.Image;
        else
            __instance.StartCoroutine(__instance.CoLoadAssetAsync<NamePlateViewData>(nameplate.ViewDataRef, (Action<NamePlateViewData>)(viewData => chip.image.sprite = viewData?.Image)));
    }
}