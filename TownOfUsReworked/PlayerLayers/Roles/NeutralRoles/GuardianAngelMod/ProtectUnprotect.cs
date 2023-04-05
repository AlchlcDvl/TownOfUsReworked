using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuardianAngelMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class ProtectUnportect
    {
        public static void Postfix()
        {
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var ga in Role.GetRoles<GuardianAngel>(RoleEnum.GuardianAngel))
            {
                if (ga.Protecting)
                    ga.Protect();
                else if (ga.Enabled)
                    ga.UnProtect();
            }
        }
    }
}