using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BansheeMod
{
    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
    public class NoSpawn
    {
        public static bool Prefix(SpawnInMinigame __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Banshee))
            {
                if (!Role.GetRole<Banshee>(PlayerControl.LocalPlayer).Caught)
                {
                    __instance.Close();
                    return false;
                }
            }

            return true;
        }
    }
}