namespace TownOfUsReworked.Patches.Cosmetics;

[HarmonyPatch(typeof(HatManager), nameof(HatManager.GetNamePlateById))]
public static class HatManagerPatches
{
    private static bool CosmeticsLoaded;

    public static void Prefix(HatManager __instance)
    {
        if (CosmeticsLoaded)
            return;

        var allPlates = __instance.allNamePlates.ToList();
        allPlates.InsertRange(1, NameplateLoader.CustomCosmeticRegistry.Values.Select(x => x.CosmeticData));
        allPlates.ForEach((i, x) => x.displayOrder = i);
        __instance.allNamePlates = allPlates.ToArray();

        var allHats = __instance.allHats.ToList();
        allHats.InsertRange(1, HatLoader.CustomCosmeticRegistry.Values.Select(x => x.CosmeticData));
        allHats.ForEach((i, x) => x.displayOrder = i);
        __instance.allHats = allHats.ToArray();

        var allVisors = __instance.allVisors.ToList();
        allVisors.InsertRange(1, VisorLoader.CustomCosmeticRegistry.Values.Select(x => x.CosmeticData));
        allVisors.ForEach((i, x) => x.displayOrder = i);
        __instance.allVisors = allVisors.ToArray();

        // var allSkins = __instance.allSkins.ToList();
        // allSkins.InsertRange(1, SkinLoader.CustomCosmeticRegistry.Values.Select(x => x.CosmeticData));
        // allSkins.ForEach((i, x) => x.displayOrder = i);
        // __instance.allSkins = allSkins.ToArray();

        // foreach (var skin in SkinLoader.CustomCosmeticRegistry.Values)
        // {
        //     var refAsset = allSkins.Find(x => x.ProductId == skin.BaseSkin).CreateAddressableAsset();
        //     refAsset.LoadAsync((Action)CloneProps);

        //     void CloneProps()
        //     {
        //         var asset = refAsset.GetAsset();
        //         skin.ViewData.IdleAnim = asset.IdleAnim;
        //         skin.ViewData.IdleLeftAnim = asset.IdleLeftAnim;
        //         skin.ViewData.RunAnim = asset.RunAnim;
        //         skin.ViewData.RunLeftAnim = asset.RunLeftAnim;
        //         skin.ViewData.EnterVentAnim = asset.EnterVentAnim;
        //         skin.ViewData.EnterLeftVentAnim = asset.EnterLeftVentAnim;
        //         skin.ViewData.ExitVentAnim = asset.ExitVentAnim;
        //         skin.ViewData.ExitLeftVentAnim = asset.ExitLeftVentAnim;
        //         skin.ViewData.ClimbAnim = asset.ClimbAnim;
        //         skin.ViewData.ClimbDownAnim = asset.ClimbDownAnim;
        //         skin.ViewData.SpawnAnim = asset.SpawnAnim;
        //         skin.ViewData.SpawnLeftAnim = asset.SpawnLeftAnim;
        //         skin.ViewData.KillTongueImpostor = asset.KillTongueImpostor;
        //         skin.ViewData.KillTongueVictim = asset.KillTongueVictim;
        //         skin.ViewData.KillShootImpostor = asset.KillShootImpostor;
        //         skin.ViewData.KillShootVictim = asset.KillShootVictim;
        //         skin.ViewData.KillNeckImpostor = asset.KillNeckImpostor;
        //         skin.ViewData.KillNeckVictim = asset.KillNeckVictim;
        //         skin.ViewData.KillStabImpostor = asset.KillStabImpostor;
        //         skin.ViewData.KillStabVictim = asset.KillStabVictim;
        //         skin.ViewData.KillRHMVictim = asset.KillRHMVictim;
        //     }
        // }

        CosmeticsLoaded = true;
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

    // [HarmonyPatch(nameof(CosmeticsCache.GetSkin))]
    // public static bool Prefix(CosmeticsCache __instance, string id, ref SkinViewData __result)
    // {
    //     if (!SkinLoader.CustomCosmeticRegistry.TryGetValue(id, out var cv))
    //         return true;

    //     return !(__result = cv.ViewData ?? __instance.skins["skin_None"].GetAsset());
    // }
}