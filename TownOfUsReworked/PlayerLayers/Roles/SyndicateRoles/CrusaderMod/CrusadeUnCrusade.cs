using HarmonyLib;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.CrusaderMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class CrusadeUnCrusade
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Crusader))
            {
                var crus = (Crusader)role;

                if (crus.OnCrusade)
                    crus.Crusade();
                else if (crus.Enabled)
                    crus.UnCrusade();
            }
        }
    }
}