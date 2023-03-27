using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GhoulMod
{
    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
    public static class NoSpawn
    {
        public static bool Prefix(SpawnInMinigame __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Ghoul) && !Role.GetRole<Ghoul>(PlayerControl.LocalPlayer).Caught)
            {
                __instance.Close();
                return false;
            }

            return true;
        }
    }
}