using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.CrusaderMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class CrusadeUnCrusade
    {
        public static void Postfix()
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