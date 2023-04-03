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
            foreach (var role in Role.GetRoles(RoleEnum.Morphling))
            {
                var morphling = (Morphling)role;

                if (morphling.Morphed)
                    morphling.Morph();
                else if (morphling.Enabled)
                    morphling.Unmorph();
            }
        }
    }
}