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
        allPlates.AddRange(RegisteredNameplates);
        __instance.allNamePlates = allPlates.ToArray();
        RegisteredNameplates.Clear();
    }

    private static bool HatsLoaded;

    [HarmonyPatch(nameof(HatManager.GetHatById)), HarmonyPrefix]
    public static void GetHatByIdPrefix(HatManager __instance)
    {
        if (HatsLoaded)
            return;

        HatsLoaded = true;
        var allHats = __instance.allHats.ToList();
        allHats.AddRange(RegisteredHats);
        __instance.allHats = allHats.ToArray();
        RegisteredHats.Clear();
    }

    private static bool VisorsLoaded;

    [HarmonyPatch(nameof(HatManager.GetVisorById)), HarmonyPrefix]
    public static void GetVisorByIdPrefix(HatManager __instance)
    {
        if (VisorsLoaded)
            return;

        VisorsLoaded = true;
        var allVisors = __instance.allVisors.ToList();
        allVisors.AddRange(RegisteredVisors);
        __instance.allVisors = allVisors.ToArray();
        RegisteredVisors.Clear();
    }
}

[HarmonyPatch(typeof(CosmeticsCache))]
public static class CosmeticsCacheGetCosmeticsPatches
{
    [HarmonyPatch(nameof(CosmeticsCache.GetNameplate))]
    public static bool Prefix(CosmeticsCache __instance, string id, ref NamePlateViewData __result)
    {
        if (!CustomNameplateViewDatas.TryGetValue(id, out __result))
            return true;

        return !(__result ??= __instance.nameplates["nameplate_NoPlate"].GetAsset());
    }

    [HarmonyPatch(nameof(CosmeticsCache.GetHat))]
    public static bool Prefix(CosmeticsCache __instance, string id, ref HatViewData __result)
    {
        if (!CustomHatViewDatas.TryGetValue(id, out __result))
            return true;

        return !(__result ??= __instance.hats["hat_NoHat"].GetAsset());
    }

    [HarmonyPatch(nameof(CosmeticsCache.GetVisor))]
    public static bool Prefix(CosmeticsCache __instance, string id, ref VisorViewData __result)
    {
        if (!CustomVisorViewDatas.TryGetValue(id, out __result))
            return true;

        return !(__result ??= __instance.visors["visor_EmptyVisor"].GetAsset());
    }
}