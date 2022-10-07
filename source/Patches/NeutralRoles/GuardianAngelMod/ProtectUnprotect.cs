using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.GuardianAngelMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class ProtectUnportect
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel) role;
                if (ga.Protecting)
                    ga.Protect();
                else if (ga.Enabled) ga.UnProtect();
            }
        }
    }
}