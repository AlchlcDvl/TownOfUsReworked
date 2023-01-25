using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TimeLordMod
{
    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Begin))]
    public class NoVitals
    {
        public static bool Prefix(VitalsMinigame __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.TimeLord) && !PlayerControl.LocalPlayer.Data.IsDead && VitalsMinigame.Instance)
                __instance.Close();

            return true;
        }
    }
}