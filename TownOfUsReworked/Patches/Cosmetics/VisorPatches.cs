namespace TownOfUsReworked.Patches.Cosmetics;

[HarmonyPatch(typeof(VisorsTab), nameof(VisorsTab.OnEnable))]
public static class VisorsTabOnEnablePatch
{
    public static bool Prefix(VisorsTab __instance)
    {
        __instance.BaseOnEnable();
        var array = HatManager.Instance.GetUnlockedVisors();
        var packages = CosmeticTabPatches.GeneratePackages<VisorData, CustomVisor, VisorViewData>(array, VisorLoader.CustomCosmeticRegistry);
        __instance.CreatePackages(packages, false, __instance.SelectVisor, () => __instance.SelectVisor(HatManager.Instance.GetVisorById(DataManager.Player.Customization.Visor)), out var yOffset);
        __instance.visorId = DataManager.Player.Customization.Visor;
        __instance.currentVisorIsEquipped = true;
        __instance.EndOnEnable(yOffset, array);
        return false;
    }
}

[HarmonyPatch(typeof(VisorLayer))]
public static class VisorPatches
{
    [HarmonyPatch(nameof(VisorLayer.UpdateMaterial)), HarmonyPrefix]
    public static bool UpdateMaterialPrefix(VisorLayer __instance)
    {
        if (!__instance.visorData)
            return true;

        try
        {
            __instance.viewAsset.GetAsset();
            return true;
        } catch {}

        if (!VisorLoader.CustomCosmeticRegistry.TryGetValue(__instance.visorData.ProductId, out var cv))
            return true;

        var maskType = __instance.matProperties.MaskType;
        var masked = __instance.visorData && __instance.IsLoaded && cv.ViewData.MatchPlayerColor;
        __instance.Image.sharedMaterial = maskType is PlayerMaterial.MaskType.ComplexUI or PlayerMaterial.MaskType.ScrollingUI
            ? (masked ? HatManager.Instance.MaskedPlayerMaterial : HatManager.Instance.MaskedMaterial)
            : (masked ? HatManager.Instance.PlayerMaterial : HatManager.Instance.DefaultShader);
        __instance.Image.maskInteraction = maskType switch
        {
            PlayerMaterial.MaskType.SimpleUI => SpriteMaskInteraction.VisibleInsideMask,
            PlayerMaterial.MaskType.Exile => SpriteMaskInteraction.VisibleOutsideMask,
            _ => SpriteMaskInteraction.None,
        };
        __instance.Image.material.SetInt(PlayerMaterial.MaskLayer, __instance.matProperties.MaskLayer);

        if (__instance.visorData && __instance.IsLoaded && cv.ViewData.MatchPlayerColor)
            PlayerMaterial.SetColors(__instance.matProperties.ColorId, __instance.Image);

        if (__instance.matProperties.MaskLayer <= 0)
            PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(__instance.Image, __instance.matProperties.IsLocalPlayer);

        return false;
    }

    [HarmonyPatch(nameof(VisorLayer.SetFlipX)), HarmonyPrefix]
    public static bool SetFlipXPrefix(VisorLayer __instance, bool flipX)
    {
        if (!__instance.visorData || !VisorLoader.CustomCosmeticRegistry.TryGetValue(__instance.visorData.ProductId, out var cv))
            return true;

        __instance.Image.flipX = flipX;

        if (!__instance.IsLoaded || !cv.ViewData)
            return false;

        __instance.Image.sprite = flipX && cv.ViewData.LeftIdleFrame ? cv.ViewData.LeftIdleFrame : cv.ViewData.IdleFrame;
        return false;
    }

    [HarmonyPatch(nameof(VisorLayer.SetVisor), typeof(VisorData), typeof(int)), HarmonyPrefix]
    public static bool SetVisorPrefix(VisorLayer __instance, VisorData data, int color)
    {
        if (!VisorLoader.CustomCosmeticRegistry.ContainsKey(data.ProductId))
            return true;

        __instance.visorData = data;
        __instance.viewAsset = null;
        __instance.SetMaterialColor(color);
        __instance.PopulateFromViewData();
        return false;
    }

    [HarmonyPatch(nameof(VisorLayer.PopulateFromViewData)), HarmonyPrefix]
    public static bool PopulateFromViewDataPrefix(VisorLayer __instance)
    {
        if (!__instance.visorData)
            return true;

        try
        {
            __instance.viewAsset.GetAsset();
            return true;
        } catch {}

        if (!VisorLoader.CustomCosmeticRegistry.ContainsKey(__instance.visorData.ProductId))
            return true;

        __instance.UpdateMaterial();
        __instance.transform.SetLocalZ(__instance.DesiredLocalZPosition);
        __instance.SetFlipX(__instance.Image.flipX);
        return false;
    }

    [HarmonyPatch(nameof(VisorLayer.SetFloorAnim)), HarmonyPrefix]
    public static bool SetFloorAnimPrefix(VisorLayer __instance)
    {
        if (!__instance.visorData)
            return true;

        try
        {
            __instance.viewAsset.GetAsset();
            return true;
        } catch {}

        if (!VisorLoader.CustomCosmeticRegistry.TryGetValue(__instance.visorData.ProductId, out var cv))
            return true;

        __instance.Image.sprite = cv.ViewData.FloorFrame;
        return false;
    }

    [HarmonyPatch(nameof(VisorLayer.SetClimbAnim)), HarmonyPrefix]
    public static bool SetClimbAnimPrefix(VisorLayer __instance, PlayerBodyTypes bodyType)
    {
        if (!__instance.visorData || __instance.options.HideDuringClimb || bodyType == PlayerBodyTypes.Horse || !VisorLoader.CustomCosmeticRegistry.TryGetValue(__instance.visorData.name, out var visor))
            return true;

        __instance.transform.SetLocalZ(0f);
        __instance.Image.sprite = visor.ViewData.ClimbFrame;
        return false;
    }

    [HarmonyPatch(nameof(VisorLayer.IsLoaded), MethodType.Getter)]
    public static bool Prefix(VisorLayer __instance, ref bool __result)
    {
        if (!__instance.visorData || !VisorLoader.CustomCosmeticRegistry.ContainsKey(__instance.visorData.ProductId))
            return true;

        return !(__result = true);
    }
}