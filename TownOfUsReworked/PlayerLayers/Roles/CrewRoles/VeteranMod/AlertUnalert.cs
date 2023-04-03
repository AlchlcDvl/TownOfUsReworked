using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VeteranMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class AlertUnalert
    {
        public static void Postfix()
        {
            foreach (var role in Role.GetRoles(RoleEnum.Veteran))
            {
                var veteran = (Veteran)role;

                if (veteran.OnAlert)
                    veteran.Alert();
                else if (veteran.Enabled)
                    veteran.UnAlert();
            }
        }
    }
}