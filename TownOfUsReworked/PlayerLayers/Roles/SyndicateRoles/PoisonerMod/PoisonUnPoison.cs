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
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var pois in Role.GetRoles<Poisoner>(RoleEnum.Poisoner))
            {
                if (pois.Poisoned)
                    pois.Poison();
                else if (pois.Enabled)
                    pois.PoisonKill();
            }
        }
    }
}