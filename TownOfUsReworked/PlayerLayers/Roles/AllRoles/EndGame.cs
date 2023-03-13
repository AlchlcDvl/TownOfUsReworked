using HarmonyLib;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.RpcEndGame))]
    public class EndGame
    {
        public static void Reset()
        {
            foreach (var role in Role.AllRoles)
                role.AllPrints.Clear();
        }

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
        public static class EndGamePatch
        {
            public static void Prefix(AmongUsClient __instance)
            {
                Reset();
            }
        }

        [HarmonyPatch(typeof(GameManager), nameof(GameManager.RpcEndGame))]
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