using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
    public static class HandleAnimation
    {
        public static void Prefix(PlayerPhysics __instance, [HarmonyArgument(0)] ref bool amDead)
        {
            if (__instance.myPlayer.Is(RoleEnum.Ghoul))
                amDead = Role.GetRole<Ghoul>(__instance.myPlayer).Caught;
            else if (__instance.myPlayer.Is(RoleEnum.Banshee))
                amDead = Role.GetRole<Banshee>(__instance.myPlayer).Caught;
            else if (__instance.myPlayer.Is(RoleEnum.Phantom))
                amDead = Role.GetRole<Phantom>(__instance.myPlayer).Caught;
            else if (__instance.myPlayer.Is(RoleEnum.Revealer))
                amDead = Role.GetRole<Revealer>(__instance.myPlayer).Caught;
        }
    }
}