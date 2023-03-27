using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BansheeMod
{
    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
    public static class NoSpawn
    {
        public static bool Prefix(SpawnInMinigame __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Banshee) && !Role.GetRole<Banshee>(PlayerControl.LocalPlayer).Caught)
            {
                __instance.Close();
                return false;
            }

            return true;
        }
    }
}