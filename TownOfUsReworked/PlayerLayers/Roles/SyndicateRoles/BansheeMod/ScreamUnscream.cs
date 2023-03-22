using HarmonyLib;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BansheeMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class ScreamUnscream
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
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