using HarmonyLib;
using TownOfUsReworked.BetterMaps.Airship;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.PlayerLayers
{
    [HarmonyPatch]
    public static class EndGame
    {
        public static void Reset()
        {
            foreach (var role in Role.AllRoles)
                role.AllPrints.Clear();
        }

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
        public static class EndGamePatch
        {
            public static void Prefix()
            {
                Reset();
                SpawnInMinigamePatch.ResetGlobalVariable();
            }
        }

        [HarmonyPatch(typeof(GameManager), nameof(GameManager.RpcEndGame))]
        public static class ShipStatusPatch
        {
            public static bool Prefix()
            {
                Reset();
                return true;
            }
        }
    }
}