using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.DrunkardMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class ConfuseUnconfuse
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
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