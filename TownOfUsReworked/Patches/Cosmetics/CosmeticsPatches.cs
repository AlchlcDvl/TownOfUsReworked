namespace TownOfUsReworked.Patches.Cosmetics;

[HarmonyPatch(typeof(HatManager))]
public static class HatManagerPatches
{
    private static bool NameplatesLoaded;

    [HarmonyPatch(nameof(HatManager.GetNamePlateById)), HarmonyPrefix]
    public static void GetNamePlateByIdPrefix(HatManager __instance)
    {
        if (NameplatesLoaded)
            return;

        var allPlates = __instance.allNamePlates.ToList();
        allPlates.InsertRange(1, NameplateLoader.CustomCosmeticRegistry.Values.Select(x => x.CosmeticData));
        allPlates.ForEach((i, x) => x.displayOrder = i);
        __instance.allNamePlates = allPlates.ToArray();
        NameplatesLoaded = true;
    }

    private static bool HatsLoaded;

    [HarmonyPatch(nameof(HatManager.GetHatById)), HarmonyPrefix]
    public static void GetHatByIdPrefix(HatManager __instance)
    {
        if (HatsLoaded)
            return;

        var allHats = __instance.allHats.ToList();
        allHats.InsertRange(1, HatLoader.CustomCosmeticRegistry.Values.Select(x => x.CosmeticData));
        allHats.ForEach((i, x) => x.displayOrder = i);
        __instance.allHats = allHats.ToArray();
        HatsLoaded = true;
    }

    private static bool VisorsLoaded;

    [HarmonyPatch(nameof(HatManager.GetVisorById)), HarmonyPrefix]
    public static void GetVisorByIdPrefix(HatManager __instance)
    {
        if (VisorsLoaded)
            return;

        var allVisors = __instance.allVisors.ToList();
        allVisors.InsertRange(1, VisorLoader.CustomCosmeticRegistry.Values.Select(x => x.CosmeticData));
        allVisors.ForEach((i, x) => x.displayOrder = i);
        __instance.allVisors = allVisors.ToArray();
        VisorsLoaded = true;
    }
}

[HarmonyPatch(typeof(CosmeticsCache))]
public static class CosmeticsCacheGetCosmeticsPatches
{
    [HarmonyPatch(nameof(CosmeticsCache.GetNameplate))]
    public static bool Prefix(CosmeticsCache __instance, string id, ref NamePlateViewData __result)
    {
        if (!NameplateLoader.CustomCosmeticRegistry.TryGetValue(id, out var cn))
            return true;

        return !(__result = cn.ViewData ?? __instance.nameplates["nameplate_NoPlate"].GetAsset());
    }

    [HarmonyPatch(nameof(CosmeticsCache.GetHat))]
    public static bool Prefix(CosmeticsCache __instance, string id, ref HatViewData __result)
    {
        if (!HatLoader.CustomCosmeticRegistry.TryGetValue(id, out var ch))
            return true;

        return !(__result = ch.ViewData ?? __instance.hats["hat_NoHat"].GetAsset());
    }

    [HarmonyPatch(nameof(CosmeticsCache.GetVisor))]
    public static bool Prefix(CosmeticsCache __instance, string id, ref VisorViewData __result)
    {
        if (!VisorLoader.CustomCosmeticRegistry.TryGetValue(id, out var cv))
            return true;

        return !(__result = cv.ViewData ?? __instance.visors["visor_EmptyVisor"].GetAsset());
    }
}