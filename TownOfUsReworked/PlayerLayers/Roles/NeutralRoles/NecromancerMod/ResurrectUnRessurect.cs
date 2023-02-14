using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NecromancerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class ResurrectUnRessurect
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Necromancer))
            {
                var necro = (Necromancer)role;
                
                if (necro.IsResurrecting)
                    necro.Resurrect();
                else if (necro.Resurrecting)
                    necro.UnResurrect();
            }
        }
    }
}