using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PhantomMod
{
    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
    public class NoSpawn
    {
        public static bool Prefix(SpawnInMinigame __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Phantom))
            {
                if (!Role.GetRole<Phantom>(PlayerControl.LocalPlayer).Caught)
                {
                    __instance.Close();
                    return false;
                }
            }

            return true;
        }
    }
}