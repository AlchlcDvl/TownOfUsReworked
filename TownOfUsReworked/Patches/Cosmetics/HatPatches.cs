using PowerTools;

namespace TownOfUsReworked.Patches.Cosmetics;

[HarmonyPatch(typeof(HatsTab), nameof(HatsTab.OnEnable))]
public static class HatsTabOnEnablePatch
{
    public static bool Prefix(HatsTab __instance)
    {
        __instance.BaseOnEnable();
        var array = HatManager.Instance.GetUnlockedHats();
        var packages = CosmeticTabPatches.GeneratePackages<HatData, CustomHat, HatViewData>(array, HatLoader.CustomCosmeticRegistry);
        __instance.CreatePackages(packages, false, __instance.SelectHat, () => __instance.SelectHat(HatManager.Instance.GetHatById(DataManager.Player.Customization.visor)), out var yOffset);
        __instance.currentHatIsEquipped = true;
        __instance.scroller.ContentYBounds.max = -(yOffset + 4.1f);
        __instance.EndOnEnable(yOffset, array);
        return false;
    }
}

[HarmonyPatch(typeof(HatParent))]
public static class HatPatches
{
    [HarmonyPatch(nameof(HatParent.UpdateMaterial)), HarmonyPrefix]
    public static bool UpdateMaterialPrefix(HatParent __instance)
    {
        __instance.UpdateMaterial(__instance.matProperties.ColorId);
        return false;
    }

    [HarmonyPatch(nameof(HatParent.IsLoaded), MethodType.Getter)]
    public static bool Prefix(HatParent __instance, ref bool __result)
    {
        if (!__instance.Hat || !HatLoader.CustomCosmeticRegistry.ContainsKey(__instance.Hat.ProductId))
            return true;

        return !(__result = true);
    }

    [HarmonyPatch(nameof(HatParent.PopulateFromViewData)), HarmonyPrefix]
    public static bool PopulateFromViewDataPrefix(HatParent __instance)
    {
        try
        {
            __instance.viewAsset.GetAsset();
            return true;
        } catch {}

        if (!__instance.Hat || !HatLoader.CustomCosmeticRegistry.TryGetValue(__instance.Hat.ProductId, out var ch) || !ch.ViewData)
            return true;

        var asset = ch.ViewData;
        __instance.UpdateMaterial();
        __instance.SpriteSyncNode ??= __instance.GetComponent<SpriteAnimNodeSync>();

        if (__instance.SpriteSyncNode)
            __instance.SpriteSyncNode.NodeId = __instance.Hat.NoBounce ? 1 : 0;

        if (__instance.Hat.InFront)
        {
            __instance.BackLayer.enabled = false;
            __instance.FrontLayer.enabled = true;
            __instance.FrontLayer.sprite = asset.MainImage;
        }
        else if (asset.BackImage)
        {
            __instance.BackLayer.enabled = true;
            __instance.FrontLayer.enabled = true;
            __instance.BackLayer.sprite = asset.BackImage;
            __instance.FrontLayer.sprite = asset.MainImage;
        }
        else
        {
            __instance.BackLayer.enabled = true;
            __instance.FrontLayer.enabled = false;
            __instance.FrontLayer.sprite = null;
            __instance.BackLayer.sprite = asset.MainImage;
        }

        if (__instance.HideHat())
        {
            __instance.FrontLayer.enabled = false;
            __instance.BackLayer.enabled = false;
        }

        return false;
    }

    [HarmonyPatch(nameof(HatParent.LateUpdate)), HarmonyPrefix]
    public static bool LateUpdatePrefix(HatParent __instance)
    {
        if (!__instance.Hat)
            return false;

        try
        {
            __instance.viewAsset.GetAsset();
            return true;
        } catch {}

        if (!HatLoader.CustomCosmeticRegistry.TryGetValue(__instance.Hat.ProductId, out var ch) || !ch.ViewData)
            return false;

        if (__instance.FrontLayer.sprite != ch.ViewData.ClimbImage && __instance.FrontLayer.sprite != ch.ViewData.FloorImage)
        {
            if ((__instance.Hat.InFront || ch.ViewData.BackImage) && ch.ViewData.LeftMainImage)
                __instance.FrontLayer.sprite = __instance.Parent.flipX ? ch.ViewData.LeftMainImage : ch.ViewData.MainImage;

            if (ch.ViewData.BackImage && ch.ViewData.LeftBackImage)
            {
                __instance.BackLayer.sprite = __instance.Parent.flipX ? ch.ViewData.LeftBackImage : ch.ViewData.BackImage;
                return false;
            }

            if (!ch.ViewData.BackImage && !__instance.Hat.InFront && ch.ViewData.LeftMainImage)
            {
                __instance.BackLayer.sprite = __instance.Parent.flipX ? ch.ViewData.LeftMainImage : ch.ViewData.MainImage;
                return false;
            }
        }
        else if (__instance.FrontLayer.sprite == ch.ViewData.ClimbImage || __instance.FrontLayer.sprite == ch.ViewData.LeftClimbImage)
        {
            __instance.SpriteSyncNode ??= __instance.GetComponent<SpriteAnimNodeSync>();

            if (__instance.SpriteSyncNode)
                __instance.SpriteSyncNode.NodeId = 0;
        }

        return false;
    }

    [HarmonyPatch(nameof(HatParent.SetHat), typeof(int)), HarmonyPrefix]
    public static bool SetHatPrefix(HatParent __instance, int color)
    {
        if (!__instance.Hat || !HatLoader.CustomCosmeticRegistry.ContainsKey(__instance.Hat.ProductId))
            return true;

        __instance.UnloadAsset();
        __instance.viewAsset = null;
        __instance.SetMaterialColor(color);
        __instance.PopulateFromViewData();
        return false;
    }

    [HarmonyPatch(nameof(HatParent.SetFloorAnim))]
    public static bool SetFloorAnimPrefix(HatParent __instance)
    {
        if (!__instance.Hat)
            return true;

        try
        {
            __instance.viewAsset.GetAsset();
            return true;
        } catch {}

        if (!HatLoader.CustomCosmeticRegistry.TryGetValue(__instance.Hat.ProductId, out var ch))
            return true;

        __instance.BackLayer.enabled = false;
        __instance.FrontLayer.enabled = true;
        __instance.FrontLayer.flipX = false;
        __instance.FrontLayer.sprite = ch.ViewData.FloorImage;
        return false;
    }

    [HarmonyPatch(nameof(HatParent.SetClimbAnim)), HarmonyPrefix]
    public static bool SetClimbAnimPrefix(HatParent __instance)
    {
        if (!__instance.Hat)
            return true;

        try
        {
            __instance.viewAsset.GetAsset();
            return true;
        } catch {}

        if (!HatLoader.CustomCosmeticRegistry.TryGetValue(__instance.Hat.ProductId, out var ch))
            return true;

        if (!__instance.options.ShowForClimb)
            return false;

        __instance.BackLayer.enabled = false;
        __instance.FrontLayer.enabled = true;
        __instance.FrontLayer.sprite = ch.ViewData.ClimbImage;
        return false;
    }
}