using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.ConcealerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ConcealUnconceal
    {
        public static bool ConcealEnabled;

        public static void Postfix(HudManager __instance)
        {
            ConcealEnabled = false;

            foreach (var role in Role.GetRoles(RoleEnum.Concealer))
            {
                var concealer = (Concealer) role;

                if (concealer.Concealed)
                {
                    ConcealEnabled = true;
                    concealer.Conceal();
                }
                else if (concealer.Enabled)
                {
                    ConcealEnabled = false;
                    concealer.UnConceal();
                }
            }
        }
    }
}