using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Objects;

namespace TownOfUsReworked.PlayerLayers
{
    [HarmonyPatch]
    public static class EndGame
    {
        public static void Reset()
        {
            foreach (var role in Role.AllRoles)
            {
                Footprint.DestroyAll(role);
                role.AllPrints.Clear();
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