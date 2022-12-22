using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.InvestigatorMod
{
    public class EndGame
    {
        public static void Reset()
        {
            foreach (var role in Role.GetRoles(RoleEnum.Investigator))
                ((Investigator) role).AllPrints.Clear();
        }

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
        public static class EndGamePatch
        {
            public static void Prefix(AmongUsClient __instance)
            {
                Reset();
            }
        }

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
        public static class ShipStatusPatch
        {
            public static bool Prefix(ShipStatus __instance)
            {
                Reset();
                return true;
            }
        }
    }
}