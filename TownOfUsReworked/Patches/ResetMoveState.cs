using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ResetMoveState))]
    public static class ResetMoveState
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            if (__instance.myPlayer.Is(RoleEnum.Ghoul))
            {
                var role = Role.GetRole<Ghoul>(__instance.myPlayer);
                __instance.myPlayer.Collider.enabled = !role.Caught;
            }
            else if (__instance.myPlayer.Is(RoleEnum.Banshee))
            {
                var role = Role.GetRole<Banshee>(__instance.myPlayer);
                __instance.myPlayer.Collider.enabled = !role.Caught;
            }
            else if (__instance.myPlayer.Is(RoleEnum.Phantom))
            {
                var role = Role.GetRole<Phantom>(__instance.myPlayer);
                __instance.myPlayer.Collider.enabled = !role.Caught;
            }
            else if (__instance.myPlayer.Is(RoleEnum.Revealer))
            {
                var role = Role.GetRole<Revealer>(__instance.myPlayer);
                __instance.myPlayer.Collider.enabled = !role.Caught;
            }
        }
    }
}