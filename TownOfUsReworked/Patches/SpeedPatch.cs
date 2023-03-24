using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class SpeedPatch
    {
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        [HarmonyPostfix]
        public static void PostfixPhysics(PlayerPhysics __instance)
        {
            if (__instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove)
            {
                __instance.body.velocity *= __instance.myPlayer.GetAppearance().SpeedFactor * (__instance.myPlayer.Data.IsDead && !__instance.myPlayer.Is(RoleEnum.Phantom) &&
                    !__instance.myPlayer.Is(RoleEnum.Revealer) && !__instance.myPlayer.Is(RoleEnum.Ghoul) && !__instance.myPlayer.Is(RoleEnum.Banshee) ? CustomGameOptions.GhostSpeed : 1);
            }
        }

        [HarmonyPatch(typeof(CustomNetworkTransform), nameof(CustomNetworkTransform.FixedUpdate))]
        [HarmonyPostfix]
        public static void PostfixNetwork(CustomNetworkTransform __instance)
        {
            if (!__instance.AmOwner && __instance.interpolateMovement != 0.0f && GameData.Instance)
            {
                var player = __instance.gameObject.GetComponent<PlayerControl>();
                __instance.body.velocity *= player.GetAppearance().SpeedFactor * (player.Data.IsDead && !player.Is(RoleEnum.Phantom) && !player.Is(RoleEnum.Revealer) &&
                    !player.Is(RoleEnum.Ghoul) && !player.Is(RoleEnum.Banshee) ? CustomGameOptions.GhostSpeed : 1);
            }
        }
    }
}