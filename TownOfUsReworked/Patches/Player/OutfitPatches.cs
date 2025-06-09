namespace TownOfUsReworked.Patches.Player;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.SpeedMod), MethodType.Getter)]
public static class PlayerOutfitPatches
{
    public static bool Prefix(PlayerPhysics __instance, ref float __result)
    {
        try
        {
            __result = __instance.myPlayer.GetFinalSpeed();
        }
        catch
        {
            __result = 1f;
        }

        return false;
    }
}

[HarmonyPatch(typeof(CustomNetworkTransform), nameof(CustomNetworkTransform.FixedUpdate))]
public static class SpeedNetworkPatch
{
    public static void Postfix(CustomNetworkTransform __instance)
    {
        if (__instance.AmOwner)
            return;

        try
        {
            __instance.body.velocity *= __instance.myPlayer.GetFinalSpeed();
        } catch {}
    }
}