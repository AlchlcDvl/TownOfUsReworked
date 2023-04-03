using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.DrunkardMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class ConfuseUnconfuse
    {
        public static void Postfix()
        {
            foreach (var role in Role.GetRoles(RoleEnum.Drunkard))
            {
                var drunk = (Drunkard)role;

                if (drunk.Confused)
                    drunk.Confuse();
                else if (drunk.Enabled)
                    drunk.Unconfuse();
            }
        }
    }
}