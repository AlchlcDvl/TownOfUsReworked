using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.Patches
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
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Banshee) && !Role.GetRole<Banshee>(PlayerControl.LocalPlayer).Caught)
            {
                __instance.Close();
                return false;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Phantom) && !Role.GetRole<Phantom>(PlayerControl.LocalPlayer).Caught)
            {
                __instance.Close();
                return false;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Revealer) && !Role.GetRole<Revealer>(PlayerControl.LocalPlayer).Caught)
            {
                __instance.Close();
                return false;
            }

            return true;
        }
    }
}