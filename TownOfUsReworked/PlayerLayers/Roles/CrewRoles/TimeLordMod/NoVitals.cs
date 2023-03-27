using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TimeLordMod
{
    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Begin))]
    public static class NoVitals
    {
        public static bool Prefix(VitalsMinigame __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.TimeLord) && Minigame.Instance && !PlayerControl.LocalPlayer.Data.IsDead)
                __instance.Close();

            return true;
        }
    }
}