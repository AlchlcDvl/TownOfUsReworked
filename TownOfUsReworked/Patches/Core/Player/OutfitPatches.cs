namespace TownOfUsReworked.Patches.Core.Player;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.SpeedMod), MethodType.Getter)]
public static class PlayerOutfitPatches
{
    public static bool Prefix(PlayerPhysics __instance, ref float __result)
    {
        __result = __instance.myPlayer.GetFinalSpeed();
        return false;
    }
}

[HarmonyPatch(typeof(CustomNetworkTransform), nameof(CustomNetworkTransform.FixedUpdate))]
public static class SpeedNetworkPatch
{
    public static void Postfix(CustomNetworkTransform __instance)
    {
        if (!__instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove)
            __instance.body.velocity *= __instance.myPlayer.GetFinalSpeed();
    }
}