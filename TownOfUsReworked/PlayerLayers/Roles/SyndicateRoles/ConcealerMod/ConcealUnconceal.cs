using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.ConcealerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class ConcealUnconceal
    {
        public static void Postfix()
        {
            foreach (var role in Role.GetRoles(RoleEnum.Concealer))
            {
                var concealer = (Concealer)role;

                if (concealer.Concealed)
                    concealer.Conceal();
                else if (concealer.Enabled)
                    concealer.UnConceal();
            }
        }
    }
}