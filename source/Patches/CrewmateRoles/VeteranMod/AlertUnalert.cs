using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.VeteranMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class AlertUnalert
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Veteran))
            {
                var veteran = (Veteran) role;
                if (veteran.OnAlert)
                    veteran.Alert();
                else if (veteran.Enabled) veteran.UnAlert();
            }
        }
    }
}