using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Visible), MethodType.Setter)]
    public static class VisibleOverride
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] ref bool value)
        {
            if (!__instance.Is(RoleEnum.Revealer) && !__instance.Is(RoleEnum.Ghoul) && !__instance.Is(RoleEnum.Phantom) && !__instance.Is(RoleEnum.Banshee))
                return;

            if (__instance.Is(RoleEnum.Ghoul) && Role.GetRole<Ghoul>(__instance).Caught)
                return;

            if (__instance.Is(RoleEnum.Banshee) && Role.GetRole<Banshee>(__instance).Caught)
                return;

            if (__instance.Is(RoleEnum.Phantom) && Role.GetRole<Phantom>(__instance).Caught)
                return;

            if (__instance.Is(RoleEnum.Revealer) && Role.GetRole<Revealer>(__instance).Caught)
                return;

            value = !__instance.inVent;
        }
    }
}