using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EscortMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class BlockUnblock
    {
        public static void Postfix()
        {
            foreach (var role in Role.GetRoles(RoleEnum.Escort))
            {
                var escort = (Escort)role;

                if (escort.Blocking)
                    escort.Block();
                else if (escort.Enabled)
                    escort.UnBlock();
            }
        }
    }
}