using static TownOfUsReworked.Managers.CustomNameplateManager;
using static TownOfUsReworked.Managers.CustomVisorManager;
using static TownOfUsReworked.Managers.CustomHatManager;

namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(HatManager))]
public static class HatManagerPatches
{
    private static bool NameplatesLoaded;

    [HarmonyPatch(nameof(HatManager.GetNamePlateById)), HarmonyPrefix]
    public static void GetNamePlateByIdPrefix(HatManager __instance)
    {
        if (NameplatesLoaded)
            return;

        NameplatesLoaded = true;
        var allPlates = __instance.allNamePlates.ToList();
        allPlates.AddRange(CustomNameplateRegistry.Values.Select(x => x.CosmeticData));
        __instance.allNamePlates = allPlates.ToArray();
    }

    private static bool HatsLoaded;

    [HarmonyPatch(nameof(HatManager.GetHatById)), HarmonyPrefix]
    public static void GetHatByIdPrefix(HatManager __instance)
    {
        if (HatsLoaded)
            return;

        HatsLoaded = true;
        var allHats = __instance.allHats.ToList();
        allHats.AddRange(CustomHatRegistry.Values.Select(x => x.CosmeticData));
        __instance.allHats = allHats.ToArray();
    }

    private static bool VisorsLoaded;

    [HarmonyPatch(nameof(HatManager.GetVisorById)), HarmonyPrefix]
    public static void GetVisorByIdPrefix(HatManager __instance)
    {
        if (VisorsLoaded)
            return;

        VisorsLoaded = true;
        var allVisors = __instance.allVisors.ToList();
        allVisors.AddRange(CustomVisorRegistry.Values.Select(x => x.CosmeticData));
        __instance.allVisors = allVisors.ToArray();
    }
}

[HarmonyPatch(typeof(CosmeticsCache))]
public static class CosmeticsCacheGetCosmeticsPatches
{
    [HarmonyPatch(nameof(CosmeticsCache.GetNameplate))]
    public static bool Prefix(CosmeticsCache __instance, string id, ref NamePlateViewData __result)
    {
        if (!CustomNameplateRegistry.TryGetValue(id, out var cn))
            return true;

        return !(__result = cn.ViewData ?? __instance.nameplates["nameplate_NoPlate"].GetAsset());
    }

    [HarmonyPatch(nameof(CosmeticsCache.GetHat))]
    public static bool Prefix(CosmeticsCache __instance, string id, ref HatViewData __result)
    {
        if (!CustomHatRegistry.TryGetValue(id, out var ch))
            return true;

        return !(__result = ch.ViewData ?? __instance.hats["hat_NoHat"].GetAsset());
    }

    [HarmonyPatch(nameof(CosmeticsCache.GetVisor))]
    public static bool Prefix(CosmeticsCache __instance, string id, ref VisorViewData __result)
    {
        if (!CustomVisorRegistry.TryGetValue(id, out var cv))
            return true;

        return !(__result = cv.ViewData ?? __instance.visors["visor_EmptyVisor"].GetAsset());
    }
}