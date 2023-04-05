using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NecromancerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class ResurrectUnRessurect
    {
        public static void Postfix()
        {
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var necro in Role.GetRoles<Necromancer>(RoleEnum.Necromancer))
            {
                if (necro.IsResurrecting)
                    necro.Resurrect();
                else if (necro.Resurrecting)
                    necro.UnResurrect();
            }
        }
    }
}