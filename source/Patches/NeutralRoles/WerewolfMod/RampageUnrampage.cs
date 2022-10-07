using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.WerewolfMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class RampageUnrampage
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Werewolf))
            {
                var werewolf = (Werewolf) role;
                if (werewolf.Rampaged)
                    werewolf.Rampage();
                else if (werewolf.Enabled) werewolf.Unrampage();
            }
        }
    }
}