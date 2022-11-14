/*using System;
using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EscortMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Escort))
            {
                var esc = (Escort) role;
                esc.LastBlocked = DateTime.UtcNow;
                esc.LastBlocked = esc.LastBlocked.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DouseCd);
            }
        }
    }
}*/