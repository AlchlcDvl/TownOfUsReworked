using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BansheeMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class ScreamUnscream
    {
        public static void Postfix()
        {
            foreach (var role in Role.GetRoles(RoleEnum.Banshee))
            {
                var banshee = (Banshee)role;

                if (banshee.Screaming)
                    banshee.Scream();
                else if (banshee.Enabled)
                    banshee.UnScream();
            }
        }
    }
}