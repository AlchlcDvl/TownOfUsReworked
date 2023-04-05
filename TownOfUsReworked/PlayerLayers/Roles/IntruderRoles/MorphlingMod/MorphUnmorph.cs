using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.MorphlingMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class MorphUnmorph
    {
        public static void Postfix()
        {
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var morphling in Role.GetRoles<Morphling>(RoleEnum.Morphling))
            {
                if (morphling.Morphed)
                    morphling.Morph();
                else if (morphling.Enabled)
                    morphling.Unmorph();
            }
        }
    }
}