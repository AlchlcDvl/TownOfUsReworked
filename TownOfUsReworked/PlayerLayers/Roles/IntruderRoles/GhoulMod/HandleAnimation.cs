using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GhoulMod
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
    public class HandleAnimation
    {
        public static void Prefix(PlayerPhysics __instance, [HarmonyArgument(0)] ref bool amDead)
        {
            if (__instance.myPlayer.Is(RoleEnum.Ghoul))
                amDead = Role.GetRole<Ghoul>(__instance.myPlayer).Caught;
        }
    }
}