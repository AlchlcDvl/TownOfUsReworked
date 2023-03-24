using HarmonyLib;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ConsortMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class BlockUnblock
    {
        public static void Postfix()
        {
            foreach (var role in Role.GetRoles(RoleEnum.Consort))
            {
                var consort = (Consort)role;

                if (consort.Blocking)
                    consort.Block();
                else if (consort.Enabled)
                    consort.UnBlock();
            }
        }
    }
}