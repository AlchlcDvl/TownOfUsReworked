namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(LongBoiPlayerBody), nameof(LongBoiPlayerBody.Awake))]
public static class WakeUpLongBoi
{
    public static bool Prefix(LongBoiPlayerBody __instance)
    {
        __instance.cosmeticLayer.OnSetBodyAsGhost += (Action)__instance.SetPoolableGhost;
        __instance.cosmeticLayer.OnColorChange += (Action<int>)__instance.SetHeightFromColor;
        __instance.cosmeticLayer.OnCosmeticSet += (Action<string, int, CosmeticsLayer.CosmeticKind>)__instance.OnCosmeticSet;
        __instance.gameObject.layer = 8;
        return false;
    }
}

[HarmonyPatch(typeof(LongBoiPlayerBody), nameof(LongBoiPlayerBody.Start))]
public static class StartTheLongBoi
{
    public static bool Prefix(LongBoiPlayerBody __instance)
    {
        __instance.ShouldLongAround = true;

        if (__instance.hideCosmeticsQC)
            __instance.cosmeticLayer.SetHatVisorVisible(false);

        __instance.SetupNeckGrowth(false, true);

        if (__instance.isExiledPlayer && Ship?.Type != ShipStatus.MapType.Fungle)
                __instance.cosmeticLayer.AdjustCosmeticRotations(-17.75f);

        if (!__instance.isPoolablePlayer)
            __instance.cosmeticLayer.ValidateCosmetics();

        return false;
    }
}

[HarmonyPatch(typeof(LongBoiPlayerBody), nameof(LongBoiPlayerBody.SetHeighFromDistanceHnS))]
public static class HeightDistanceLongBoi
{
    public static bool Prefix(LongBoiPlayerBody __instance, ref float distance)
    {
        __instance.targetHeight = (distance / 10f) + 0.5f;
        __instance.SetupNeckGrowth(true, true);
        return false;
    }
}

[HarmonyPatch(typeof(HatManager), nameof(HatManager.CheckLongModeValidCosmetic))]
public static class CheckLongModeValidCosmetic
{
    public static bool Prefix(HatManager __instance, ref bool __result, ref string cosmeticID, ref bool ignoreLongMode)
    {
        if (!ignoreLongMode)
        {
            var id = cosmeticID;
            __result = __instance.longModeBlackList.Any(x => x.ProdId == id);
        }

        return false;
    }
}