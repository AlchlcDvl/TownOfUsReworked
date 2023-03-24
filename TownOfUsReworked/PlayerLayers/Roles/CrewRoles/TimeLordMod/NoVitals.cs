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
            if (PlayerControl.LocalPlayer.Is(RoleEnum.TimeLord) && !PlayerControl.LocalPlayer.Data.IsDead && Minigame.Instance)
                __instance.Close();

            return true;
        }
    }
}