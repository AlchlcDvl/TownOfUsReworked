using System;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.SheriffMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Sheriff))
            {
                var sheriff = (Sheriff) role;
                sheriff.LastInterrogated = DateTime.UtcNow;
                sheriff.LastInterrogated = sheriff.LastInterrogated.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InterrogateCd);
            }
        }
    }
}