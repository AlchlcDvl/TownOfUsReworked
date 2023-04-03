using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.PoisonerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class ConfuseUnconfuse
    {
        public static void Postfix()
        {
            foreach (var role in Role.GetRoles(RoleEnum.Poisoner))
            {
                var pois = (Poisoner)role;

                if (pois.Poisoned)
                    pois.Poison();
                else if (pois.Enabled)
                    pois.PoisonKill();
            }
        }
    }
}