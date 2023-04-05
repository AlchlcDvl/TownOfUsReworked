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
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var veteran in Role.GetRoles<Veteran>(RoleEnum.Veteran))
            {
                if (veteran.OnAlert)
                    veteran.Alert();
                else if (veteran.Enabled)
                    veteran.UnAlert();
            }
        }
    }
}