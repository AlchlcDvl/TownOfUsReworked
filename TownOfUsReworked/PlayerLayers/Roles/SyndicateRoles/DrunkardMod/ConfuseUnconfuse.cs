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
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var drunk in Role.GetRoles<Drunkard>(RoleEnum.Drunkard))
            {
                if (drunk.Confused)
                    drunk.Confuse();
                else if (drunk.Enabled)
                    drunk.Unconfuse();
            }
        }
    }
}