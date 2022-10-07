using System;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.VeteranMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Veteran))
            {
                var veteran = (Veteran)role;
                veteran.LastAlerted = DateTime.UtcNow;
                veteran.LastAlerted = veteran.LastAlerted.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.AlertCd);
            }
        }
    }
}