using System;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.SurvivorMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Survivor))
            {
                var surv = (Survivor)role;
                surv.LastVested = DateTime.UtcNow;
                surv.LastVested = surv.LastVested.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.VestCd);
            }
        }
    }
}