using System;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.VigilanteMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Vigilante))
            {
                var vigilante = (Vigilante) role;
                vigilante.LastKilled = DateTime.UtcNow;
                vigilante.LastKilled = vigilante.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.VigiKillCd);
            }
        }
    }
}