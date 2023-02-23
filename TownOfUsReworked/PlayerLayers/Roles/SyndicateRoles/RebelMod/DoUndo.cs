using HarmonyLib;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.RebelMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class DoUndo
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Rebel))
            {
                var reb = (Rebel)role;

                if (reb.Concealed)
                    reb.Conceal();
                else if (reb.ConcealEnabled)
                    reb.UnConceal();
            }
        }
    }
}