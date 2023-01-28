using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.WraithMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class InvisUninvis
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Wraith))
            {
                var wraith = (Wraith) role;
                
                if (wraith.IsInvis)
                    wraith.Invis();
                else if (wraith.Enabled)
                    wraith.Uninvis();
            }
        }
    }
}