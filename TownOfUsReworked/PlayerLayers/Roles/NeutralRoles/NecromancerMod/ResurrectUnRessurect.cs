using HarmonyLib;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NecromancerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class ResurrectUnRessurect
    {
        public static void Postfix()
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