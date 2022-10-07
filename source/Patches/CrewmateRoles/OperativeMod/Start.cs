using System;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.OperativeMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Operative))
            {
                var op = (Operative)role;
                op.lastBugged = DateTime.UtcNow;
                op.lastBugged = op.lastBugged.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BugCooldown);
            }
        }
    }
}
